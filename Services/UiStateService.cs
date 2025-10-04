using System.Text.Json;

namespace Ams.Media.Web.Services
{
    public sealed class UiStateService : IUiStateService
    {
        private readonly string _jsonPath;
        private static readonly SemaphoreSlim _lock = new(1, 1);

        private sealed class UiState { public string ShowData { get; set; } = "C"; }

        public UiStateService(IWebHostEnvironment env)
        {
            _jsonPath = Path.Combine(env.ContentRootPath, "App_Data", "ui-state.json");
            Directory.CreateDirectory(Path.GetDirectoryName(_jsonPath)!);
            if (!File.Exists(_jsonPath))
            {
                var init = new UiState { ShowData = "C" };
                File.WriteAllText(_jsonPath, JsonSerializer.Serialize(init, new JsonSerializerOptions { WriteIndented = true }));
            }
        }

        public async Task<string> GetShowDataAsync()
        {
            await _lock.WaitAsync();
            try
            {
                using var fs = File.OpenRead(_jsonPath);
                var state = await JsonSerializer.DeserializeAsync<UiState>(fs) ?? new UiState();
                var v = (state.ShowData ?? "C").Trim().ToUpperInvariant();
                return (v == "A" || v == "C") ? v : "C";
            }
            finally { _lock.Release(); }
        }

        public async Task<string> ToggleShowDataAsync()
        {
            await _lock.WaitAsync();
            try
            {
                UiState state;
                using (var fs = File.OpenRead(_jsonPath))
                    state = await JsonSerializer.DeserializeAsync<UiState>(fs) ?? new UiState();

                var cur = (state.ShowData ?? "C").Trim().ToUpperInvariant();
                state.ShowData = (cur == "A") ? "C" : "A";

                await using var fsw = new FileStream(_jsonPath, FileMode.Create, FileAccess.Write, FileShare.None);
                await JsonSerializer.SerializeAsync(fsw, state, new JsonSerializerOptions { WriteIndented = true });

                return state.ShowData;
            }
            finally { _lock.Release(); }
        }

        public async Task<string> SetShowDataAsync(string mode)
        {
            var v = (mode ?? "C").Trim().ToUpperInvariant();
            if (v != "A" && v != "C") v = "C";

            await _lock.WaitAsync();
            try
            {
                var state = new UiState { ShowData = v };
                await using var fsw = new FileStream(_jsonPath, FileMode.Create, FileAccess.Write, FileShare.None);
                await JsonSerializer.SerializeAsync(fsw, state, new JsonSerializerOptions { WriteIndented = true });
                return v;
            }
            finally { _lock.Release(); }
        }
    }
}
