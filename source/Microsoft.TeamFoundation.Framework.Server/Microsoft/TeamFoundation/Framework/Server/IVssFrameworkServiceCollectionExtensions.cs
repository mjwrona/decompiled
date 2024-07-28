// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IVssFrameworkServiceCollectionExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class IVssFrameworkServiceCollectionExtensions
  {
    public static IVssFrameworkServiceCollection Add<TService, TImplementation>(
      this IVssFrameworkServiceCollection services)
      where TService : class
      where TImplementation : class, TService
    {
      services.Add(VssFrameworkServiceDescriptor.Create<TService, TImplementation>());
      return services;
    }

    public static IVssFrameworkServiceCollection Add(
      this IVssFrameworkServiceCollection services,
      Type serviceType,
      Type implementationType)
    {
      if (serviceType == (Type) null)
        throw new ArgumentNullException(nameof (serviceType));
      if (implementationType == (Type) null)
        throw new ArgumentNullException(nameof (implementationType));
      services.Add(VssFrameworkServiceDescriptor.Create(serviceType, implementationType));
      return services;
    }

    public static IVssFrameworkServiceCollection Add(
      this IVssFrameworkServiceCollection services,
      Type serviceType,
      object implementationInstance)
    {
      if (serviceType == (Type) null)
        throw new ArgumentNullException(nameof (serviceType));
      if (implementationInstance == null)
        throw new ArgumentNullException(nameof (implementationInstance));
      services.Add(VssFrameworkServiceDescriptor.Create(serviceType, implementationInstance));
      return services;
    }

    public static IVssFrameworkServiceCollection Add(
      this IVssFrameworkServiceCollection services,
      Type serviceType)
    {
      if (serviceType == (Type) null)
        throw new ArgumentNullException(nameof (serviceType));
      services.Add(VssFrameworkServiceDescriptor.Create(serviceType));
      return services;
    }

    public static IVssFrameworkServiceCollection Add(
      this IVssFrameworkServiceCollection services,
      VssFrameworkServiceDescriptor descriptor)
    {
      if (descriptor == null)
        throw new ArgumentNullException(nameof (descriptor));
      if (services.ContainsKey(descriptor.ServiceTypeName))
        throw new ArgumentException("Service " + descriptor.ServiceTypeName + " is already in the collection", nameof (descriptor));
      services[descriptor.ServiceTypeName] = descriptor;
      return services;
    }

    public static void TryAdd<TService, TImplementation>(
      this IVssFrameworkServiceCollection services)
      where TService : class
      where TImplementation : class, TService
    {
      services.TryAdd(VssFrameworkServiceDescriptor.Create<TService, TImplementation>());
    }

    public static void TryAdd(
      this IVssFrameworkServiceCollection services,
      Type serviceType,
      Type implementationType)
    {
      if (serviceType == (Type) null)
        throw new ArgumentNullException(nameof (serviceType));
      if (implementationType == (Type) null)
        throw new ArgumentNullException(nameof (implementationType));
      services.TryAdd(VssFrameworkServiceDescriptor.Create(serviceType, implementationType));
    }

    public static void TryAdd(
      this IVssFrameworkServiceCollection services,
      Type serviceType,
      object implementationInstance)
    {
      if (serviceType == (Type) null)
        throw new ArgumentNullException(nameof (serviceType));
      if (implementationInstance == null)
        throw new ArgumentNullException(nameof (implementationInstance));
      services.TryAdd(VssFrameworkServiceDescriptor.Create(serviceType, implementationInstance));
    }

    public static void TryAdd(this IVssFrameworkServiceCollection services, Type serviceType)
    {
      if (serviceType == (Type) null)
        throw new ArgumentNullException(nameof (serviceType));
      services.TryAdd(VssFrameworkServiceDescriptor.Create(serviceType));
    }

    public static void TryAdd(
      this IVssFrameworkServiceCollection services,
      VssFrameworkServiceDescriptor descriptor)
    {
      if (descriptor == null)
        throw new ArgumentNullException(nameof (descriptor));
      if (services.ContainsKey(descriptor.ServiceTypeName))
        return;
      services[descriptor.ServiceTypeName] = descriptor;
    }

    public static void TryAdd(
      this IVssFrameworkServiceCollection services,
      IEnumerable<VssFrameworkServiceDescriptor> descriptors)
    {
      if (descriptors == null)
        throw new ArgumentNullException(nameof (descriptors));
      foreach (VssFrameworkServiceDescriptor descriptor in descriptors)
        services.TryAdd(descriptor);
    }

    public static IVssFrameworkServiceCollection Replace(
      this IVssFrameworkServiceCollection services,
      VssFrameworkServiceDescriptor descriptor)
    {
      services[descriptor.ServiceTypeName] = descriptor != null ? descriptor : throw new ArgumentNullException(nameof (descriptor));
      return services;
    }
  }
}
