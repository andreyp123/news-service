#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# to run on raspberry pi
FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim-arm32v7 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["NewsService/NewsService.csproj", "NewsService/"]
RUN dotnet restore "NewsService/NewsService.csproj"
COPY . .
WORKDIR "/src/NewsService"
RUN dotnet build "NewsService.csproj" -c Release -r linux-arm -o /app/build

FROM build AS publish
RUN dotnet publish "NewsService.csproj" -c Release -r linux-arm -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NewsService.dll"]