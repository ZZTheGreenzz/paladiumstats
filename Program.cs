using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DotNetEnv;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Charger les variables depuis le fichier .env
        Env.Load();

        var builder = WebApplication.CreateBuilder(args);

        // Ajouter les services nécessaires ici, y compris CORS
        builder.Services.AddControllers();

        // Ajouter la configuration CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins", builder =>
            {
                builder
                    .AllowAnyOrigin()  // Autoriser toutes les origines
                    .AllowAnyHeader()  // Autoriser tous les en-têtes
                    .AllowAnyMethod(); // Autoriser toutes les méthodes (GET, POST, etc.)
            });
        });



        var app = builder.Build();

        // Configurez le pipeline de requêtes HTTP
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles(); // Permet d'utiliser les fichiers statiques
        app.UseRouting();

        // Appliquer le middleware CORS ici
        app.UseCors("AllowAllOrigins");


        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/", async context =>
            {
                // Utilisation de await pour rendre le code asynchrone
                await Task.Run(() => context.Response.Redirect("/index.html"));
            });
            endpoints.MapControllers(); // Permet d'accéder aux contrôleurs
        });

        // Utilisation de await pour exécuter l'application de manière asynchrone
        await app.RunAsync();
    }
}
