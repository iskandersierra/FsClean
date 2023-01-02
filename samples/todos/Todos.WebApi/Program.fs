namespace Todos.WebApi

open Microsoft.OpenApi.Models

#nowarn "20"

open System
open System.Collections.Generic
open System.IO
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.HttpsPolicy
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging

open FsClean.Application
open FsClean.Business.UseCases

open Todos.Application.TodoList
open Todos.Business.TodoList
open Todos.Domain.TodoList

module Program =
    let exitCode = 0

    [<EntryPoint>]
    let main args =

        let builder = WebApplication.CreateBuilder(args)

        builder.Services.AddControllers()

        builder
            .Services
            .AddRazorPages()
            .AddRazorRuntimeCompilation()

        builder.Services.AddSwaggerGen (fun options ->
            let info = OpenApiInfo()
            info.Title <- "Todos.WebApi"
            info.Version <- "v1"
            options.SwaggerDoc(info.Version, info))

        builder.Services.AddSingleton<Clock>(implementationFactory = fun _ -> Clock.system)
        builder.Services.AddSingleton<IdGenerator>(implementationFactory = fun _ -> IdGenerator.entityIdGenerator)

        builder.Services.AddSingleton<_>(implementationFactory = fun _ -> Aggregate.Stateless.create)

        builder.Services.AddSingleton<_>(
            implementationFactory =
                fun svp ->
                    let useCase =
                        svp.GetRequiredService<Aggregate.Stateless.UseCase<State, Event, Command>>()

                    CommandControllers.Stateless.create useCase
        )

        let app = builder.Build()

        app.UseSwagger()
        app.UseSwaggerUI()
        app.UseAuthorization()
        app.MapControllers()
        app.MapRazorPages()

        app.Run()

        exitCode
