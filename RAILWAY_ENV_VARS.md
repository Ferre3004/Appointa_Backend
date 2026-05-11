# Variables de entorno requeridas en Railway

Configurar estas variables en el panel de Railway → Service → Variables:

```
ConnectionStrings__DefaultConnection=Host=db.sjtksmwihrktbdfsyebg.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=ZBaV6XlcsntVfcLh;SSL Mode=Require;Trust Server Certificate=true

Jwt__Key=a3f8c2d1e4b7f9a0c3d2e1f4b8a7c6d5e3f2a1b4c7d8e9f0a3b2c1d4e7f8a9b0

Twilio__AccountSid=AC10b408612c72444def5152ac1237a620
Twilio__AuthToken=dde7c63c9093acca0dae1540d2cbd65a

LemonSqueezy__ApiKey=eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9...
LemonSqueezy__StoreId=338189
LemonSqueezy__WebhookSecret=nuevedeORO

App__FrontendUrl=https://prenotare.pro
```

## Notas
- `.NET` mapea `Section__Key` (doble guión bajo) a la sección correspondiente en `IConfiguration`.
- `appsettings.json` en el repositorio NO debe contener secretos reales.
- `appsettings.Development.json` contiene los secretos para desarrollo local y NO debe commitearse.
- Agregar `appsettings.Development.json` al `.gitignore` si se usa git.
