# 执行 docker build 之前请先编译发布GUI项目，为了减少容器体积，容器只导入了dotnet runtime ，容器中并未包含dotnet SDK，nodejs等编译环境
# dotnet publish --configuration Release -o out


FROM microsoft/dotnet:2.1-aspnetcore-runtime
MAINTAINER gavin.d.liu
EXPOSE 5001

WORKDIR /usr/local/apps

COPY ./out  .

CMD ["dotnet", "appstore-appservice.dll"]

# 执行 docker build --rm=true -t gavin/appstore-appservice:2.0 .

# 执行 docker run -d -p 8000:5001 gavin/appstore-appservice:2.0
