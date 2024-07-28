// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.LocationHelper
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class LocationHelper
  {
    public static string GetRootServerUrl(string locationServiceUrl)
    {
      if (locationServiceUrl.EndsWith(LocationServiceConstants.CollectionLocationServiceRelativePath, StringComparison.OrdinalIgnoreCase))
        return new Uri(UriUtility.TrimEndingPathSeparator(locationServiceUrl.Remove(locationServiceUrl.Length - LocationServiceConstants.CollectionLocationServiceRelativePath.Length))).AbsoluteUri;
      return locationServiceUrl.EndsWith(LocationServiceConstants.ApplicationLocationServiceRelativePath, StringComparison.OrdinalIgnoreCase) ? new Uri(UriUtility.TrimEndingPathSeparator(locationServiceUrl.Remove(locationServiceUrl.Length - LocationServiceConstants.ApplicationLocationServiceRelativePath.Length))).AbsoluteUri : new Uri(locationServiceUrl).AbsoluteUri;
    }

    public static string CreateSecurityToken(string serviceType, Guid identifier) => FrameworkSecurity.ServiceDefinitionsToken + (object) FrameworkSecurity.LocationPathSeparator + serviceType + (object) FrameworkSecurity.LocationPathSeparator + identifier.ToString("D");
  }
}
