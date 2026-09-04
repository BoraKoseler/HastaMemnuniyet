# ---- Derleme aşaması ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Önce csproj dosyalarını kopyala ve restore et (katman cache'lemesi)
COPY src/HastaMemnuniyet.Domain/HastaMemnuniyet.Domain.csproj src/HastaMemnuniyet.Domain/
COPY src/HastaMemnuniyet.Application/HastaMemnuniyet.Application.csproj src/HastaMemnuniyet.Application/
COPY src/HastaMemnuniyet.Infrastructure/HastaMemnuniyet.Infrastructure.csproj src/HastaMemnuniyet.Infrastructure/
COPY src/HastaMemnuniyet.Web/HastaMemnuniyet.Web.csproj src/HastaMemnuniyet.Web/
RUN dotnet restore src/HastaMemnuniyet.Web/HastaMemnuniyet.Web.csproj

# Tüm kaynak kodu kopyala ve yayınla
COPY . .
RUN dotnet publish src/HastaMemnuniyet.Web/HastaMemnuniyet.Web.csproj \
    -c Release -o /app/publish --no-restore

# ---- Çalıştırma aşaması ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Render $PORT environment variable kullanır
ENV ASPNETCORE_URLS=http://+:${PORT:-10000}
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 10000

ENTRYPOINT ["dotnet", "HastaMemnuniyet.Web.dll"]
