#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["CommunicationLayer/Components/OrderComponent/OrderComponent.csproj", "CommunicationLayer/Components/OrderComponent/"]
RUN dotnet restore "CommunicationLayer/Components/OrderComponent/OrderComponent.csproj"
COPY . .
WORKDIR "/src/CommunicationLayer/Components/OrderComponent"
RUN dotnet build "OrderComponent.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderComponent.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderComponent.dll"]