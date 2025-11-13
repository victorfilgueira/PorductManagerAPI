# Product Manager API

API para gerenciar produtos com autenticação e controle de acesso.

##  Como rodar

1. Abra o terminal na pasta do projeto
2. Execute:
   dotnet run

## Onde acessar

Depois de rodar, acesse:
- **Swagger (documentação interativa)**: `https://localhost:7036/swagger`
- **API**: `https://localhost:7036/api`

## Login

Use estas credenciais para testar:

- **Admin** (acesso total)
  - Usuário: `admin`
  - Senha: `Admin123!`

- **Manager** (pode criar/editar produtos)
  - Usuário: `manager`
  - Senha: `Manager123!`

- **User** (apenas visualizar)
  - Usuário: `user`
  - Senha: `User123!`

## Para testar todos os Endpoints

1. Faça login em `/api/auth/login`
2. Copie o token que aparece na resposta
3. No Swagger, clique no botão "Authorize"
4. Adicione "Bearer ", cole o token e clique em "Authorize"
5. Agora você pode testar o endpoint create-user

## Banco de dados

O banco SQLite é criado automaticamente quando você roda a aplicação pela primeira vez.


