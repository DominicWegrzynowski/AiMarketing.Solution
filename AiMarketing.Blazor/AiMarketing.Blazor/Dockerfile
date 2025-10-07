# ─── Stage 1: Build the Blazor Server app ─────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy everything and restore dependencies
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ─── Stage 2: Run ─────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Railway (and most containers) expect port 8080
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# 👇 Replace with *your* project’s DLL name (check bin/Release/net8.0/)
ENTRYPOINT ["dotnet", "AiMarketing.Blazor.dll"]
