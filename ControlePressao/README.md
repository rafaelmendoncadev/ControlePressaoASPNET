# Sistema de Controle de Pressão Arterial

Sistema web desenvolvido em ASP.NET Core MVC para controle e monitoramento de pressão arterial, glicose e peso.

## Funcionalidades

- 📊 Registro e acompanhamento de pressão arterial
- 🩸 Controle de glicose
- ⚖️ Monitoramento de peso
- 📈 Gráficos interativos com Chart.js
- 📄 Relatórios em PDF e Excel
- 🔐 Sistema de autenticação
- 🌐 Interface responsiva

## Tecnologias Utilizadas

- **Backend**: ASP.NET Core 8.0 MVC
- **Banco de Dados**: SQLite com Entity Framework Core
- **Frontend**: Bootstrap 5, Chart.js
- **Relatórios**: iText7 (PDF), EPPlus (Excel)
- **Containerização**: Docker

## Deploy na Railway

### Pré-requisitos

1. Conta na [Railway](https://railway.app/)
2. Projeto conectado ao GitHub

### Passos para Deploy

1. **Fork/Clone do Repositório**
   ```bash
   git clone <seu-repositorio>
   cd ControlePressao
   ```

2. **Conectar à Railway**
   - Acesse [Railway Dashboard](https://railway.app/dashboard)
   - Clique em "New Project"
   - Selecione "Deploy from GitHub repo"
   - Escolha seu repositório

3. **Configuração Automática**
   - A Railway detectará automaticamente o `Dockerfile`
   - O arquivo `railway.toml` configurará o build e deploy

4. **Variáveis de Ambiente (Opcional)**
   - `ASPNETCORE_ENVIRONMENT=Production`
   - `PORT` (configurado automaticamente pela Railway)

### Estrutura de Arquivos para Deploy

```
├── Dockerfile                 # Configuração do container
├── .dockerignore             # Arquivos ignorados no build
├── railway.toml              # Configurações da Railway
├── appsettings.Production.json # Configurações de produção
└── Program.cs                # Configurado para Railway
```

### Características do Deploy

- **Banco de Dados**: SQLite persistente em `/app/data/`
- **Porta**: Configuração dinâmica via variável `PORT`
- **HTTPS**: Desabilitado em produção (Railway gerencia SSL)
- **Logs**: Configurados para nível Warning em produção
- **Health Check**: Endpoint `/` com timeout de 5 minutos

### Monitoramento

- Logs disponíveis no dashboard da Railway
- Health check automático
- Restart automático em caso de falha (máximo 3 tentativas)

### Desenvolvimento Local

```bash
# Restaurar dependências
dotnet restore

# Executar aplicação
dotnet run

# Acessar em
http://localhost:5224
```

### Build Docker Local

```bash
# Build da imagem
docker build -t controle-pressao .

# Executar container
docker run -p 8080:8080 controle-pressao
```

## Estrutura do Projeto

```
ControlePressao/
├── Controllers/          # Controladores MVC
├── Data/                # Contexto e modelos do banco
├── Models/              # ViewModels e DTOs
├── Services/            # Serviços de negócio
├── Views/               # Views Razor
├── wwwroot/             # Arquivos estáticos
└── Program.cs           # Configuração da aplicação
```

## Licença

Este projeto está sob a licença MIT.