FROM ://microsoft.com AS build
WORKDIR /app

# Copia los archivos y restaura dependencias
COPY . ./
RUN dotnet restore

# Compila la aplicación
RUN dotnet publish Agendamiento.Api/Agendamiento.Api.csproj -c Release -o out

# Imagen de ejecución
FROM ://microsoft.com
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "Agendamiento.Api.dll"]
