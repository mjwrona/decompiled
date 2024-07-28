// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ClientTypeAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false)]
  public sealed class ClientTypeAttribute : Attribute
  {
    public ClientTypeAttribute(string typeName)
      : this(typeName, (Type) null)
    {
    }

    public ClientTypeAttribute(Type type)
      : this((string) null, type)
    {
    }

    public ClientTypeAttribute(string typeName, Type type)
    {
      this.TypeName = typeName;
      this.Type = type;
    }

    public string TypeName { get; }

    public Type Type { get; }
  }
}
