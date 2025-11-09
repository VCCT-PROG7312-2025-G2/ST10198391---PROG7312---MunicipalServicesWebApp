using Task_2___Issue_Report_Web_.Data;

namespace Task_2___Issue_Report_Web_
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Adds services to the container.
            builder.Services.AddControllersWithViews();

            // Register ServiceRequestStore as singleton
            builder.Services.AddSingleton<ServiceRequestStore>(provider =>
            {
                var env = provider.GetRequiredService<IWebHostEnvironment>();
                return new ServiceRequestStore(env.ContentRootPath);
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
