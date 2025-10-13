# ====== STAGE 1: BUILD ======
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy toàn bộ file vào container
COPY . .

# Restore dependencies
RUN dotnet restore "SWP_EVBatteryChangeStation_BE.csproj"

# Build và publish ứng dụng
RUN dotnet publish "SWP_EVBatteryChangeStation_BE.csproj" -c Release -o /app/publish

# ====== STAGE 2: RUNTIME ======
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copy từ stage build
COPY --from=build /app/publish .

# Chạy ứng dụng
ENTRYPOINT ["dotnet", "SWP_EVBatteryChangeStation_BE.dll"]
