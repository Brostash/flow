#!/bin/bash

# This script sets up Git configuration in the container
# It will be executed as part of the post-create command

# Set up Git configurations
git config --global user.name "${GIT_USER_NAME:-VS Code Remote User}"
git config --global user.email "${GIT_USER_EMAIL:-vscode-remote@example.com}"
git config --global core.editor "code --wait"
git config --global init.defaultBranch main
git config --global pull.rebase false
git config --global push.default simple
git config --global credential.helper "cache --timeout=3600"

echo "Git configuration complete!"