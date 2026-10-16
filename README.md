# DBMASK — Plataforma de Mascaramento de Dados

**ASP.NET Core MVC | C# | SQL Server | Entity Framework Core | ASP.NET Core Identity**

**Autora:** Flórida Araújo

## 1. Sobre o projeto

O **DBMASK** é uma aplicação de demonstração desenvolvida em ASP.NET Core MVC, destinada a implementar e demonstrar mecanismos de mascaramento de dados sensíveis de acordo com os perfis de acesso dos utilizadores.

O projeto tem como objetivo explorar conceitos e boas práticas relacionados com:

- Proteção de dados sensíveis.
- Mascaramento dinâmico de informação.
- Controlo de acessos baseado em perfis (*Role-Based Access Control — RBAC*).
- Autenticação e gestão de utilizadores.
- Auditoria e rastreabilidade de operações.
- Separação de responsabilidades na arquitetura aplicacional.

O mascaramento é realizado **no servidor**, antes de os dados serem enviados para a interface. Desta forma, os valores originais não são disponibilizados ao navegador quando o perfil autenticado não está autorizado a consultá-los.

Este projeto foi desenvolvido para fins de aprendizagem, experimentação técnica e partilha de conhecimento.

---

## 2. Tecnologias utilizadas

| Tecnologia | Finalidade |
|---|---|
| C# | Linguagem de programação |
| ASP.NET Core MVC | Desenvolvimento da aplicação web |
| Entity Framework Core | Acesso a dados e gestão de Migrations |
| Microsoft SQL Server | Sistema de gestão de base de dados |
| ASP.NET Core Identity | Autenticação, utilizadores e perfis |
| Razor Views | Construção das interfaces |
| LINQ | Consultas e manipulação de dados |
| HTML, CSS e Bootstrap | Apresentação da aplicação |
| Git e GitHub | Controlo de versões e partilha do projeto |

---

## 3. Funcionalidades

### 3.1. Autenticação e autorização

A aplicação utiliza **ASP.NET Core Identity** para gerir a autenticação e o acesso às funcionalidades.

Estão definidos três perfis:

| Perfil | Descrição |
|---|---|
| **Administrador** | Gestão de utilizadores, regras de mascaramento e auditoria |
| **Gestor** | Consulta de clientes com as regras de mascaramento aplicáveis ao perfil |
| **Operador** | Consulta de clientes com as regras de mascaramento aplicáveis ao perfil |

O acesso às funcionalidades administrativas é restringido através de autorização baseada em perfis.

### 3.2. Mascaramento de dados

O motor de mascaramento, implementado através do serviço `MaskEngine`, suporta as seguintes estratégias:

| Estratégia | Descrição |
|---|---|
| `None` | Apresentação integral do valor |
| `Full` | Ocultação completa do valor |
| `Last4` | Apresentação dos últimos quatro caracteres |
| `EmailPartial` | Mascaramento parcial do endereço de email |
| `NamePartial` | Mascaramento parcial do nome |

As regras são configuráveis por campo e perfil de acesso.

O serviço `CustomerReader` é responsável por aplicar as regras de mascaramento aos dados dos clientes antes de os disponibilizar à interface.

### 3.3. Gestão de regras de mascaramento

O Administrador pode gerir as regras de mascaramento, definindo as estratégias aplicáveis aos diferentes campos e perfis.

As regras são armazenadas na base de dados, permitindo ajustar o comportamento da aplicação sem modificar diretamente o motor de mascaramento.

### 3.4. Gestão de utilizadores

A aplicação disponibiliza uma interface administrativa para:

- Criar utilizadores.
- Atribuir perfis de acesso.
- Consultar contas existentes.
- Identificar contas bloqueadas.
- Desbloquear manualmente contas.

### 3.5. Segurança da autenticação

Foram implementados mecanismos de proteção das contas, incluindo:

- Comprimento mínimo de 12 caracteres para palavras-passe.
- Bloqueio após cinco tentativas de autenticação falhadas.
- Bloqueio temporário de 15 minutos.
- Desbloqueio administrativo de contas.
- Cookies de autenticação com `HttpOnly`, `Secure` e `SameSite=Lax`.

### 3.6. Auditoria e rastreabilidade

A aplicação mantém um histórico de operações relevantes, incluindo:

- Consulta de dados de clientes.
- Tentativas de autenticação falhadas.
- Bloqueios de contas.
- Desbloqueios administrativos.

A interface de auditoria permite consultar os registos, aplicar filtros e navegar pelos resultados através de paginação.

---

## 4. Arquitetura da aplicação

