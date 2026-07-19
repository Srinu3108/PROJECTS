FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY EnergyPulse.Api.csproj .
RUN dotnet restore

# Copy all files and build
COPY . .
RUN dotnet publish -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

EXPOSE 5015
ENV ASPNETCORE_URLS="http://0.0.0.0:5015"
ENTRYPOINT ["dotnet", "EnergyPulse.Api.dll"]