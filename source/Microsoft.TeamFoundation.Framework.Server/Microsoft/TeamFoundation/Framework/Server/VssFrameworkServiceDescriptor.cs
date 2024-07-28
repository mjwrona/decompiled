// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssFrameworkServiceDescriptor
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class VssFrameworkServiceDescriptor
  {
    private VssFrameworkServiceDescriptor(string serviceTypeName, Type implementationType)
    {
      this.ServiceTypeName = serviceTypeName;
      this.ImplementationType = implementationType;
    }

    private VssFrameworkServiceDescriptor(Type serviceType, Type implementationType)
    {
      this.ServiceTypeName = serviceType.Name;
      this.ServiceType = serviceType;
      this.ImplementationType = implementationType;
    }

    private VssFrameworkServiceDescriptor(Type serviceType, object implementationInstance)
    {
      this.ServiceTypeName = serviceType.Name;
      this.ServiceType = serviceType;
      this.ImplementationType = implementationInstance.GetType();
      this.ImplementationInstance = implementationInstance;
    }

    private VssFrameworkServiceDescriptor(Type serviceType)
      : this(serviceType, serviceType)
    {
    }

    public static VssFrameworkServiceDescriptor Create(
      string serviceTypeName,
      Type implementationType)
    {
      return new VssFrameworkServiceDescriptor(serviceTypeName, implementationType);
    }

    public static VssFrameworkServiceDescriptor Create<TService, TImplementation>()
      where TService : class
      where TImplementation : class, TService
    {
      return new VssFrameworkServiceDescriptor(typeof (TService), typeof (TImplementation));
    }

    public static VssFrameworkServiceDescriptor Create(Type serviceType, Type implementationType)
    {
      if (serviceType == (Type) null)
        throw new ArgumentNullException(nameof (serviceType));
      return !(implementationType == (Type) null) ? new VssFrameworkServiceDescriptor(serviceType, implementationType) : throw new ArgumentNullException(nameof (implementationType));
    }

    public static VssFrameworkServiceDescriptor Create(
      Type serviceType,
      object implementationInstance)
    {
      if (serviceType == (Type) null)
        throw new ArgumentNullException(nameof (serviceType));
      return implementationInstance != null ? new VssFrameworkServiceDescriptor(serviceType, implementationInstance) : throw new ArgumentNullException(nameof (implementationInstance));
    }

    public static VssFrameworkServiceDescriptor Create(Type serviceType) => !(serviceType == (Type) null) ? new VssFrameworkServiceDescriptor(serviceType) : throw new ArgumentNullException(nameof (serviceType));

    public string ServiceTypeName { get; }

    public Type ServiceType { get; }

    public Type ImplementationType { get; }

    public object ImplementationInstance { get; }
  }
}
