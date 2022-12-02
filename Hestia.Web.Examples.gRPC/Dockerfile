#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Hestia.Web.Examples.gRPC/Hestia.Web.Examples.gRPC.csproj", "Hestia.Web.Examples.gRPC/"]
RUN dotnet restore "Hestia.Web.Examples.gRPC/Hestia.Web.Examples.gRPC.csproj"
COPY . .
WORKDIR "/src/Hestia.Web.Examples.gRPC"
RUN dotnet build "Hestia.Web.Examples.gRPC.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Hestia.Web.Examples.gRPC.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hestia.Web.Examples.gRPC.dll"]