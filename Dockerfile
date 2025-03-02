FROM mcr.microsoft.com/dotnet/sdk:8.0

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

# Set the working directory
WORKDIR /workspaces

# Pre-create the Azure Functions folder to avoid permission issues
RUN mkdir -p /root/.azure

# Set environment variables
ENV DOTNET_NOLOGO=true
ENV DOTNET_CLI_TELEMETRY_OPTOUT=true