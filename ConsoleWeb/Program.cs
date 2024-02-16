using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.MapGet("/console-app", async () =>
{
    // 設置 ProcessStartInfo
    var startInfo = new ProcessStartInfo
    {
        FileName = $@"{AppDomain.CurrentDomain.BaseDirectory}\{nameof(ConsoleApp)}.exe",
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    Console.WriteLine(startInfo.FileName);

    using (var process = new Process { StartInfo = startInfo })
    {
        process.Start();
        var output = await process.StandardOutput.ReadToEndAsync(); // 讀取標準輸出
        process.WaitForExit(); // 等待程序結束
        return output; // 將執行結果輸出
    }
});
app.Run();
