# 📚 BookStore Web Application

A full-stack Book Store application built with **ASP.NET Core (Web API)** and **React.js (Vite)**, featuring secure user login, book management, and RESTful API integration.

---

## 🔧 Technologies Used

### Backend
- ASP.NET Core 8
- Entity Framework Core
- SQL Server
- JWT Authentication
- xUnit for Unit Testing

### Frontend
- React + Vite
- Bootstrap 5
- React Router
- Fetch API

---

## ✨ Features

- 🔐 **Login with JWT Authentication**
- 📖 **Book Dashboard** with:
  - View all books
  - Search books
  - Add new book
  - Update existing book
  - Delete book
- ✅ Unit tests with 80%+ code coverage
- 🌐 HTTPS support using self-signed certificates

---

## 🚀 Getting Started

### 🖥️ Backend Setup (ASP.NET Core)

```bash
cd book-store-backend/BookStoreAPI
dotnet restore
dotnet ef database update
dotnet run
