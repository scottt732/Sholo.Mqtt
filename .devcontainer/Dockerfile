FROM mcr.microsoft.com/devcontainers/dotnet:7.0-bookworm

ARG USERNAME=vscode

RUN set -x \
    && curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key | gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg \
    && echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_20.x nodistro main" > /etc/apt/sources.list.d/nodesource.list \
    && curl -sSL -O https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && apt-get update \
    && apt-get install -y apt-transport-https ca-certificates libc6 libgcc1 libgssapi-krb5-2 libicu72 libssl3 libstdc++6 zlib1g gnupg tmux curl nano bat direnv htop shellcheck jq mosquitto-clients dotnet-runtime-6.0 nodejs

# install jless (because its not available at https://containers.dev/features at time of writing)
RUN set -x \
    && NONINTERACTIVE=1 /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)" \
    && (echo; echo 'eval "$(/home/linuxbrew/.linuxbrew/bin/brew shellenv)"') >> /home/vscode/.profile \
    && eval "$(/home/linuxbrew/.linuxbrew/bin/brew shellenv)" \
    && brew install jless
