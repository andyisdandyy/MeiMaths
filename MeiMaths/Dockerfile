# Multi-stage build — .NET SDK bruges kun til build, runtime i final image
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Restore (cachet lag)
COPY *.slnx ./
COPY MeiMaths/MeiMaths.csproj MeiMaths/
RUN dotnet restore MeiMaths/MeiMaths.csproj

# Build + publish
COPY . .
WORKDIR /src/MeiMaths
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime image — kun ASP.NET runtime, ingen SDK
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Opret data-mappe til SQLite database (mountes som volume)
RUN mkdir -p /app/data

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 80

ENTRYPOINT ["dotnet", "MeiMaths.dll"]
