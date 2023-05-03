FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
#EXPOSE 80
#EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ZimaHrm.Web/ZimaHrm.Web.csproj", "ZimaHrm.Web/"]
COPY ["ZimaHrm.Core/ZimaHrm.Core.csproj", "ZimaHrm.Core/"]
COPY ["ZimaHrm.Data/ZimaHrm.Data.csproj", "ZimaHrm.Data/"]
COPY ["ZimaHrm.Infrastructure/ZimaHrm.Infrastructure.csproj", "ZimaHrm.Infrastructure/"]
RUN dotnet restore "ZimaHrm.Web/ZimaHrm.Web.csproj"
COPY . .
WORKDIR "/src/ZimaHrm.Web"
RUN dotnet build "ZimaHrm.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ZimaHrm.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZimaHrm.Web.dll"]
