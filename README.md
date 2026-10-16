# DBMASK — Plataforma de Mascaramento Dinâmico de Dados

> Prova de conceito (PoC) de uma aplicação web para consulta de dados com mascaramento dinâmico, controlo de acesso baseado em perfis e rastreabilidade de operações.

## Enquadramento

O **DBMASK** demonstra como aplicar regras de proteção de dados no servidor, de acordo com o perfil autenticado. A informação apresentada a cada utilizador é condicionada pelas permissões e regras de mascaramento configuradas na plataforma.

**Estado:** protótipo funcional; funcionalidades principais verificadas através de testes manuais. Não é uma solução certificada ou pronta para produção.

## Funcionalidades implementadas

- Autenticação de utilizadores com **ASP.NET Core Identity**.
- Perfis **Administrador**, **Gestor** e **Operador**, com autorização baseada em funções (RBAC).
- Consulta de clientes com mascaramento de campos no servidor, de acordo com regras associadas ao perfil.
- Métodos de mascaramento: `None`, `Full`, `Last4`, `EmailPartial` e `NamePartial`.
- Administração das regras de mascaramento pelo perfil Administrador.
- Criação de utilizadores e atribuição de perfis.
- Bloqueio temporário de autenticação após cinco tentativas falhadas, durante 15 minutos, e desbloqueio automático após o prazo.
- Desbloqueio manual de contas pelo Administrador.
- Auditoria de operações com pesquisa por utilizador, operação e intervalo de datas, e paginação.

## Tecnologias

| Camada | Tecnologia |
|---|---|
| Aplicação web | ASP.NET Core MVC / C# |
| Autenticação e autorização | ASP.NET Core Identity |
| Persistência | Entity Framework Core |
| Base de dados | Microsoft SQL Server |
| Interface | Razor Views / Bootstrap |
| Ambiente de desenvolvimento | Visual Studio / SQL Server Management Studio |

## Organização funcional

```text
Utilizador autenticado
        |
        v
ASP.NET Core Identity + RBAC
        |
        v
Controlador / serviço de leitura
        |
        +--> Regras de mascaramento por perfil
        |           |
        |           v
        +------> MaskEngine
        |           |
        v           v
   SQL Server --> Dados apresentados com mascaramento
        |
        v
  Registos de auditoria
```

## Perfis e acesso

- **Administrador:** gestão de utilizadores, regras e auditoria; consulta sem mascaramento segundo a configuração da PoC.
- **Gestor:** consulta de dados com mascaramento parcial, conforme regras configuradas.
- **Operador:** consulta de dados com mascaramento mais restritivo, conforme regras configuradas.

As regras são configuráveis: a apresentação de cada campo depende da política em vigor, não apenas do nome do perfil.

## Auditoria

Entre os eventos implementados encontram-se `Auth.Login`, `Auth.LoginFailed`, `Auth.Lockout`, `Auth.Logout`, `Customers.Read`, `Rule.Change` e `User.Unlock`. O evento `User.Unlock` foi verificado na interface e no SQL Server. A distinção entre um novo bloqueio e tentativas posteriores durante o bloqueio constitui uma melhoria prevista.

## Evidências visuais

Consultar a **Ficha Técnica DBMASK** (PDF), com capturas de ecrã das áreas de gestão de utilizadores e auditoria, incluindo o processo de desbloqueio manual.

## Execução local

Requisitos: SDK .NET compatível com o projeto, SQL Server e Visual Studio (ou CLI .NET).

1. Abrir a solução no Visual Studio.
2. Configurar a ligação SQL Server através de *User Secrets* ou variáveis de ambiente, evitando guardar credenciais no repositório.
3. Rever a configuração de inicialização de utilizadores e fornecer credenciais iniciais apenas por um canal seguro.
4. Executar as migrações EF Core de acordo com a configuração do projeto.
5. Iniciar a aplicação por HTTPS e autenticar-se com uma conta de teste criada localmente.

> **Segurança:** não publicar `appsettings.Development.json` com segredos, palavras-passe de bootstrap, ficheiros `.env`, bases de dados, backups, dados pessoais reais ou chaves. Antes de tornar o repositório público, rever o histórico de commits, e não apenas a versão atual dos ficheiros.

## Próximas etapas

- Testes automatizados com **xUnit** para `MaskEngine`, regras de acesso e auditoria.
- Reforço da consistência transacional no desbloqueio manual e no registo de auditoria.
- Melhoria da classificação dos eventos de autenticação (`Auth.Lockout` / tentativas durante bloqueio).
- Revisão de segurança e configuração antes de qualquer utilização em produção.

## Nota

Projeto demonstrativo para portefólio técnico e aprendizagem. Utilizar exclusivamente dados fictícios em demonstrações e capturas públicas.
