// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Extensions.HttpConfigurationExtensions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Adapters;
using Microsoft.AspNet.OData.Batch;
using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Batch;
using System.Web.Http.Dispatcher;
using System.Web.Http.Filters;
using System.Web.Http.Routing;

namespace Microsoft.AspNet.OData.Extensions
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class HttpConfigurationExtensions
  {
    private const string ETagHandlerKey = "Microsoft.AspNet.OData.ETagHandler";
    private const string TimeZoneInfoKey = "Microsoft.AspNet.OData.TimeZoneInfo";
    private const string UrlKeyDelimiterKey = "Microsoft.AspNet.OData.UrlKeyDelimiterKey";
    private const string ContinueOnErrorKey = "Microsoft.AspNet.OData.ContinueOnErrorKey";
    private const string NullDynamicPropertyKey = "Microsoft.AspNet.OData.NullDynamicPropertyKey";
    private const string ContainerBuilderFactoryKey = "Microsoft.AspNet.OData.ContainerBuilderFactoryKey";
    private const string PerRouteContainerKey = "Microsoft.AspNet.OData.PerRouteContainerKey";
    private const string DefaultQuerySettingsKey = "Microsoft.AspNet.OData.DefaultQuerySettings";
    private const string NonODataRootContainerKey = "Microsoft.AspNet.OData.NonODataRootContainerKey";
    private const string CompatibilityOptionsKey = "Microsoft.AspNet.OData.CompatibilityOptionsKey";

    public static void AddODataQueryFilter(this HttpConfiguration configuration) => configuration.AddODataQueryFilter((IActionFilter) new EnableQueryAttribute());

    public static void SetDefaultQuerySettings(
      this HttpConfiguration configuration,
      DefaultQuerySettings defaultQuerySettings)
    {
      if (configuration == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (configuration));
      if (defaultQuerySettings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (defaultQuerySettings));
      if (defaultQuerySettings.MaxTop.HasValue)
      {
        int? maxTop = defaultQuerySettings.MaxTop;
        int num = 0;
        if (!(maxTop.GetValueOrDefault() > num & maxTop.HasValue))
          goto label_7;
      }
      ModelBoundQuerySettings.DefaultModelBoundQuerySettings.MaxTop = defaultQuerySettings.MaxTop;
label_7:
      configuration.Properties[(object) "Microsoft.AspNet.OData.DefaultQuerySettings"] = (object) defaultQuerySettings;
    }

    public static DefaultQuerySettings GetDefaultQuerySettings(this HttpConfiguration configuration)
    {
      object defaultQuerySettings1;
      if (configuration.Properties.TryGetValue((object) "Microsoft.AspNet.OData.DefaultQuerySettings", out defaultQuerySettings1))
        return defaultQuerySettings1 as DefaultQuerySettings;
      DefaultQuerySettings defaultQuerySettings2 = new DefaultQuerySettings();
      configuration.SetDefaultQuerySettings(defaultQuerySettings2);
      return defaultQuerySettings2;
    }

    public static HttpConfiguration MaxTop(this HttpConfiguration configuration, int? maxTopValue)
    {
      configuration.GetDefaultQuerySettings().MaxTop = maxTopValue;
      if (maxTopValue.HasValue)
      {
        int? nullable = maxTopValue;
        int num = 0;
        if (!(nullable.GetValueOrDefault() > num & nullable.HasValue))
          goto label_3;
      }
      ModelBoundQuerySettings.DefaultModelBoundQuerySettings.MaxTop = maxTopValue;
label_3:
      return configuration;
    }

    public static HttpConfiguration Expand(
      this HttpConfiguration configuration,
      QueryOptionSetting setting)
    {
      configuration.GetDefaultQuerySettings().EnableExpand = setting == QueryOptionSetting.Allowed;
      return configuration;
    }

    public static HttpConfiguration Expand(this HttpConfiguration configuration)
    {
      configuration.GetDefaultQuerySettings().EnableExpand = true;
      return configuration;
    }

    public static HttpConfiguration Select(
      this HttpConfiguration configuration,
      QueryOptionSetting setting)
    {
      configuration.GetDefaultQuerySettings().EnableSelect = setting == QueryOptionSetting.Allowed;
      return configuration;
    }

    public static HttpConfiguration Select(this HttpConfiguration configuration)
    {
      configuration.GetDefaultQuerySettings().EnableSelect = true;
      return configuration;
    }

    public static HttpConfiguration Filter(
      this HttpConfiguration configuration,
      QueryOptionSetting setting)
    {
      configuration.GetDefaultQuerySettings().EnableFilter = setting == QueryOptionSetting.Allowed;
      return configuration;
    }

    public static HttpConfiguration Filter(this HttpConfiguration configuration)
    {
      configuration.GetDefaultQuerySettings().EnableFilter = true;
      return configuration;
    }

    public static HttpConfiguration OrderBy(
      this HttpConfiguration configuration,
      QueryOptionSetting setting)
    {
      configuration.GetDefaultQuerySettings().EnableOrderBy = setting == QueryOptionSetting.Allowed;
      return configuration;
    }

    public static HttpConfiguration OrderBy(this HttpConfiguration configuration)
    {
      configuration.GetDefaultQuerySettings().EnableOrderBy = true;
      return configuration;
    }

    public static HttpConfiguration SkipToken(
      this HttpConfiguration configuration,
      QueryOptionSetting setting)
    {
      configuration.GetDefaultQuerySettings().EnableSkipToken = setting == QueryOptionSetting.Allowed;
      return configuration;
    }

    public static HttpConfiguration SkipToken(this HttpConfiguration configuration)
    {
      configuration.GetDefaultQuerySettings().EnableSkipToken = true;
      return configuration;
    }

    public static HttpConfiguration Count(
      this HttpConfiguration configuration,
      QueryOptionSetting setting)
    {
      configuration.GetDefaultQuerySettings().EnableCount = setting == QueryOptionSetting.Allowed;
      return configuration;
    }

    public static HttpConfiguration Count(this HttpConfiguration configuration)
    {
      configuration.GetDefaultQuerySettings().EnableCount = true;
      return configuration;
    }

    public static void AddODataQueryFilter(
      this HttpConfiguration configuration,
      IActionFilter queryFilter)
    {
      if (configuration == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (configuration));
      configuration.Services.Add(typeof (IFilterProvider), (object) new QueryFilterProvider(queryFilter));
    }

    public static IETagHandler GetETagHandler(this HttpConfiguration configuration)
    {
      if (configuration == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (configuration));
      object obj;
      if (!configuration.Properties.TryGetValue((object) "Microsoft.AspNet.OData.ETagHandler", out obj))
      {
        IETagHandler handler = (IETagHandler) new DefaultODataETagHandler();
        configuration.SetETagHandler(handler);
        return handler;
      }
      if (obj == null)
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.NullETagHandler);
      return obj is IETagHandler etagHandler ? etagHandler : throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.InvalidETagHandler, (object) obj.GetType());
    }

    public static void SetETagHandler(this HttpConfiguration configuration, IETagHandler handler)
    {
      if (configuration == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (configuration));
      configuration.Properties[(object) "Microsoft.AspNet.OData.ETagHandler"] = handler != null ? (object) handler : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (handler));
    }

    public static TimeZoneInfo GetTimeZoneInfo(this HttpConfiguration configuration)
    {
      if (configuration == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (configuration));
      object obj;
      if (!configuration.Properties.TryGetValue((object) "Microsoft.AspNet.OData.TimeZoneInfo", out obj))
      {
        TimeZoneInfo local = TimeZoneInfo.Local;
        configuration.SetTimeZoneInfo(local);
        return local;
      }
      return obj is TimeZoneInfo timeZoneInfo ? timeZoneInfo : throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.InvalidTimeZoneInfo, (object) obj.GetType(), (object) typeof (TimeZoneInfo));
    }

    public static void SetTimeZoneInfo(
      this HttpConfiguration configuration,
      TimeZoneInfo timeZoneInfo)
    {
      if (configuration == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (configuration));
      configuration.Properties[(object) "Microsoft.AspNet.OData.TimeZoneInfo"] = timeZoneInfo != null ? (object) timeZoneInfo : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (timeZoneInfo));
      TimeZoneInfoHelper.TimeZone = timeZoneInfo;
    }

    public static void EnableContinueOnErrorHeader(this HttpConfiguration configuration)
    {
      if (configuration == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (configuration));
      configuration.Properties[(object) "Microsoft.AspNet.OData.ContinueOnErrorKey"] = (object) true;
    }

    internal static bool HasEnabledContinueOnErrorHeader(this HttpConfiguration configuration)
    {
      if (configuration == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (configuration));
      object obj;
      return configuration.Properties.TryGetValue((object) "Microsoft.AspNet.OData.ContinueOnErrorKey", out obj) && (bool) obj;
    }

    public static void SetSerializeNullDynamicProperty(
      this HttpConfiguration configuration,
      bool serialize)
    {
      if (configuration == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (configuration));
      configuration.Properties[(object) "Microsoft.AspNet.OData.NullDynamicPropertyKey"] = (object) serialize;
    }

    public static void SetUrlKeyDelimiter(
      this HttpConfiguration configuration,
      ODataUrlKeyDelimiter urlKeyDelimiter)
    {
      if (configuration == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (configuration));
      configuration.Properties[(object) "Microsoft.AspNet.OData.UrlKeyDelimiterKey"] = (object) urlKeyDelimiter;
    }

    public static void SetCompatibilityOptions(
      this HttpConfiguration configuration,
      CompatibilityOptions options)
    {
      if (configuration == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (configuration));
      configuration.Properties[(object) "Microsoft.AspNet.OData.CompatibilityOptionsKey"] = (object) options;
    }

    internal static bool HasEnabledNullDynamicProperty(this HttpConfiguration configuration)
    {
      if (configuration == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (configuration));
      object obj;
      return configuration.Properties.TryGetValue((object) "Microsoft.AspNet.OData.NullDynamicPropertyKey", out obj) && (bool) obj;
    }

    internal static ODataUrlKeyDelimiter GetUrlKeyDelimiter(this HttpConfiguration configuration)
    {
      if (configuration == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (configuration));
      object urlKeyDelimiter;
      if (configuration.Properties.TryGetValue((object) "Microsoft.AspNet.OData.UrlKeyDelimiterKey", out urlKeyDelimiter))
        return urlKeyDelimiter as ODataUrlKeyDelimiter;
      configuration.Properties[(object) "Microsoft.AspNet.OData.UrlKeyDelimiterKey"] = (object) null;
      return (ODataUrlKeyDelimiter) null;
    }

    internal static CompatibilityOptions GetCompatibilityOptions(
      this HttpConfiguration configuration)
    {
      if (configuration == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (configuration));
      object compatibilityOptions;
      if (configuration.Properties.TryGetValue((object) "Microsoft.AspNet.OData.CompatibilityOptionsKey", out compatibilityOptions))
        return (CompatibilityOptions) compatibilityOptions;
      configuration.Properties[(object) "Microsoft.AspNet.OData.CompatibilityOptionsKey"] = (object) CompatibilityOptions.None;
      return CompatibilityOptions.None;
    }

    public static HttpConfiguration UseCustomContainerBuilder(
      this HttpConfiguration configuration,
      Func<IContainerBuilder> builderFactory)
    {
      if (configuration == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (configuration));
      configuration.Properties[(object) "Microsoft.AspNet.OData.ContainerBuilderFactoryKey"] = builderFactory != null ? (object) builderFactory : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (builderFactory));
      return configuration;
    }

    public static void EnableDependencyInjection(this HttpConfiguration configuration) => configuration.EnableDependencyInjection((Action<IContainerBuilder>) null);

    public static void EnableDependencyInjection(
      this HttpConfiguration configuration,
      Action<IContainerBuilder> configureAction)
    {
      if (configuration == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (configuration));
      if (configuration.Properties.ContainsKey((object) "Microsoft.AspNet.OData.NonODataRootContainerKey"))
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.CannotReEnableDependencyInjection);
      configuration.GetPerRouteContainer().CreateODataRootContainer((string) null, HttpConfigurationExtensions.ConfigureDefaultServices(configuration, configureAction));
    }

    public static ODataRoute MapODataServiceRoute(
      this HttpConfiguration configuration,
      string routeName,
      string routePrefix,
      Action<IContainerBuilder> configureAction)
    {
      if (configuration == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (configuration));
      if (routeName == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (routeName));
      IServiceProvider odataRootContainer = configuration.CreateODataRootContainer(routeName, configureAction);
      IODataPathHandler requiredService = ServiceProviderServiceExtensions.GetRequiredService<IODataPathHandler>(odataRootContainer);
      if (requiredService != null && requiredService.UrlKeyDelimiter == null)
      {
        ODataUrlKeyDelimiter urlKeyDelimiter = configuration.GetUrlKeyDelimiter();
        requiredService.UrlKeyDelimiter = urlKeyDelimiter;
      }
      ODataPathRouteConstraint pathConstraint = new ODataPathRouteConstraint(routeName);
      ServiceProviderServiceExtensions.GetServices<IODataRoutingConvention>(odataRootContainer);
      HttpRouteCollection routes = configuration.Routes;
      routePrefix = HttpConfigurationExtensions.RemoveTrailingSlash(routePrefix);
      HttpMessageHandler service1 = ServiceProviderServiceExtensions.GetService<HttpMessageHandler>(odataRootContainer);
      ODataRoute route;
      if (service1 != null)
      {
        route = new ODataRoute(routePrefix, pathConstraint, (HttpRouteValueDictionary) null, (HttpRouteValueDictionary) null, (HttpRouteValueDictionary) null, service1);
      }
      else
      {
        ODataBatchHandler service2 = ServiceProviderServiceExtensions.GetService<ODataBatchHandler>(odataRootContainer);
        if (service2 != null)
        {
          service2.ODataRouteName = routeName;
          string routeTemplate = string.IsNullOrEmpty(routePrefix) ? ODataRouteConstants.Batch : routePrefix + "/" + ODataRouteConstants.Batch;
          routes.MapHttpBatchRoute(routeName + "Batch", routeTemplate, (HttpBatchHandler) service2);
        }
        route = new ODataRoute(routePrefix, pathConstraint);
      }
      routes.Add(routeName, (IHttpRoute) route);
      return route;
    }

    public static ODataRoute MapODataServiceRoute(
      this HttpConfiguration configuration,
      string routeName,
      string routePrefix,
      IEdmModel model)
    {
      return configuration.MapODataServiceRoute(routeName, routePrefix, (Action<IContainerBuilder>) (builder => builder.AddService<IEdmModel>(Microsoft.OData.ServiceLifetime.Singleton, (Func<IServiceProvider, IEdmModel>) (sp => model)).AddService<IEnumerable<IODataRoutingConvention>>(Microsoft.OData.ServiceLifetime.Singleton, (Func<IServiceProvider, IEnumerable<IODataRoutingConvention>>) (sp => (IEnumerable<IODataRoutingConvention>) ODataRoutingConventions.CreateDefaultWithAttributeRouting(routeName, configuration)))));
    }

    public static ODataRoute MapODataServiceRoute(
      this HttpConfiguration configuration,
      string routeName,
      string routePrefix,
      IEdmModel model,
      ODataBatchHandler batchHandler)
    {
      return configuration.MapODataServiceRoute(routeName, routePrefix, (Action<IContainerBuilder>) (builder => builder.AddService<IEdmModel>(Microsoft.OData.ServiceLifetime.Singleton, (Func<IServiceProvider, IEdmModel>) (sp => model)).AddService<ODataBatchHandler>(Microsoft.OData.ServiceLifetime.Singleton, (Func<IServiceProvider, ODataBatchHandler>) (sp => batchHandler)).AddService<IEnumerable<IODataRoutingConvention>>(Microsoft.OData.ServiceLifetime.Singleton, (Func<IServiceProvider, IEnumerable<IODataRoutingConvention>>) (sp => (IEnumerable<IODataRoutingConvention>) ODataRoutingConventions.CreateDefaultWithAttributeRouting(routeName, configuration)))));
    }

    public static ODataRoute MapODataServiceRoute(
      this HttpConfiguration configuration,
      string routeName,
      string routePrefix,
      IEdmModel model,
      HttpMessageHandler defaultHandler)
    {
      return configuration.MapODataServiceRoute(routeName, routePrefix, (Action<IContainerBuilder>) (builder => builder.AddService<IEdmModel>(Microsoft.OData.ServiceLifetime.Singleton, (Func<IServiceProvider, IEdmModel>) (sp => model)).AddService<HttpMessageHandler>(Microsoft.OData.ServiceLifetime.Singleton, (Func<IServiceProvider, HttpMessageHandler>) (sp => defaultHandler)).AddService<IEnumerable<IODataRoutingConvention>>(Microsoft.OData.ServiceLifetime.Singleton, (Func<IServiceProvider, IEnumerable<IODataRoutingConvention>>) (sp => (IEnumerable<IODataRoutingConvention>) ODataRoutingConventions.CreateDefaultWithAttributeRouting(routeName, configuration)))));
    }

    public static ODataRoute MapODataServiceRoute(
      this HttpConfiguration configuration,
      string routeName,
      string routePrefix,
      IEdmModel model,
      IODataPathHandler pathHandler,
      IEnumerable<IODataRoutingConvention> routingConventions)
    {
      return configuration.MapODataServiceRoute(routeName, routePrefix, (Action<IContainerBuilder>) (builder => builder.AddService<IEdmModel>(Microsoft.OData.ServiceLifetime.Singleton, (Func<IServiceProvider, IEdmModel>) (sp => model)).AddService<IODataPathHandler>(Microsoft.OData.ServiceLifetime.Singleton, (Func<IServiceProvider, IODataPathHandler>) (sp => pathHandler)).AddService<IEnumerable<IODataRoutingConvention>>(Microsoft.OData.ServiceLifetime.Singleton, (Func<IServiceProvider, IEnumerable<IODataRoutingConvention>>) (sp => routingConventions.ToList<IODataRoutingConvention>().AsEnumerable<IODataRoutingConvention>()))));
    }

    public static ODataRoute MapODataServiceRoute(
      this HttpConfiguration configuration,
      string routeName,
      string routePrefix,
      IEdmModel model,
      IODataPathHandler pathHandler,
      IEnumerable<IODataRoutingConvention> routingConventions,
      ODataBatchHandler batchHandler)
    {
      return configuration.MapODataServiceRoute(routeName, routePrefix, (Action<IContainerBuilder>) (builder => builder.AddService<IEdmModel>(Microsoft.OData.ServiceLifetime.Singleton, (Func<IServiceProvider, IEdmModel>) (sp => model)).AddService<IODataPathHandler>(Microsoft.OData.ServiceLifetime.Singleton, (Func<IServiceProvider, IODataPathHandler>) (sp => pathHandler)).AddService<IEnumerable<IODataRoutingConvention>>(Microsoft.OData.ServiceLifetime.Singleton, (Func<IServiceProvider, IEnumerable<IODataRoutingConvention>>) (sp => routingConventions.ToList<IODataRoutingConvention>().AsEnumerable<IODataRoutingConvention>())).AddService<ODataBatchHandler>(Microsoft.OData.ServiceLifetime.Singleton, (Func<IServiceProvider, ODataBatchHandler>) (sp => batchHandler))));
    }

    public static ODataRoute MapODataServiceRoute(
      this HttpConfiguration configuration,
      string routeName,
      string routePrefix,
      IEdmModel model,
      IODataPathHandler pathHandler,
      IEnumerable<IODataRoutingConvention> routingConventions,
      HttpMessageHandler defaultHandler)
    {
      return configuration.MapODataServiceRoute(routeName, routePrefix, (Action<IContainerBuilder>) (builder => builder.AddService<IEdmModel>(Microsoft.OData.ServiceLifetime.Singleton, (Func<IServiceProvider, IEdmModel>) (sp => model)).AddService<IODataPathHandler>(Microsoft.OData.ServiceLifetime.Singleton, (Func<IServiceProvider, IODataPathHandler>) (sp => pathHandler)).AddService<IEnumerable<IODataRoutingConvention>>(Microsoft.OData.ServiceLifetime.Singleton, (Func<IServiceProvider, IEnumerable<IODataRoutingConvention>>) (sp => routingConventions.ToList<IODataRoutingConvention>().AsEnumerable<IODataRoutingConvention>())).AddService<HttpMessageHandler>(Microsoft.OData.ServiceLifetime.Singleton, (Func<IServiceProvider, HttpMessageHandler>) (sp => defaultHandler))));
    }

    private static string RemoveTrailingSlash(string routePrefix)
    {
      if (!string.IsNullOrEmpty(routePrefix))
      {
        int index = routePrefix.Length - 1;
        if (routePrefix[index] == '/')
          routePrefix = routePrefix.Substring(0, routePrefix.Length - 1);
      }
      return routePrefix;
    }

    internal static IServiceProvider CreateODataRootContainer(
      this HttpConfiguration configuration,
      string routeName,
      Action<IContainerBuilder> configureAction)
    {
      return configuration.GetPerRouteContainer().CreateODataRootContainer(routeName, HttpConfigurationExtensions.ConfigureDefaultServices(configuration, configureAction));
    }

    internal static IPerRouteContainer GetPerRouteContainer(this HttpConfiguration configuration) => (IPerRouteContainer) configuration.Properties.GetOrAdd((object) "Microsoft.AspNet.OData.PerRouteContainerKey", (Func<object, object>) (key =>
    {
      IPerRouteContainer perRouteContainer = (IPerRouteContainer) new PerRouteContainer(configuration);
      object obj;
      if (configuration.Properties.TryGetValue((object) "Microsoft.AspNet.OData.ContainerBuilderFactoryKey", out obj))
      {
        Func<IContainerBuilder> func = (Func<IContainerBuilder>) obj;
        perRouteContainer.BuilderFactory = func;
      }
      return (object) perRouteContainer;
    }));

    internal static IServiceProvider GetODataRootContainer(
      this HttpConfiguration configuration,
      string routeName)
    {
      return configuration.GetPerRouteContainer().GetODataRootContainer(routeName);
    }

    internal static IServiceProvider GetNonODataRootContainer(this HttpConfiguration configuration)
    {
      object odataRootContainer;
      if (configuration.Properties.TryGetValue((object) "Microsoft.AspNet.OData.NonODataRootContainerKey", out odataRootContainer))
        return (IServiceProvider) odataRootContainer;
      throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.NoNonODataHttpRouteRegistered);
    }

    internal static void SetNonODataRootContainer(
      this HttpConfiguration configuration,
      IServiceProvider rootContainer)
    {
      if (configuration == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (configuration));
      if (rootContainer == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (rootContainer));
      if (configuration.Properties.ContainsKey((object) "Microsoft.AspNet.OData.NonODataRootContainerKey"))
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.CannotReEnableDependencyInjection);
      configuration.Properties[(object) "Microsoft.AspNet.OData.NonODataRootContainerKey"] = (object) rootContainer;
    }

    private static Action<IContainerBuilder> ConfigureDefaultServices(
      HttpConfiguration configuration,
      Action<IContainerBuilder> configureAction)
    {
      return (Action<IContainerBuilder>) (builder =>
      {
        IAssembliesResolver resolver = configuration.Services.GetAssembliesResolver() ?? (IAssembliesResolver) new DefaultAssembliesResolver();
        builder.AddService<IAssembliesResolver>(Microsoft.OData.ServiceLifetime.Singleton, (Func<IServiceProvider, IAssembliesResolver>) (sp => resolver));
        builder.AddService<IWebApiAssembliesResolver, WebApiAssembliesResolver>(Microsoft.OData.ServiceLifetime.Transient);
        builder.AddService<HttpConfiguration>(Microsoft.OData.ServiceLifetime.Singleton, (Func<IServiceProvider, HttpConfiguration>) (sp => configuration));
        builder.AddService<DefaultQuerySettings>(Microsoft.OData.ServiceLifetime.Singleton, (Func<IServiceProvider, DefaultQuerySettings>) (sp => configuration.GetDefaultQuerySettings()));
        builder.AddDefaultWebApiServices();
        if (configureAction == null)
          return;
        configureAction(builder);
      });
    }
  }
}
