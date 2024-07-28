// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SmartRouterContext
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;


#nullable enable
namespace Microsoft.TeamFoundation.Framework.Server
{
  public class SmartRouterContext
  {
    public string? Status { get; private set; }

    public string? Reason { get; private set; }

    public string? TargetServer { get; private set; }

    public Uri? VssRewriteUrl { get; private set; }

    public bool IsStatus(string? status) => string.Equals(this.Status, status);

    public void SetReverseProxyReceived()
    {
      this.Status = "ProxyReceived";
      this.Reason = (string) null;
    }

    public void SetNotRoutable(string reason)
    {
      this.Status = "NotRoutable";
      this.Reason = reason;
    }

    public void SetRoutabled()
    {
      this.Status = "Routable";
      this.Reason = (string) null;
    }

    public void SetNotRouted(string reason)
    {
      this.Status = "NotRouted";
      this.Reason = reason;
    }

    public void SetRouted(string targetServer, string policyName)
    {
      this.Status = "Routed";
      this.TargetServer = targetServer;
      this.Reason = policyName;
    }

    public void SetRoutedReason(string reason)
    {
      if (!this.IsStatus("Routed"))
        return;
      this.Reason = reason;
    }

    public void SetException(Exception ex)
    {
      this.Status = "Exception";
      this.Reason = ex.Message;
    }
  }
}
