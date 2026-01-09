# Travel Journal API

REST API для управления журналом путешествий на ASP.NET Core 8.0.

## Технологии

- ASP.NET Core 8.0 Web API
- PostgreSQL 16
- Redis 7
- Docker Compose
- Entity Framework Core + Dapper
- JWT Bearer + API Key аутентификация
- Swagger/OpenAPI
- Prometheus метрики
- Health Checks

## Структура проекта

```
WebApplication1/
├── Controllers/          # HTTP контроллеры
├── Services/            # Бизнес-логика
├── Repositories/        # Доступ к данным (EF Core + Dapper)
├── Models/
│   ├── Entities/        # Сущности БД
│   └── DTO/             # Data Transfer Objects
├── Data/                # DbContext
├── Middleware/          # Промежуточное ПО
├── Validators/          # Валидация
└── Liquibase/           # Миграции БД
```

## Запуск

### Требования
- Docker и Docker Compose

### Команды

```bash
# Запуск всех сервисов
sudo docker-compose up -d

# Проверка статуса
sudo docker-compose ps

# Просмотр логов
sudo docker-compose logs api

# Остановка
sudo docker-compose down
```

### Доступ

- **Swagger UI**: http://localhost:5000/swagger
- **Health Check**: http://localhost:5000/health
- **Metrics**: http://localhost:5000/metrics

## Аутентификация

### JWT Bearer

1. Регистрация:
```json
POST /api/Auth/register
{
  "username": "user1",
  "email": "user1@example.com",
  "password": "Password123!",
  "role": "Admin"
}
```


2. Использование токена:
```
Authorization: Bearer YOUR_JWT_TOKEN
```

### API Key

```
X-API-KEY: test-api-key-12345
```

### Роли

| Роль | Права |
|------|-------|
| **Admin** | Полный доступ |
| **Manager** | Чтение, создание, обновление (без удаления) |
| **User** | Только свои ресурсы |

**Предустановленный пользователь:**
- `admin` / `Admin123!`

## API Endpoints

### Аутентификация
- `POST /api/Auth/register` - Регистрация
- `POST /api/Auth/login` - Вход

### Поездки (Trips)
- `GET /api/Trips` - Список (пагинация, фильтрация)
- `GET /api/Trips/{id}` - Получить по ID
- `POST /api/Trips` - Создать
- `PUT /api/Trips/{id}` - Обновить
- `DELETE /api/Trips/{id}` - Удалить

### Журналы (Travel Journals)
- `GET /api/TravelJournals` - Список
- `GET /api/TravelJournals/{id}` - Получить по ID
- `POST /api/TravelJournals` - Создать
- `PUT /api/TravelJournals/{id}` - Обновить
- `DELETE /api/TravelJournals/{id}` - Удалить

### Локации (Locations)
- `GET /api/Locations` - Список
- `GET /api/Locations/{id}` - Получить по ID
- `POST /api/Locations` - Создать (Admin/Manager)
- `PUT /api/Locations/{id}` - Обновить (Admin/Manager)
- `DELETE /api/Locations/{id}` - Удалить (Admin)

### Теги (Tags)
- `GET /api/Tags` - Список

## Примеры запросов

### Регистрация с ролью Admin
```json
POST /api/Auth/register
{
  "username": "new_admin",
  "email": "admin@example.com",
  "password": "AdminPass123!",
  "role": "Admin"
}
```

### Создание журнала
```json
POST /api/TravelJournals
Authorization: Bearer YOUR_TOKEN
{
  "title": "Мои путешествия",
  "description": "Описание журнала"
}
```

### Создание поездки
```json
POST /api/Trips
Authorization: Bearer YOUR_TOKEN
{
  "travelJournalId": 1,
  "locationId": 1,
  "title": "Поездка в Париж",
  "description": "Описание поездки",
  "startDate": "2024-06-01",
  "endDate": "2024-06-07",
  "rating": 5,
  "photoUrls": ["https://example.com/photo4.jpg"],
  "tagIds": [1, 3]
}
```

### Получение поездок с фильтрацией
```
GET /api/Trips?page=1&pageSize=10&search=Paris&minRating=4&locationId=1
Authorization: Bearer YOUR_TOKEN
```

## Предустановленные данные

После запуска в БД доступны:

**Локации:**
- Эйфелева башня (Париж, Франция)
- Биг-Бен (Лондон, Великобритания)
- Колизей (Рим, Италия)
- Саграда Фамилия (Барселона, Испания)
- Акрополь (Афины, Греция)

**Теги:**
- Приключения, Отдых, Культура, Природа, Еда, История

