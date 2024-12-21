## 🚀 GitHub Repo Lister API

Esta API foi desenvolvida para facilitar a listagem e filtragem de repositórios da organização Takenet no GitHub. Com ela, você pode acessar informações relevantes sobre os projetos de forma rápida e eficiente.

## 📋 Funcionalidades
- **Listar Repositórios**: Obtém uma lista completa de todos os repositórios da organização Takenet.
- **Filtrar Repositórios**: Permite filtrar os repositórios com base em:
  - Linguagem de programação
  - Nome (busca por partes do nome)
  - Data de criação (opções de ordenação crescente ou decrescente)

## 💻 Tecnologias Utilizadas
- **ASP.NET Core**: Framework principal para a construção da API.
- **HttpClient**: Utilizado para realizar requisições à API do GitHub.
- **JSON**: Formato utilizado para manipulação e retorno dos dados.

## 🔧 Requisitos
Antes de iniciar o projeto, verifique se você possui os seguintes pré-requisitos instalados:
- .NET 6 ou superior
- Git (necessário para clonar o repositório)

## 🚀 Como Usar
1. **Clonar o Repositório**
   Clone este repositório para o seu ambiente local:
git clone https://github.com/caiquemain/takeablip.git
 

2. **Restaurar as Dependências**
No diretório do projeto, execute o seguinte comando:
dotnet restore
 

3. **Executar a Aplicação**
No diretório do projeto, execute:
dotnet run
 
A API será iniciada em `http://localhost:5000` (ou outra porta configurada).

4. **Acessar a Documentação**
Utilize a documentação Swagger disponível na seguinte URL:
👉 https://takeablip.onrender.com/documentation/index.html

5. **Testar os Endpoints**
- **Listar Todos os Repositórios**
  - Endpoint:
  ```
  GET /api/github/repositories/listall  
  ```

- **Filtrar Repositórios**
  - Endpoint:
  ```
  GET /api/github/repositories/filter  
  ```
  - **Parâmetros**:
    - `language` (opcional): Linguagem de programação (ex.: C#, Java).
    - `name` (opcional): Parte do nome do repositório.
    - `sortOrder` (opcional): Ordem de classificação: `asc` (crescente) ou `desc` (decrescente).
    - `limit` (opcional): Número máximo de repositórios a serem retornados.

## ⚙️ Como Implantar
1. **Publicar a Aplicação**
Para gerar os arquivos necessários para produção, utilize:
dotnet publish -c Release -o ./publish
 

2. **Configurar o Servidor**
- Para Azure: Use o Azure Web Apps ou implemente uma aplicação Docker.
- Para AWS: Utilize Elastic Beanstalk ou implemente via AWS Lambda.
Certifique-se de configurar as variáveis de ambiente, incluindo o `GITHUB_TOKEN`.

## 🧰 Cache de Repositórios
A API utiliza um sistema de cache que armazena informações dos repositórios por 10 minutos. Isso reduz a quantidade de chamadas para a API do GitHub e melhora a performance. Se o cache expirar ou não houver dados armazenados, as informações serão recarregadas diretamente do GitHub.

## 📄 Licença
Este projeto está licenciado sob a MIT License.

## 📞 Contato
📧 Para dúvidas ou sugestões, abra uma issue no repositório ou entre em contato diretamente comigo:  
👨‍💻 Desenvolvido por Caique Main  
🔧 Tecnologias: ASP.NET Core, GitHub API, HttpClient, JSON
