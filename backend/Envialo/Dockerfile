# ==========================================
# Etapa 1: Base de ejecución (Runtime)
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
# Render asigna dinámicamente un puerto a la variable de entorno PORT.
# ASP.NET Core 9 escucha por defecto en el puerto 8080, pero lo forzamos a leer PORT.
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# ==========================================
# Etapa 2: Compilación y Restauración (SDK)
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar los archivos de proyecto (.csproj) primero para aprovechar la caché de Docker
COPY ["Envialo.Domain/Envialo.Domain.csproj", "Envialo.Domain/"]
COPY ["Envialo.Application/Envialo.Application.csproj", "Envialo.Application/"]
COPY ["Envialo.Infrastructure/Envialo.Infrastructure.csproj", "Envialo.Infrastructure/"]
COPY ["Envialo.API/Envialo.API.csproj", "Envialo.API/"]

# Restaurar las dependencias de NuGet de toda la solución
RUN dotnet restore "Envialo.API/Envialo.API.csproj"

# Copiar el resto del código fuente
COPY . .
WORKDIR "/src/Envialo.API"

# Compilar el proyecto en modo Release sin volver a restaurar
RUN dotnet build "Envialo.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ==========================================
# Etapa 3: Publicación (Optimización)
# ==========================================
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Envialo.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ==========================================
# Etapa 4: Imagen Final Creada para Correr
# ==========================================
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Envialo.API.dll"]