📦 OrderAccumulator & OrderGenerator
Uma solução para integração FIX, composta por serviços de Acceptor e Initiator, além de workers para processamento de ordens e um frontend em React.

📝 Descrição
Projeto desenvolvido para simular comunicação via protocolo FIX, com envio e recebimento de ordens financeiras, incluindo backend em .NET 10 e frontend em React.

🚀 Tecnologias utilizadas
Linguagem: C#

Framework: .NET 10

Frontend: React 19.2.7

Bibliotecas principais:

QuickFIX/n (para comunicação FIX)

ASP.NET Core (para a WebAPI do Initiator)

BackgroundService (para execução de workers)

SignalR (para comunicação em tempo real entre backend e frontend) 

NSubstitute / xUnit (para testes unitários)

⚙️ Instalação e uso
Pré-requisitos
.NET 10 SDK instalado

Node.js e npm instalados para rodar o frontend React

Arquivos de configuração FIX (.cfg) disponíveis em Config/

Passos
Clone o repositório:

bash
git clone https://github.com/WarnerBatista/FlowaChallenge
cd seu-repositorio
Restaure as dependências do backend:

bash
dotnet restore
Execute o FixAcceptor (console app):

bash
dotnet run --project OrderAccumulator
Execute o FixInitiator (WebAPI):

bash
dotnet run --project OrderGenerator.Api
Instale e rode o frontend React:

bash
cd frontend
npm install
npm start
Para rodar os testes:

bash
dotnet test

🧪 Estrutura
OrderAccumulator → Console app que atua como FIX Acceptor

OrderGenerator.Api → WebAPI que atua como FIX Initiator

OrderWorker → Worker responsável por consumir ordens da fila e enviá-las via FIX

Frontend React → Interface para interação com o sistema (versão 19.2.7)

UnitTests → Testes unitários cobrindo serviços e workers

## 🔮 Features Futuras

Algumas funcionalidades planejadas, mas ainda não implementadas:

- Worker dedicado para envio de notificações via SignalR
- Exibição do histórico de ordens no frontend React
- Testes de integração cobrindo fluxo completo (backend + frontend + FIX)
- Monitoramento e métricas de performance dos serviços
- Docker Compose para orquestração simplificada de backend e frontend

📌 Challenge
This is a challenge by Coodesh