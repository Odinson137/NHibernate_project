FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["NHibernate_project/NHibernate_project.csproj", "NHibernate_project/"]
RUN dotnet restore "NHibernate_project/NHibernate_project.csproj"
COPY . .
WORKDIR "/src/NHibernate_project"
RUN dotnet build "NHibernate_project.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NHibernate_project.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NHibernate_project.dll"]
