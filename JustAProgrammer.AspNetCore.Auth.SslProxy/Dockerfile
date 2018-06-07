FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY JustAProgrammer.AspNetCore.Auth.SslProxy.sln ./
COPY JustAProgrammer.AspNetCore.Auth.SslProxy/JustAProgrammer.AspNetCore.Auth.SslProxy.csproj JustAProgrammer.AspNetCore.Auth.SslProxy/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/JustAProgrammer.AspNetCore.Auth.SslProxy
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "JustAProgrammer.AspNetCore.Auth.SslProxy.dll"]
