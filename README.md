# ASP.NET Travel Site – Core MVC Application

**Live Application:**  
https://cis-iis2.temple.edu/Fall2025/CIS3342_tuo46790/

A full-featured travel booking web application developed using **ASP.NET Core MVC** that allows users to search, book, and manage travel services including hotels, flights, rental cars, events, and short-term rentals across multiple cities.

This project was built as part of CIS 3342 (Server-Side Web Application Development) and demonstrates full-stack development, RESTful API integration, secure data handling, and scalable system design.

---

## Table of Contents
- Overview
- Key Features
- Application Workflow
- Homepage Overview
- Architecture
- Web API Integration
- Database Design
- Security Features
- Tech Stack
- Deployment
- Author

---

## Overview

The ASP.NET Travel Site is designed to simulate a real-world travel booking platform similar to Expedia or Priceline. Users can browse available travel options, build a customized vacation package step-by-step, and manage their selections through a centralized **My Vacation** workflow.

The system supports both **guest users** and **registered users**, offering enhanced features such as saved payment methods, profile management, and personalized dashboards for authenticated users.

---

## Key Features

### Travel Booking Modules
- Hotel and room search with availability and pricing
- Flight search and seat reservation
- Rental car selection
- Event and attraction booking
- Short-term rental integration (Zillow-style listings)

### User Experience
- Guest users can browse and temporarily save selections
- Registered users can:
  - Create and manage accounts
  - Save encrypted payment methods
  - View and modify vacation packages
  - Navigate securely using session-based authentication

### Data & Pricing
- Centralized pricing calculation across all modules
- Shared vacation totals updated dynamically
- SQL Server–backed data consistency

---

## Application Workflow

1. User visits the homepage and selects a destination city
2. User searches for hotels, flights, cars, events, or rentals
3. Each selection is added to the **My Vacation** session
4. Pricing is recalculated after each addition
5. Registered users can proceed to checkout with saved or new payment methods
6. Reservations are finalized and persisted in the database

---

## Homepage Overview

The homepage serves as the primary entry point for the application and provides:
- City-based navigation for travel searches
- Access to all booking modules
- Clear call-to-action buttons for browsing hotels or rentals
- A responsive UI built with Bootstrap for desktop and mobile users

Live Homepage:  
https://cis-iis2.temple.edu/Fall2025/CIS3342_tuo46790/Termproject/Account/Login
---

## Architecture

The application follows the **ASP.NET Core MVC pattern**:
- **Models** define business entities such as Hotel, Room, Flight, Car, Event, and Payment
- **Views** use Razor syntax for dynamic UI rendering
- **Controllers** handle routing, session state, and business logic

The architecture cleanly separates concerns and supports scalability and maintainability.

---

## Web API Integration

A custom **ASP.NET Web API** is used for the Hotels module to allow decoupled data access and reuse across applications.

### API Capabilities
- `GetHotels`
- `GetRoomsByHotel`
- `FindRooms`
- `Reserve`

### Benefits
- Secure, reusable endpoints
- Centralized hotel logic
- DataSets returned with pricing used across multiple booking modules

---

## Database Design

- SQL Server database with normalized relational tables
- Stored procedures used for:
  - Searching availability
  - Reserving inventory
  - Retrieving user-specific data
- Business rules and ER diagrams designed using Oracle Data Modeler

This approach improves performance, security, and maintainability.

---

## Security Features

- **Session-based authentication** for protected routes
- **AES encryption** for storing payment methods securely
- **CAPTCHA verification** implemented using a custom View Component
  - Prevents automated login and registration attempts
  - Generates randomized verification codes per request
- Sensitive configuration files excluded via `.gitignore`

---

## Tech Stack

### Backend
- ASP.NET Core MVC
- C#
- RESTful Web APIs
- SQL Server
- Stored Procedures

### Frontend
- Razor Views
- HTML5
- CSS3
- JavaScript
- Bootstrap

### Tools & Utilities
- Visual Studio
- Git & GitHub
- Oracle Data Modeler
- Session Management
- Encryption Libraries

---

## Deployment

- Hosted on Temple University IIS server
- Published using Visual Studio
- Configured for live database connections
- Optimized for production environment deployment

---

## Author

**Thakib Aroworowon**  
B.S. Information Science & Technology  
Temple University  
GitHub: https://github.com/Thakib123  
LinkedIn: https://www.linkedin.com/in/Thakib

---

## Notes

This project demonstrates full-stack development skills, secure system design, and real-world application architecture suitable for enterprise-scale web systems.
