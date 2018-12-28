FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/out .

EXPOSE 5001

ENTRYPOINT ["dotnet", "CZ.TUL.PWA.Messenger.Server.dll", "--urls", "http://*:5000;https://*:5001"]