# Learnify.ai MCP Azure Functions Deployment Guide

## Overview
This guide covers deploying the Learnify.ai MCP (Model Context Protocol) Azure Functions application that integrates in-process MCP server tools for managing courses, lessons, categories, quizzes, and answers.

## Architecture
- **learnify.ai.mcp.server**: Class library containing MCP tools (converted from console app)
- **learnify.ai.mcp.client**: Azure Functions app with in-process tool execution
- **In-Process Communication**: No stdio/process spawning, direct method calls via dependency injection

## Prerequisites
- .NET 8 SDK
- Azure CLI or Azure Portal access
- Azure Functions Core Tools (for local testing)
- Application Insights (recommended for monitoring)

## Configuration

### Required App Settings
Configure these in your Function App's Application Settings:

```json
{
  "AzureOpenAI:Endpoint": "https://your-openai-instance.openai.azure.com/",
  "AzureOpenAI:Key": "your-api-key",
  "AzureOpenAI:DeploymentName": "your-deployment-name",
  "ApiBaseUrl": "https://your-learnify-api.azurewebsites.net"
}
```

### local.settings.json (for local development)
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "AzureOpenAI:Endpoint": "https://your-openai-instance.openai.azure.com/",
    "AzureOpenAI:Key": "your-api-key",
    "AzureOpenAI:DeploymentName": "your-deployment-name",
    "ApiBaseUrl": "https://localhost:5001"
  }
}
```

## Deployment Methods

### Method 1: Azure Portal
1. Create a Function App:
   - Runtime: .NET 8 (isolated)
   - Plan: Consumption or Premium
   - Enable Application Insights

2. Deploy code:
   - Use Visual Studio publish profile
   - Or use VS Code Azure Functions extension
   - Or use GitHub Actions (see below)

3. Configure app settings in Azure Portal

### Method 2: Azure Functions Core Tools
```bash
# Build the project
dotnet build --configuration Release

# Navigate to the Functions project
cd learnify.ai.mcp.client

# Deploy to Azure
func azure functionapp publish <your-function-app-name>
```

### Method 3: Azure CLI
```bash
# Create resource group
az group create --name learnify-mcp-rg --location "East US"

# Create storage account
az storage account create \
  --name learnifymcpstorage \
  --location "East US" \
  --resource-group learnify-mcp-rg \
  --sku Standard_LRS

# Create Function App
az functionapp create \
  --resource-group learnify-mcp-rg \
  --consumption-plan-location "East US" \
  --runtime dotnet-isolated \
  --runtime-version 8 \
  --functions-version 4 \
  --name learnify-mcp-functions \
  --storage-account learnifymcpstorage

# Configure app settings
az functionapp config appsettings set \
  --name learnify-mcp-functions \
  --resource-group learnify-mcp-rg \
  --settings \
  "AzureOpenAI:Endpoint=https://your-openai-instance.openai.azure.com/" \
  "AzureOpenAI:Key=your-api-key" \
  "AzureOpenAI:DeploymentName=your-deployment-name" \
  "ApiBaseUrl=https://your-learnify-api.azurewebsites.net"

# Deploy using func tools
func azure functionapp publish learnify-mcp-functions
```

## GitHub Actions CI/CD

Create `.github/workflows/deploy-functions.yml`:

```yaml
name: Deploy Azure Functions

on:
  push:
    branches: [ main ]
    paths: [ 'backend/learnify.ai.api/learnify.ai.mcp.client/**' ]

env:
  AZURE_FUNCTIONAPP_NAME: 'learnify-mcp-functions'
  AZURE_FUNCTIONAPP_PACKAGE_PATH: 'backend/learnify.ai.api/learnify.ai.mcp.client'
  DOTNET_VERSION: '8.0.x'

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Restore dependencies
      run: dotnet restore
      working-directory: ${{ env.AZURE_FUNCTIONAPP_PACKAGE_PATH }}
    
    - name: Build project
      run: dotnet build --configuration Release --no-restore
      working-directory: ${{ env.AZURE_FUNCTIONAPP_PACKAGE_PATH }}
    
    - name: Publish project
      run: dotnet publish --configuration Release --no-build --output ./output
      working-directory: ${{ env.AZURE_FUNCTIONAPP_PACKAGE_PATH }}
    
    - name: Deploy to Azure Functions
      uses: Azure/functions-action@v1
      with:
        app-name: ${{ env.AZURE_FUNCTIONAPP_NAME }}
        package: ${{ env.AZURE_FUNCTIONAPP_PACKAGE_PATH }}/output
        publish-profile: ${{ secrets.AZURE_FUNCTIONAPP_PUBLISH_PROFILE }}
```

## Monitoring and Troubleshooting

### Application Insights
- Monitor function executions
- Track custom telemetry
- Set up alerts for failures

### Health Check Endpoint
Test the deployment:
```bash
curl https://your-function-app.azurewebsites.net/api/health
```

### Common Issues

1. **Cold Start Performance**
   - Consider Premium plan for better performance
   - Configure always-on if using App Service plan

2. **Configuration Issues**
   - Verify all app settings are configured
   - Check Application Insights logs

3. **Authentication**
   - Ensure Azure OpenAI credentials are correct
   - Verify API base URL is accessible

## Function Endpoints

After deployment, your function app will expose:

- `GET /api/health` - Health check and service status
- `POST /api/mcp/tools` - List available MCP tools
- `POST /api/mcp/call` - Execute MCP tools
- `POST /api/chat` - AI agent chat interface

## Scaling Considerations

### Consumption Plan
- Automatically scales based on demand
- Pay-per-execution model
- Cold start latency

### Premium Plan
- Pre-warmed instances
- Virtual network connectivity
- Longer execution duration

### App Service Plan
- Dedicated resources
- Predictable cost
- No cold starts

## Security

1. **Function-level Security**
   - Use function keys in production
   - Configure CORS if needed

2. **API Access**
   - Secure the Learnify API endpoints
   - Use managed identity where possible

3. **Configuration Security**
   - Use Key Vault for sensitive settings
   - Enable managed identity

## Next Steps

1. Set up monitoring alerts
2. Configure backup and disaster recovery
3. Implement API versioning strategy
4. Set up staging/production environments
5. Configure custom domains and SSL certificates

## Useful Commands

```bash
# View function logs
func azure functionapp logstream <function-app-name>

# Download app settings
func azure functionapp fetch-app-settings <function-app-name>

# Sync triggers
func azure functionapp sync-triggers <function-app-name>
```