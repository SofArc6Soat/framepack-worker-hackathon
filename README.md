 - [Aplicação Framepack-WebApi (backend)](#aplicação-framepack-webapi)
   - [Funcionalidades Principais](#funcionalidades-principais)
   - [Estrutura do Projeto](#estrutura-do-projeto)
   - [Tecnologias Utilizadas](#tecnologias-utilizadas)
   - [Serviços Utilizados](#serviços-utilizados)
   - [Motivações para Utilizar o SQL Server como Banco de Dados da Aplicação](#motivações-para-utilizar-o-sql-server-como-banco-de-dados-da-aplicação)
   - [Como Executar o Projeto](#como-executar-o-projeto)
     - [Clonar o repositório](#clonar-o-repositório)
     - [Executar com docker-compose](#executar-com-docker-compose)
     - [Executar com Kubernetes](#executar-com-kubernetes)
   - [Collection com todas as APIs com exemplos de requisição](#collection-com-todas-as-apis-com-exemplos-de-requisição)
   - [Desenho da arquitetura](#desenho-da-arquitetura)
   - [Demonstração em vídeo](#demonstração-em-vídeo)
   - [Relatório de Cobertura](#relatório-de-cobertura)
   - [Autores](#autores)
   - [Documentação Adicional](#documentação-adicional)
   - [Repositórios microserviços](#repositórios-microserviços)
   - [Repositórios diversos](#repositórios-diversos)

---

 # Aplicação Framepack-Worker

 Este projeto visa o desenvolvimento do worker para o processamento de vídeos. O worker faz o download de um vídeo, extrai os frames, compacta os frames em um arquivo .zip e faz o upload para o S3.<br>
 Utilizando a arquitetura limpa, .NET 8, Amazon S3, Docker e Kubernetes, o objetivo é criar uma base sólida e escalável para suportar as funcionalidades necessárias para um sistema de processamento de vídeos.<br>
 O foco principal é a criação de uma aplicação robusta, modular e de fácil manutenção.<br>

 ## Funcionalidades Principais

 - **Download de Vídeos**: Realiza o download de vídeos a partir de uma URL fornecida.
 - **Extração de Frames de Vídeos**: Extrai frames dos vídeos baixados em intervalos específicos.
 - **Compactação de Frames**: Compacta os frames extraídos em um arquivo .zip para facilitar o armazenamento e a transferência.
 - **Upload de Arquivos Compactados para o S3**: Faz o upload dos arquivos .zip contendo os frames para o Amazon S3, garantindo a disponibilidade e a escalabilidade do armazenamento.

 ## Estrutura do Projeto

 - **BuildingBlocks**: Contém serviços e utilitários comuns, como o serviço de integração com o S3.
 - **Controllers**: Contém os controladores responsáveis por lidar com as requisições HTTP.
 - **DevOps**: Contém scripts e configurações para Docker e Kubernetes.
 - **Gateways**: Contém os handlers responsáveis pelo processamento de vídeos.
 - **Infra**: Contém a infraestrutura necessária para o funcionamento do projeto, como configurações de banco de dados e serviços externos.
 - **UseCases**: Contém os casos de uso principais do worker.
 - **Worker**: Contém a lógica principal do worker para download de vídeos, extração de frames, compactação e upload.

 ## Tecnologias Utilizadas

 - .NET 8
 - Amazon S3
 - Docker
 - Kubernetes

 ## Como Executar o Projeto

 ### Clonar o repositório
 ```
 git clone https://github.com/SofArc6Soat/framepack-worker-hackathon.git
 ```

 ### Executar com docker-compose
 1. Docker (docker-compose)
 1.1. Navegue até o diretório do projeto:
 ```
 cd framepack-worker-hackathon\src\DevOps
 ```
 2. Configure o ambiente Docker:
 ```
 docker-compose up --build
 ```
 2.1. A aplicação estará disponível em http://localhost:5001

 ### Executar com Kubernetes
 2. Kubernetes
 2.1. Navegue até o diretório do projeto:
 ```
 cd framepack-worker-hackathon\src\DevOps\kubernetes
 ```
 2.2. Configure o ambiente Docker:
 ```
 kubectl apply -f 06-framepack-worker-deployment.yaml
 kubectl apply -f 07-framepack-worker-service.yaml
 kubectl apply -f 08-framepack-worker-hpa.yaml
 kubectl port-forward svc/framepack-worker 8080:80
 ```
 ou executar todos scripts via PowerShell
 ```
 Get-ExecutionPolicy
 Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process
 .\delete-k8s-resources.ps1
 .\apply-k8s-resources.ps1
 ```
 2.3. A aplicação estará disponível em http://localhost:8080

 ## Autores

 - **Anderson Lopez de Andrade RM: 350452** <br>
 - **Henrique Alonso Vicente RM: 354583**<br>

 ## Documentação Adicional

 - **Miro - Domain Storytelling, Context Map, Linguagem Ubíqua e Event Storming**: [Link para o Event Storming](https://miro.com/app/board/uXjVKST91sw=/)
 - **Github - Domain Storytelling**: [Link para o Domain Storytelling](https://github.com/SofArc6Soat/quickfood-domain-story-telling)
 - **Github - Context Map**: [Link para o Domain Storytelling](https://github.com/SofArc6Soat/quickfood-ubiquitous-language)
 - **Github - Linguagem Ubíqua**: [Link para o Domain Storytelling](https://github.com/SofArc6Soat/quickfood-ubiquitous-language)

 ## Repositórios microserviços

 - **Framepack-WebApi**: [Link](https://github.com/SofArc6Soat/framepack-api-hackathon)
 - **Framepack-Worker**: [Link](https://github.com/SofArc6Soat/framepack-worker-hackathon)

 ## Repositórios diversos

 - **Documentação**: [Link](https://github.com/SofArc6Soat/framepack-api)
 - **Lambda Autenticação**: [Link](https://github.com/SofArc6Soat/quickfood-auth-function)