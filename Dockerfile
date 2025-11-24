# Use .NET 10 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["PulseGroup/PulseGroup.csproj", "PulseGroup/"]
RUN dotnet restore "PulseGroup/PulseGroup.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/PulseGroup"
RUN dotnet build "PulseGroup.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "PulseGroup.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use .NET 10 runtime for the final image
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app

# Set environment variables for UTF-8 support
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV LC_ALL=ru_RU.UTF-8
ENV LANG=ru_RU.UTF-8

# Copy published app
COPY --from=publish /app/publish .

# Create volume for persistent data (config and statistics)
VOLUME ["/app/data"]

# Run the application
ENTRYPOINT ["dotnet", "PulseGroup.dll"]
