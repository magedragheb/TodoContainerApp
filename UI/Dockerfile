FROM mcr.microsoft.com/dotnet/sdk:latest AS build
WORKDIR /src

COPY UI.csproj .
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM nginx:alpine AS runtime
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY nginx.conf /etc/nginx/nginx.conf

EXPOSE 80

CMD ["/bin/sh", "-c", "nginx -g \"daemon off;\""]