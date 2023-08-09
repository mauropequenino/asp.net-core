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
 * O m�todo app.Use. Middleware no ASP.NET Core s�o componentes de software que processam solicita��es e respostas conforme elas passam pela cadeia de processamento.
 * Nesse caso, o middleware verifica se a solicita��o HTTP recebida � um pedido GET e possui um par�metro de consulta chamado "custom" com o valor "true". 
 * Se essas condi��es forem atendidas, o middleware define o tipo de conte�do da resposta como "text/plain" e escreve "Custom MiddleWare\n" na resposta. 
 * Depois disso, ele chama o delegado next para continuar o processamento da solicita��o
 

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

/*A fun��o de middleware fornecida ser� executada para cada solicita��o HTTP.
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
 * Realizar o "short-circuiting" (interrup��o do pipeline de solicita��o) no ASP.NET Core envolve interromper a execu��o normal dos middlewares e manipuladores com base em certas condi��es, normalmente antes que a resposta seja gerada. 
 * Isso pode ser �til quando voc� deseja lidar com solicita��es ou condi��es espec�ficas separadamente, sem processar todo o pipeline
 * 

*/
app.Use(async (context, next) =>
{
    /*
     * Este middleware verifica se o caminho da solicita��o � "/short". Se for, ele envia uma resposta sem prosseguir para o pr�ximo middleware. Caso contr�rio, ele chama o pr�ximo middleware na sequ�ncia.
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
 * adiciona middlewares a um ramo espec�fico e define rotas para lidar com solicita��es HTTP. 
 * adiciona um middleware espec�fico para um ramo usando o m�todo .Map(). O middleware escreve "Middleware de ramo" na resposta quando a rota /branch � acessada.
 * O middleware global processa solicita��es para todas as rotas na aplica��o.
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
