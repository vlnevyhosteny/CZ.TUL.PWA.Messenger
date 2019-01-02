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

RUN openssl req -new -x509 -newkey rsa:2048 -keyout server.key -out server.cer -days 365 -passin pass:password -passout pass:password -subj /CN=localhost
RUN openssl pkcs12 -export -out server.pfx -inkey server.key -in server.cer -passin pass:password -passout pass:password

EXPOSE 5001

ENTRYPOINT ["dotnet", "CZ.TUL.PWA.Messenger.Server.dll", "--urls", "http://*:5000;https://*:5001"]