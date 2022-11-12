#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["LeedsBeerQuest.Api/LeedsBeerQuest.Api.csproj", "LeedsBeerQuest.Api/"]
RUN dotnet restore "LeedsBeerQuest.Api/LeedsBeerQuest.Api.csproj"
COPY . .
WORKDIR "/src/LeedsBeerQuest.Api"
RUN dotnet build "LeedsBeerQuest.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LeedsBeerQuest.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LeedsBeerQuest.Api.dll"]