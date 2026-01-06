# Riwi Courses Assessment API
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["Src/WebApi/WebApi.csproj", "Src/WebApi/"]
COPY ["Src/Application/Application.csproj", "Src/Application/"]
COPY ["Src/Domain/Domain.csproj", "Src/Domain/"]
COPY ["Src/Infrastructure/Infrastructure.csproj", "Src/Infrastructure/"]

RUN dotnet restore "Src/WebApi/WebApi.csproj"

# Copy all source files
COPY . .

# Build the application
WORKDIR "/src/Src/WebApi"
RUN dotnet build "WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Riwi.CoursesAssessment.WebApi.dll"]

