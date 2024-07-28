// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.WebApiConfiguration
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Routing;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class WebApiConfiguration
  {
    public const string FallbackRouteName = "FallbackRoute";
    public const string FallbackRouteArea = "Fallback";
    public const string FallbackRouteResourceName = "NotFound";
    private static bool s_initialized;
    private static Exception s_initializeEx;
    private static HttpConfiguration s_configuration;
    private static HttpServer s_httpServer;
    private static string s_virtualPathRoot;
    private static ResourceAreaCollection s_resourceAreas;

    internal static void Initialize(IVssRequestContext requestContext)
    {
      if (WebApiConfiguration.s_initialized)
        return;
      if (WebApiConfiguration.s_initializeEx != null)
        throw new WebApiConfiguration.WebApiInitializationPreviouslyFailedException(WebApiConfiguration.s_initializeEx);
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
        vssRequestContext.GetService<ILocationService>();
        string virtualPathRoot = WebApiConfiguration.GetVirtualPathRoot(vssRequestContext);
        ResourceAreaCollection areas = new ResourceAreaCollection();
        HttpConfiguration httpConfiguration;
        HttpServer httpServer;
        if (HostingEnvironment.IsHosted)
        {
          httpConfiguration = GlobalConfiguration.Configuration;
          httpServer = GlobalConfiguration.DefaultServer;
        }
        else
        {
          httpConfiguration = new HttpConfiguration(new HttpRouteCollection(virtualPathRoot));
          httpServer = new HttpServer(httpConfiguration);
          string str = !string.IsNullOrEmpty(vssRequestContext.ServiceHost.PhysicalDirectory) ? vssRequestContext.ServiceHost.PhysicalDirectory : AppDomain.CurrentDomain.BaseDirectory;
          IVssAssembliesResolverService service = vssRequestContext.GetService<IVssAssembliesResolverService>();
          IAssembliesResolver resolverForPath = service.GetResolverForPath(requestContext, str);
          string path = Path.Combine(str, "bin");
          if (Directory.Exists(path) && resolverForPath.GetAssemblies().Count == 0)
            resolverForPath = service.GetResolverForPath(requestContext, path);
          httpConfiguration.Services.Replace(typeof (IAssembliesResolver), (object) resolverForPath);
        }
        httpConfiguration.Formatters.Remove((MediaTypeFormatter) httpConfiguration.Formatters.XmlFormatter);
        httpConfiguration.Formatters.Remove((MediaTypeFormatter) httpConfiguration.Formatters.JsonFormatter);
        httpConfiguration.Formatters.Add((MediaTypeFormatter) new ServerVssJsonMediaTypeFormatter());
        httpConfiguration.Formatters.Add((MediaTypeFormatter) new VssJsonPatchMediaTypeFormatter());
        WebApiConfiguration.RegisterResourceRoutes(vssRequestContext, httpConfiguration, areas);
        httpConfiguration.Routes.RegisterResourceOptionsHandler();
        WebApiConfiguration.RegisterFallbackRoute(httpConfiguration);
        WebApiConfiguration.VerifyNoDuplicateControllerNames(httpConfiguration);
        httpConfiguration.MessageHandlers.Add((DelegatingHandler) new TfsRequestMessageHandler());
        httpConfiguration.Filters.Add((IFilter) new VssfAuthorizationFilterAttribute());
        httpConfiguration.Filters.Add((IFilter) new ActivityLoggingAttribute());
        httpConfiguration.Services.Replace(typeof (IExceptionHandler), (object) new VssExceptionHandler());
        ILockName lockName = vssRequestContext.ServiceHost.CreateLockName("WebApiConfiguration/InitLock");
        using (vssRequestContext.Lock(lockName))
        {
          if (WebApiConfiguration.s_initialized)
            return;
          WebApiConfiguration.s_configuration = httpConfiguration;
          WebApiConfiguration.s_httpServer = httpServer;
          WebApiConfiguration.s_virtualPathRoot = virtualPathRoot;
          WebApiConfiguration.s_resourceAreas = areas;
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          VssSecureJsonConverterHelper.Validate = WebApiConfiguration.\u003C\u003EO.\u003C0\u003E__VssSecureJsonConverterObjectValidation ?? (WebApiConfiguration.\u003C\u003EO.\u003C0\u003E__VssSecureJsonConverterObjectValidation = new Action<object, JsonSerializer>(ServerJsonSerializationHelper.VssSecureJsonConverterObjectValidation));
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          PublicAccessJsonConverter.ShouldClearValueFunction = WebApiConfiguration.\u003C\u003EO.\u003C1\u003E__IsCurrentContextAnonymousOrPublic ?? (WebApiConfiguration.\u003C\u003EO.\u003C1\u003E__IsCurrentContextAnonymousOrPublic = new Func<bool>(ServerJsonSerializationHelper.IsCurrentContextAnonymousOrPublic));
          WebApiConfiguration.s_initialized = true;
          WebApiConfiguration.TraceRoutes(requestContext, httpConfiguration.Routes);
        }
      }
      catch (Exception ex)
      {
        WebApiConfiguration.s_initializeEx = ex;
        throw;
      }
    }

    private static void TraceRoutes(IVssRequestContext requestContext, HttpRouteCollection routes)
    {
      foreach (IHttpRoute route in routes)
      {
        if (route is GroupedWebApiRoute groupedWebApiRoute)
        {
          WebApiConfiguration.TraceRoutes(requestContext, groupedWebApiRoute.Routes);
        }
        else
        {
          try
          {
            string format = JsonConvert.SerializeObject((object) route);
            requestContext.TraceAlways(956093027, TraceLevel.Info, nameof (WebApiConfiguration), "RegisteredRoutes", format);
          }
          catch (JsonSerializationException ex)
          {
            requestContext.TraceAlways(956093028, TraceLevel.Info, nameof (WebApiConfiguration), "RegisteredRoutes", "Route Template: {0}{1}{2}", (object) route.RouteTemplate, (object) Environment.NewLine, (object) ex.Message);
          }
        }
      }
    }

    public static HttpConfiguration GetHttpConfiguration(IVssRequestContext requestContext)
    {
      WebApiConfiguration.Initialize(requestContext);
      return WebApiConfiguration.s_configuration;
    }

    public static HttpServer GetHttpServer(IVssRequestContext requestContext)
    {
      WebApiConfiguration.Initialize(requestContext);
      return WebApiConfiguration.s_httpServer;
    }

    public static string GetVirtualPathRoot(IVssRequestContext requestContext)
    {
      if (WebApiConfiguration.s_virtualPathRoot == null)
      {
        if (HostingEnvironment.IsHosted)
        {
          WebApiConfiguration.s_virtualPathRoot = HostingEnvironment.ApplicationVirtualPath;
        }
        else
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          WebApiConfiguration.s_virtualPathRoot = VirtualPathUtility.RemoveTrailingSlash(new Uri(vssRequestContext.GetService<ILocationService>().GetPublicAccessMapping(vssRequestContext).AccessPoint).AbsolutePath);
        }
      }
      return WebApiConfiguration.s_virtualPathRoot;
    }

    public static ResourceAreaCollection GetResourceAreas(IVssRequestContext requestContext)
    {
      WebApiConfiguration.Initialize(requestContext);
      return WebApiConfiguration.s_resourceAreas;
    }

    private static void RegisterResourceRoutes(
      IVssRequestContext systemRequestContext,
      HttpConfiguration config,
      ResourceAreaCollection areas)
    {
      using (IDisposableReadOnlyList<IVssApiResourceProvider> extensions = systemRequestContext.GetExtensions<IVssApiResourceProvider>())
      {
        ArgumentException argumentException = (ArgumentException) null;
        foreach (IVssApiResourceProvider resourceProvider in (IEnumerable<IVssApiResourceProvider>) extensions)
        {
          try
          {
            resourceProvider.RegisterResources(systemRequestContext, areas, config.Routes);
          }
          catch (ArgumentException ex)
          {
            if (argumentException == null)
              argumentException = ex;
          }
        }
        if (argumentException != null)
          throw argumentException;
      }
    }

    private static void RegisterFallbackRoute(HttpConfiguration config)
    {
      if (config.Routes.ContainsKey("FallbackRoute"))
        return;
      HttpRouteCollection routes = config.Routes;
      Guid locationId = new Guid("232B00F3-C6B8-48C6-883F-1A8DC6CBEF8A");
      Version version = VssRestApiVersion.v1_0.ToVersion();
      object obj = (object) new
      {
        fallbackConstraint = new FallbackRouteConstraint()
      };
      var defaults = new{ action = "notFound" };
      object constraints = obj;
      routes.MapLegacyResourceRoute(TeamFoundationHostType.All, locationId, "Fallback", "NotFound", "{*params}", version, defaults: (object) defaults, constraints: constraints, routeName: "FallbackRoute");
    }

    private static void VerifyNoDuplicateControllerNames(HttpConfiguration configuration)
    {
      Dictionary<string, Type> dictionary = new Dictionary<string, Type>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (Type controllerType in (IEnumerable<Type>) configuration.Services.GetHttpControllerTypeResolver().GetControllerTypes(configuration.Services.GetAssembliesResolver()))
      {
        Type type;
        if (dictionary.TryGetValue(controllerType.Name, out type))
        {
          TeamFoundationServiceException serviceException = new TeamFoundationServiceException(string.Format("Multiple controllers found with name '{0}'.\r\nWhile this could be perfectly valid, depending on route/namespace constraints, the majority of our routes do not have a controller namespace constraint, so this check enforces unique controller names to make sure we do not hit an exception at runtime during controller selection. Current VSS SDK guidance is to make controller names globally unique by prefixing them with your area name.\r\n\r\nDuplicate controller class information:\r\n1. {1} in {2} at {3}\r\n2. {4} in {5} at {6}", (object) controllerType.Name, (object) type.FullName, (object) type.Assembly.FullName, (object) type.Assembly.Location, (object) controllerType.FullName, (object) controllerType.Assembly.FullName, (object) controllerType.Assembly.Location));
          serviceException.LogException = true;
          serviceException.LogLevel = EventLogEntryType.Error;
          throw serviceException;
        }
        dictionary.Add(controllerType.Name, controllerType);
      }
    }

    private class WebApiInitializationPreviouslyFailedException : Exception
    {
      public WebApiInitializationPreviouslyFailedException(Exception innerEx)
        : base(FrameworkResources.WebApiInitializationPreviouslyFailed(), innerEx)
      {
      }
    }
  }
}
