// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostRouteContext
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class HostRouteContext
  {
    private string m_virtualPath;
    private string m_webApplicationPath;

    public Guid HostId { get; set; }

    public string VirtualPath
    {
      get => this.m_virtualPath;
      set => this.m_virtualPath = VirtualPathUtility.AppendTrailingSlash(value);
    }

    public string WebApplicationPath
    {
      get => this.m_webApplicationPath;
      set => this.m_webApplicationPath = VirtualPathUtility.AppendTrailingSlash(value);
    }

    public string ExpectedRouteKey { get; set; }

    public int? RouteKeyVersion { get; set; }

    public RouteFlags RouteFlags { get; set; }

    public string AccessMappingMonikers { get; set; }

    public bool HasRouteFlags(RouteFlags flags) => (this.RouteFlags & flags) == flags;
  }
}
