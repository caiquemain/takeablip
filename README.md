# 🚀 GitHub Repo Lister API

Esta API foi desenvolvida para facilitar a **listagem e filtragem** de repositórios da organização **Takenet** no GitHub. Com ela, você pode acessar informações relevantes sobre os projetos de forma rápida e eficiente.

## 📋 Funcionalidades

- **Listar Repositórios**: Obtém uma lista completa de todos os repositórios da organização **Takenet**.
- **Filtrar Repositórios**: Permite filtrar os repositórios com base em:
  - **Linguagem de programação**
  - **Nome** (busca por partes do nome)
  - **Data de criação** (opções de ordenação crescente ou decrescente)

## 💻 Tecnologias Utilizadas

- **ASP.NET Core**: Framework principal para a construção da API.
- **HttpClient**: Utilizado para realizar requisições à API do GitHub.
- **JSON**: Formato utilizado para manipulação e retorno dos dados.

## 🔧 Requisitos

Antes de iniciar o projeto, verifique se você possui os seguintes pré-requisitos instalados:

- [**.NET 6 ou superior**](https://dotnet.microsoft.com/download)
- **Git** (necessário para clonar o repositório)

## 🚀 Como Usar

### 1. Clonar o Repositório

Clone este repositório para o seu ambiente local:
git clone https://github.com/seu-usuario/GitHubRepoListerAPI.git

### 2. Restaurar as Dependências

No diretório do projeto, execute o seguinte comando:

dotnet restore


A API será iniciada em `http://localhost:5000` (ou outra porta configurada).

### 4. Usar a API

#### Listar Todos os Repositórios

Para listar todos os repositórios disponíveis na organização Takenet, utilize o seguinte endpoint:

GET /api/github/repositories/listall


#### Filtrar Repositórios

Você pode filtrar repositórios por linguagem, nome ou ordenar por data de criação. Exemplo de uso:


Parâmetros:
- `language`: Linguagem de programação (ex.: C#, Java).
- `name`: Parte do nome do repositório.
- `sortOrder`: Ordem de classificação: `asc` (crescente) ou `desc` (decrescente).

### 5. Testar a API

Teste os endpoints utilizando ferramentas como Postman ou diretamente no navegador para o endpoint de listagem.

## ⚙️ Como Implantar

### 1. Configurar o Ambiente de Produção

Realize o deploy da API em um servidor web, como Azure, AWS ou Heroku. Para publicar a API, utilize o comando:

dotnet publish -c Release -o ./publish


### 2. Configurar o Servidor

- **Para Azure**: Utilize Azure Web Apps ou configure uma aplicação Docker.
- **Para AWS**: Use Elastic Beanstalk ou implemente via AWS Lambda.

## 🧰 Cache de Repositórios

A API implementa um sistema de cache que armazena informações dos repositórios para minimizar as requisições à API do GitHub. O cache é atualizado a cada 10 minutos. Se não houver dados no cache, as informações serão recarregadas diretamente do GitHub.

## 📄 Licença

Este projeto está licenciado sob a **MIT License**.

## 📞 Contato

Para dúvidas ou sugestões, sinta-se à vontade para abrir uma issue ou me contatar diretamente.

👨‍💻 Desenvolvido por Caique Main
🔧 Tecnologias: ASP.NET Core, GitHub API, HttpClient, JSON  
