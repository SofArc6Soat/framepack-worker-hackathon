FROM mcr.microsoft.com/dotnet/aspnet:8.0.11-bookworm-slim AS base
RUN mkdir -p /app && adduser --disabled-password --gecos "" userapp && chown -R userapp /app
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0.404-bookworm-slim AS build
WORKDIR /src
COPY . .
RUN dotnet restore "Worker/Worker.csproj"
COPY . .
WORKDIR /src
RUN dotnet build "Worker/Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Worker/Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Worker.dll"]
