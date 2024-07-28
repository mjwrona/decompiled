// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Routing.SupportedRouteAreaAttribute
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Routing
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class SupportedRouteAreaAttribute : Attribute
  {
    public SupportedRouteAreaAttribute()
    {
    }

    public SupportedRouteAreaAttribute(NavigationContextLevels levels)
      : this("", levels)
    {
    }

    public SupportedRouteAreaAttribute(string area, NavigationContextLevels levels)
    {
      this.Area = area;
      this.NavigationLevels = levels;
    }

    public string Area { get; set; }

    public NavigationContextLevels NavigationLevels { get; set; }
  }
}
