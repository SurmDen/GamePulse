# 🎮 GamePulse - Аналитическая платформа игровых релизов

<div align="center">

![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0-purple.svg)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-blue.svg)
![Docker](https://img.shields.io/badge/Docker-Ready-blue.svg)
![Kafka](https://img.shields.io/badge/Apache_Kafka-Streaming-orange.svg)
![CQRS](https://img.shields.io/badge/Architecture-CQRS-green.svg)
![JWT](https://img.shields.io/badge/Auth-JWT-orange.svg)

**Мощная аналитическая платформа для сбора и анализа данных об игровых релизах**

*Всегда в курсе игровых трендов! 🚀*

</div>

## 📖 О проекте

**GamePulse** — это комплексное решение для аналитики игровой индустрии, предоставляющее актуальную информацию о релизах, трендах и статистике игр. Платформа объединяет данные из различных источников, включая Steam Store, Steam API, и предоставляет удобный API для разработчиков, аналитиков и геймеров.

### 🌟 Ключевые преимущества

- 🕒 **Актуальные данные** - Ежедневное обновление информации о релизах
- 🔍 **Глубокая аналитика** - Детальная статистика и трендовый анализ
- 🛡️ **Безопасность** - JWT авторизация с ролевой моделью
- 🐳 **Простота развертывания** - Полная контейнеризация через Docker Compose
- ⚡ **Асинхронная обработка** - Apache Kafka для обработки событий
- 📊 **Удобный API** - RESTful архитектура с полной документацией

## 🏗️ Архитектура системы

### Технологический стек

| Компонент | Технологии |
|-----------|-------------|
| **Backend** | ASP.NET Core 8, Entity Framework Core |
| **База данных** | PostgreSQL с расширениями для JSON |
| **Аутентификация** | JWT Bearer Tokens, ASP.NET Core Identity |
| **Архитектура** | CQRS, Mediator Pattern (MediatR) |
| **Очереди сообщений** | Apache Kafka, Confluent.Kafka |
| **Парсинг данных** | AngleSharp, HttpClient, Polly |
| **Контейнеризация** | Docker, Docker Compose |
| **Логирование** | Serilog с структурированным выводом |
| **Кэширование** | Distributed Redis Cache |
| **Документация** | Swagger/OpenAPI 8.0 |

## 📡 API Эндпоинты

### 🔐 User Controller

| Метод | Эндпоинт | Описание | Аутентификация |
|-------|----------|-----------|----------------|
| `POST` | `/api/v1/auth/register` | Регистрация нового пользователя | ❌ |
| `POST` | `/api/v1/auth/login` | Вход в систему и получение JWT токена | ❌ |
| `GET` | `/api/v1/auth/logout` | Удаление JWT токена | ✅ |


### 🎯 Games Controller

| Метод | Эндпоинт | Описание | Аутентификация |
|-------|----------|-----------|----------------|
| `POST` | `/api/v1/games/load/{month}` | Отправка события в брокер сообщений на поиск релизов игр с дальнешим сохранением в БД | ✅ |
| `GET` | `/api/v1/games/calendar?month=12&tag_id=123&platform=linux` | Получение календаря релизов по параметрам | ✅ |
| `GET` | `/api/v1/games?month=11&tag_id=321platform=mac` | Получение списка игр по параметрам | ✅ |


### 📅 Genres Controller

| Метод | Эндпоинт | Описание | Аутентификация |
|-------|----------|-----------|----------------|
| `GET` | `/api/v1/genres/{count}` | Получение топ-{count} жанров | ✅ |
| `GET` | `/api/v1/genres/statistics?year=2025&month=12&months_count=3&genres_count=5` | Получение статистики по топ-{genres_count} жанрам, за {months_count} месяцев начиная с {year}/{month} | ✅ |


## 🐳 Запуск через Docker Compose

### Быстрый старт (рекомендуется)

```
# Клонирование репозитория
git clone https://github.com/SurmDen/GamePulse.git
cd GamePulse/Docker

# Запуск всех сервисов (приложение + Kafka + PostgreSQL)
docker-compose up -d
```
### 🚀 Первые шаги после запуска

1. **Откройте Swagger UI в браузере:**
[http://localhost:5135/swagger/index.html](http://localhost:5135/swagger/index.html)

2. **Создайте аккаунт или войдите под тестовым пользователем:**
- Email: `surm@den`
- Password: `123456`

3. **Авторизация в Swagger:**
- Нажмите кнопку "Authorize" в правом верхнем углу
- Вставьте полученный JWT токен в формате:
  ```
  Bearer ваш_токен_здесь
  ```
- Swagger автоматически добавит токен в заголовки всех запросов

4. **Начните использовать API:**
- Теперь все защищенные эндпоинты доступны для тестирования
- Токен автоматически передается в header `Authorization` каждого запроса
