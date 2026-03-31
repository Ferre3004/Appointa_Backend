# 1. Etapa de compilación (SDK de .NET 10)
FROM ://microsoft.com AS build
WORKDIR /app

# Copia todo el contenido de la raíz y restaura
COPY . ./
RUN dotnet restore

# Publica el proyecto apuntando a la carpeta de tu API
RUN dotnet publish Agendamiento.Api/Agendamiento.Api.csproj -c Release -o out

# 2. Etapa de ejecución (Runtime de .NET 10)
FROM ://microsoft.com
WORKDIR /app

# Copia los archivos publicados desde la etapa de construcción
COPY --from=build /app/out .

# Configuración de puerto para Railway
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# El nombre de la DLL es el nombre de tu proyecto .Api
ENTRYPOINT ["dotnet", "Agendamiento.Api.dll"]
