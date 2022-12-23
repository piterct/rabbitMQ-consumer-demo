#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Demo1-consumer/Demo1-consumer.csproj", "Demo1-consumer/"]
RUN dotnet restore "Demo1-consumer/Demo1-consumer.csproj"
COPY . .
WORKDIR "/src/Demo1-consumer"
RUN dotnet build "Demo1-consumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Demo1-consumer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Demo1-consumer.dll"]