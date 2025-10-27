FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем ВСЕ файлы проекта
COPY . .

# Восстанавливаем зависимости основного проекта
RUN dotnet restore "src/GamePulse.Web/GamePulse.Web.csproj"

# Собираем проект в режиме Debug
RUN dotnet build "src/GamePulse.Web/GamePulse.Web.csproj" -c Debug -o /app/build

# Публикуем проект
FROM build AS publish
RUN dotnet publish "src/GamePulse.Web/GamePulse.Web.csproj" -c Debug -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GamePulse.Web.dll"]