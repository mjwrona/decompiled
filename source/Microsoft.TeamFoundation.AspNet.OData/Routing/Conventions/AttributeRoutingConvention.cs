// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.Conventions.AttributeRoutingConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Adapters;
using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.AspNet.OData.Routing.Template;
using Microsoft.OData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.AspNet.OData.Routing.Conventions
{
  public class AttributeRoutingConvention : IODataRoutingConvention
  {
    private static readonly DefaultODataPathHandler _defaultPathHandler = new DefaultODataPathHandler();
    private readonly string _routeName;
    private IDictionary<ODataPathTemplate, IWebApiActionDescriptor> _attributeMappings;

    public AttributeRoutingConvention(string routeName, HttpConfiguration configuration)
      : this(routeName, configuration, (IODataPathTemplateHandler) AttributeRoutingConvention._defaultPathHandler)
    {
    }

    public AttributeRoutingConvention(
      string routeName,
      HttpConfiguration configuration,
      IODataPathTemplateHandler pathTemplateHandler)
      : this(routeName)
    {
      AttributeRoutingConvention routingConvention = this;
      if (configuration == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (configuration));
      this.ODataPathTemplateHandler = pathTemplateHandler != null ? pathTemplateHandler : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (pathTemplateHandler));
      if (pathTemplateHandler is IODataPathHandler odataPathHandler && odataPathHandler.UrlKeyDelimiter == null)
      {
        ODataUrlKeyDelimiter urlKeyDelimiter = configuration.GetUrlKeyDelimiter();
        odataPathHandler.UrlKeyDelimiter = urlKeyDelimiter;
      }
      Action<HttpConfiguration> oldInitializer = configuration.Initializer;
      bool initialized = false;
      configuration.Initializer = (Action<HttpConfiguration>) (config =>
      {
        if (initialized)
          return;
        initialized = true;
        oldInitializer(config);
        routingConvention._attributeMappings = routingConvention.BuildAttributeMappings((IEnumerable<HttpControllerDescriptor>) config.Services.GetHttpControllerSelector().GetControllerMapping().Values);
      });
    }

    public AttributeRoutingConvention(
      string routeName,
      IEnumerable<HttpControllerDescriptor> controllers)
      : this(routeName, controllers, (IODataPathTemplateHandler) AttributeRoutingConvention._defaultPathHandler)
    {
    }

    public AttributeRoutingConvention(
      string routeName,
      IEnumerable<HttpControllerDescriptor> controllers,
      IODataPathTemplateHandler pathTemplateHandler)
      : this(routeName)
    {
      if (controllers == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (controllers));
      this.ODataPathTemplateHandler = pathTemplateHandler != null ? pathTemplateHandler : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (pathTemplateHandler));
      this._attributeMappings = this.BuildAttributeMappings(controllers);
    }

    internal IDictionary<ODataPathTemplate, IWebApiActionDescriptor> AttributeMappings => this._attributeMappings != null ? this._attributeMappings : throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.Object_NotYetInitialized);

    public virtual bool ShouldMapController(HttpControllerDescriptor controller) => true;

    public string SelectController(ODataPath odataPath, HttpRequestMessage request)
    {
      SelectControllerResult controllerResult = AttributeRoutingConvention.SelectControllerImpl(odataPath, (IWebApiRequestMessage) new WebApiRequestMessage(request), this.AttributeMappings);
      if (controllerResult != null)
        request.Properties["AttributeRouteData"] = (object) controllerResult.Values;
      return controllerResult?.ControllerName;
    }

    public string SelectAction(
      ODataPath odataPath,
      HttpControllerContext controllerContext,
      ILookup<string, HttpActionDescriptor> actionMap)
    {
      if (odataPath == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (odataPath));
      if (controllerContext == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (controllerContext));
      if (actionMap == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (actionMap));
      object values = (object) null;
      controllerContext.Request.Properties.TryGetValue("AttributeRouteData", out values);
      SelectControllerResult controllerResult = new SelectControllerResult(controllerContext.ControllerDescriptor.ControllerName, values as IDictionary<string, object>);
      return AttributeRoutingConvention.SelectActionImpl((IWebApiControllerContext) new WebApiControllerContext(controllerContext, controllerResult));
    }

    private IDictionary<ODataPathTemplate, IWebApiActionDescriptor> BuildAttributeMappings(
      IEnumerable<HttpControllerDescriptor> controllers)
    {
      Dictionary<ODataPathTemplate, IWebApiActionDescriptor> dictionary = new Dictionary<ODataPathTemplate, IWebApiActionDescriptor>();
      foreach (HttpControllerDescriptor controller in controllers)
      {
        if (AttributeRoutingConvention.IsODataController(controller) && this.ShouldMapController(controller))
        {
          HttpActionDescriptor[] array = controller.Configuration.Services.GetActionSelector().GetActionMapping(controller).SelectMany<IGrouping<string, HttpActionDescriptor>, HttpActionDescriptor>((Func<IGrouping<string, HttpActionDescriptor>, IEnumerable<HttpActionDescriptor>>) (a => (IEnumerable<HttpActionDescriptor>) a)).ToArray<HttpActionDescriptor>();
          foreach (string odataRoutePrefix in AttributeRoutingConvention.GetODataRoutePrefixes(controller))
          {
            foreach (HttpActionDescriptor actionDescriptor in array)
            {
              foreach (ODataPathTemplate odataPathTemplate in this.GetODataPathTemplates(odataRoutePrefix, actionDescriptor))
                dictionary.Add(odataPathTemplate, (IWebApiActionDescriptor) new WebApiActionDescriptor(actionDescriptor));
            }
          }
        }
      }
      return (IDictionary<ODataPathTemplate, IWebApiActionDescriptor>) dictionary;
    }

    private static bool IsODataController(HttpControllerDescriptor controller) => typeof (ODataController).IsAssignableFrom(controller.ControllerType);

    private static IEnumerable<string> GetODataRoutePrefixes(
      HttpControllerDescriptor controllerDescriptor)
    {
      return AttributeRoutingConvention.GetODataRoutePrefixes((IEnumerable<ODataRoutePrefixAttribute>) controllerDescriptor.GetCustomAttributes<ODataRoutePrefixAttribute>(false), controllerDescriptor.ControllerType.FullName);
    }

    private IEnumerable<ODataPathTemplate> GetODataPathTemplates(
      string prefix,
      HttpActionDescriptor action)
    {
      Collection<ODataRouteAttribute> customAttributes = action.GetCustomAttributes<ODataRouteAttribute>(false);
      IServiceProvider requestContainer = action.Configuration.GetODataRootContainer(this._routeName);
      string controllerName = action.ControllerDescriptor.ControllerName;
      string actionName = action.ActionName;
      Func<ODataRouteAttribute, bool> predicate = (Func<ODataRouteAttribute, bool>) (route => string.IsNullOrEmpty(route.RouteName) || route.RouteName == this._routeName);
      return customAttributes.Where<ODataRouteAttribute>(predicate).Select<ODataRouteAttribute, ODataPathTemplate>((Func<ODataRouteAttribute, ODataPathTemplate>) (route => this.GetODataPathTemplate(prefix, route.PathTemplate, requestContainer, controllerName, actionName))).Where<ODataPathTemplate>((Func<ODataPathTemplate, bool>) (template => template != null));
    }

    private AttributeRoutingConvention(string routeName) => this._routeName = routeName != null ? routeName : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (routeName));

    public IODataPathTemplateHandler ODataPathTemplateHandler { get; private set; }

    internal static SelectControllerResult SelectControllerImpl(
      ODataPath odataPath,
      IWebApiRequestMessage request,
      IDictionary<ODataPathTemplate, IWebApiActionDescriptor> attributeMappings)
    {
      Dictionary<string, object> values = new Dictionary<string, object>();
      foreach (KeyValuePair<ODataPathTemplate, IWebApiActionDescriptor> attributeMapping in (IEnumerable<KeyValuePair<ODataPathTemplate, IWebApiActionDescriptor>>) attributeMappings)
      {
        ODataPathTemplate key = attributeMapping.Key;
        IWebApiActionDescriptor actionDescriptor = attributeMapping.Value;
        if (actionDescriptor.IsHttpMethodSupported(request.Method) && key.TryMatch(odataPath, (IDictionary<string, object>) values))
        {
          values["action"] = (object) actionDescriptor.ActionName;
          return new SelectControllerResult(actionDescriptor.ControllerName, (IDictionary<string, object>) values);
        }
      }
      return (SelectControllerResult) null;
    }

    internal static string SelectActionImpl(IWebApiControllerContext controllerContext)
    {
      IDictionary<string, object> routeData = controllerContext.RouteData;
      IDictionary<string, object> conventionsStore = controllerContext.Request.Context.RoutingConventionsStore;
      IDictionary<string, object> values = controllerContext.ControllerResult.Values;
      if (values == null)
        return (string) null;
      foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) values)
      {
        if (keyValuePair.Key.StartsWith("DF908045-6922-46A0-82F2-2F6E7F43D1B1_", StringComparison.Ordinal) && keyValuePair.Value is ODataParameterValue)
          conventionsStore.Add(keyValuePair);
        else
          routeData.Add(keyValuePair);
      }
      return values["action"] as string;
    }

    private static IEnumerable<string> GetODataRoutePrefixes(
      IEnumerable<ODataRoutePrefixAttribute> prefixAttributes,
      string controllerName)
    {
      if (!prefixAttributes.Any<ODataRoutePrefixAttribute>())
      {
        yield return (string) null;
      }
      else
      {
        foreach (ODataRoutePrefixAttribute prefixAttribute in prefixAttributes)
        {
          string odataRoutePrefix = prefixAttribute.Prefix;
          if (odataRoutePrefix != null && odataRoutePrefix.StartsWith("/", StringComparison.Ordinal))
            throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.RoutePrefixStartsWithSlash, (object) odataRoutePrefix, (object) controllerName);
          if (odataRoutePrefix != null && odataRoutePrefix.EndsWith("/", StringComparison.Ordinal))
            odataRoutePrefix = odataRoutePrefix.TrimEnd('/');
          yield return odataRoutePrefix;
        }
      }
    }

    private ODataPathTemplate GetODataPathTemplate(
      string prefix,
      string pathTemplate,
      IServiceProvider requestContainer,
      string controllerName,
      string actionName)
    {
      if (prefix != null && !pathTemplate.StartsWith("/", StringComparison.Ordinal))
        pathTemplate = !string.IsNullOrEmpty(pathTemplate) ? (!pathTemplate.StartsWith("(", StringComparison.Ordinal) ? prefix + "/" + pathTemplate : prefix + pathTemplate) : prefix;
      if (pathTemplate.StartsWith("/", StringComparison.Ordinal))
        pathTemplate = pathTemplate.Substring(1);
      try
      {
        return this.ODataPathTemplateHandler.ParseTemplate(pathTemplate, requestContainer);
      }
      catch (ODataException ex)
      {
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.InvalidODataRouteOnAction, (object) pathTemplate, (object) actionName, (object) controllerName, (object) ex.Message);
      }
    }
  }
}
