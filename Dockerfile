# STAGE 1: Build the project
# ---------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copy all files
COPY . ./

# Restore dependencies
RUN dotnet restore ./oep/OEP.csproj

# Build the project in Release mode
RUN dotnet publish ./oep/OEP.csproj -c Release -o /app/publish

# ---------------------------
# STAGE 2: Run the app
# ---------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

# Copy the build output
COPY --from=build /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port 8080 for Render or Docker
EXPOSE 8080

# Start the app
ENTRYPOINT ["dotnet", "OEP.dll"]
