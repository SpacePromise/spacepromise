FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY NuGet.Config ./
COPY spnode.app.docker/*.csproj ./spnode.app.docker/
COPY spnode/*.csproj ./spnode/
COPY spnode.communication.bolt/*.csproj ./spnode.communication.bolt/
COPY spnode.communication.proton/*.csproj ./spnode.communication.proton/
COPY spnode.utils/*.csproj ./spnode.utils/
COPY spnode.world.data/*.csproj ./spnode.world.data/
COPY spnode.world.procedural/*.csproj ./spnode.world.procedural/
WORKDIR /app/spnode.app.docker
RUN dotnet restore

# copy and publish app and libraries
WORKDIR /app/
COPY spnode.app.docker/. ./spnode.app.docker/
COPY spnode/. ./spnode/
COPY spnode.communication.bolt/. ./spnode.communication.bolt/
COPY spnode.communication.proton/. ./spnode.communication.proton/
COPY spnode.utils/. ./spnode.utils/
COPY spnode.world.data/. ./spnode.world.data/
COPY spnode.world.procedural/. ./spnode.world.procedural/
WORKDIR /app/spnode.app.docker
RUN dotnet publish -c Release -o out

# build runtime version
FROM microsoft/dotnet:2.1-runtime AS runtime
WORKDIR /app
COPY --from=build /app/spnode.app.docker/out ./
ENTRYPOINT ["dotnet", "spnode.app.docker.dll"]
