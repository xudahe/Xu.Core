// NetCore 部分     案例地址：https://www.cnblogs.com/laozhang-is-phi/p/beautifulPublish-mostBugs.html#autoid-2-1-0

0、编译项目，保证代码没问题，并配置 CORS 跨域服务；
   c.AddPolicy("LimitRequests", policy =>
   {
       // 支持多个域名端口，注意端口号后不要带/斜杆：比如localhost:8000/，是错的
       // 注意，http://127.0.0.1:1818 和 http://localhost:1818 是不一样的，尽量写两个
       policy
       .WithOrigins("http://127.0.0.1:1002", "http://localhost:1002")
       .AllowAnyHeader()
       .AllowAnyMethod();
   });

①、发布项目；执行Xu.Publish.bat   //请注意我的 publish 路径是 bin/Debug/netcore3.1，因为这样肯定不会漏掉文件！！！

②、拷贝到服务器指定文件夹；

③、IIS 添加站点；// 端口 1001

④、修改应用程序池为“无托管”；//这也是为啥要安装 windows hosting 的原因

⑤、安装 windows hosting（服务托管）；//这是一个捆包，已经包含 Runtime
      下载地址： https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-desktop-3.1.4-windows-x64-installer

⑥、检查是否有指定的 Runtime（运行时）；

⑦、重启项目，查看是否正常；http://localhost:1001/