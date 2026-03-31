# 1. Etapa de compilación
FROM ://microsoft.com AS build
WORKDIR /app

# Copia todo y restaura
COPY . ./
RUN dotnet restore

# Publica el proyecto
RUN dotnet publish Agendamiento.Api/Agendamiento.Api.csproj -c Release -o out

# 2. Etapa de ejecución (REVISA ESTA LÍNEA 13)
FROM ://microsoft.com
WORKDIR /app
COPY --from=build /app/out .

# Configuración de puerto para Railway
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Agendamiento.Api.dll"]
