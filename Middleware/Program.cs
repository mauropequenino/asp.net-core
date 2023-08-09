using Core;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

//Services

builder.Services.Configure<FruitOptions>(options =>
{
    options.Name = "watermelon";
});

var app = builder.Build();

app.MapGet("/fruit", async (HttpContext context, IOptions<FruitOptions> FruitOptions) =>
{
    FruitOptions options = FruitOptions.Value;
    await context.Response.WriteAsync($"{options.Name}, {options.Color}");
});


//Middlewares
/*
 * O método app.Use. Middleware no ASP.NET Core são componentes de software que processam solicitações e respostas conforme elas passam pela cadeia de processamento.
 * Nesse caso, o middleware verifica se a solicitação HTTP recebida é um pedido GET e possui um parâmetro de consulta chamado "custom" com o valor "true". 
 * Se essas condições forem atendidas, o middleware define o tipo de conteúdo da resposta como "text/plain" e escreve "Custom MiddleWare\n" na resposta. 
 * Depois disso, ele chama o delegado next para continuar o processamento da solicitação
 

app.Use(async (context, next) =>
{
    if (context.Request.Method == HttpMethods.Get && context.Request.Query["custom"] == "true")
    {
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync("Custom MiddleWare\n");
    }
    await next();
});

*/

//==========================================

/*A função de middleware fornecida será executada para cada solicitação HTTP.
 * Retornar um status code 200
 * 200 OK status code means that the request was successful

app.Use(async (context, next) =>
{
    await next();
    await context.Response.WriteAsync($"\n Status code: {context.Response.StatusCode}");
    
});
*/

// ==========================================

/*
 * Realizar o "short-circuiting" (interrupção do pipeline de solicitação) no ASP.NET Core envolve interromper a execução normal dos middlewares e manipuladores com base em certas condições, normalmente antes que a resposta seja gerada. 
 * Isso pode ser útil quando você deseja lidar com solicitações ou condições específicas separadamente, sem processar todo o pipeline
 * 

*/
app.Use(async (context, next) =>
{
    /*
     * Este middleware verifica se o caminho da solicitação é "/short". Se for, ele envia uma resposta sem prosseguir para o próximo middleware. Caso contrário, ele chama o próximo middleware na sequência.
     */
    if (context.Request.Path == "/short")
    {
        await context.Response.WriteAsync("Request short-circuited");
    }
    else
    {
        await next();
    }
});

/*
 * adiciona middlewares a um ramo específico e define rotas para lidar com solicitações HTTP. 
 * adiciona um middleware específico para um ramo usando o método .Map(). O middleware escreve "Middleware de ramo" na resposta quando a rota /branch é acessada.
 * O middleware global processa solicitações para todas as rotas na aplicação.
 */

((IApplicationBuilder)app).Map("/branch", branch =>
{
    branch.Use(async (HttpContext context, Func<Task> next) =>
    {
        await context.Response.WriteAsync("Branch middleware");
    });
});


//Chamar o middleware global criado
app.UseMiddleware<Middleware>();

app.MapGet("/", () => "Hello World!");

app.Run();
