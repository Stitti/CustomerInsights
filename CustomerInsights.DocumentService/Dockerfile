# Stellen Sie sicher, dass der Service Fabric-Cluster, für den Sie bereitstellen, das von Ihnen verwendete Containerimage unterstützt. Informationen zur Versionskompatibilität von Windows-Containern finden Sie unter "https://aka.ms/containerimagehelp".
FROM mcr.microsoft.com/dotnet/aspnet:9.0-nanoserver-1809
ARG source
WORKDIR /app
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "CustomerInsights.DocumentService.dll"]
