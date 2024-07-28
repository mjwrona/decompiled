// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TenantPolicy.PolicyExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.TenantPolicy
{
  public static class PolicyExtensions
  {
    public static Microsoft.VisualStudio.Services.TenantPolicy.Client.Policy ToClient(this Policy x)
    {
      if (x == null)
        return (Microsoft.VisualStudio.Services.TenantPolicy.Client.Policy) null;
      Microsoft.VisualStudio.Services.TenantPolicy.Client.Policy client = new Microsoft.VisualStudio.Services.TenantPolicy.Client.Policy();
      client.Name = x.Name;
      client.Value = x.Value;
      IDictionary<string, string> properties = x.Properties;
      client.Properties = properties != null ? properties.Copy<string, string>((IDictionary<string, string>) new Dictionary<string, string>()) : (IDictionary<string, string>) null;
      return client;
    }

    public static Policy ToServer(this Microsoft.VisualStudio.Services.TenantPolicy.Client.Policy x) => x == null ? (Policy) null : new Policy(x.Name, x.Value, x.Properties);

    public static Microsoft.VisualStudio.Services.TenantPolicy.Client.PolicyInfo ToClient(
      this PolicyInfo x)
    {
      if (x == null)
        return (Microsoft.VisualStudio.Services.TenantPolicy.Client.PolicyInfo) null;
      return new Microsoft.VisualStudio.Services.TenantPolicy.Client.PolicyInfo()
      {
        Name = x.Name,
        Description = x.Description,
        MoreInfoLink = x.MoreInfoLink
      };
    }
  }
}
