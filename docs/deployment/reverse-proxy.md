# 反向代理（Nginx/Caddy）

Telegram Panel 的主后台已经迁移为 **Vue SPA**。常规后台页面和 API 只需要普通 HTTP 反向代理。

旧 Razor/Blazor 模块页面仍然作为兼容入口保留。如果你还在使用旧模块页面，反向代理也要保留 WebSocket 支持（`/_blazor`），否则旧页面可能卡住、断开或一直重连。

另外，如果你遇到「打开页面被跳到 `http://localhost/login?ReturnUrl=%2F`」这种情况，说明反代没有把正确的 `Host`/`Proto` 透传给上游应用。

## Nginx（HTTP）示例

注意：请确保你的上游是实际映射到容器 `5000` 的地址。例如默认 `docker-compose.yml` 是 `http://127.0.0.1:5000`；如果你改成 `18080:5000`，这里应写 `http://127.0.0.1:18080`。

```nginx
server {
  listen 80;
  server_name example.com;

  location / {
    proxy_pass http://127.0.0.1:5000;
    proxy_http_version 1.1;
    proxy_set_header Upgrade $http_upgrade;
    proxy_set_header Connection "Upgrade";
    proxy_set_header Host $host;
    proxy_set_header X-Forwarded-Host $host;
    proxy_set_header X-Forwarded-Proto $scheme;
    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;

    proxy_read_timeout 3600;
    proxy_send_timeout 3600;
  }
}
```

## Caddy 示例

```caddy
example.com {
  reverse_proxy 127.0.0.1:5000
}
```

如果你只使用 Vue 主后台，CDN/面板不需要额外启用 Blazor WebSocket。若仍有旧模块页面，请确认 CDN/面板对 WebSocket 的支持与超时设置。

## 宝塔面板（BT）反向代理要点

- 反代目标：`http://127.0.0.1:5000`（或你实际映射的宿主机端口，例如 `18080`）
- 需要透传：`Host` + `X-Forwarded-Host` + `X-Forwarded-Proto`（否则可能跳到 `localhost`）
- 旧 Razor/Blazor 模块页面仍需要 WebSocket；只用 Vue 主后台时不依赖 `/_blazor`

如果你要启用 Bot Webhook，还需要确保 Webhook 路径可外网访问：见 [Bot Webhook](bot-webhook.md)。
