# ControlePressao - Deploy

Sistema de controle de pressão arterial e glicose desenvolvido em ASP.NET Core 9.0.

## Credenciais de Acesso
- **Email:** admin@admin.com
- **Senha:** admin123

## Opções de Deploy

### 1. Deploy com IIS (Windows)

#### Pré-requisitos:
- IIS instalado com ASP.NET Core Hosting Bundle
- .NET 9.0 Runtime instalado

#### Instruções:
1. Execute o script: `deploy-iis.bat`
2. Copie os arquivos da pasta `publish` para o diretório do IIS
3. Configure o site no IIS apontando para a pasta publicada

### 2. Deploy com Docker

#### Pré-requisitos:
- Docker instalado e em execução

#### Instruções:
1. Execute o script: `deploy-docker.bat`
2. Ou use Docker Compose: `docker-compose up -d`
3. Acesse: http://localhost:8080

### 3. Deploy Manual

#### Build e Publicação:
```bash
cd ControlePressao
dotnet clean
dotnet build --configuration Release
dotnet publish --configuration Release --output ../publish
```

#### Executar:
```bash
cd publish
dotnet ControlePressao.dll
```

## Configurações de Produção

### Banco de Dados:
- SQLite (arquivo `app.db`)
- Localizado na pasta raiz da aplicação
- Criado automaticamente na primeira execução

### Variáveis de Ambiente:
- `ASPNETCORE_ENVIRONMENT=Production`
- `ASPNETCORE_URLS=http://+:80`

### Logs:
- Configurados para nível Warning em produção
- Arquivos de log do IIS em `logs/stdout`

## Troubleshooting

### Problemas comuns:
1. **Erro de conexão com banco:** Verifique se o arquivo `app.db` tem permissões de escrita
2. **Erro 500:** Verifique os logs de aplicação
3. **Porta ocupada:** Altere a porta no `docker-compose.yml` ou IIS

### Verificação de saúde:
- Endpoint principal: `/`
- Página de login deve carregar corretamente
- Teste com as credenciais padrão

## Suporte

Para problemas de deploy, verifique:
- Logs da aplicação
- Configuração do ambiente
- Permissões de arquivo
- Firewall e portas
