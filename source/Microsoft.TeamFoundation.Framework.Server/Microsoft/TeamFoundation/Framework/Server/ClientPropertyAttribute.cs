// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ClientPropertyAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
  public sealed class ClientPropertyAttribute : Attribute
  {
    public ClientPropertyAttribute()
      : this(ClientVisibility.Public)
    {
    }

    public ClientPropertyAttribute(ClientVisibility getterVisibility)
      : this(getterVisibility, getterVisibility)
    {
    }

    public ClientPropertyAttribute(
      ClientVisibility getterVisibility,
      ClientVisibility setterVisibility)
    {
      this.GetterVisibility = getterVisibility;
      this.SetterVisibility = setterVisibility;
      this.Direction = ClientPropertySerialization.Bidirectional;
    }

    public ClientVisibility GetterVisibility { get; }

    public ClientVisibility SetterVisibility { get; }

    public string PropertyName { get; set; }

    public ClientPropertySerialization Direction { get; set; }

    public bool UseClientDefinedProperty { get; set; }
  }
}
