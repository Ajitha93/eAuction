FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["SellerService/SellerService.csproj", "SellerService/"]
COPY ["BuyerService/BuyerService.csproj", "BuyerService/"]
COPY ["eAuction.Gateway/eAuction.Gateway.csproj", "eAuction.Gateway/"]
COPY ["eAuction.DataAccess/eAuction.DataAccess.csproj", "eAuction.DataAccess/"]
COPY ["eAuction.Models/eAuction.Models.csproj", "eAuction.Models/"]
COPY ["eAuction.DataAccess.Test/eAuction.DataAccess.Test.csproj", "eAuction.DataAccess.Test/"]
COPY ["eAuction.Business/eAuction.Business.csproj", "eAuction.Business/"]
COPY ["BuyerServiceConsumer/BuyerServiceConsumer.csproj", "BuyerServiceConsumer/"]
COPY ["SellerProcessingService/SellerProcessingService.csproj", "SellerProcessingService/"]
RUN dotnet restore "SellerService/SellerService.csproj"
COPY . .
WORKDIR "/src/SellerService"
RUN dotnet build "SellerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN apt-get update -yq \
    && apt-get install curl gnupg -yq \
    && curl -sL https://deb.nodesource.com/setup_14.x | bash \
    && apt-get install nodejs -yq
RUN dotnet publish "SellerService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SellerService.dll"]
