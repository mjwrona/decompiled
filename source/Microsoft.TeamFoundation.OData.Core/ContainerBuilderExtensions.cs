// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ContainerBuilderExtensions
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Microsoft.OData.UriParser;
using System;

namespace Microsoft.OData
{
  public static class ContainerBuilderExtensions
  {
    public static IContainerBuilder AddService<TService, TImplementation>(
      this IContainerBuilder builder,
      ServiceLifetime lifetime)
      where TService : class
      where TImplementation : class, TService
    {
      return builder.AddService(lifetime, typeof (TService), typeof (TImplementation));
    }

    public static IContainerBuilder AddService(
      this IContainerBuilder builder,
      ServiceLifetime lifetime,
      Type serviceType)
    {
      return builder.AddService(lifetime, serviceType, serviceType);
    }

    public static IContainerBuilder AddService<TService>(
      this IContainerBuilder builder,
      ServiceLifetime lifetime)
      where TService : class
    {
      return builder.AddService(lifetime, typeof (TService));
    }

    public static IContainerBuilder AddService<TService>(
      this IContainerBuilder builder,
      ServiceLifetime lifetime,
      Func<IServiceProvider, TService> implementationFactory)
      where TService : class
    {
      return builder.AddService(lifetime, typeof (TService), (Func<IServiceProvider, object>) implementationFactory);
    }

    public static IContainerBuilder AddServicePrototype<TService>(
      this IContainerBuilder builder,
      TService instance)
    {
      return builder.AddService<ServicePrototype<TService>>(ServiceLifetime.Singleton, (Func<IServiceProvider, ServicePrototype<TService>>) (sp => new ServicePrototype<TService>(instance)));
    }

    public static IContainerBuilder AddDefaultODataServices(this IContainerBuilder builder) => builder.AddDefaultODataServices(ODataVersion.V4);

    public static IContainerBuilder AddDefaultODataServices(
      this IContainerBuilder builder,
      ODataVersion odataVersion)
    {
      builder.AddService<IJsonReaderFactory, DefaultJsonReaderFactory>(ServiceLifetime.Singleton);
      builder.AddService<IJsonWriterFactory>(ServiceLifetime.Singleton, (Func<IServiceProvider, IJsonWriterFactory>) (sp => (IJsonWriterFactory) new DefaultJsonWriterFactory()));
      builder.AddService<ODataMediaTypeResolver>(ServiceLifetime.Singleton, (Func<IServiceProvider, ODataMediaTypeResolver>) (sp => ODataMediaTypeResolver.GetMediaTypeResolver((IServiceProvider) null)));
      builder.AddService<ODataMessageInfo>(ServiceLifetime.Scoped);
      builder.AddServicePrototype<ODataMessageReaderSettings>(new ODataMessageReaderSettings());
      builder.AddService<ODataMessageReaderSettings>(ServiceLifetime.Scoped, (Func<IServiceProvider, ODataMessageReaderSettings>) (sp => sp.GetServicePrototype<ODataMessageReaderSettings>().Clone()));
      builder.AddServicePrototype<ODataMessageWriterSettings>(new ODataMessageWriterSettings());
      builder.AddService<ODataMessageWriterSettings>(ServiceLifetime.Scoped, (Func<IServiceProvider, ODataMessageWriterSettings>) (sp => sp.GetServicePrototype<ODataMessageWriterSettings>().Clone()));
      builder.AddService<ODataPayloadValueConverter>(ServiceLifetime.Singleton, (Func<IServiceProvider, ODataPayloadValueConverter>) (sp => ODataPayloadValueConverter.GetPayloadValueConverter((IServiceProvider) null)));
      builder.AddService<IEdmModel>(ServiceLifetime.Singleton, (Func<IServiceProvider, IEdmModel>) (sp => (IEdmModel) EdmCoreModel.Instance));
      builder.AddService<ODataUriResolver>(ServiceLifetime.Singleton, (Func<IServiceProvider, ODataUriResolver>) (sp => ODataUriResolver.GetUriResolver((IServiceProvider) null)));
      builder.AddService<ODataUriParserSettings>(ServiceLifetime.Scoped);
      builder.AddService<UriPathParser>(ServiceLifetime.Scoped);
      builder.AddServicePrototype<ODataSimplifiedOptions>(new ODataSimplifiedOptions(new ODataVersion?(odataVersion)));
      builder.AddService<ODataSimplifiedOptions>(ServiceLifetime.Scoped, (Func<IServiceProvider, ODataSimplifiedOptions>) (sp => sp.GetServicePrototype<ODataSimplifiedOptions>().Clone()));
      return builder;
    }
  }
}
