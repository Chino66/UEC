# UEC工具

项目地址：[https://github.com/Chino66/UEC](https://github.com/Chino66/UEC)

UEC是UPM Environment Configure的简称。它用于简单方便配置UPM开发所需要的配置内容而无需手动去编写配置。如何基于Github开发管理upm包，请参看：[https://github.com/Chino66/UPM\_Kits\_Develop](https://github.com/Chino66/UPM_Kits_Develop)

## 在Unity上安装：

1.  在Package Manager界面下选择git URL的方式引入包

![image](https://alidocs.oss-cn-zhangjiakou.aliyuncs.com/res/eLbnj8y5do1NOaNY/img/d6ce1124-7804-4533-84b2-f32cb27253d9.png)

2.  输入`https://github.com/Chino66/UEC.git#upm`回车或点击Install

![image](https://alidocs.oss-cn-zhangjiakou.aliyuncs.com/res/eLbnj8y5do1NOaNY/img/7bd72dbe-f212-4234-84fe-04af5a92e7cc.png)

3.  安装完成后可以在下方看

![image](https://alidocs.oss-cn-zhangjiakou.aliyuncs.com/res/eLbnj8y5do1NOaNY/img/28611d37-96fe-4c84-9e93-a879dba9fd35.png)

*   [ ] Unity2023引入包后报错，原因是Newtonsoft.Json命名空间改了变成Unity.Plastic.Newtonsoft.Json了需要处理

## 配置用户信息：

1.  打开UECWindow面板，Tools/UECWindow

![image](https://alidocs.oss-cn-zhangjiakou.aliyuncs.com/res/eLbnj8y5do1NOaNY/img/21300ecd-a866-4489-9020-ac482b1c6640.png)

![image](https://alidocs.oss-cn-zhangjiakou.aliyuncs.com/res/eLbnj8y5do1NOaNY/img/f1a82bfc-c94d-49bc-b67c-8a0515aaf0d1.png)

2.  添加你Github用户信息，点击“+”号添加

![image](https://alidocs.oss-cn-zhangjiakou.aliyuncs.com/res/eLbnj8y5do1NOaNY/img/9f4dd377-a863-45d3-9c7f-d248971e70d8.png)

**Username**是你的Github账号名称，它是访问Github的依据。

**Developer**勾选，同时会显示需要输入一个**Token**，这个Token从Gihtub官方获取，作为访问，下载，发布在Github上package的凭证。[如何获取Token](https://docs.github.com/zh/authentication/keeping-your-account-and-data-secure/managing-your-personal-access-tokens)

注意：Token是重要信息需要妥善保存！

**Scopes**添加查找的包范围，比如想要添加的包名是`com.chino.xx.yy`和`com.chino.zz`，那么添加范围是`com.chino`（如上图示例）

配置完成后点击Apply后如下所示：

![image](https://alidocs.oss-cn-zhangjiakou.aliyuncs.com/res/eLbnj8y5do1NOaNY/img/df7d1680-25b7-4b4c-9017-9faa41c4ba42.png)

:::
**注意**：你不是chino66，所以你不能在chino66上勾选Developer，但你要访问chino66的包如com.chino.upm.kits所以你需要添加chino66以及对应的Scopes。同时你需要配置自己的Github账号勾选Developer并填入Token
:::

3.  更新配置，点击Update将前面配好的信息更新，同时在PackageManager界面上后自动配置好访问信息

![image](https://alidocs.oss-cn-zhangjiakou.aliyuncs.com/res/eLbnj8y5do1NOaNY/img/035ef58f-0b81-4b5a-b80d-28df6f2a2bd7.png)

4.  查看配置的信息，点击UEC面板右上角的view config按钮，会跳转到user目录下，可以看到几个配置文件

![image](https://alidocs.oss-cn-zhangjiakou.aliyuncs.com/res/eLbnj8y5do1NOaNY/img/1e469224-f968-420e-a10b-5effa32c350f.png)

其中.uecconfig记录的就是面板上配置的信息

![image](https://alidocs.oss-cn-zhangjiakou.aliyuncs.com/res/eLbnj8y5do1NOaNY/img/518df3bb-697b-49e0-a444-4273c91121f5.png)

## 安装其他包

上面的信息配置完成后就可以方便访问Github上发布的package了，以面板上的`com.chino.upm.kits`为例，点击按钮即可安装这个包以及这个包对应的依赖包

![image](https://alidocs.oss-cn-zhangjiakou.aliyuncs.com/res/eLbnj8y5do1NOaNY/img/2f6617f5-3d54-4e18-a1a6-858b3a1a1082.png)

![image](https://alidocs.oss-cn-zhangjiakou.aliyuncs.com/res/eLbnj8y5do1NOaNY/img/7d283137-51d6-4d43-b132-5d574e9360fd.png)

红框的UPMKits就是安装的`com.chino.upm.kits`包名，绿框内的是UPMKits的依赖项，它们都在蓝框的chino66用户下，所有包名都符合`com.chino`前缀。

现在你可以使用UPMKits开发，发布，修改你自己的npm包了，关于UPMKits的使用参看：[https://github.com/Chino66/UPM\_Kits\_Develop](https://github.com/Chino66/UPM_Kits_Develop)