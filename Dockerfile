FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Копируем ВСЕ файлы проекта (не только csproj)
COPY . .

# Восстанавливаем и публикуем
RUN dotnet restore "onitLab1/onitLab1.csproj"
RUN dotnet publish "onitLab1/onitLab1.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 80
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "onitLab1.dll"]
