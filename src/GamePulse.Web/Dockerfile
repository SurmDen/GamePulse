FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["GamePulse.Web/GamePulse.Web.csproj", "GamePulse.Web/"]
COPY ["GamePulse.Infrastructure/GamePulse.Infrastructure.csproj", "GamePulse.Infrastructure/"]
COPY ["GamePulse.Application/GamePulse.Application.csproj", "GamePulse.Application/"]
COPY ["GamePulse.Domain/GamePulse.Domain.csproj", "GamePulse.Domain/"]

RUN dotnet restore "GamePulse.Web/GamePulse.Web.csproj"

COPY . .

WORKDIR "/src/GamePulse.Web"
RUN dotnet build "GamePulse.Web.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "GamePulse.Web.csproj" -c Debug -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GamePulse.Web.dll"]