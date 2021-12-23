#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["CommunicationLayer/Gateways/WebGateway/WebGateway.csproj", "CommunicationLayer/Gateways/WebGateway/"]
RUN dotnet restore "CommunicationLayer/Gateways/WebGateway/WebGateway.csproj"
COPY . .
WORKDIR "/src/CommunicationLayer/Gateways/WebGateway"
RUN dotnet build "WebGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebGateway.dll"]
