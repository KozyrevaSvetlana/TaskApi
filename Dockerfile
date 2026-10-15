FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Restore first so the layer is cached until a .csproj changes
COPY Directory.Build.props ./
COPY src/TaskApi.Data/TaskApi.Data.csproj src/TaskApi.Data/
COPY src/TaskApi/TaskApi.csproj src/TaskApi/
RUN dotnet restore src/TaskApi/TaskApi.csproj

COPY src/ src/
RUN dotnet publish src/TaskApi/TaskApi.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
USER $APP_UID

# exec form => dotnet is PID 1 and receives SIGTERM (graceful shutdown)
ENTRYPOINT ["dotnet", "TaskApi.dll"]
