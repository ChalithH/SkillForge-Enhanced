# Production Dockerfile for .NET 8 with multi-stage build
# Stage 1: Build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies first (better layer caching)
COPY *.csproj ./
RUN dotnet restore

# Copy source code
COPY . .

# Build the application in Release mode
RUN dotnet build -c Release -o /app/build

# Stage 2: Publish the application
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish --no-restore

# Stage 3: Runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Create non-root user for security
RUN groupadd -r skillforge && useradd -r -g skillforge skillforge

# Create uploads directory and set permissions
RUN mkdir -p /app/uploads && chown -R skillforge:skillforge /app

# Copy published application
COPY --from=publish /app/publish .

# Set ownership to non-root user
RUN chown -R skillforge:skillforge /app

# Switch to non-root user
USER skillforge

# Expose port
EXPOSE 5000

# Configure environment for production
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5000

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
  CMD curl -f http://localhost:5000/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "SkillForge.Api.dll"]