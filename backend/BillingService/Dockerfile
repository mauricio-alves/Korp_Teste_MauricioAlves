FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY BillingService.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .
EXPOSE 5002
ENV ASPNETCORE_URLS=http://+:5002
ENTRYPOINT ["dotnet", "BillingService.dll"]
