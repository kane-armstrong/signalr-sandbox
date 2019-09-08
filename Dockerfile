FROM microsoft/dotnet:2.2-sdk-alpine AS build
WORKDIR /app
COPY . ./
RUN dotnet restore

WORKDIR /app/src/SignalRSandbox.Web
RUN dotnet publish -c Release -o out

# Finalize image
FROM microsoft/dotnet:2.2-aspnetcore-runtime-alpine AS runtime
WORKDIR /app
COPY --from=build /app/src/SignalRSandbox.Web/out ./
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "SignalRSandbox.Web.dll"]