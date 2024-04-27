FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["NHibernate.Migration/NHibernate.Migration.csproj", "NHibernate.Migration/"]
RUN dotnet restore "NHibernate.Migration/NHibernate.Migration.csproj"
COPY . .
WORKDIR "/src/NHibernate.Migration"
RUN dotnet build "NHibernate.Migration.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NHibernate.Migration.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NHibernate.Migration.dll"]
