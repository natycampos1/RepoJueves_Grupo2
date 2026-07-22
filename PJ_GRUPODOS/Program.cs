var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Espacio de dependencias
builder.Services.AddHttpClient(); //Dependencia para consumir servicios web
builder.Services.AddSession(); //Dependencia para manejar sesiones

var app = builder.Build();

//Middleware de Errores
app.UseExceptionHandler("/Error/CapturarError");

app.UseHsts();

app.UseHttpsRedirection();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
