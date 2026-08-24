# 🍻 ControleDeBar

> Sistema web para gerenciamento de bares, desenvolvido em **C# e .NET**, com foco em organização, controle de atendimento, persistência de dados, testes automatizados e publicação em nuvem.

[![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet\&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-13-239120?logo=csharp\&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4?logo=dotnet\&logoColor=white)](https://learn.microsoft.com/aspnet/core/)
[![Entity Framework Core](https://img.shields.io/badge/Entity%20Framework%20Core-ORM-512BD4?logo=dotnet\&logoColor=white)](https://learn.microsoft.com/ef/core/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?logo=microsoftsqlserver\&logoColor=white)](https://www.microsoft.com/sql-server)
[![Azure](https://img.shields.io/badge/Azure-Cloud-0078D4?logo=microsoftazure\&logoColor=white)](https://azure.microsoft.com/)
[![GitHub Actions](https://img.shields.io/badge/GitHub%20Actions-CI%2FCD-2088FF?logo=githubactions\&logoColor=white)](https://github.com/features/actions)

---

## 📌 Sobre o projeto

O **ControleDeBar** é uma aplicação web desenvolvida para auxiliar na gestão operacional de bares, centralizando informações relacionadas a **estabelecimentos, mesas, garçons, produtos, contas, pedidos e faturamento**.

A aplicação foi construída com **C# e .NET**, utilizando uma arquitetura organizada em camadas e separação de responsabilidades entre domínio, aplicação, infraestrutura e apresentação.

Além da aplicação principal, o projeto possui uma estrutura completa de testes automatizados, contemplando **testes unitários, testes de integração e testes End-to-End com Playwright**.

O projeto também conta com **integração contínua e entrega contínua (CI/CD)** por meio do GitHub Actions, com possibilidade de publicação da aplicação e utilização de banco de dados em ambiente de nuvem.

---

## 🎯 Objetivos

O projeto foi desenvolvido com os seguintes objetivos:

* Centralizar o gerenciamento de um bar em uma única aplicação.
* Organizar os dados de estabelecimentos e seus recursos.
* Controlar mesas e seus estados.
* Gerenciar garçons.
* Cadastrar e controlar produtos e preços.
* Registrar contas e pedidos.
* Controlar o fluxo de atendimento.
* Aplicar regras de negócio de forma organizada.
* Garantir isolamento entre estabelecimentos.
* Utilizar persistência relacional com SQL Server.
* Automatizar testes.
* Automatizar build e deploy por meio de CI/CD.
* Aplicar conceitos de orientação a objetos e separação de responsabilidades.

---

## 🏗️ Arquitetura

O projeto utiliza uma arquitetura em camadas, separando responsabilidades entre diferentes projetos:

```text
ControleDeBar
│
├── src
│   ├── ControleDeBar.Dominio
│   ├── ControleDeBar.Aplicacao
│   ├── ControleDeBar.Infra
│   ├── ControleDeBar.WebApp
│   └── ControleDeBar.Database
│
└── tests
    ├── ControleDeBar.Testes.Unidade
    ├── ControleDeBar.Testes.Integracao
    └── ControleDeBar.Testes.E2E
```

### 🔵 ControleDeBar.Dominio

Camada responsável pelas regras e conceitos centrais do sistema.

Contém as entidades, contratos, interfaces e abstrações relacionadas ao domínio da aplicação.

Entre os módulos estão conceitos como:

* Estabelecimento
* Conta
* Garçom
* Mesa
* Pedido
* Produto
* Faturamento

---

### 🟣 ControleDeBar.Aplicacao

Camada responsável pelos casos de uso da aplicação.

Aqui ficam os serviços, DTOs, contratos e demais componentes responsáveis por coordenar as operações entre a apresentação, o domínio e a infraestrutura.

A camada de aplicação evita que regras de negócio e acesso a dados fiquem diretamente acoplados à interface web.

---

### 🟠 ControleDeBar.Infra

Responsável pela infraestrutura da aplicação.

Inclui:

* Entity Framework Core
* DbContext
* Repositórios
* Persistência SQL Server
* Migrations
* Configurações de infraestrutura
* Integração com serviços externos utilizados pela aplicação
* Logging

A camada utiliza o **Entity Framework Core** como ORM para comunicação com o banco de dados.

---

### 🟢 ControleDeBar.WebApp

Camada de apresentação da aplicação.

É responsável pela aplicação web utilizando **ASP.NET Core MVC**, incluindo:

* Controllers
* Views
* ViewModels
* Autenticação
* Configuração da aplicação
* Rotas
* Interface do usuário
* Integração com os serviços da camada de aplicação

---

### 🗄️ ControleDeBar.Database

Projeto relacionado aos recursos e configurações de banco de dados utilizados pelo sistema.

---

## 🧩 Módulos

A aplicação é organizada por módulos de negócio.

### 🏢 Estabelecimento

Representa o estabelecimento administrado pelo usuário.

O sistema utiliza o estabelecimento como referência para organizar e isolar os recursos pertencentes a cada bar.

---

### 🪑 Mesas

Permite controlar as mesas do estabelecimento.

Cada mesa possui informações como:

* Número
* Quantidade de lugares
* Situação
* Estabelecimento ao qual pertence

A identificação das mesas é contextualizada pelo estabelecimento, permitindo que diferentes bares possam utilizar os mesmos números de mesa sem conflito.

---

### 👨‍🍳 Garçons

Permite gerenciar os garçons vinculados ao estabelecimento.

Os garçons possuem informações próprias e são relacionados ao estabelecimento correspondente.

---

### 🍔 Produtos

Responsável pelo cadastro e gerenciamento dos produtos comercializados pelo bar.

Entre as informações estão:

* Nome
* Preço
* Estabelecimento

---

### 🧾 Contas

Representa a conta referente ao atendimento realizado.

A conta pode estar relacionada às mesas e aos pedidos realizados durante o atendimento.

---

### 🛒 Pedidos

Responsável pelo registro dos produtos solicitados durante um atendimento.

Os pedidos relacionam informações como:

* Conta
* Produto
* Quantidade

---

### 💰 Faturamento

Responsável pelo fluxo relacionado ao fechamento e faturamento dos atendimentos.

---

## 🔐 Autenticação e isolamento

O projeto possui autenticação integrada à aplicação e utiliza o usuário autenticado como referência para o gerenciamento dos estabelecimentos.

Um dos objetivos da arquitetura é garantir o **isolamento entre estabelecimentos**, evitando que dados pertencentes a um bar sejam acessados indevidamente por outro.

Esse comportamento também é contemplado nos testes End-to-End do projeto.

---

## 🧪 Testes automatizados

O projeto possui três níveis principais de testes.

### 🔬 Testes unitários

Localizados em:

```text
tests/ControleDeBar.Testes.Unidade
```

Validam componentes individuais da aplicação e regras de negócio de forma isolada.

---

### 🔗 Testes de integração

Localizados em:

```text
tests/ControleDeBar.Testes.Integracao
```

Validam a integração entre componentes da aplicação, incluindo operações relacionadas à persistência e aos repositórios.

---

### 🌐 Testes End-to-End

Localizados em:

```text
tests/ControleDeBar.Testes.E2E
```

Utilizam **Playwright** para validar fluxos completos através da aplicação web.

Entre os cenários testados estão:

* Autenticação
* Autorização
* Isolamento entre estabelecimentos
* Jornada de atendimento
* Fluxos relacionados aos módulos da aplicação

---

## 🛠️ Tecnologias utilizadas

| Tecnologia                | Utilização                           |
| ------------------------- | ------------------------------------ |
| **C#**                    | Linguagem principal                  |
| **.NET 10**               | Plataforma de desenvolvimento        |
| **ASP.NET Core MVC**      | Aplicação web                        |
| **Entity Framework Core** | ORM e persistência                   |
| **SQL Server**            | Banco de dados relacional            |
| **ASP.NET Core Identity** | Autenticação                         |
| **Playwright**            | Testes End-to-End                    |
| **MSTest**                | Testes automatizados                 |
| **Serilog**               | Logging                              |
| **FluentResults**         | Tratamento de resultados             |
| **AutoMapper**            | Mapeamento entre objetos             |
| **GitHub Actions**        | CI/CD                                |
| **Microsoft Azure**       | Hospedagem e infraestrutura em nuvem |

---

## 📁 Estrutura do projeto

```text
ControleDeBar/
│
├── .github/
│   └── workflows/
│       └── main_controledebarweb.yml
│
├── src/
│   ├── ControleDeBar.Aplicacao/
│   ├── ControleDeBar.Database/
│   ├── ControleDeBar.Dominio/
│   ├── ControleDeBar.Infra/
│   │   └── Migrations/
│   └── ControleDeBar.WebApp/
│
├── tests/
│   ├── ControleDeBar.Testes.Unidade/
│   ├── ControleDeBar.Testes.Integracao/
│   └── ControleDeBar.Testes.E2E/
│
├── ControleDeBar.slnx
└── README.md
```

---

## 🚀 Como executar o projeto

### Pré-requisitos

Antes de executar a aplicação, certifique-se de possuir:

* [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0)
* SQL Server ou uma instância SQL Server compatível
* Git

Clone o repositório:

```bash
git clone https://github.com/GuardioesCodigo/ControleDeBar.git
cd ControleDeBar
```

Restaure as dependências:

```bash
dotnet restore
```

Compile a solução:

```bash
dotnet build
```

Configure a connection string do banco de dados conforme o ambiente de execução.

Depois execute a aplicação:

```bash
dotnet run --project src/ControleDeBar.WebApp
```

A aplicação será disponibilizada pelo ASP.NET Core no endereço indicado pelo terminal.

---

## 🗃️ Entity Framework Core

O projeto utiliza migrations do Entity Framework Core para versionamento do banco de dados.

Para criar uma nova migration:

```bash
dotnet ef migrations add NomeDaMigration \
  --project src/ControleDeBar.Infra \
  --startup-project src/ControleDeBar.WebApp
```

Para aplicar as migrations:

```bash
dotnet ef database update \
  --project src/ControleDeBar.Infra \
  --startup-project src/ControleDeBar.WebApp
```

> Em ambientes compartilhados ou de produção, revise a migration antes de aplicá-la ao banco de dados.

---

## 🧪 Executando os testes

### Todos os testes

```bash
dotnet test
```

### Testes unitários

```bash
dotnet test tests/ControleDeBar.Testes.Unidade/ControleDeBar.Testes.Unidade.csproj
```

### Testes de integração

```bash
dotnet test tests/ControleDeBar.Testes.Integracao/ControleDeBar.Testes.Integracao.csproj
```

### Testes E2E

Os testes End-to-End utilizam Playwright.

Após o build, instale o Chromium utilizado pelo Playwright:

```bash
tests/ControleDeBar.Testes.E2E/bin/Debug/net10.0/playwright.ps1 install chromium
```

Depois execute os testes:

```bash
dotnet test tests/ControleDeBar.Testes.E2E/ControleDeBar.Testes.E2E.csproj
```

---

## ⚙️ CI/CD

O projeto utiliza **GitHub Actions** para automatizar o processo de integração e entrega.

O workflow executa etapas como:

```text
Push para main
      ↓
Build
      ↓
Testes unitários
      ↓
Testes de integração
      ↓
Testes E2E
      ↓
Publish
      ↓
Migrations
      ↓
Deploy
      ↓
Azure Web App
```

A aplicação é preparada para publicação em **Microsoft Azure**, utilizando Azure Web App para hospedagem e SQL Server/Azure SQL para persistência.

---

## ☁️ Deploy

A infraestrutura de publicação utiliza:

* **Azure Web App**
* **Azure SQL**
* **GitHub Actions**
* Secrets do GitHub para configurações sensíveis
* Entity Framework Core para migrations

Informações sensíveis, como strings de conexão e chaves de serviços, **não devem ser armazenadas diretamente no código-fonte**.

---

## 📐 Princípios aplicados

Durante o desenvolvimento foram aplicados conceitos de:

* Programação Orientada a Objetos
* Separação de responsabilidades
* Injeção de dependência
* Repository Pattern
* DTOs
* ViewModels
* Interfaces e abstrações
* Mapeamento de objetos
* Persistência com ORM
* Validação
* Autenticação
* Testes automatizados
* CI/CD
* Arquitetura em camadas

---

## 🎓 Contexto acadêmico

O **ControleDeBar** foi desenvolvido como projeto prático para aplicação dos conhecimentos adquiridos durante a formação em desenvolvimento de software.

O projeto busca demonstrar, em uma aplicação realista, a utilização conjunta de conceitos de:

**C# + POO + .NET + ASP.NET Core + SQL Server + Entity Framework Core + testes automatizados + CI/CD + Azure.**

---

## 👥 Equipe

Desenvolvido por:

* **Iago Pereira**
* **Thiago Silva**

Projeto desenvolvido em colaboração durante a formação em programação.

---

## 📄 Licença

Este projeto foi desenvolvido para fins educacionais e de portfólio.

---

## 🔗 Repositório

[GitHub — GuardioesCodigo/ControleDeBar](https://github.com/GuardioesCodigo/ControleDeBar)

---

<p align="center">
  Desenvolvido com ☕, C# e .NET
</p>
