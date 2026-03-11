# ChitMeo

**ChitMeo** is a lightweight modular platform for building applications with **independent modules** on ASP.NET Core.

Instead of building a single monolithic application, ChitMeo allows you to compose products by combining reusable modules such as **Auth**, **Blog**, or **Accounting**.

The main host application acts only as a **runtime container** responsible for bootstrapping the system and loading modules.

---

## ✨ Key Ideas

* Modular architecture
* Independent modules
* Custom lightweight mediator for CQRS
* WebHost without business logic
* Modules can use separate databases
* Easy evolution to microservices

---

## 🧩 Architecture

```
src
 ├── ChitMeo.Host
 │
 ├── Modules
 │   ├── ChitMeo.Auth
 │   ├── ChitMeo.Blog
 │   └── ChitMeo.Accounting
 │
 └── Shared
     └── ChitMeo.Abstractions
```

### Host

The host application is responsible for:

* Bootstrapping ASP.NET Core
* Loading modules
* Dependency injection
* Routing and middleware
* Application configuration

It does **not contain business logic**.

---

### Modules

Each module is a self-contained unit that may include:

* Domain logic
* Commands / Queries (CQRS)
* Handlers
* Infrastructure
* API endpoints
* Database context

Example modules:

```
ChitMeo.Auth
ChitMeo.Blog
ChitMeo.Accounting
```

Modules are designed to be **loosely coupled** and **independently developed**.

---

### Shared Abstractions

Shared contracts used across modules:

* Interfaces
* Abstract classes
* Event contracts
* Service contracts

Modules reference this layer instead of referencing each other directly.

---

## ⚙ CQRS & Mediator

ChitMeo uses a **custom lightweight mediator** implementation that:

* Automatically discovers handlers inside module assemblies
* Registers them via dependency injection
* Keeps CQRS logic isolated within each module

This approach supports **vertical slice architecture**.

---

## 🧱 Product Composition

Products can be created by combining modules.

Example:

```
Blog Product
 ├── Auth
 └── Blog
```

```
Accounting Product
 ├── Auth
 └── Accounting
```

---

## 🚧 Status

This project is currently under development as an experimental modular platform.

---

## License

MIT
