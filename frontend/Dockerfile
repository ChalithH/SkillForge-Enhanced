# Production Dockerfile for React with multi-stage build and nginx
# Stage 1: Build environment
FROM node:20-alpine AS build
WORKDIR /app

# Build arguments for environment variables
ARG VITE_API_URL=http://localhost:5000/api
ARG VITE_SIGNALR_URL=http://localhost:5000

# Set environment variables for build
ENV VITE_API_URL=$VITE_API_URL
ENV VITE_SIGNALR_URL=$VITE_SIGNALR_URL

# Copy package files and install dependencies (better layer caching)
COPY package*.json ./
RUN npm ci --silent

# Copy source code
COPY . .

# Build the application for production (skip type checking for faster build)
RUN npx vite build

# Stage 2: Production runtime with nginx
FROM nginx:alpine AS runtime

# Install curl for health checks
RUN apk add --no-cache curl

# Copy custom nginx configuration
COPY nginx.conf /etc/nginx/nginx.conf

# Copy built application from build stage
COPY --from=build /app/dist /usr/share/nginx/html

# Nginx runs as root to bind to port 80 in container environment

# Expose port
EXPOSE 80

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost/ || exit 1

# Start nginx
CMD ["nginx", "-g", "daemon off;"]