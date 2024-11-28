FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./Justwish.Users.Domain/Justwish.Users.Domain.csproj", "./Justwish.Users.Domain/Justwish.Users.Domain.csproj"]
RUN dotnet restore "Justwish.Users.Domain/Justwish.Users.Domain.csproj"
COPY ["./Justwish.Users.Contracts/Justwish.Users.Contracts.csproj", "./Justwish.Users.Contracts/Justwish.Users.Contracts.csproj"]
RUN dotnet restore "Justwish.Users.Contracts/Justwish.Users.Contracts.csproj"
COPY ["./Justwish.Users.Application/Justwish.Users.Application.csproj", "./Justwish.Users.Application/Justwish.Users.Application.csproj"]
RUN dotnet restore "Justwish.Users.Application/Justwish.Users.Application.csproj"
COPY ["./Justwish.Users.Infrastructure/Justwish.Users.Infrastructure.csproj", "./Justwish.Users.Infrastructure/Justwish.Users.Infrastructure.csproj"]
RUN dotnet restore "Justwish.Users.Infrastructure/Justwish.Users.Infrastructure.csproj"
COPY ["./Justwish.Users.WebApi/Justwish.Users.WebApi.csproj", "./Justwish.Users.WebApi/Justwish.Users.WebApi.csproj"]
RUN dotnet restore "Justwish.Users.WebApi/Justwish.Users.WebApi.csproj"
COPY [".", "."]
WORKDIR "/src/Justwish.Users.WebApi"
RUN dotnet build "Justwish.Users.WebApi.csproj" -c $BUILD_CONFIGURATION

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Justwish.Users.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Justwish.Users.WebApi.dll"]
