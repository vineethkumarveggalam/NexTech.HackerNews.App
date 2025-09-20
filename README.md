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

This project was generated with Angular CLI version 18.2.21.

## Prerequisites

* Node.js version 20.19.5 (Make sure you have this or a compatible version installed)
* Angular CLI version 18.2.21 installed globally 

To check installed versions, run:
`node -v`
`ng version`

If Angular CLI is not installed globally, you can install it using:
`npm install -g @angular/cli@18.2.21`

## Development server

Run the following command to start the development server:
`ng serve`

Navigate to [http://localhost:4200/](http://localhost:4200/). The application will automatically reload if you change any of the source files.

## Code scaffolding

To generate new components or other Angular features, use:
`ng generate component component-name`

You can also generate directives, pipes, services, classes, guards, interfaces, enums, and modules by replacing `component` with the desired schematic, like:
`ng generate directive|pipe|service|class|guard|interface|enum|module`

## Build

To build the project for production, run:
`ng build`

The build artifacts will be stored in the `dist/` directory.

## Running unit tests

Run unit tests using Karma with:
`ng test`

## Additional setup (if needed)

* Make sure to install all project dependencies before running the app:
  `npm install`

* If you want to run end-to-end tests, you can use:
  `ng e2e`

## Further help

For more help on Angular CLI, use:
`ng help`

or visit the Angular CLI Overview and Command Reference page:
[https://angular.dev/tools/cli](https://angular.dev/tools/cli)


