FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY CZ.TUL.PWA.Messenger.Server/CZ.TUL.PWA.Messenger.Server.csproj CZ.TUL.PWA.Messenger.Server/
RUN dotnet restore CZ.TUL.PWA.Messenger.Server/CZ.TUL.PWA.Messenger.Server.csproj
COPY . .
WORKDIR /src/CZ.TUL.PWA.Messenger.Server
RUN dotnet build CZ.TUL.PWA.Messenger.Server.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish CZ.TUL.PWA.Messenger.Server.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CZ.TUL.PWA.Messenger.Server.dll"]
