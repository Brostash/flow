# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

# Install system dependencies
RUN apt-get update && apt-get install -y \
    curl \
    gnupg \
    lsb-release \
    git \
    git-lfs \
    procps \
    wget \
    unzip \
    openssh-client \
    && rm -rf /var/lib/apt/lists/*

# Configure Git
RUN git config --system core.editor "code --wait" \
    && git config --system push.default simple \
    && git config --system pull.rebase false \
    && git lfs install

# Install Node.js (required for Azure Functions Core Tools)
RUN curl -fsSL https://deb.nodesource.com/setup_18.x | bash - \
    && apt-get install -y nodejs \
    && rm -rf /var/lib/apt/lists/*

# Install Azure Functions Core Tools
RUN npm install -g azure-functions-core-tools@4

# Install Azure CLI
RUN curl -sL https://aka.ms/InstallAzureCLIDeb | bash

# Set working directory
WORKDIR /src

# Copy project files first to leverage Docker cache
COPY *.csproj ./
RUN dotnet restore

# Copy remaining files
COPY . .

# Build and publish
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app

# Copy built artifacts from build stage
COPY --from=build-env /src/out .

# Pre-create the Azure Functions folder
RUN mkdir -p /root/.azure

# Set environment variables
ENV DOTNET_NOLOGO=true
ENV DOTNET_CLI_TELEMETRY_OPTOUT=true
ENV AzureWebJobsScriptRoot=/app

# Entry point for Azure Functions
ENTRYPOINT ["dotnet", "Microsoft.Azure.WebJobs.Script.WebHost.dll"]