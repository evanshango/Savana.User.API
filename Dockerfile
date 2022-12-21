FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Savana.User.API/Savana.User.API.csproj", "Savana.User.API/"]
RUN dotnet restore "Savana.User.API/Savana.User.API.csproj"
COPY . .
WORKDIR "/src/Savana.User.API"
RUN dotnet build "Savana.User.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Savana.User.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Savana.User.API.dll"]
