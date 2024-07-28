// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ServiceProviderExtensions
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;

namespace Microsoft.OData
{
  internal static class ServiceProviderExtensions
  {
    public static T GetService<T>(this IServiceProvider container) => (T) container.GetService(typeof (T));

    public static object GetRequiredService(this IServiceProvider container, Type serviceType) => container.GetService(serviceType) ?? throw new ODataException(Strings.ServiceProviderExtensions_NoServiceRegistered((object) serviceType));

    public static T GetRequiredService<T>(this IServiceProvider container) => (T) container.GetRequiredService(typeof (T));

    public static IEnumerable<T> GetServices<T>(this IServiceProvider container) => container.GetRequiredService<IEnumerable<T>>();

    public static IEnumerable<object> GetServices(this IServiceProvider container, Type serviceType)
    {
      Type serviceType1 = typeof (IEnumerable<>).MakeGenericType(serviceType);
      return (IEnumerable<object>) container.GetRequiredService(serviceType1);
    }

    public static TService GetServicePrototype<TService>(this IServiceProvider container) => container.GetRequiredService<ServicePrototype<TService>>().Instance;
  }
}
