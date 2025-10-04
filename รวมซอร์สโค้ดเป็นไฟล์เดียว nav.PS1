# === รวมซอร์สโค้ดเป็นไฟล์เดียว: navin_full_bundle.txt ===
$ErrorActionPreference='Stop'
$ext = @(".cs",".cshtml",".csproj",".sln",".json",".yml",".yaml",
         ".config",".props",".targets",".ts",".js",".css",".scss",
         ".sql",".md",".editorconfig",".gitignore")

$files = Get-ChildItem -Recurse -File | Where-Object {
  $ext -contains $_.Extension.ToLower() -and
  $_.FullName -notmatch '\\(bin|obj|\.git|node_modules|packages|\.vs)\\'
}

$bundle = "navin_full_bundle.txt"
"===== BUNDLE $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss') =====" | Set-Content -Encoding UTF8 $bundle

foreach ($f in $files) {
  "`r`n===== FILE: $($f.FullName.Replace($PWD.Path+'\','')) =====`r`n" | Add-Content -Encoding UTF8 $bundle
  Get-Content -Raw $f.FullName | Add-Content -Encoding UTF8 $bundle
}

Write-Host "✅ สร้าง $bundle แล้ว"

# ถ้าไฟล์ใหญ่เกิน ~4MB ให้แบ่งพาร์ตอัตโนมัติ
if ((Get-Item $bundle).Length -gt 4MB) {
  Write-Host "ℹ กำลังแบ่งเป็นพาร์ต ~4MB"
  $bytes = [IO.File]::ReadAllBytes($bundle)
  $chunk = 4MB
  $total = [int][Math]::Ceiling($bytes.Length/$chunk)
  0..($total-1) | ForEach-Object {
    $start = $_*$chunk; $len = [Math]::Min($chunk, $bytes.Length-$start)
    $part = "navin_full_bundle.part$('{0:D2}' -f ($_+1)).txt"
    [IO.File]::WriteAllBytes($part, $bytes[$start..($start+$len-1)])
    Write-Host "  → $part"
  }
  Write-Host "✅ แบ่งพาร์ตเสร็จ — อัปโหลดพาร์ตทั้งหมดในแชตนี้"
}
