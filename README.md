# ChitMeo

**ChitMeo** is a modular ASP.NET Core platform designed to build independent products by composing reusable modules.

The project focuses on a **lightweight modular architecture** where each module encapsulates its own business logic, CQRS handlers, infrastructure, and optionally its own database. The main host application acts only as a runtime container responsible for bootstrapping the system and loading modules.

## Goals

* Build applications by **composing independent modules**
* Keep the **WebHost free of business logic**
* Allow modules to operate **independently and loosely coupled**
* Support **separate databases per module**
* Enable future **microservice extraction**
* Keep the architecture **simple and lightweight**

## Architecture Overview

```
WebHost
   │
   ├── Modules
   │    ├── Auth
   │    ├── Blog
   │    └── Accounting
   │
   └── Shared
        └── Abstractions
```

### WebHost

The host application is responsible for:

* Bootstrapping ASP.NET Core
* Loading modules
* Dependency Injection
* Routing and middleware
* Application configuration

It does **not contain business logic**.

### Modules

Each module is a self-contained unit that may include:

* Domain models
* Commands and Queries (CQRS)
* Handlers
* Infrastructure
* API endpoints
* Database context

Modules are designed to be **independent and loosely coupled**.

Examples:

```
ChitMeo.Auth
ChitMeo.Blog
ChitMeo.Accounting
```

### Shared Abstractions

Shared contracts used across modules:

* Interfaces
* Abstract classes
* Event contracts
* Common service contracts

This layer prevents modules from referencing each other directly.

## CQRS and Mediator

ChitMeo uses a **custom lightweight mediator** implementation that:

* Automatically discovers handlers inside module assemblies
* Registers them through dependency injection
* Keeps CQRS logic contained within each module

This allows each module to maintain its own **vertical slice architecture**.

## Example Product Composition

Modules can be combined to form products:

```
Blog Product
 ├── Auth Module
 └── Blog Module

Accounting Product
 ├── Auth Module
 └── Accounting Module
```

## Design Principles

* Modular Monolith architecture
* Low coupling between modules
* Clear dependency boundaries
* Independent module development
* Simple infrastructure with minimal framework overhead

## Status

This project is currently under development as an experimental modular platform.

## License

MIT
