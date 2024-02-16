using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapFallback(async (context) =>
    {
        var app = context.Request.Path.ToString().TrimStart('/');
        var args = context.Request.Query["args"].ToString() ?? string.Empty;
        // 設置 ProcessStartInfo
        var startInfo = new ProcessStartInfo
        {
            FileName = $"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, app)}.exe",
            Arguments = args,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        Console.WriteLine(startInfo.FileName);

        if (File.Exists(startInfo.FileName))
        {
            using var process = new Process { StartInfo = startInfo };
            process.Start();
            var output = await process.StandardOutput.ReadToEndAsync(); // 讀取標準輸出
            process.WaitForExit(); // 等待程序結束
            await context.Response.WriteAsync(output); // 將執行結果輸出
        }
        else
        {
            await context.Response.WriteAsync($"'{app}' not found");
        }
    });
});
app.Run();
