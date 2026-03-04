FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY dotnet-api.sln .
COPY B2BPlatform.API/B2BPlatform.API.csproj B2BPlatform.API/
COPY B2BPlatform.Shared/B2BPlatform.Shared.csproj B2BPlatform.Shared/
COPY B2BPlatform.Infrastructure/B2BPlatform.Infrastructure.csproj B2BPlatform.Infrastructure/
COPY B2BPlatform.Infrastructure.IoC/B2BPlatform.Infrastructure.IoC.csproj B2BPlatform.Infrastructure.IoC/
COPY B2BPlatform.Services.Authentications/B2BPlatform.Services.Authentications.csproj B2BPlatform.Services.Authentications/
COPY B2BPlatform.Services.BusinessUnit/B2BPlatform.Services.BusinessUnit.csproj B2BPlatform.Services.BusinessUnit/
COPY B2BPlatform.Services.Chats/B2BPlatform.Services.Chats.csproj B2BPlatform.Services.Chats/
COPY B2BPlatform.Services.Company/B2BPlatform.Services.Company.csproj B2BPlatform.Services.Company/
COPY B2BPlatform.Services.CompanyOwner/B2BPlatform.Services.CompanyOwner.csproj B2BPlatform.Services.CompanyOwner/
COPY B2BPlatform.Services.Employees/B2BPlatform.Services.Employees.csproj B2BPlatform.Services.Employees/
RUN dotnet restore dotnet-api.sln
COPY . .
RUN dotnet publish B2BPlatform.API/B2BPlatform.API.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "B2BPlatform.API.dll"]
