// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.ODataRouteAttribute
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;

namespace Microsoft.AspNet.OData.Routing
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
  public sealed class ODataRouteAttribute : Attribute
  {
    public ODataRouteAttribute()
      : this((string) null)
    {
    }

    public ODataRouteAttribute(string pathTemplate) => this.PathTemplate = pathTemplate ?? string.Empty;

    public string PathTemplate { get; private set; }

    public string RouteName { get; set; }
  }
}
