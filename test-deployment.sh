#!/bin/bash

echo "=== SkillForge Azure Deployment Test ==="
echo "Time: $(date)"
echo ""

echo "1. Backend Health Check:"
curl -s http://skillforge-backend.australiaeast.azurecontainer.io:5000/api/health
echo -e "\n"

echo "2. Frontend Status:"
curl -s -o /dev/null -w "HTTP Status: %{http_code}\n" http://skillforge-frontend.australiaeast.azurecontainer.io
echo ""

echo "3. Testing CORS (Register endpoint):"
curl -X POST http://skillforge-backend.australiaeast.azurecontainer.io:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -H "Origin: http://skillforge-frontend.australiaeast.azurecontainer.io" \
  -d '{"email":"test@example.com","password":"Test123!","name":"Test User"}' \
  -w "\nHTTP Status: %{http_code}\n" \
  -s

echo -e "\n4. Live URLs:"
echo "Frontend: http://skillforge-frontend.australiaeast.azurecontainer.io"
echo "Backend API: http://skillforge-backend.australiaeast.azurecontainer.io:5000/api"
echo "Backend Health: http://skillforge-backend.australiaeast.azurecontainer.io:5000/api/health"