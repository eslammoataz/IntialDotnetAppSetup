FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["TestingProjectSetup.Domain/TestingProjectSetup.Domain.csproj", "TestingProjectSetup.Domain/"]
COPY ["TestingProjectSetup.Application/TestingProjectSetup.Application.csproj", "TestingProjectSetup.Application/"]
COPY ["TestingProjectSetup.Infrastructure/TestingProjectSetup.Infrastructure.csproj", "TestingProjectSetup.Infrastructure/"]
COPY ["TestingProjectSetup.Api/TestingProjectSetup.Api.csproj", "TestingProjectSetup.Api/"]

# Restore
RUN dotnet restore "TestingProjectSetup.Api/TestingProjectSetup.Api.csproj"

# Copy everything and build
COPY . .
WORKDIR "/src/TestingProjectSetup.Api"
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestingProjectSetup.Api.dll"]
