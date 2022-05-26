#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["./Discord_Bot.csproj", "./"]
RUN dotnet restore "./Discord_Bot.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "Discord_Bot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Discord_Bot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY "/token.txt" "/app/"
ENTRYPOINT ["dotnet", "Discord_Bot.dll"]