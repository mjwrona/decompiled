// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.PerRouteContainerBase
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData;
using Microsoft.OData.UriParser;
using System;

namespace Microsoft.AspNet.OData
{
  public abstract class PerRouteContainerBase : IPerRouteContainer
  {
    public Func<IContainerBuilder> BuilderFactory { get; set; }

    public IServiceProvider CreateODataRootContainer(
      string routeName,
      Action<IContainerBuilder> configureAction)
    {
      IServiceProvider odataRootContainer = this.CreateODataRootContainer(configureAction);
      this.SetContainer(routeName, odataRootContainer);
      return odataRootContainer;
    }

    public IServiceProvider CreateODataRootContainer(Action<IContainerBuilder> configureAction)
    {
      IContainerBuilder withCoreServices = this.CreateContainerBuilderWithCoreServices();
      if (configureAction != null)
        configureAction(withCoreServices);
      return withCoreServices.BuildContainer() ?? throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.NullContainer);
    }

    public bool HasODataRootContainer(string routeName) => this.GetContainer(routeName) != null;

    public IServiceProvider GetODataRootContainer(string routeName)
    {
      IServiceProvider container = this.GetContainer(routeName);
      if (container != null)
        return container;
      if (string.IsNullOrEmpty(routeName))
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.MissingNonODataContainer);
      throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.MissingODataContainer, (object) routeName);
    }

    internal void SetODataRootContainer(string routeName, IServiceProvider rootContainer) => this.SetContainer(routeName, rootContainer);

    protected abstract IServiceProvider GetContainer(string routeName);

    protected abstract void SetContainer(string routeName, IServiceProvider rootContainer);

    protected IContainerBuilder CreateContainerBuilderWithCoreServices()
    {
      IContainerBuilder builder;
      if (this.BuilderFactory != null)
      {
        builder = this.BuilderFactory();
        if (builder == null)
          throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.NullContainerBuilder);
      }
      else
        builder = (IContainerBuilder) new DefaultContainerBuilder();
      builder.AddDefaultODataServices();
      builder.AddService(ServiceLifetime.Singleton, typeof (ODataUriResolver), (Func<IServiceProvider, object>) (sp =>
      {
        return (object) new UnqualifiedODataUriResolver()
        {
          EnableCaseInsensitive = true
        };
      }));
      return builder;
    }
  }
}
