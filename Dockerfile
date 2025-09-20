FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Barbearia.sln", "./"]
COPY ["src/Barbearia.Api/Barbearia.Api.csproj", "src/Barbearia.Api/"]
COPY ["src/Barbearia.Application/Barbearia.Application.csproj", "src/Barbearia.Application/"]
COPY ["src/Barbearia.Infrastructure/Barbearia.Infrastructure.csproj", "src/Barbearia.Infrastructure/"]
COPY ["src/Barbearia.Domain/Barbearia.Domain.csproj", "src/Barbearia.Domain/"]
RUN dotnet restore "src/Barbearia.Api/Barbearia.Api.csproj"
COPY . .
WORKDIR "/src/src/Barbearia.Api"
RUN dotnet publish "Barbearia.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Barbearia.Api.dll"]
