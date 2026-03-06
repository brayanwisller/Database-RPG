**Para a execução desse projeto em C# é necessário que tenha instalado os seguintes programas e bibliotecas (para a execução no VS Code):**
- C# e .NET 9;
- C# Dev Kit;
- SQLite Viewer;
- Postman (não obrigatório, apenas se quiser);


**Stacks Utilizadas:**
- C# / .NET
- ASP.NET Core Web API
- Entity Framework Core;
- SQLite (Banco de Dados)
- Swagger (API)

1. Execução
**Caso seja a primeira vez, instale as ferramentas do EF Core com os seguintes comandos:**
- dotnet add package Microsoft.EntityFrameworkCore
- dotnet add package Microsoft.EntityFrameworkCore.Sqlite
- dotnet add package Microsoft.EntityFrameworkCore.Design
- dotnet new tool-manifest
- dotnet tool install dotnet-ef

**Modifique o arquivo RPG_BD.csproj**
- Adicione o .Web após o .Sdk: <Project Sdk="Microsoft.NET.Sdk">  ==>   <Project Sdk="Microsoft.NET.Sdk.Web">
*Caso já esteja com o .Web não será necessario fazer alterações

**Depois, para o banco de dados crie as migrations, apenas uma vez:**
- dotnet ef migrations add InitialCreate
- dotnet ef database update

**Para que o projeto execute rode o seguinte comando:**
- dotnet run

**Assim a API e o banco de dados vão executar juntos!**
**Uma URL irá aparecer assim que o comando for dado, caso queira fazer pela web entre na URL gerada "http://localhost:5099" e adicione o /swagger ao final**

2. Executando
**Ao rodar o código o CLI Interativo do banco de dados estará disponível no mesmo terminal onde foi executado o comando "dotnet run"**
**Também ficará disponível na web na URL informada anteriormente**

3. Descrição das Entidades

| Campo     | Tipo          | Restrições                          | Descrição                          |
|-----------|---------------|-------------------------------------|------------------------------------|
| `Id`      | `integer`     | Chave Primária, Auto-incremento     | Identificador único do item.       |
| `Name`    | `varchar(100)`| Obrigatório, Único, Mín. 3 chars    | Nome do item.                      |
| `Rarity`  | `varchar(50)` | Obrigatório                         | Raridade do item.                  |
| `Preco`   | `decimal(10,2)`| Obrigatório, Não negativo          | Preço do item em moedas.           |
| `CriadoEm`| `datetime`    | Gerado automaticamente              | Data e hora de criação do registro.|

4. Rotas da API (Endpoints)

A URL base é `http://localhost:5099`.

---

### **Listar todos os itens**
-   **Endpoint:** `GET /api/v1/items`
-   **Descrição:** Retorna uma lista de todos os itens cadastrados.
-   **Resposta de Sucesso (200 OK):**
    ```json
    [
      {
        "id": 1,
        "name": "Poção de Cura",
        "rarity": "Comum",
        "preco": 100.00,
        "CriadoEm": "2025-10-27T10:00:00Z"
      }
    ]
    ```

---

### **Buscar item por ID**
-   **Endpoint:** `GET /api/v1/items/{id}`
-   **Resposta de Sucesso (200 OK):**
    ```json
    {
      "id": 1,
      "name": "Poção de Cura",
      "rarity": "Comum",
      "preco": 100.00,
      "CriadoEm": "2025-10-27T10:00:00Z"
    }
    ```
-   **Resposta de Erro (404 Not Found):** `{"message": "Item não encontrado."}`

---

### **Criar um novo item**
-   **Endpoint:** `POST /api/v1/items`
-   **Corpo da Requisição (JSON):**
    ```json
    {
      "name": "Espada Longa de Aço",
      "rarity": "Incomum",
      "preco": 250.00
    }
    ```
-   **Resposta de Sucesso (201 Created):** Retorna o item recém-criado.
-   **Respostas de Erro:**
    -   **422 Unprocessable Entity:** Se os dados de validação falharem.
    -   **409 Conflict:** Se já existir um item com o mesmo nome.

---

### **Atualizar um item**
-   **Endpoint:** `PUT /api/v1/items/{id}`
-   **Corpo da Requisição (JSON):** (pode enviar todos os campos)
    ```json
    {
      "name": "Espada Longa de Aço +1",
      "rarity": "Raro",
      "preco": 500.00
    }
    ```
-   **Resposta de Sucesso (204 No Content):** Nenhuma resposta no corpo.
-   **Respostas de Erro:**
    -   **400 Bad Request:** Se os dados do corpo forem inválidos.
    -   **404 Not Found:** Se o item com o ID informado não existir.
    -   **409 Conflict:** Se o novo nome já estiver em uso por outro item.

---

### **Remover um item**
-   **Endpoint:** `DELETE /api/v1/items/{id}`
-   **Resposta de Sucesso (204 No Content):** Nenhuma resposta no corpo.

-   **Resposta de Erro (404 Not Found):** `{"message": "Item não encontrado para exclusão."}`


