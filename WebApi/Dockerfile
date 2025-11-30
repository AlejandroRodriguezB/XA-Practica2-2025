FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
RUN apt-get update && apt-get install -y --no-install-recommends wget && rm -rf /var/lib/apt/lists/*
RUN useradd -m appuser
COPY --from=build /app/publish .
RUN chown -R appuser:appuser /app

RUN mkdir -p /keys && chown -R appuser:appuser /keys

USER appuser
EXPOSE 8080
CMD ["dotnet", "WebApi.dll"]