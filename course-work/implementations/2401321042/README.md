# Eventify — Event Management System

**Факултетен номер:** 2401321042  
**Студент:** Шефкет Молаали

---

## 📋 Описание

**Eventify** е уеб базирана система за управление на събития, разработена с ASP.NET Core MVC. Платформата позволява на организатори да създават и управляват събития, а на потребители да се регистрират за тях.

### Функционалности:
- 🎪 Преглед на предстоящи събития с карти на началната страница
- 🎫 Регистрация за събития с автоматично изчисляване на цена
- 👥 Система от роли — Admin, Organizer, Attendee
- 🔐 Автентикация с BCrypt хеширане на пароли и Cookie сесии
- 📁 Качване на снимки за събития
- 🔍 Филтриране, сортиране и пагинация навсякъде
- ⚠️ Централизирана обработка на грешки (RFC 7807)

---

## 🗄️ Структура на базата данни

| Таблица | Описание |
|---------|----------|
| `Users` | Потребители с роли (Admin, Organizer, Attendee) |
| `Venues` | Места за провеждане на събития |
| `Events` | Основна таблица със събития |
| `Categories` | Категории за класификация на събития |
| `EventCategories` | Връзка много-към-много между Events и Categories |
| `Registrations` | Регистрации на потребители за събития |

---

## 🛠️ Технологии

- **Back-end:** ASP.NET Core MVC (.NET 8)
- **База данни:** SQL Server (LocalDB)
- **ORM:** Entity Framework Core
- **Автентикация:** Cookie Authentication + BCrypt.Net
- **Front-end:** Razor Views + Bootstrap 5
- **Валидация:** DataAnnotations

---

## 🚀 Инсталация и стартиране

### Изисквания
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) или [VS Code](https://code.visualstudio.com/)
- SQL Server LocalDB (идва с Visual Studio)

### Стъпки

**1. Клонирай репозиторието:**
```bash
git clone https://github.com/YOUR_USERNAME/YOUR_REPO.git
cd YOUR_REPO/course-work/implementations/2401321042
```

**2. Инсталирай зависимостите:**
```bash
dotnet restore
```

**3. Приложи миграциите (създава базата автоматично):**
```bash
dotnet ef database update
```

**4. Стартирай проекта:**
```bash
dotnet run
```

Или отвори `WebApplication6.sln` във Visual Studio и натисни **F5**.

**5. Отвори браузър на:**
```
https://localhost:7068
```

---

## 👤 Тестови акаунти

При първо стартиране seed данните се зареждат автоматично:

| Роля | Username | Password |
|------|----------|----------|
| Admin | `admin` | `Admin123!` |
| Organizer | `organizer` | `Organizer123!` |
| Attendee | `attendee` | `Attendee123!` |

---

## 📁 Структура на проекта

```
WebApplication6/
├── Controllers/          # MVC контролери
│   ├── AuthController.cs
│   ├── EventsController.cs
│   ├── VenuesController.cs
│   ├── RegistrationsController.cs
│   ├── CategoriesController.cs
│   ├── EventCategoriesController.cs
│   └── UsersController.cs
├── Models/               # Модели на базата данни
├── Views/                # Razor изгледи
├── Data/                 # DbContext и Seed данни
├── Middleware/           # Global Exception Handling
└── wwwroot/              # Статични файлове и uploads
```

---

## ✅ Изпълнени изисквания

- [x] RESTful уеб услуги (ASP.NET Core MVC)
- [x] Минимум 3 свързани таблици (6 таблици)
- [x] Минимум 6 колони на таблица с различни типове данни
- [x] Пълен CRUD за всеки модел
- [x] Сигурност — Cookie Authentication + BCrypt
- [x] Валидация — DataAnnotations на модели и форми
- [x] Филтриране по минимум 2 критерия
- [x] Пагинация и сортиране
- [x] Global Exception Handling (RFC 7807)
- [x] Асинхронност — async/await навсякъде
- [x] **Бонус:** Качване на снимки
- [x] **Бонус:** Система от роли (Admin/Organizer/Attendee)
- [x] **Бонус:** Seed данни
