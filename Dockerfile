# Use the official .NET SDK image to build and publish the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything and restore
COPY . . 
RUN dotnet restore

# Build and publish the release version
RUN dotnet publish -c Release -o out

# Use the runtime image for a smaller final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "Julfiker_Portfolio.dll"]
