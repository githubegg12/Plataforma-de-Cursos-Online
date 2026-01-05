FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CursosOnline.Web/CursosOnline.Web.csproj", "CursosOnline.Web/"]
COPY ["CursosOnline.Application/CursosOnline.Application.csproj", "CursosOnline.Application/"]
COPY ["CursosOnline.Domain/CursosOnline.Domain.csproj", "CursosOnline.Domain/"]
COPY ["CursosOnline.Infrastructure/CursosOnline.Infrastructure.csproj", "CursosOnline.Infrastructure/"]
COPY ["CursosOnline.Identity/CursosOnline.Identity.csproj", "CursosOnline.Identity/"]
RUN dotnet restore "CursosOnline.Web/CursosOnline.Web.csproj"
COPY . .
WORKDIR "/src/CursosOnline.Web"
RUN dotnet build "CursosOnline.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CursosOnline.Web.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CursosOnline.Web.dll"]
