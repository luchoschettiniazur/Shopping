using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shooping.Data;
using Shooping.Data.Identity;
using Shooping.Helpers.Auth;
using Shooping.Helpers.Blob;
using Shooping.Helpers.Combo;
using Shooping.Helpers.Combos;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


////NO VEO QUE HAGA NADA EN MVC (EN API SI FUNCIONA) VER COMO HACER ESTO DE OTRA FORMA:
//builder.Services.AddControllersWithViews()
//         .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddControllersWithViews();



builder.Services.AddDbContext<DataContext>(o =>
{
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


//TODO: Make strongest password
builder.Services.AddIdentity<User, IdentityRole>(cfg =>
{
    cfg.User.RequireUniqueEmail = true;
    cfg.Password.RequireDigit = false;
    cfg.Password.RequiredUniqueChars = 0;
    cfg.Password.RequireLowercase = false;
    cfg.Password.RequireNonAlphanumeric = false;
    cfg.Password.RequireUppercase = false;


    cfg.Lockout.MaxFailedAccessAttempts = 3; //por defecto es 5
    cfg.Lockout.AllowedForNewUsers = true;  //tambien bloquear nuevos usuarios 
    cfg.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);


    //cfg.Password.RequiredLength = 6;  //es el predeterminado, si quieres cambiarlo, puedes utilizar esta propiedad.

}).AddEntityFrameworkStores<DataContext>();



builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = "/Account/NotAuthorized"; //cuando hay problemas con el login lo enviamos a esta pagina.
	options.AccessDeniedPath = "/Account/NotAuthorized"; //cuando hay problemas con el acceso lo enviamos a esta pagina.
});



builder.Services.AddTransient<SeedDb>();
builder.Services.AddScoped<IUserHelper, UserHelper>();
builder.Services.AddScoped<ICombosHelper, CombosHelper>();
builder.Services.AddScoped<IBlobHelper, BlobHelper>();



var app = builder.Build();


SeedData(app);
void SeedData(WebApplication app)
{
    IServiceScopeFactory? scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (IServiceScope? scope = scopedFactory!.CreateScope())
    {
        SeedDb? service = scope.ServiceProvider.GetService<SeedDb>();
        service!.SeedAsync().Wait();
    }
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


//cuando no encuetre una pagina 
//cada vez que hay un error el va ejecutar el error y se le va enviar el codigo de error.
//esto se ejecuta siempre en el Homecontroller, ya que es el que maneja la pagina principal.
//si el codigo es el 404  -> se le envia al controlador Home ->  [Route("error/404")]
app.UseStatusCodePagesWithReExecute("/error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
