FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8084
EXPOSE 8085

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PaymentService/PaymentService.csproj", "PaymentService/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "PaymentService/PaymentService.csproj"
COPY . .
WORKDIR "/src/PaymentService"
RUN dotnet build "./PaymentService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PaymentService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentService.dll"]
