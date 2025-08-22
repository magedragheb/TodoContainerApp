FROM mcr.microsoft.com/dotnet/sdk:latest AS build
WORKDIR /src

COPY TodoBlazorServer.csproj .
RUN dotnet restore

COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0-noble-chiseled-extra AS runtime
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TodoBlazorServer.dll"]