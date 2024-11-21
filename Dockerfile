FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ./*.props ./

COPY ["src/Lab5.Tools/Lab5.Tools.csproj", "src/Lab5.Tools/"]
COPY ["src/Application/Lab5.Tools.Application.Models/Lab5.Tools.Application.Models.csproj", "src/Application/Lab5.Tools.Application.Models/"]
COPY ["src/Presentation/Lab5.Tools.Presentation.Kafka/Lab5.Tools.Presentation.Kafka.csproj", "src/Presentation/Lab5.Tools.Presentation.Kafka/"]
COPY ["src/Application/Lab5.Tools.Application.Contracts/Lab5.Tools.Application.Contracts.csproj", "src/Application/Lab5.Tools.Application.Contracts/"]
COPY ["src/Application/Lab5.Tools.Application.Abstractions/Lab5.Tools.Application.Abstractions.csproj", "src/Application/Lab5.Tools.Application.Abstractions/"]
COPY ["src/Application/Lab5.Tools.Application/Lab5.Tools.Application.csproj", "src/Application/Lab5.Tools.Application/"]
COPY ["src/Infrastructure/Lab5.Tools.Infrastructure.Persistence/Lab5.Tools.Infrastructure.Persistence.csproj", "src/Infrastructure/Lab5.Tools.Infrastructure.Persistence/"]

RUN dotnet restore "src/Lab5.Tools/Lab5.Tools.csproj"

COPY . .
WORKDIR "/src/src/Lab5.Tools"
RUN dotnet build "Lab5.Tools.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Lab5.Tools.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Lab5.Tools.dll"]
