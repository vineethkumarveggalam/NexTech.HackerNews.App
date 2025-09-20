# HackerNews API and Client Application

This project consists of two main parts:
- **HackerNews.API**: The backend API for fetching and serving Hacker News stories.
- **Client**: The Angular frontend that displays Hacker News stories and allows searching, paging, and viewing article details.

## Requirements

### Prerequisites

- **HackerNews.API**:
  - .NET 8 SDK and runtime installed.
  - A code editor such as Visual Studio or Visual Studio Code.

- **Client**:
  - Node.js version 20.19.5.
  - Angular CLI version 18.2.21 installed globally 

### Dependencies

- **HackerNews.API**:
  - Caching mechanism for storing the newest stories.
  - Usage of Dependency Injection (DI) for services.
  - Automated tests to ensure the API works correctly.

- **Client**:
  - Angular 18.2.21.

## Setup Instructions

### 1. Clone the Repository

Clone the repository from GitHub:

```bash
git clone https://github.com/vineethkumarveggalam/NexTech.HackerNews.Core
cd HackerNews ```

### Run HackerNews.API
Open the API project in visual studio and run the Application

OR 

You can also use following commands to run API project

1. dotnet restore
2. dotnet build
3. dotnet run

## To run Test Cases
dotnet test

Once the server is running, the frontend will be available at http://localhost:4200.

