FROM mcr.microsoft.com/dotnet/core/sdk AS build
WORKDIR /app
COPY * ./
RUN dotnet restore
RUN dotnet publish -c Release -o output

FROM mcr.microsoft.com/dotnet/core/aspnet
WORKDIR /app
COPY --from=build /app/output .
ENTRYPOINT [ "dotnet", "TempPi.dll" ]