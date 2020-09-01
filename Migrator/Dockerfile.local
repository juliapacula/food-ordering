FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./*.sln ./
COPY ./*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*} && mv $file ${file%.*}; done
RUN dotnet restore --interactive

# Copy everything else and build
COPY . ./

RUN dotnet build -c Release --no-restore

WORKDIR /app/Migrator
RUN dotnet publish -c Release --no-build -o /app/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Migrator.dll"]
