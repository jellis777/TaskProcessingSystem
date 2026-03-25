FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY TaskProcessing.Worker/TaskProcessing.Worker.csproj TaskProcessing.Worker/
COPY TaskProcessing.Api/TaskProcessing.Api.csproj TaskProcessing.Api/

RUN dotnet restore TaskProcessing.Worker/TaskProcessing.Worker.csproj

COPY TaskProcessing.Worker/ TaskProcessing.Worker/
COPY TaskProcessing.Api/ TaskProcessing.Api/

RUN dotnet publish TaskProcessing.Worker/TaskProcessing.Worker.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TaskProcessing.Worker.dll"]