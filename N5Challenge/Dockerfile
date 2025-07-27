FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["N5Challenge/N5Challenge.csproj", "N5Challenge/"]
RUN dotnet restore "N5Challenge/N5Challenge.csproj"
COPY . .
WORKDIR "/src/N5Challenge"
RUN dotnet build "./N5Challenge.csproj" -c $BUILD_CONFIGURATION -o /app/build 

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./N5Challenge.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false 

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "N5Challenge.dll"]
