// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.UrlDeploymentCorrelationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class UrlDeploymentCorrelationService : 
    VssMemoryCacheService<CorrelationKey, string>,
    IUrlDeploymentCorrelationService,
    IVssFrameworkService
  {
    public CorrelationKey GetCorrelationKey(Uri url)
    {
      ArgumentUtility.CheckForNull<Uri>(url, nameof (url));
      ArgumentUtility.CheckStringForNullOrEmpty(url.Host, "Host");
      if (url.Segments.Length < 2 || url.Segments[1][0] == '_')
        return (CorrelationKey) null;
      string str1 = url.Segments[1].TrimEnd('/');
      if (string.Equals(str1, "serviceHosts", StringComparison.OrdinalIgnoreCase) || string.Equals(str1, "serviceDeployments", StringComparison.OrdinalIgnoreCase))
      {
        string str2;
        if (url.Segments.Length < 3)
          str2 = (string) null;
        else
          str2 = url.Segments[2].TrimEnd('/');
        str1 = str2;
      }
      return url.Host == null || str1 == null ? (CorrelationKey) null : new CorrelationKey(url.Host, str1);
    }

    public string GetCorrelation(CorrelationKey key)
    {
      string str;
      return key == null || !this.MemoryCache.TryGetValue(key, out str) ? (string) null : str;
    }

    public void SetCorrelation(CorrelationKey key, string deploymentId)
    {
      if (key == null)
        return;
      this.MemoryCache[key] = deploymentId;
    }
  }
}
