FROM alpine

FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS base
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /src
COPY ["src/CoffeeMachine.Web/CoffeeMachine.Web.csproj", "src/CoffeeMachine.Web/"]
COPY ["src/CoffeeMachine.Application/CoffeeMachine.Application.csproj", "src/CoffeeMachine.Application/"]
COPY ["src/CoffeeMachine.Infrastructure/CoffeeMachine.Infrastructure.csproj", "src/CoffeeMachine.Infrastructure/"]
COPY ["src/CoffeeMachine.Domain/CoffeeMachine.Domain.csproj", "src/CoffeeMachine.Domain/"]
RUN dotnet restore "src/CoffeeMachine.Web/CoffeeMachine.Web.csproj"
COPY . .
WORKDIR "/src/src/CoffeeMachine.Web"
RUN dotnet build "CoffeeMachine.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CoffeeMachine.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoffeeMachine.Web.dll"]