O DBMASK utiliza o padrão **Model-View-Controller (MVC)**, com separação entre apresentação, lógica aplicacional e persistência.

Os principais componentes incluem:

- **Controllers:** processamento dos pedidos e controlo de acesso.
- **Models:** representação das entidades e modelos da aplicação.
- **Views:** apresentação das interfaces.
- **Services:** implementação das regras de mascaramento e leitura de dados.
- **Data:** contexto de dados e inicialização da aplicação.
- **Migrations:** evolução e criação do esquema da base de dados.

### Estrutura do projeto

```text
DBMASK.Web/
├── Controllers/
├── Data/
├── Migrations/
├── Models/
├── Services/
├── Views/
├── database/
│   └── seed_customers.sql
├── docs/
│   ├── DBMASK_Ficha_Tecnica.pdf
│   └── screenshots/
├── Properties/
├── Program.cs
├── appsettings.json
├── .gitignore
├── README.md
└── DBMASK.Web.csproj
```

A estrutura apresentada inclui os diretórios previstos para a documentação e os dados de demonstração.

---

## 5. Base de dados

O DBMASK utiliza **Microsoft SQL Server**, com persistência implementada através de Entity Framework Core.

As principais tabelas incluem:

| Tabela | Finalidade |
|---|---|
| `Customers` | Dados dos clientes utilizados na demonstração |
| `MaskRules` | Regras de mascaramento por campo e perfil |
| `AuditEntries` | Registos de auditoria |
| Tabelas ASP.NET Core Identity | Gestão de utilizadores, perfis e autenticação |

### 5.1. Criação da base de dados através de EF Core Migrations

As Migrations estão incluídas na pasta `Migrations` do projeto.

Durante a inicialização da aplicação, o método `Bootstrap.InitializeAsync()` aplica as Migrations pendentes e inicializa os elementos necessários ao funcionamento da aplicação.

Este processo permite recriar a estrutura da base de dados sem necessidade de restaurar um backup SQL Server (`.bak`).

### 5.2. Configuração da ligação ao SQL Server

A aplicação utiliza uma connection string identificada por `DBMASK`.

A configuração local pode ser efetuada através de **ASP.NET Core User Secrets**, evitando a publicação de credenciais no repositório.

Exemplo:

```json
{
  "ConnectionStrings": {
    "DBMASK": "Server=localhost;Database=DBMASK_Platform;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Bootstrap": {
    "AdminEmail": "admin@dbmask.local",
    "AdminPassword": "DEFINIR_LOCALMENTE"
  }
}
```

O valor `DEFINIR_LOCALMENTE` é apenas um marcador e deve ser substituído por uma palavra-passe forte.

**Nunca devem ser publicados ficheiros de segredos ou credenciais reais.**

### 5.3. Dados fictícios de demonstração

O projeto disponibiliza um script SQL destinado à inserção de clientes fictícios:

`database/seed_customers.sql`

O script deve ser executado no **SQL Server Management Studio (SSMS)**, após a criação da base de dados através das Migrations.

Os registos permitem demonstrar as diferentes estratégias de mascaramento, utilizando exclusivamente informação fictícia.

Não é necessário disponibilizar ou restaurar um backup da base de dados original.

---

## 6. Como executar o projeto

### Pré-requisitos

- Visual Studio com suporte para ASP.NET Core.
- SDK .NET compatível com o projeto.
- Microsoft SQL Server.
- SQL Server Management Studio (recomendado).
- Git (para clonar o repositório).

### Passo 1 — Clonar o repositório

```bash
git clone https://github.com/FgAraujo-web/DBMASK.git
```

### Passo 2 — Abrir a solução

Abrir o projeto no Visual Studio e restaurar os pacotes NuGet, caso seja necessário.

### Passo 3 — Configurar os User Secrets

No Visual Studio:

1. Clicar com o botão direito do rato no projeto `DBMASK.Web`.
2. Selecionar **Manage User Secrets**.
3. Configurar a ligação ao SQL Server e as credenciais iniciais do Administrador.
4. Guardar as alterações.

### Passo 4 — Inicializar a base de dados

Executar a aplicação.

Durante a inicialização, o Bootstrap aplica as Migrations pendentes e cria os perfis e as regras iniciais necessários ao funcionamento do DBMASK.

### Passo 5 — Inserir dados fictícios

No SQL Server Management Studio, executar:

`database/seed_customers.sql`

O script insere os clientes fictícios utilizados para demonstrar o mascaramento.

### Passo 6 — Testar os perfis

