FROM mcr.microsoft.com/dotnet/sdk:6.0 AS builder

ARG PACKAGE_VERSION
ARG NUGET_KEY
ARG NUGET_URL=https://api.nuget.org/v3/index.json  
WORKDIR /sln

COPY . .

RUN dotnet restore 
RUN dotnet build -c Release --no-restore  
RUN dotnet pack -c Release --no-restore --no-build -o /out/artifacts
RUN dotnet dotnet nuget push /out/artifacts/*.nupkg --skip-duplicate --source $NUGET_URL --api-key $NUGET_KEY
