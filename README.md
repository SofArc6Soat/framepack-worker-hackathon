- [📌 Aplicação Framepack-Worker](#-aplicação-framepack-worker)
  - [📖 Visão Geral](#-visão-geral)
  - [🚀 Funcionalidades Principais](#-funcionalidades-principais)
      - [Download de Vídeos](#download-de-vídeos)
      - [Extração de Frames](#extração-de-frames)
      - [Compactação de Frames](#compactação-de-frames)
      - [Upload para o S3](#upload-para-o-s3)
      - [Notificações Automáticas](#notificações-automáticas)
  - [📁 Estrutura do Projeto](#-estrutura-do-projeto)
  - [🛠 Tecnologias Utilizadas](#-tecnologias-utilizadas)
  - [▶️ Como Executar o Projeto (Framepack-Worker)](#️-como-executar-o-projeto-framepack-worker)
    - [Clonar o Repositório](#clonar-o-repositório)
    - [Executar com Docker Compose](#executar-com-docker-compose)
      - [🐳 Docker (docker-compose)](#-docker-docker-compose)
    - [Executar com Kubernetes](#executar-com-kubernetes)
      - [☸️ Kubernetes](#️-kubernetes)
  - [📚 Documentações](#-documentações)
  - [👨‍💻 Autores](#-autores)
  - [🔗 Repositórios de Microserviços](#-repositórios-de-microserviços)

---

# 📌 Aplicação Framepack-Worker

## 📖 Visão Geral
O Framepack-Worker é um serviço assíncrono projetado para processamento automatizado de vídeos, garantindo eficiência, escalabilidade e alta confiabilidade.

Ele realiza as seguintes operações:

✅ Consumo de eventos do Amazon SQS.
✅ Download de vídeos armazenados no Amazon S3.
✅ Extração de frames dos vídeos.
✅ Compactação dos frames em um arquivo .zip.
✅ Upload do arquivo .zip para o Amazon S3.
✅ Notificação automática via e-mail (Amazon SES) sobre o status do processamento.

A aplicação foi desenvolvida para ser modular, escalável e eficiente, separada da API principal para otimizar recursos e facilitar a evolução do sistema.

---

## 🚀 Funcionalidades Principais

#### Download de Vídeos
Baixa os vídeos a partir de URLs fornecidas nos eventos do Amazon SQS.

#### Extração de Frames
Extrai frames dos vídeos baixados em intervalos específicos para análise ou uso.

#### Compactação de Frames
Agrupa os frames extraídos em um arquivo .zip, otimizando armazenamento e transferência.

#### Upload para o S3
Envia os arquivos compactados para o Amazon S3, garantindo disponibilidade e escalabilidade.

#### Notificações Automáticas
Envia e-mails ao usuário informando o status do processamento (sucesso ou falha) via Amazon SES.

---

## 📁 Estrutura do Projeto

A arquitetura do Framepack-Worker segue uma abordagem modular para garantir facilidade de manutenção e escalabilidade.

- **🛠 BuildingBlocks** → Serviços e utilitários comuns (integração com Amazon S3, SQS, Email).
- **📡 Controllers** → Controladores responsáveis pelo gerenciamento de requisições HTTP.
- **⚙️ DevOps** → Scripts e configurações para Docker e Kubernetes.
- **🎬 Gateways** → Handlers especializados no processamento de vídeos.
- **🏗 Infra** → Configurações de banco de dados, serviços externos e infraestrutura necessária.
- **📌 UseCases** → Implementação dos principais casos de uso do worker.
- **🖥 Worker** → Lógica principal da aplicação, focada no consumo de eventos da mensageria.

---

## 🛠 Tecnologias Utilizadas

- **.NET 8** → Framework principal para desenvolvimento do backend.
- **Docker** → Containerização para portabilidade e facilidade de deploy.
- **Kubernetes** → Orquestração de containers para resiliência e escalabilidade.
- **Amazon DynamoDB** → Banco de dados NoSQL para armazenamento de informações.
- **Amazon SQS** → Serviço de mensageria para gerenciamento de eventos assíncronos.
- **Amazon S3** → Armazenamento eficiente para arquivos processados.
- **Amazon SES** → Serviço para envio de e-mails automáticos.
- **CI/CD Automatizado** → Pipeline de integração e entrega contínua via GitHub Actions.
- **Análise de Código** → Garantia de qualidade do código utilizando SonarQube.

---

## ▶️ Como Executar o Projeto (Framepack-Worker)

### Clonar o Repositório
```sh
git clone https://github.com/SofArc6Soat/framepack-worker-hackathon.git
```

---

### Executar com Docker Compose
#### 🐳 Docker (docker-compose)
1️⃣ **Navegue até o diretório do projeto:**
```sh
cd framepack-worker-hackathon/src/DevOps
```
2️⃣ **Configure o ambiente Docker:**
```sh
docker-compose up --build
```
3️⃣ **Acesse a aplicação:**
- API disponível em: `http://localhost:5002`
- Healthcheck da API: `http://localhost:5002/health`

---

### Executar com Kubernetes
#### ☸️ Kubernetes
Para executar o projeto com Kubernetes, siga os passos abaixo:

1️⃣ **Crie um arquivo `.env`** e configure as variáveis:
```plaintext
AWS_ACCESS_KEY_ID=your_access_key_id
AWS_SECRET_ACCESS_KEY=your_secret_access_key
AWS_REGION=your_region
```
2️⃣ **Aplique os arquivos YAML:**
```sh
kubectl apply -f 01-framepack-worker-deployment.yaml
kubectl apply -f 02-framepack-worker-service.yaml
kubectl apply -f 03-framepack-worker-hpa.yaml
```
3️⃣ **Verifique os pods ativos:**
```sh
kubectl get pods -l app=framepack-worker
```

---

## 📚 Documentações
Para acessar arquitetura, Domain Storytelling, Context Map, Linguagem Ubíqua, Event Storming e vídeos de demonstração, consulte o repositório de documentação:

🔗 **[Framepack-Doc-Hackathon](https://github.com/SofArc6Soat/framepack-doc-hackathon)**

---

## 👨‍💻 Autores

- **Anderson Lopez de Andrade** - RM: 350452
- **Henrique Alonso Vicente** - RM: 354583

---

## 🔗 Repositórios de Microserviços

- **Framepack-WebApi** → [🔗 GitHub](https://github.com/SofArc6Soat/framepack-api-hackathon)
- **Framepack-Worker** → [🔗 GitHub](https://github.com/SofArc6Soat/framepack-worker-hackathon)
- **Documentações** → [🔗 GitHub](https://github.com/SofArc6Soat/framepack-doc-hackathon)
