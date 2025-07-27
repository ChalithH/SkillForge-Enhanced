# SkillForge - Professional Skill Exchange Platform

A full-stack skill exchange platform where professionals earn time credits by teaching skills and spend them to learn from others. Successfully deployed to Microsoft Azure with complete Docker containerization.

## Theme Relevance: Networking

SkillForge directly embodies the "Networking" theme by creating meaningful professional connections through skill sharing. The platform facilitates networking in multiple ways:

- **Professional Skill Networks**: Users connect based on complementary skills, creating learning partnerships
- **Time Credit Economy**: Encourages active participation and mutual value exchange within the network
- **Real-time Communication**: SignalR enables instant networking through live notifications and status updates
- **Community Building**: Users build reputations and relationships through the review and rating system
- **Knowledge Sharing Network**: Transforms individual expertise into collective community learning

The platform goes beyond simple social networking by creating economic incentives for knowledge sharing, making professional networking both meaningful and mutually beneficial.

## Unique Features & Technical Highlights

### Production-Grade Cloud Architecture
- **Complete Azure deployment** with Container Instances, Container Registry, and SQL Database
- **Multi-stage Docker builds** for optimized production containers
- **Automated database migrations** in cloud environment
- **Professional nginx configuration** with security headers and caching

### Advanced Real-time Networking
- **SignalR WebSocket implementation** for instant communication
- **Live online/offline status** indicators creating dynamic networking presence
- **Real-time notifications** for skill exchange requests and updates
- **Instant feedback loops** encouraging active community participation

### Intelligent Time Credit Economy
- **Balanced learning ecosystem** where teaching earns credits to spend on learning
- **Prevents one-sided consumption** ensuring mutual value exchange
- **Gamified skill sharing** with credit tracking and transaction history
- **Economic incentives** for quality teaching and active participation

## Live Deployment
- **Frontend**: http://skillforge-frontend.australiaeast.azurecontainer.io
- **Backend API**: http://skillforge-backend.australiaeast.azurecontainer.io:5000
- **Health Check**: http://skillforge-backend.australiaeast.azurecontainer.io:5000/api/health

## Architecture

### Frontend
- **React 18** with TypeScript for type safety
- **Tailwind CSS** for responsive design
- **Redux Toolkit** for state management
- **Axios** for API communication
- **React Router** for navigation

### Backend
- **.NET 8 Web API** with Entity Framework Core
- **JWT Authentication** with refresh tokens
- **Azure SQL Database** with automated migrations
- **SignalR** for real-time features
- **Health monitoring** endpoints

### Infrastructure
- **Azure Container Instances** for scalable deployment
- **Azure Container Registry** for image management
- **Azure SQL Database** for data persistence
- **Docker multi-stage builds** for optimized containers
- **nginx** reverse proxy for frontend

## Demo Video

**[SkillForge Demo Video - 5 minutes](https://www.youtube.com/watch?v=7qwzlWVy1w4)**

### Video Content Overview
- Project introduction and networking theme relevance
- Demonstration of unique features and technical achievements
- Live application walkthrough showing all basic and advanced requirements
- Architecture overview highlighting Azure deployment and containerization

## Advanced Features (MSA Requirements)

### 1. Docker Containerization
- **Multi-stage Docker builds** for optimized production images
- **Container orchestration** with Azure Container Instances
- **Environment-specific configurations** for development and production
- **Automated deployment** using containerized architecture

### 2. Real-time Communication (WebSockets)
- **SignalR implementation** for live user interactions
- **Real-time notifications** for exchange requests and updates
- **Live online/offline status** indicators
- **Instant messaging** capabilities between users

### 3. Advanced State Management (Redux)
- **Redux Toolkit** for predictable state management
- **Complex application state** handling across components
- **Optimistic updates** for better user experience
- **Centralized data flow** for scalable architecture

## Core Features

### User Management
- Secure registration and authentication
- JWT-based authorization
- User profiles with skill management
- Time credit system (earn by teaching, spend by learning)

### Skill Exchange System
- Browse available skills by category
- Request skill exchanges with other users
- Time credit transactions
- Review and rating system

### Real-time Features
- Live notifications for exchange requests
- Online/offline status indicators
- Real-time updates via SignalR

## Deployment

### Container Configuration
- **Multi-stage Docker builds** for production optimization
- **Environment-specific configurations** (development/production)
- **Automated database migrations** on startup
- **Health checks** for container monitoring

## Local Development

### Prerequisites
- Docker Desktop
- .NET 8 SDK (for local development)
- Node.js 18+ (for local development)

### Quick Start
```bash
# Clone repository
git clone [repository-url]
cd skillforge

# Start with Docker Compose
docker-compose up

# Access locally
# Frontend: http://localhost:3000
# Backend: http://localhost:5000
```

### Development Commands
```bash
# Backend (local development)
cd backend
dotnet run
dotnet ef migrations add [MigrationName]
dotnet ef database update

# Frontend (local development)  
cd frontend
npm install
npm run dev
npm run build
```

## Production Deployment

The application is containerized and ready for cloud deployment:

```bash
# Build production images
docker build -t skillforge-backend ./backend
docker build -t skillforge-frontend ./frontend

# Deploy to Azure Container Instances
# Application deployed using Azure CLI and Docker commands
```

## Technical Achievements

### Performance & Scalability
- **Container orchestration** with Azure Container Instances
- **Database connection pooling** for efficient resource usage
- **Static asset optimization** with nginx caching
- **Responsive design** for mobile and desktop

### Security
- **JWT authentication** with secure token handling
- **CORS configuration** for cross-origin security
- **SQL injection prevention** via Entity Framework
- **Input validation** on both frontend and backend

### DevOps & Monitoring
- **Health check endpoints** for monitoring
- **Automated database migrations** 
- **Environment-based configuration**
- **Docker best practices** with multi-stage builds

## Future Enhancements
- Enhanced search and filtering
- Email notifications for exchanges
- Calendar integration
- Mobile application
- Advanced matching algorithms

---

**Built with modern full-stack technologies and deployed to Microsoft Azure**