using ecommerce_api.Data;
using ecommerce_api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "E-commerce API", Version = "v1" });
        });

        // Register services with in-memory collections
        services.AddSingleton<CustomerService>();
        services.AddSingleton<OrderService>();
        services.AddSingleton<OrderDetailService>();
        services.AddSingleton<ProductService>();

        // Register services with in-memory collections
        services.AddSingleton<CustomerService>();
        services.AddSingleton<OrderService>();
        services.AddSingleton<OrderDetailService>();
        services.AddSingleton<ProductService>();
    }


    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, CustomerService customerService, OrderService orderService, OrderDetailService orderDetailService, ProductService productService)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        // Initialize mock data
        MockDataInitializer.Initialize(customerService, orderService, orderDetailService, productService);

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-commerce API V1");
        });
    }
}