# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies as distinct layers (leverage Docker cache)
COPY ["TaskManager.Api/TaskManager.Api.csproj", "TaskManager.Api/"]
RUN dotnet restore "TaskManager.Api/TaskManager.Api.csproj"

# Copy all source and publish
COPY . .
WORKDIR "/app/TaskManager.Api"
RUN dotnet publish "TaskManager.Api.csproj" -c Release -o /app/publish

# Stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .


ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT="Development"
EXPOSE 8080

ENTRYPOINT ["dotnet", "TaskManager.Api.dll"]