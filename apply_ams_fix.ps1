# AMS Media Web – Auto Fix Script (safe quoting)
$ErrorActionPreference = "Stop"

function Backup-File($path) { if (Test-Path $path) { Copy-Item $path "$path.bak" -Force } }
function Read-Text($path)  { if (Test-Path $path) { Get-Content $path -Raw } else { $null } }
function Write-Text($p,$c) { New-Item -ItemType File -Path $p -Force | Out-Null; Set-Content -Path $p -Value $c -Encoding UTF8 }

# ---------- 1) AmsDbContext.cs ----------
$ctx = "Ams.Media.Web\Data\AmsDbContext.cs"
if (Test-Path $ctx) {
  Backup-File $ctx
  $code = Read-Text $ctx

  # ปรับ Security_Log: ชื่อโต๊ะ + PK
  $code = $code -replace 'ToTable\("Security_log"\)', 'ToTable("Security_Log")'
  $code = $code -replace 'HasNoKey\(\s*\);', 'HasKey(x => x.Username);'

  # แทรก mapping คอลัมน์ใน block ของ SecurityLog ถ้ายังไม่มี
  $mapLog = @'
                e.Property(x => x.Username).HasColumnName("username");
                e.Property(x => x.UserDateTime).HasColumnName("userdatetime");
                e.Property(x => x.ComputerName).HasColumnName("computername");
                e.Property(x => x.Processing).HasColumnName("processing");
'@
  if ($code -notmatch '(?s)Entity<\s*SecurityLog.*HasColumnName\("username"\)') {
    $code = [regex]::Replace(
      $code,
      '(?s)(modelBuilder\.Entity<\s*SecurityLog\s*>\s*\(\s*e\s*=>\s*\{\s*[^}]*?ToTable\("Security_Log"\);\s*)',
      ('$1' + $mapLog)
    )
  }

  # แทรก mapping username ให้ security_menu ถ้ายังไม่มี
  if ($code -notmatch '(?s)Entity<\s*SecurityMenu.*HasColumnName\("username"\)') {
    $ins = '                e.Property(x => x.Username).HasColumnName("username");' + "`r`n"
    $code = [regex]::Replace(
      $code,
      '(?s)(modelBuilder\.Entity<\s*SecurityMenu\s*>\s*\(\s*e\s*=>\s*\{\s*[^}]*?ToTable\("security_menu"\);\s*)',
      ('$1' + $ins)
    )
  }

  Write-Text $ctx $code
  Write-Host "[OK] Updated $ctx"
} else {
  Write-Host "[WARN] Not found: $ctx"
}

# ---------- 2) Models/SecurityMenu.cs ----------
$smModel = "Ams.Media.Web\Models\SecurityMenu.cs"
if (Test-Path $smModel) {
  Backup-File $smModel
  $code = Read-Text $smModel
  if ($code -notmatch '(?m)^\s*public\s+string\?\s+Username\s*\{\s*get;\s*set;\s*\}\s*$') {
    $code = $code -replace '(?m)(class\s+SecurityMenu\s*\{)', '$1' + "`r`n        public string? Username { get; set; } // username"
    Write-Text $smModel $code
    Write-Host "[OK] Added Username property to $smModel"
  } else {
    Write-Host "[OK] Username already present in $smModel"
  }
} else {
  Write-Host "[WARN] Not found: $smModel"
}

# ---------- 3) Services/MenuGate.cs ----------
$menuGate = "Ams.Media.Web\Services\MenuGate.cs"
if (Test-Path $menuGate) {
  Backup-File $menuGate
  $code = Read-Text $menuGate

  # โหลด security_menu ตามผู้ใช้
  $code = $code -replace 'SecurityMenus\.AsNoTracking\(\)\.FirstOrDefaultAsync\(\)', 'SecurityMenus.AsNoTracking().FirstOrDefaultAsync(x => x.Username == (user?.Identity?.Name ?? ""))'

  # เมนู Client → Account/Denied
  $code = $code -replace '(Code\s*=\s*"mclient"[^}]*Controller\s*=\s*")mClient(")', '$1Account$2'
  $code = [regex]::Replace($code, '(Code\s*=\s*"mclient"[^}]*Controller\s*=\s*"Account"[^}]*)\}', '$1, Action = "Denied" }')

  Write-Text $menuGate $code
  Write-Host "[OK] Updated $menuGate"
} else {
  Write-Host "[WARN] Not found: $menuGate"
}

# ---------- 4) Views/Account/Denied.cshtml ----------
$denied = "Ams.Media.Web\Views\Account\Denied.cshtml"
if (!(Test-Path $denied)) {
  $html = @'
@{
    Layout = "_Layout";
    ViewData["Title"] = "Access denied";
}
<div class="container" style="max-width:640px">
  <div class="card shadow-sm my-4">
    <div class="card-body">
      <h5 class="mb-2">Access denied</h5>
      <p class="text-muted mb-3">
        คุณไม่มีสิทธิ์เข้าถึงเมนูนี้ หรือฟีเจอร์ถูกปิดใช้งานชั่วคราว
      </p>
      <a asp-controller="Home" asp-action="Index" class="btn btn-primary">กลับหน้าแรก</a>
    </div>
  </div>
</div>
'@
  New-Item -ItemType Directory -Force -Path (Split-Path $denied) | Out-Null
  Write-Text $denied $html
  Write-Host "[OK] Created $denied"
} else {
  Write-Host "[OK] Exists $denied"
}

Write-Host "=== Done. Build & test now. ==="
