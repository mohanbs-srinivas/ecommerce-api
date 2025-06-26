using ecommerce_api.Data;
using ecommerce_api.Services;
using ecommerce_api.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureServices((context, services) =>
                {
                    // Add Entity Framework and Identity
                    services.AddDbContext<EcommerceContext>(options =>
                        options.UseInMemoryDatabase("EcommerceDb"));

                    services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                    {
                        // Password settings
                        options.Password.RequireDigit = true;
                        options.Password.RequiredLength = 8;
                        options.Password.RequireNonAlphanumeric = true;
                        options.Password.RequireUppercase = true;
                        options.Password.RequireLowercase = true;
                        
                        // User settings
                        options.User.RequireUniqueEmail = true;
                        
                        // Email confirmation
                        options.SignIn.RequireConfirmedEmail = true;
                    })
                    .AddEntityFrameworkStores<EcommerceContext>()
                    .AddDefaultTokenProviders();

                    services.AddControllers();
                    services.AddSwaggerGen(c =>
                    {
                        c.SwaggerDoc("v1", new OpenApiInfo { Title = "E-commerce API", Version = "v1" });
                    });
                    services.AddSingleton<CustomerService>();
                    services.AddSingleton<OrderService>();
                    services.AddSingleton<OrderDetailService>();
                    services.AddSingleton<ProductService>();
                });
                webBuilder.Configure((context, app) =>
                {
                    var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
                    if (env.IsDevelopment())
                    {
                        app.UseDeveloperExceptionPage();
                    }
                    
                    app.UseRouting();
                    app.UseAuthentication();
                    app.UseAuthorization();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
                    
                    // Initialize data
                    using (var scope = app.ApplicationServices.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<EcommerceContext>();
                        dbContext.Database.EnsureCreated();
                        
                        var customerService = scope.ServiceProvider.GetRequiredService<CustomerService>();
                        var orderService = scope.ServiceProvider.GetRequiredService<OrderService>();
                        var orderDetailService = scope.ServiceProvider.GetRequiredService<OrderDetailService>();
                        var productService = scope.ServiceProvider.GetRequiredService<ProductService>();
                        MockDataInitializer.Initialize(customerService, orderService, orderDetailService, productService);
                    }
                    
                    app.UseSwagger();
                    app.UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-commerce API V1");
                    });
                });
            });
}