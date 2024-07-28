// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WebModule`1
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Bundling;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public abstract class WebModule<TApplication> : IHttpModule, IWebContextProvider where TApplication : VisualStudioServicesApplication
  {
    private static bool s_isHosted = false;
    private static bool sm_initialized = false;
    private static object sm_initLock = new object();
    private static IWebContextProvider s_defaultContextProvider = (IWebContextProvider) new DefaultWebContextProvider();

    protected string TraceArea => "WebAccess";

    public void Dispose()
    {
    }

    public void Init(HttpApplication context)
    {
      if (!(context is TApplication application))
        throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} can only work with {1}", (object) this.GetType().Name, (object) typeof (TApplication).Name));
      if (context.Modules.Get("ErrorEventModule") is ErrorEventModule errorEventModule)
        errorEventModule.OnFormatError += new ErrorFormatterDelegate(this.OnApplicationFormatError);
      application.OnReportError += new ErrorReporterDelegate(this.OnApplicationReportError);
      application.OnFormatError += new ErrorFormatterDelegate(this.OnApplicationFormatError);
      application.BeginRequest += new EventHandler(this.Module_BeginRequest);
      WebContextFactory.SetContextProvider((IWebContextProvider) this);
      this.EnsureInitialization(application);
    }

    protected void Module_BeginRequest(object sender, EventArgs e) => this.EnsureInitialization(sender as TApplication);

    protected virtual void OnBeginInitialize()
    {
    }

    private void EnsureInitialization(TApplication application)
    {
      if (!TeamFoundationApplicationCore.DeploymentInitialized || WebModule<TApplication>.sm_initialized)
        return;
      lock (WebModule<TApplication>.sm_initLock)
      {
        if (WebModule<TApplication>.sm_initialized)
          return;
        try
        {
          string appSetting = WebConfigurationManager.AppSettings["IsHosted"];
          WebModule<TApplication>.s_isHosted = !string.IsNullOrEmpty(appSetting) && string.Equals(appSetting, "true", StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
        }
        this.OnBeginInitialize();
        AreaRegistration.RegisterAllAreas();
        this.RegisterDefaultRoutes(RouteTable.Routes);
        this.RegisterControllerRouteAreas();
        BundleMetadata.LoadBundleManifests();
        this.ConfigurePaths();
        WebModule<TApplication>.sm_initialized = true;
      }
    }

    protected virtual bool? OnApplicationReportError(Exception exception) => new bool?();

    protected virtual bool OnApplicationFormatError(
      HttpContextBase httpContext,
      HttpStatusCode statusCode,
      Exception exception,
      string errorMessage,
      string responseText)
    {
      if (httpContext == null)
      {
        httpContext = HttpContextFactory.Current;
        if (httpContext == null)
          return false;
      }
      if (exception != null)
      {
        while (exception is HttpException && exception.InnerException != null)
          exception = exception.InnerException;
      }
      int httpCode = (int) statusCode;
      if (httpCode == 0)
        httpCode = 500;
      if (exception == null)
        exception = (Exception) new HttpException(httpCode, errorMessage ?? WACommonResources.InternalServerError);
      this.LogExceptionInternal(httpContext, exception);
      bool flag1 = false;
      if (httpContext.Request.IsAjaxRequest())
      {
        try
        {
          httpContext.Response.ContentType = "application/json";
        }
        catch (HttpException ex)
        {
        }
        bool debuggingEnabled = httpContext.IsDebuggingEnabled;
        JsObject json = exception.ToJson(true, debuggingEnabled);
        JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
        httpContext.Response.Write(scriptSerializer.Serialize((object) json));
        flag1 = true;
      }
      else if (MediaTypeFormatUtility.DoesRequestAcceptMediaType(httpContext.Request.AcceptTypes, "text/html", "application/json", "text/plain"))
      {
        IVssRequestContext vssRequestContext = (IVssRequestContext) null;
        Uri requestUri = (Uri) null;
        string userAgent = string.Empty;
        if (HttpContext.Current != null && HttpContext.Current.Request != null)
        {
          requestUri = HttpContext.Current.Request.Url;
          userAgent = HttpContext.Current.Request.UserAgent;
          if (HttpContext.Current.Request.RequestContext != null && HttpContext.Current.Request.RequestContext.HttpContext != null && HttpContext.Current.Request.RequestContext.HttpContext.Items != null)
            vssRequestContext = (IVssRequestContext) HttpContext.Current.Request.RequestContext.HttpContext.Items[(object) "IVssRequestContext"];
        }
        bool flag2;
        try
        {
          flag2 = vssRequestContext != null && vssRequestContext.IsFeatureEnabled("VisualStudio.Services.Framework.EnableNewErrorHandling");
        }
        catch (Exception ex)
        {
          flag2 = false;
        }
        if (!flag2)
        {
          System.Web.Routing.RequestContext requestContext = new System.Web.Routing.RequestContext(httpContext, new RouteData()
          {
            Values = {
              {
                "controller",
                (object) "error"
              },
              {
                "action",
                (object) "index"
              },
              {
                nameof (exception),
                (object) exception
              },
              {
                nameof (statusCode),
                (object) (int) statusCode
              },
              {
                "description",
                (object) errorMessage
              }
            }
          });
          ControllerBuilder.Current.GetControllerFactory().CreateController(requestContext, "error").Execute(requestContext);
        }
        else
        {
          try
          {
            httpContext.Response.ContentType = "text/html";
          }
          catch (HttpException ex)
          {
          }
          ErrorContext errorContext = ErrorManager.Instance.CreateErrorContext(vssRequestContext, WebModule<TApplication>.s_isHosted, exception, (int) statusCode, errorMessage, requestUri, userAgent);
          if (statusCode != (HttpStatusCode) 0)
          {
            try
            {
              httpContext.Response.StatusCode = (int) statusCode;
              httpContext.Response.TrySkipIisCustomErrors = true;
            }
            catch (HttpException ex)
            {
            }
          }
          ErrorManager.Instance.WriteResponse(vssRequestContext, httpContext.Response.OutputStream, errorContext);
        }
        flag1 = true;
      }
      return flag1;
    }

    protected virtual void LogExceptionInternal(HttpContextBase httpContext, Exception exception)
    {
    }

    protected virtual void RegisterDefaultRoutes(RouteCollection routes)
    {
      this.RemoveFallbackRoute(routes);
      routes.MapViewRoute(TeamFoundationHostType.Unknown, "CssBundles", "_static/tfs/{staticContentVersion}/_cssbundles/{themeName}/{bundle}", (object) new
      {
        controller = "Bundling",
        action = "CssContent"
      });
      routes.MapViewRoute(TeamFoundationHostType.Application | TeamFoundationHostType.ProjectCollection, "Redirect", "_redirect", (object) new
      {
        controller = "RedirectWithIdentity",
        action = "Redirect"
      });
    }

    protected virtual void RegisterControllerRouteAreas()
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      IEnumerable<Type> allTypes = TypeUtils.GetAllTypes(WebModule<TApplication>.\u003C\u003EO.\u003C0\u003E__IsControllerType ?? (WebModule<TApplication>.\u003C\u003EO.\u003C0\u003E__IsControllerType = new Func<Type, bool>(TypeUtils.IsControllerType)));
      WebModule<TApplication>.VerifyNoDuplicateControllerNames(allTypes);
      TfsRouteAreaConstraint.RegisterControllerRouteAreas(allTypes);
    }

    protected virtual void ConfigurePaths()
    {
      WebFormViewEngine webFormViewEngine = (WebFormViewEngine) ViewEngines.Engines.FirstOrDefault<IViewEngine>();
      webFormViewEngine.MasterLocationFormats = new string[2]
      {
        "~/_views/{1}/{0}.master",
        "~/_views/Shared/{0}.master"
      };
      webFormViewEngine.AreaMasterLocationFormats = new string[2]
      {
        "~/_areas/{2}/Views/{1}/{0}.master",
        "~/_areas/{2}/Views/Shared/{0}.master"
      };
      webFormViewEngine.ViewLocationFormats = new string[4]
      {
        "~/_views/{1}/{0}.aspx",
        "~/_views/{1}/{0}.ascx",
        "~/_views/Shared/{0}.aspx",
        "~/_views/Shared/{0}.ascx"
      };
      webFormViewEngine.AreaViewLocationFormats = new string[4]
      {
        "~/_areas/{2}/Views/{1}/{0}.aspx",
        "~/_areas/{2}/Views/{1}/{0}.ascx",
        "~/_areas/{2}/Views/Shared/{0}.aspx",
        "~/_areas/{2}/Views/Shared/{0}.ascx"
      };
      webFormViewEngine.PartialViewLocationFormats = webFormViewEngine.ViewLocationFormats;
      webFormViewEngine.AreaPartialViewLocationFormats = webFormViewEngine.AreaViewLocationFormats;
    }

    protected static void VerifyNoDuplicateControllerNames(IEnumerable<Type> controllerTypes)
    {
      Dictionary<string, Type> dictionary = new Dictionary<string, Type>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (Type controllerType in controllerTypes)
      {
        Type type;
        if (dictionary.TryGetValue(controllerType.Name, out type))
          throw new TeamFoundationServiceException(string.Format("Multiple MVC controllers found with name '{0}'.\r\nWeb Access uses generic routes (for all controllers) that do provide namespace constraints for controller do not have a controller namespace constraint, so this check enforces unique controller names to make sure selection. So we do this controller-name-uniqueness check upfront to fail fast and avoid bugs caused by issues during runtime.\r\n\r\nDuplicate controller class information:\r\n1. {1} in {2} at {3}\r\n2. {4} in {5} at {6}", (object) controllerType.Name, (object) type.FullName, (object) type.Assembly.FullName, (object) type.Assembly.Location, (object) controllerType.FullName, (object) controllerType.Assembly.FullName, (object) controllerType.Assembly.Location));
        dictionary.Add(controllerType.Name, controllerType);
      }
    }

    public virtual WebContext CreateWebContext(System.Web.Routing.RequestContext requestContext) => WebModule<TApplication>.s_defaultContextProvider.CreateWebContext(requestContext);

    public virtual PageContext CreatePageContext(System.Web.Routing.RequestContext requestContext) => WebModule<TApplication>.s_defaultContextProvider.CreatePageContext(requestContext);

    public virtual ContributedServiceContext CreateContributedServiceContext(
      System.Web.Routing.RequestContext requestContext)
    {
      return WebModule<TApplication>.s_defaultContextProvider.CreateContributedServiceContext(requestContext);
    }

    protected void RemoveFallbackRoute(RouteCollection routes)
    {
      RouteBase route = routes["FallbackRoute"];
      if (route == null)
        return;
      routes.Remove(route);
    }
  }
}
