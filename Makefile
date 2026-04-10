# Docker Compose is the default: builds and runs the Host on http://localhost:8080
# (Aspire AppHost is not used inside Compose; use `dotnet run` / `aspire run` locally for that.)

COMPOSE := docker compose
DOTNET_SDK_IMAGE ?= mcr.microsoft.com/dotnet/sdk:10.0
REPO_ROOT := $(dir $(abspath $(lastword $(MAKEFILE_LIST))))
HOST_PROJECT := src/CSharpModulith.Host/CSharpModulith.Host.csproj
CONFIG ?= Release

# Dev container (.NET SDK + non-root user with sudo); build with `make build-dev-image`
DEV_IMAGE ?= csharp-modulith-dev:latest
# Match bind-mounted repo ownership (override if not building on the machine that owns the files)
DEV_UID ?= $(shell id -u)
DEV_GID ?= $(shell id -g)
# Extra docker build options only (UID/GID are always passed below)
# Optional: pin CLI, e.g. DEV_BUILD_ARGS='--build-arg ASPIRE_CLI_VERSION=9.4'
DEV_BUILD_ARGS ?=
HOST_HTTP_PORT ?= 8080
APP_LISTEN_PORT ?= 8080

DOCKER_DEV := docker run --rm -v "$(REPO_ROOT):/src" -w /src $(DEV_IMAGE)

.PHONY: help build run up down logs shell test test-docker build-dev-image build-app run-app clean

help:
	@echo "Docker (default workflow):"
	@echo "  make build   Build the Host image (docker compose build)"
	@echo "  make run     Build (if needed) and start the stack in the foreground"
	@echo "  make up      Build and start in detached mode (host + csharp SDK container)"
	@echo "  SDK tooling: docker compose exec -T csharp dotnet test CSharpModulith.sln -c Release"
	@echo "  ArchUnitNET layer tests: tests/Architecture/CSharpModulith.Architecture.Tests (prefer -c Debug per ArchUnitNET docs)"
	@echo "  make down    Stop and remove containers"
	@echo "  make logs    Follow compose logs (use after make up)"
	@echo ""
	@echo "Dev SDK container (non-root devuser, passwordless sudo; no host dotnet required):"
	@echo "  make build-dev-image  Build $(DEV_IMAGE) from Dockerfile.dev"
	@echo "  make build-app   dotnet build Host inside the dev container"
	@echo "  make run-app     Build then run Host in the dev container (http://localhost:$(HOST_HTTP_PORT))"
	@echo "  make test        dotnet test solution inside the dev container"
	@echo "  make shell       Interactive bash as devuser in the dev container"
	@echo "  make test-docker Same as make test (dev container)"
	@echo "  Override ports: make run-app HOST_HTTP_PORT=5280 APP_LISTEN_PORT=5280"
	@echo "  Dev image uses DEV_UID/DEV_GID (default: \$$(id -u)/\$$(id -g)) so bin/obj on the mount stay writable"
	@echo "  Dev image includes Aspire CLI (aspire); pin with DEV_BUILD_ARGS='--build-arg ASPIRE_CLI_VERSION=<ver>' on build-dev-image"

build:
	$(COMPOSE) build

run:
	$(COMPOSE) up --build

up:
	$(COMPOSE) up -d --build

down:
	$(COMPOSE) down

logs:
	$(COMPOSE) logs -f

build-dev-image:
	docker build -f Dockerfile.dev -t $(DEV_IMAGE) $(DEV_BUILD_ARGS) \
		--build-arg DEV_UID=$(DEV_UID) \
		--build-arg DEV_GID=$(DEV_GID) \
		.

shell: build-dev-image
	docker run --rm -it -v "$(REPO_ROOT):/src" -w /src $(DEV_IMAGE) bash

test-docker: build-dev-image
	$(DOCKER_DEV) dotnet test CSharpModulith.sln -c $(CONFIG)

test: build-dev-image
	$(DOCKER_DEV) dotnet test CSharpModulith.sln -c $(CONFIG)

build-app: build-dev-image
	$(DOCKER_DEV) dotnet build $(HOST_PROJECT) -c $(CONFIG)

run-app: build-app
	docker run --rm -it \
		-p $(HOST_HTTP_PORT):$(APP_LISTEN_PORT) \
		-e ASPNETCORE_URLS=http://0.0.0.0:$(APP_LISTEN_PORT) \
		-v "$(REPO_ROOT):/src" -w /src $(DEV_IMAGE) \
		dotnet run --project $(HOST_PROJECT) -c $(CONFIG) --no-launch-profile --no-build

clean:
	$(COMPOSE) down -v --rmi local 2>/dev/null || true
	-docker run --rm -v "$(REPO_ROOT):/src" -w /src $(DEV_IMAGE) dotnet clean CSharpModulith.sln -c $(CONFIG)
	-dotnet clean CSharpModulith.sln -c $(CONFIG) 2>/dev/null || true
