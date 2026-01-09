
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// إعداد Serilog (Console + File)
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/myapp-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
    .CreateLogger();

builder.Host.UseSerilog();

// إضافة الخدمات
builder.Services.AddControllers()
    // تفعيل ProblemDetails تلقائيًا لأخطاء التحقق وغيرها
    .AddJsonOptions(opts =>
    {
        // اختياري: إعدادات JSON
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// استخدام معالجة أخطاء عامة قياسية
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // للعرض التفصيلي أثناء التطوير
}
else
{
    // ميدلوير قياسي لمعالجة الاستثناءات وإرجاع ProblemDetails
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
            var exception = exceptionHandlerFeature?.Error;

            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred.",
                Detail = app.Environment.IsDevelopment() ? exception?.ToString() : "Please try again later.",
                Instance = context.Request.Path
            };

            Log.Error(exception, "Unhandled exception caught by global handler.");

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(problem);
        });
    });
}

app.UseSerilogRequestLogging(); // تسجيل الطلبات تلقائيًا

// Swagger للتوثيق
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
