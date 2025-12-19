FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore "TelegramPanel.sln"
RUN dotnet publish "src/TelegramPanel.Web/TelegramPanel.Web.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

# 持久化目录：/data（通过 docker-compose 挂载）
# - 数据库：/data/telegram-panel.db
# - session：/data/sessions/
# - 本地配置：/data/appsettings.local.json（UI 保存 Telegram ApiId/ApiHash/同步开关等）
# - 后台密码：/data/admin_auth.json
RUN mkdir -p /data /data/sessions /data/logs \
    && rm -rf /app/logs \
    && ln -s /data/logs /app/logs \
    && ln -s /data/appsettings.local.json /app/appsettings.local.json || true

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TelegramPanel.Web.dll"]

