Software Version Manager
===

<img src="SVM.png" alt="SVM" height="150px" />

软件版本管理器是一个让软件支持更新和激活检查的网站和类库

通过使用SVM,您可以很方便的通知用户升级他们的软件并告知升级项目

以及为您的软件设置激活码,并且可以随时撤销或修改激活信息

该项目兼容 [WordWebCMS](https://github.com/LorisYounger/WordWebCMS) 您可以共享用户信息从您的博客/论坛至软件版本管理器

## 项目文件解释

### SoftwareVerison.Manager

一个网站可以

1. 让软件检查更新版本并提示用户升级
2. 让软件校对激活码确保激活码有效并对用户开放更多功能
3. 管理员可以发放更多激活码和为软件做出升级提醒
4. 用户可以检查自己的激活码并修改激活设备

### SoftwareVersion.Client

一个类库可以

1. 让软件检查更新版本并提示用户升级
2. 让软件校对激活码确保激活码有效并对用户开放更多功能

## 使用&部署

### SoftwareVerison.Manager

Software Version Manager 暂时未开发完毕 不推荐进行部署 (虽然说你也可以自己改了直接发)

#### 部署方法:

1. 运行 [setup.sql](https://github.com/LorisYounger/SoftwareVersionManager/blob/main/Software%20Version%20Manager/SetUp.sql) (注:如果是共用用户数据库 不需要运行创建用户表 见[setup.sql:L7](https://github.com/LorisYounger/SoftwareVersionManager/blob/main/Software%20Version%20Manager/SetUp.sql#L7)里的详细注释
2. 修改 [Web.config](https://github.com/LorisYounger/SoftwareVersionManager/blob/main/Software%20Version%20Manager/Web.config#L13) 中 connStr 和 connUsrStr 为自己的数据库连接方式 用户数据若使用相同数据库 则写一样的即可
3. 将 [**app.publish**](https://github.com/LorisYounger/SoftwareVersionManager/tree/main/Software%20Version%20Manager/app.publish) 内容上传至网站服务器根目录(不是根目录可能需要在设置内修改目录位置)

 ### SoftwareVersion.Client

1. 通过Parckage Manager

```
Install-Package SoftwareVersion.Client
```
2. 通过nuget.org

   https://www.nuget.org/packages/SoftwareVersion.Client/



