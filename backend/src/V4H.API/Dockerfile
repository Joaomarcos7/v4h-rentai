FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/V4H.API/V4H.API.csproj", "src/V4H.API/"]
COPY ["src/V4H.Application/V4H.Application.csproj", "src/V4H.Application/"]
COPY ["src/V4H.Infrastructure/V4H.Infrastructure.csproj", "src/V4H.Infrastructure/"]
COPY ["src/V4H.Domain/V4H.Domain.csproj", "src/V4H.Domain/"]
RUN dotnet restore "src/V4H.API/V4H.API.csproj"
COPY . .
WORKDIR "/src/src/V4H.API"
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "V4H.API.dll"]
