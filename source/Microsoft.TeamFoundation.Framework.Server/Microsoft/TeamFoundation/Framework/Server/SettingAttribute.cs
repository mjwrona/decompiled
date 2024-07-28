// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SettingAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
  public sealed class SettingAttribute : Attribute
  {
    public SettingAttribute()
    {
      this.IsRequired = true;
      this.ResolveSecret = true;
    }

    public bool IsRequired { get; set; }

    public bool ResolveSecret { get; set; }

    public string SettingName { get; set; }
  }
}
