# Leave Management System

## Overview

This is a Leave Management System built using ASP.NET Core. It allows employees to submit leave requests which can be reviewed and approved by managers. 
The application uses Docker for easy setup and deployment.

## Features

- Employee can create leave requests.
- Leave requests are validated based on leave type.
- A simple API for managing leave requests.

## Setup Instructions

### Prerequisites

- **Docker**: Ensure Docker is installed on your machine.
- **.NET SDK**: Ensure .NET SDK 7.0 or later is installed.

### Steps to Run Locally

1. **Clone the repository**:
    ```bash
    git clone https://github.com/MayssaBenOthmane/LeaveManagementSystem.git
    cd LeaveManagementSystem
    ```

2. **Build and Run with Docker**:
    - Run the following command to build and start the Docker containers:
      ```bash
      docker-compose up --build
      ```

3. **Access the API**:
    - Once the containers are up, the API will be accessible at `http://localhost:5000`.

4. **Run Without Docker (Optional)**:
    - Open the project in Visual Studio or any preferred IDE.
    - Run the project using the built-in web server.

### Testing the API with Postman

You can use Postman or any API client to test the following endpoints:

- **GET** `/api/LeaveRequests`: Retrieve all leave requests.
- **POST** `/api/LeaveRequests`: Create a new leave request.

## Docker Setup

The project includes a `Dockerfile` for building the application in a containerized environment. 
The `docker-compose.yml` file is used to set up both the application and the SQLite database.

## Conclusion
This simple Leave Management System allows employees to request leaves and managers to approve them. 
Docker makes the setup easy and ensures consistency across different environments.
