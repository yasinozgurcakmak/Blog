FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Blog/Blog.csproj", "Blog/"]
COPY ["BusinessLayer/BusinessLayer.csproj", "BusinessLayer/"]
COPY ["DataAccessLayer/DataAccessLayer.csproj", "DataAccessLayer/"]
COPY ["DtoLayer/DtoLayer.csproj", "DtoLayer/"]
COPY ["EntityLayer/EntityLayer.csproj", "EntityLayer/"]
RUN dotnet restore "Blog/Blog.csproj"
COPY . .
WORKDIR "/src/Blog"
RUN dotnet publish "Blog.csproj" -c Release -o /app/publish /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
ENV ASPNETCORE_ENVIRONMENT=Production
COPY --from=build /app/publish .
RUN mkdir -p /app/wwwroot/uploads
EXPOSE 8080
ENTRYPOINT ["dotnet", "Blog.dll"]