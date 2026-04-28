# BankMore

BankMore is a simple microservices-based banking system built with .NET 8.

The project simulates basic operations between current accounts, focusing on clean architecture, error handling, and integration between services.

---

## 🧱 Architecture

The solution is composed of two microservices:

- **ContaCorrente API**
  - Manages accounts and balances
  - Handles debit and credit operations
  - Applies business validations

- **Transferencia API**
  - Orchestrates transfers between accounts
  - Calls ContaCorrente API for debit and credit
  - Handles rollback (refund) in case of failure

---

## ⚙️ Features

- Account creation and authentication (JWT)
- Credit and debit operations
- Transfer between accounts
- Idempotency handling
- Standardized error handling
- Unit tests
- Integration tests
- Docker support with docker-compose

---

## 🚀 Running the project

### Using Docker (recommended)

```bash
docker compose up --build