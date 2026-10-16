# DBMASK — Plataforma de Mascaramento de Dados

**Aplicação ASP.NET Core MVC | SQL Server | Entity Framework Core | ASP.NET Core Identity**

**Autora:** Flórida Araújo

## Sobre o projeto

O **DBMASK** é uma aplicação de demonstração desenvolvida em ASP.NET Core MVC, cujo objetivo é implementar mecanismos de mascaramento de dados sensíveis de acordo com os perfis de acesso dos utilizadores.

O projeto demonstra a aplicação de princípios de segurança, controlo de acessos, proteção de dados e rastreabilidade das operações realizadas sobre informação sensível.

O mascaramento é aplicado no servidor, antes da apresentação dos dados na interface, evitando que os valores originais sejam enviados ao navegador quando o perfil não tem autorização para os consultar.

## Tecnologias utilizadas

- C# e ASP.NET Core MVC
- Entity Framework Core
- Microsoft SQL Server
- ASP.NET Core Identity
- Razor Views
- LINQ
- HTML, CSS e Bootstrap
- Git e GitHub

## Funcionalidades

### 1. Autenticação e controlo de acessos

A aplicação utiliza ASP.NET Core Identity para autenticação e gestão de utilizadores.

Estão definidos três perfis:

| Perfil | Responsabilidades |
|---|---|
| **Administrador** | Administração de utilizadores, regras de mascaramento e auditoria |
| **Gestor** | Consulta de dados com as regras de mascaramento aplicáveis |
| **Operador** | Consulta de dados com as regras de mascaramento aplicáveis |

O acesso às funcionalidades administrativas é restringido através de autorização baseada em perfis (*Role-Based Access Control — RBAC*).

### 2. Mascaramento de dados

O motor de mascaramento suporta as seguintes estratégias:

| Estratégia | Comportamento |
|---|---|
| `None` | Sem mascaramento |
| `Full` | Ocultação completa |
| `Last4` | Apresentação dos últimos quatro caracteres |
| `EmailPartial` | Mascaramento parcial do endereço de email |
| `NamePartial` | Mascaramento parcial do nome |

As regras são configuráveis por perfil e campo.

O serviço `CustomerReader` aplica as regras de mascaramento antes de disponibilizar os dados à interface.

### 3. Gestão de regras

O Administrador pode consultar e gerir as regras de mascaramento, definindo a estratégia aplicável a cada campo e perfil.

As configurações são persistidas na base de dados.

### 4. Gestão de utilizadores

A aplicação disponibiliza funcionalidades administrativas para gestão de contas e atribuição de perfis.

Inclui mecanismos de segurança como:

- Requisitos de complexidade e comprimento mínimo das palavras-passe.
- Bloqueio temporário após cinco tentativas de autenticação falhadas.
- Período de bloqueio de 15 minutos.
- Desbloqueio manual de contas pelo Administrador.

### 5. Auditoria

O DBMASK regista operações relevantes para efeitos de rastreabilidade, incluindo:

- Consultas de dados de clientes.
- Tentativas de autenticação falhadas.
- Bloqueios de contas.
- Desbloqueios administrativos.

A interface de auditoria permite consultar os registos, aplicar filtros e navegar entre páginas.

## Arquitetura

O projeto segue uma organização baseada no padrão MVC, com separação entre controladores, modelos, serviços, acesso a dados e interfaces.

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
├── Program.cs
├── appsettings.json
├── README.md
└── DBMASK.Web.csproj
```

A pasta `Migrations` contém a evolução do esquema da base de dados, enquanto a pasta `database` disponibiliza os dados fictícios necessários à demonstração.

## Base de dados

O DBMASK utiliza Microsoft SQL Server e Entity Framework Core.

A base de dados inclui entidades relacionadas com:

- Clientes (`Customers`).
- Regras de mascaramento (`MaskRules`).
- Registos de auditoria (`AuditEntries`).
- Utilizadores, perfis e restantes tabelas do ASP.NET Core Identity.

### Criação da base de dados com EF Core Migrations

As Migrations encontram-se incluídas no projeto e permitem recriar a estrutura da base de dados.

Durante a inicialização da aplicação, o método `Bootstrap.InitializeAsync()` executa as Migrations pendentes, cria os perfis necessários e inicializa as regras de mascaramento, quando aplicável.

Não é necessário restaurar um backup SQL Server para executar o projeto.

### Configuração da ligação ao SQL Server

A ligação à base de dados e as credenciais iniciais do Administrador devem ser configuradas localmente através de **ASP.NET Core User Secrets**.

Exemplo ilustrativo:

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

A palavra-passe apresentada é apenas um marcador. Deve ser substituída por uma palavra-passe forte que cumpra os requisitos da aplicação.

**Não devem ser publicados ficheiros de segredos, palavras-passe ou outras credenciais reais.**

### Dados fictícios de demonstração

Após a primeira execução da aplicação, é possível inserir dados fictícios de clientes através do ficheiro:

`database/seed_customers.sql`

O script disponibiliza registos de demonstração que permitem testar as estratégias de mascaramento aplicadas aos diferentes perfis.

O script deve ser executado no SQL Server Management Studio, sobre a base de dados `DBMASK_Platform`, depois de criadas as tabelas através das Migrations.

Os dados disponibilizados destinam-se exclusivamente a testes e demonstração.

## Como executar o projeto

1. Clonar o repositório:

   ```bash
   git clone https://github.com/FgAraujo-web/DBMASK.git
   ```

2. Abrir o projeto no Visual Studio com o SDK .NET compatível instalado.

3. Garantir que existe uma instância do Microsoft SQL Server acessível.

4. Configurar os User Secrets com a ligação à base de dados e as credenciais iniciais do Administrador.

5. Executar a aplicação para aplicar as Migrations e inicializar os perfis e regras.

6. Executar `database/seed_customers.sql` no SQL Server Management Studio.

7. Autenticar-se como Administrador, criar utilizadores com os perfis Gestor e Operador e testar os diferentes níveis de mascaramento.

## Demonstração das interfaces

As capturas de ecrã serão disponibilizadas na pasta `docs/screenshots`, incluindo:

- Autenticação.
- Consulta de clientes e mascaramento por perfil.
- Gestão de regras.
- Gestão de utilizadores.
- Auditoria.

## Segurança e âmbito

O DBMASK é um projeto de demonstração técnica e partilha de conhecimento.

A implementação ilustra práticas de controlo de acessos, mascaramento de dados no servidor, autenticação e auditoria. Não deve ser considerada, por si só, uma solução certificada ou pronta para utilização em produção.

Qualquer utilização em contexto real requer avaliação adicional de segurança, proteção de dados, gestão de segredos, testes e requisitos operacionais.

## Autoria

**Flórida Araújo**

Desenvolvimento de aplicações | Sistemas de Informação | Bases de Dados | Análise Funcional e Técnica

[LinkedIn](https://www.linkedin.com/in/fgaraujo) | [GitHub](https://github.com/FgAraujo-web)