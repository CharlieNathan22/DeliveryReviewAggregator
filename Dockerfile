FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DeliveryReviewAggregator/DeliveryReviewAggregator.csproj", "DeliveryReviewAggregator/"]
RUN dotnet restore "./DeliveryReviewAggregator/./DeliveryReviewAggregator.csproj"
COPY . .
WORKDIR "/src/DeliveryReviewAggregator"
RUN dotnet build "./DeliveryReviewAggregator.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DeliveryReviewAggregator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DeliveryReviewAggregator.dll"]