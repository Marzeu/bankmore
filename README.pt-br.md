
---

# 🇧🇷 README.pt-br.md (Português)

```md
# BankMore

BankMore é um sistema bancário simples baseado em microserviços, desenvolvido em .NET 8.

O projeto simula operações entre contas correntes, com foco em arquitetura limpa, tratamento de erros e integração entre serviços.

---

## 🧱 Arquitetura

A solução é composta por dois microserviços:

- **API ContaCorrente**
  - Gerencia contas e saldos
  - Realiza operações de débito e crédito
  - Aplica validações de negócio

- **API Transferencia**
  - Orquestra transferências entre contas
  - Chama a API ContaCorrente para débito e crédito
  - Realiza estorno em caso de falha

---

## ⚙️ Funcionalidades

- Criação de conta e autenticação (JWT)
- Operações de crédito e débito
- Transferência entre contas
- Controle de idempotência
- Tratamento de erros padronizado
- Testes unitários
- Testes de integração
- Suporte a Docker com docker-compose

---

## 🚀 Como executar

### Usando Docker (recomendado)

```bash
docker compose up --build