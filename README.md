## ASP.NET Core Detection with Responsive

ASP.NET Core Detection service components for identifying details about client device, browser, engine, platform, & crawler. Responsive middleware for routing base upon request client device detection to specific view. Also in the added feature of user preference made this library even more comprehensive must for developers whom to target multiple devices with view rendered and optimized directly from the server side.

**Please show me some love and click the** :star:

<img src="https://raw.githubusercontent.com/wangkanai/Detection/dev/asset/aspnet-core-detection-3.svg?sanitize=true" width="650" alt="ASP.NET Core Detection" />

[![Build status](https://ci.appveyor.com/api/projects/status/033qv4nqv8g4altq?svg=true&retina=true)](https://ci.appveyor.com/project/wangkanai/detection)
[![NuGet Badge](https://buildstats.info/nuget/wangkanai.detection)](https://www.nuget.org/packages/wangkanai.detection)
[![NuGet Badge](https://buildstats.info/nuget/wangkanai.detection?includePreReleases=true)](https://www.nuget.org/packages/wangkanai.detection)
[![MyGet Badge](https://buildstats.info/myget/wangkanai/wangkanai.detection)](https://www.myget.org/feed/wangkanai/package/nuget/wangkanai.detection)

[![GitHub](https://img.shields.io/github/license/wangkanai/detection)](https://github.com/wangkanai/Detection/blob/dev/LICENSE)
[![Open Collective](https://img.shields.io/badge/open%20collective-support%20me-3385FF.svg)](https://opencollective.com/wangkanai)
[![Patreon](https://img.shields.io/badge/patreon-support%20me-d9643a.svg)](https://www.patreon.com/wangkanai)
 
[![Build history](https://buildstats.info/appveyor/chart/wangkanai/detection)](https://ci.appveyor.com/project/wangkanai/detection/history)

This project development has been in the long making of my little spare time. Please show your appreciation and help me provide feedback on you think will improve this library. All developers are welcome to come and improve the code by submit a pull request. We will have constructive good discussion together to the greater good.

## Installation

Installation of detection library is now done with a single package reference point.

```powershell
PM> install-package Wangkanai.Detection
```

This library host the component services to resolve the access client device type. To the servoces your web application is done by configuring the `Startup.cs` by adding the detection service in the `ConfigureServices` method.

```c#
public void ConfigureServices(IServiceCollection services)
{
    // Add detection services container and device resolver service.
    services.AddDetection();

    // Add framework services.
    services.AddControllersWithViews();
}
```

* `AddDetection()` Adds the detection services to the services container.

Or you can customize the responsive

```c#
public void ConfigureServices(IServiceCollection services)
{
    // Add responsive services.
    services.AddDetection(options =>
    {
        options.Responsive.DefaultTablet  = Device.Desktop;
        options.Responsive.DefaultMobile  = Device.Mobile;
        options.Responsive.DefaultDesktop = Device.Desktop;
        options.Responsive.IncludeWebApi  = false;
        options.Responsive.Disable        = false;
        options.Responsive.WebApiPath     = "/Api";
    });

    // Add framework services.
    services.AddControllersWithViews();
}
```

* `AddDetection(Action<DetectionOptions> options)` Adds the detection services to the services container.

The current device on a request is set in the Responsive middleware. The Responsive middleware is enabled in the `Configure` method of `Startup.cs` file.

```c#
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app.UseRouting();

    app.UseDetection();

    app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
}
```

Adding the TagHelper features to your web application with following in your `_ViewImports.cshtml`

```razor
@using WebApplication1

@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
@addTagHelper *, Wangkanai.Detection
```

## Detection Service

### Web Application

#### MVC

After you have added the basic of the Detection Services, let us learn how to utilized in your web application. Which we got the help from [dependency injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection) to access to `IDetectionService`. Here is how you would use in `Controller` of a [MVC pattern](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/) by injecting the detection service into the constructor of the controller. 

```c#
public class AboutController : Controller
{
    private readonly IDetectionService _detectionService;

    public AboutController(IDetectionService detectionService)
    {
        _detectionService = detectionService;
    }

    public IActionResult Index()
    {
        return View(_detectionService);
    }
}
```

#### Razor Pages

For [razor pages](https://docs.microsoft.com/en-us/aspnet/core/tutorials/razor-pages/) web application that only have the pages without page behind, you can access the detection service via the `@inject` tag after the `@page` in your _.cshtml_ files. Here would be the example below;

```razor
@page
@inject Wangkanai.Detection.Services.IDetectionService DetectionService
@{
    ViewData["Title"] = "Detection";
}
<ul>
    <li>@DetectionService.Device.Type</li>
    <li>@DetectionService.Browser.Name</li>
    <li>@DetectionService.Platform.Name</li>
    <li>@DetectionService.Engine.Name</li>
    <li>@DetectionService.Crawler.Name</li>
</ul>
```

While if you razor pages use the code behind model, you can still inject the detection service into it via the constructor just like the way MVC controller does it also.

```c#
public class IndexModel : PageModel
{
    private readonly IDetectionService _detectionService;

    public IndexModel(IDetectionService detectionService)
    {
        _detectionService = detectionService;
    }
    
    public void OnGet()
    {
        var device = _detectionService.Device.Type;
    }
}
```

### Middleware

Would you think that [Middleware](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/) can also use this detection service. Actually it can! and our [Responsive](#responsive-service) make good use of it too. Let us learn how that you would use detection service in your custom middleware which we would use the [Per-request middleware dependencies](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/write#per-request-middleware-dependencies). But why would we use pre-request injection for our middleware you may ask? Easy! because every user is unique. Technically answer would be that `IDetectionService` by using `TryAddTransient<TInterface, TClass>` where you can [learn more about transient](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection#transient). So now we know about the basic lets look at the code:

```c#
public class MyCustomMiddleware
{
    private readonly RequestDelegate _next;

    public MyCustomMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task InvokeAsync(HttpContext context, IDetectionService detection)
    {
        if(detection.Device.Type == Device.Mobile)
            context.Response.WriteAsync("You are Mobile!");

        await _next(context);
    }
}
```

### Fundamentals

Detection services would extract information about the visitor web client by parsing the [user agent](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/User-Agent) that the web browser gives to the web server on every http/https request. There have total of 5 resolver services the our detection services.

#### Device Resolver

#### Browser Resolver

#### Platform Resolver

#### Engine Resolver

#### Crawler Resolver

## Responsive Service

### Make your web app responsive

#### MVC

#### Razor Pages

#### Tag Helpers

#### User Preference

### Directory Structure

* `src` - The source code of this project lives here
* `test` - The test code of this project lives here
* `collection` - Collection of sample user agents for lab testing
* `sample` - Contains sample web application of usage
* `doc` - Contains the documentation on how utilized this library

### Contributing

All contribution are welcome, please contact the author.

## Contributors

### Code Contributors

This project exists thanks to all the people who contribute. [[Contribute](CONTRIBUTING.md)].
<a href="https://github.com/wangkanai/Detection/graphs/contributors"><img src="https://opencollective.com/wangkanai/contributors.svg?width=890&button=false" /></a>

### Financial Contributors

Become a financial contributor and help us sustain our community. [[Contribute](https://opencollective.com/wangkanai/contribute)]

#### Individuals

[![Individuals Contributors](https://opencollective.com/wangkanai/individuals.svg?width=890)](https://opencollective.com/wangkanai)

#### Organizations

Support this project with your organization. Your logo will show up here with a link to your website. 
[[Contribute](https://opencollective.com/wangkanai/contribute)]

<a href="https://opencollective.com/wangkanai/organization/0/website"><img src="https://opencollective.com/wangkanai/organization/0/avatar.svg"></a>
<a href="https://opencollective.com/wangkanai/organization/1/website"><img src="https://opencollective.com/wangkanai/organization/1/avatar.svg"></a>
<a href="https://opencollective.com/wangkanai/organization/2/website"><img src="https://opencollective.com/wangkanai/organization/2/avatar.svg"></a>
<a href="https://opencollective.com/wangkanai/organization/3/website"><img src="https://opencollective.com/wangkanai/organization/3/avatar.svg"></a>
<a href="https://opencollective.com/wangkanai/organization/4/website"><img src="https://opencollective.com/wangkanai/organization/4/avatar.svg"></a>
<a href="https://opencollective.com/wangkanai/organization/5/website"><img src="https://opencollective.com/wangkanai/organization/5/avatar.svg"></a>
<a href="https://opencollective.com/wangkanai/organization/6/website"><img src="https://opencollective.com/wangkanai/organization/6/avatar.svg"></a>
<a href="https://opencollective.com/wangkanai/organization/7/website"><img src="https://opencollective.com/wangkanai/organization/7/avatar.svg"></a>
<a href="https://opencollective.com/wangkanai/organization/8/website"><img src="https://opencollective.com/wangkanai/organization/8/avatar.svg"></a>
<a href="https://opencollective.com/wangkanai/organization/9/website"><img src="https://opencollective.com/wangkanai/organization/9/avatar.svg"></a>
