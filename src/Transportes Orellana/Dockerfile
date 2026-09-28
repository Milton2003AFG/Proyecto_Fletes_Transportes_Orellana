# Etapa 1: Compilación
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar csproj y restaurar paquetes NuGet
COPY ["Transportes Orellana.csproj", "./"]
RUN dotnet restore "Transportes Orellana.csproj"

# Copiar el código fuente completo y compilar
COPY . .
RUN dotnet publish "Transportes Orellana.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: Entorno de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Instalar dependencias nativas de fuentes requeridas por SkiaSharp / QuestPDF en Linux
RUN apt-get update && apt-get install -y libfontconfig1 && rm -rf /var/lib/apt/lists/*

# Copiar los binarios compilados
COPY --from=build /app/publish .

# Puerto HTTP estándar para Render
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# Comando final de inicio
ENTRYPOINT ["dotnet", "Transportes_Orellana.dll"]