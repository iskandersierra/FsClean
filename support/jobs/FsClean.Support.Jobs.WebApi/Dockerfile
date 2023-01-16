#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["support/jobs/FsClean.Support.Jobs.WebApi/FsClean.Support.Jobs.WebApi.fsproj", "support/jobs/FsClean.Support.Jobs.WebApi/"]
RUN dotnet restore "support/jobs/FsClean.Support.Jobs.WebApi/FsClean.Support.Jobs.WebApi.fsproj"
COPY . .
WORKDIR "/src/support/jobs/FsClean.Support.Jobs.WebApi"
RUN dotnet build "FsClean.Support.Jobs.WebApi.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FsClean.Support.Jobs.WebApi.fsproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FsClean.Support.Jobs.WebApi.dll"]