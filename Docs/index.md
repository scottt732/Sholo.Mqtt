![image](/images/Banner.png)

Sholo.MQTT is a lightweight ASP.NET Core-inspired framework for applications that consume and/or produce
MQTT messages.  It bridges [chkr1011/MQTTnet](https://github.com/chkr1011/MQTTnet) with the .NET Generic Host,
provides an [`IApplicationBuilder`-like pattern](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-5.0)
for configuring your MQTT subscriptions, and provides strongly-typed, validated databinding of MQTT payload and topic segments (similar to
ASP.NET Core's MVC's model binding).
