FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o /app

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app ./

# Create directory for SQLite database
RUN mkdir -p /app/Data
VOLUME /app/Data

EXPOSE 80
ENTRYPOINT ["dotnet", "MyApi.dll"] 