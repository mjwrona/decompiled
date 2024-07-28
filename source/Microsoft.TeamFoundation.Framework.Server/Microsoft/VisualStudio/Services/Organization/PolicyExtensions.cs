// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.PolicyExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Organization.Client;

namespace Microsoft.VisualStudio.Services.Organization
{
  public static class PolicyExtensions
  {
    public static Policy ToClient<T>(this Policy<T> x)
    {
      if (x == null)
        return (Policy) null;
      return new Policy()
      {
        Name = x.Name,
        Value = (object) x.Value,
        Enforce = x.Enforce,
        EffectiveValue = (object) x.EffectiveValue,
        IsValueUndefined = x.IsValueUndefined,
        ParentPolicy = x.ParentPolicy.ToClient<T>()
      };
    }

    public static Microsoft.VisualStudio.Services.Organization.Client.PolicyInfo ToClient(
      this PolicyInfo x)
    {
      if (x == null)
        return (Microsoft.VisualStudio.Services.Organization.Client.PolicyInfo) null;
      return new Microsoft.VisualStudio.Services.Organization.Client.PolicyInfo()
      {
        Name = x.Name,
        Description = x.Description,
        MoreInfoLink = x.MoreInfoLink
      };
    }
  }
}