Autenticar-se com a conta Administrador e criar as contas Gestor e Operador através da interface de gestão de utilizadores.

Em seguida, iniciar sessão com cada perfil para comparar a apresentação dos dados.

---

## 7. Contas de demonstração

O DBMASK permite testar três níveis de acesso através de contas de demonstração, possibilitando comparar a apresentação dos dados sensíveis de acordo com o perfil do utilizador.

| Perfil | Email de exemplo | Criação |
|---|---|---|
| **Administrador** | `admin@dbmask.local` | Automática, durante a inicialização |
| **Gestor** | `gestor@dbmask.local` | Através da gestão de utilizadores |
| **Operador** | `operador@dbmask.local` | Através da gestão de utilizadores |

### 7.1. Configuração das contas

A conta Administrador é criada automaticamente durante a inicialização da aplicação, utilizando as credenciais configuradas localmente através de ASP.NET Core User Secrets.

Após autenticação como Administrador, é possível criar as contas Gestor e Operador através da interface de gestão de utilizadores, atribuindo-lhes os respetivos perfis e palavras-passe.

Os endereços de email apresentados são fictícios e destinam-se exclusivamente à demonstração.

As palavras-passe não são disponibilizadas no repositório GitHub.

### 7.2. Testar o mascaramento de dados

Após a criação das contas, o utilizador pode autenticar-se com cada perfil e observar as diferenças na apresentação dos dados sensíveis.

O objetivo é demonstrar a aplicação de políticas de mascaramento de dados, controlo de acessos baseado em perfis (*Role-Based Access Control — RBAC*) e auditoria das operações.

---

## 8. Demonstração das interfaces

As imagens seguintes apresentam as principais funcionalidades do DBMASK, demonstrando o controlo de acessos, o mascaramento de dados e a auditoria das operações.

8.1. Autenticação

Acesso à aplicação através de autenticação baseada em ASP.NET Core Identity.



8.2. Mascaramento de dados por perfil

Administrador — Visualização integral

O perfil Administrador permite consultar os dados sem mascaramento, de acordo com as regras configuradas.



Gestor — Mascaramento parcial

O perfil Gestor apresenta os dados parcialmente mascarados, utilizando estratégias como NamePartial, EmailPartial e Last4.



Operador — Mascaramento completo

O perfil Operador apresenta os campos sensíveis integralmente mascarados através da estratégia Full.



8.3. Gestão de regras de mascaramento

O Administrador pode configurar as estratégias de mascaramento aplicáveis a cada campo e perfil de acesso.



8.4. Gestão de utilizadores

A interface administrativa permite consultar utilizadores, identificar os respetivos perfis e verificar o estado das contas.



8.5. Bloqueio e desbloqueio de contas

Após cinco tentativas de autenticação falhadas, a conta é temporariamente bloqueada durante 15 minutos.

O Administrador pode identificar contas bloqueadas e proceder ao seu desbloqueio manual.





8.6. Auditoria e rastreabilidade

O DBMASK regista operações relevantes, incluindo autenticações, tentativas falhadas, consultas de clientes e desbloqueios administrativos.

A interface permite filtrar os registos por utilizador, operação e intervalo de datas.



---

## 9. Segurança e âmbito do projeto

O DBMASK é uma **prova de conceito técnica**, desenvolvida para demonstrar mecanismos de proteção de informação e controlo de acessos.

O projeto permite explorar:

- Mascaramento de dados no servidor.
- Autorização baseada em perfis.
- Gestão de identidades.
- Auditoria de operações.
- Persistência e configuração de regras.
- Utilização de EF Core Migrations.
- Separação entre configuração local e código-fonte.

A aplicação não deve ser considerada uma solução certificada ou pronta para utilização em produção.

A adoção em ambientes reais exige avaliações adicionais de segurança, privacidade, testes, gestão de segredos e requisitos operacionais.

---

## 10. Objetivo da partilha

Este repositório é disponibilizado com o objetivo de **partilhar conhecimento técnico e demonstrar a implementação prática de mecanismos de mascaramento de dados em aplicações .NET**.

O projeto constitui um exemplo de integração entre desenvolvimento aplicacional, bases de dados, segurança da informação e gestão de acessos.

---

## 11. Autoria

**Flórida Araújo**

*Sistemas de Informação | Desenvolvimento de Aplicações | Bases de Dados | Análise Funcional e Técnica*

[LinkedIn](https://www.linkedin.com/in/fgaraujo) | [GitHub](https://github.com/FgAraujo-web)

---

**DBMASK — Demonstração de mascaramento de dados e controlo de acessos em ASP.NET Core MVC.**