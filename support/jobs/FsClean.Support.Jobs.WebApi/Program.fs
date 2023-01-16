namespace FsClean.Support.Jobs.WebApi

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
            info.Title <- "Support.Jobs.WebApi"
            info.Version <- "v1"
            options.SwaggerDoc(info.Version, info))

        let app = builder.Build()

        app.UseSwagger()
        app.UseSwaggerUI()
        app.UseAuthorization()
        app.MapControllers()
        app.MapRazorPages()

        app.Run()

        exitCode
