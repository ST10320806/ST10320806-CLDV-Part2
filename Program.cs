using ST10320806_Part1.Services;

namespace ST10320806_Part1
{
    public class Program
    {
        
            public static void Main(string[] args)
            {


            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHttpClient();
            builder.Services.AddControllersWithViews();

            // Register the services for dependency injection
            builder.Services.AddTransient<CustomerService>();
            builder.Services.AddScoped<BlobService>();
            builder.Services.AddScoped<OrderService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            app.Run();



        }
        }
    }

