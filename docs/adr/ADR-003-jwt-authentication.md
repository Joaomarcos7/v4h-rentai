# ADR-003: JWT para autenticação

**Data:** 2026-05-21  
**Status:** Aceito

## Contexto

O sistema tem dois perfis de usuário (Solicitante e Especialista) com permissões distintas nos endpoints. É necessário um mecanismo de autenticação stateless, compatível com o frontend Angular e com o hub SignalR.

## Decisão

Usar JWT (JSON Web Tokens) com:

- **Algoritmo:** HMAC-SHA256 (HS256) — simétrico, sem necessidade de infraestrutura PKI
- **Expiração:** 8 horas
- **Claims:** `sub` (userId), `email`, `role` (Solicitante | Especialista)
- **Secret:** variável de ambiente `JWT__Secret` (mínimo 32 caracteres)
- **Senha:** BCrypt com cost factor 12 (protege contra força bruta offline)
- **Angular:** `AuthInterceptor` injeta `Authorization: Bearer <token>` em todas as requisições
- **SignalR:** token passado via query string `access_token` para o handshake WebSocket

## Consequências

**Positivo:**
- Stateless — sem estado de sessão no servidor
- Nativo no ASP.NET Core com `Microsoft.AspNetCore.Authentication.JwtBearer`
- Claims com role permitem `[Authorize(Roles="Solicitante")]` nos controllers

**Negativo:**
- Sem refresh token — sessão expira em 8h (aceitável para o escopo)
- Token revogação requer lista negra (não implementado — fora de escopo)

## Alternativas Consideradas

- **Cookies de sessão:** stateful, requer session store; problemático com SignalR e CORS
- **OAuth2/OIDC (Keycloak):** robusto para produção, mas overhead de setup inviável para o prazo
