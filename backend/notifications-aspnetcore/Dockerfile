FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./Justwish.Notifications.Domain/Justwish.Notifications.Domain.csproj", "./Justwish.Notifications.Domain/Justwish.Notifications.Domain.csproj"]
RUN dotnet restore "Justwish.Notifications.Domain/Justwish.Notifications.Domain.csproj"
COPY ["./Justwish.Notifications.Contracts/Justwish.Notifications.Contracts.csproj", "./Justwish.Notifications.Contracts/Justwish.Notifications.Contracts.csproj"]
RUN dotnet restore "Justwish.Notifications.Contracts/Justwish.Notifications.Contracts.csproj"
COPY ["./Justwish.Notifications.Application/Justwish.Notifications.Application.csproj", "./Justwish.Notifications.Application/Justwish.Notifications.Application.csproj"]
RUN dotnet restore "Justwish.Notifications.Application/Justwish.Notifications.Application.csproj"
COPY ["./Justwish.Notifications.Infrastructure/Justwish.Notifications.Infrastructure.csproj", "./Justwish.Notifications.Infrastructure/Justwish.Notifications.Infrastructure.csproj"]
RUN dotnet restore "Justwish.Notifications.Infrastructure/Justwish.Notifications.Infrastructure.csproj"
COPY ["./Justwish.Notifications.WebApi/Justwish.Notifications.WebApi.csproj", "./Justwish.Notifications.WebApi/Justwish.Notifications.WebApi.csproj"]
RUN dotnet restore "Justwish.Notifications.WebApi/Justwish.Notifications.WebApi.csproj"
COPY [".", "."]
WORKDIR "/src/Justwish.Notifications.WebApi"
RUN dotnet build "Justwish.Notifications.WebApi.csproj" -c $BUILD_CONFIGURATION

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Justwish.Notifications.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Justwish.Notifications.WebApi.dll"]
