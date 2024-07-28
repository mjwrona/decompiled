// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.DefaultContainerBuilder
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using System;
using System.Reflection;

namespace Microsoft.AspNet.OData
{
  public class DefaultContainerBuilder : IContainerBuilder
  {
    private readonly IServiceCollection services = (IServiceCollection) new ServiceCollection();

    public virtual IContainerBuilder AddService(
      Microsoft.OData.ServiceLifetime lifetime,
      Type serviceType,
      Type implementationType)
    {
      if (serviceType == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (serviceType));
      if (implementationType == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (implementationType));
      this.services.Add(new ServiceDescriptor(serviceType, implementationType, DefaultContainerBuilder.TranslateServiceLifetime(lifetime)));
      return (IContainerBuilder) this;
    }

    public IContainerBuilder AddService(
      Microsoft.OData.ServiceLifetime lifetime,
      Type serviceType,
      Func<IServiceProvider, object> implementationFactory)
    {
      if (serviceType == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (serviceType));
      if (implementationFactory == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (implementationFactory));
      this.services.Add(new ServiceDescriptor(serviceType, implementationFactory, DefaultContainerBuilder.TranslateServiceLifetime(lifetime)));
      return (IContainerBuilder) this;
    }

    public virtual IServiceProvider BuildContainer() => (IServiceProvider) this.services.GetType().GetTypeInfo().Assembly.GetType(typeof (ServiceCollectionContainerBuilderExtensions).GetTypeInfo().FullName).GetTypeInfo().GetMethod("BuildServiceProvider", new Type[1]
    {
      typeof (IServiceCollection)
    }).Invoke((object) null, new object[1]
    {
      (object) this.services
    });

    private static Microsoft.Extensions.DependencyInjection.ServiceLifetime TranslateServiceLifetime(
      Microsoft.OData.ServiceLifetime lifetime)
    {
      if (lifetime == Microsoft.OData.ServiceLifetime.Singleton)
        return Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton;
      return lifetime == Microsoft.OData.ServiceLifetime.Scoped ? Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped : Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient;
    }
  }
}
