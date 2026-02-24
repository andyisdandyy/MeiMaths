# Multi-stage build — .NET SDK bruges kun til build, runtime i final image
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Restore (cachet lag)
# Copy project file directly and restore.
# This Dockerfile assumes you build with the project folder as the build context
# (e.g. `docker build -t meimaths ./MeiMaths`). In that case the csproj is at
# the context root and we can copy it directly.
COPY ["MeiMaths.csproj", "./"]
RUN dotnet restore "MeiMaths.csproj"

# Build + publish
COPY . .
# Build from the project root in the container. When building with the project
# folder as the build context (e.g. `docker build ./MeiMaths`), the csproj and
# sources are at /src — publish the specific project file from that location.
WORKDIR /src
RUN dotnet publish "MeiMaths.csproj" -c Release -o /app/publish /p:UseAppHost=false

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
