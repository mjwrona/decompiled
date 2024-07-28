// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DefaultServiceImplementationAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
  public sealed class DefaultServiceImplementationAttribute : Attribute
  {
    public DefaultServiceImplementationAttribute(Type type) => this.Type = type;

    public DefaultServiceImplementationAttribute(string typeName) => this.TypeName = typeName;

    public DefaultServiceImplementationAttribute(Type type, Type virtualHostType)
    {
      this.Type = type;
      this.VirtualHostType = virtualHostType;
    }

    public DefaultServiceImplementationAttribute(string typeName, string virtualHostTypeName)
    {
      this.TypeName = typeName;
      this.VirtualHostTypeName = virtualHostTypeName;
    }

    public Type Type { get; set; }

    public Type VirtualHostType { get; set; }

    public string TypeName { get; set; }

    public string VirtualHostTypeName { get; set; }

    public Type GetImplementation(bool forVirtualHost = false)
    {
      if (forVirtualHost)
      {
        if (this.VirtualHostType != (Type) null)
          return this.VirtualHostType;
        if (!string.IsNullOrEmpty(this.VirtualHostTypeName))
          return Type.GetType(this.VirtualHostTypeName);
      }
      return this.Type != (Type) null ? this.Type : Type.GetType(this.TypeName);
    }
  }
}
