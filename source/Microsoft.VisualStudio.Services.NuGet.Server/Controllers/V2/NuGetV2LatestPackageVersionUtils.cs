// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2.NuGetV2LatestPackageVersionUtils
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2
{
  internal static class NuGetV2LatestPackageVersionUtils
  {
    public static void PopulateLatestVersion(
      IList<ServerV2FeedPackage> packagesOrderedByVersion)
    {
      ServerV2FeedPackage serverV2FeedPackage1 = (ServerV2FeedPackage) null;
      ServerV2FeedPackage serverV2FeedPackage2 = (ServerV2FeedPackage) null;
      for (int index = packagesOrderedByVersion.Count - 1; index >= 0; --index)
      {
        ServerV2FeedPackage serverV2FeedPackage3 = packagesOrderedByVersion[index];
        if (serverV2FeedPackage3.Listed)
        {
          if (serverV2FeedPackage2 == null)
            serverV2FeedPackage2 = serverV2FeedPackage3;
          if (serverV2FeedPackage1 == null && !serverV2FeedPackage3.PackageIdentity.Version.NuGetVersion.IsPrerelease)
          {
            serverV2FeedPackage1 = serverV2FeedPackage3;
            break;
          }
        }
      }
      if (serverV2FeedPackage2 == null)
      {
        for (int index = packagesOrderedByVersion.Count - 1; index >= 0; --index)
        {
          ServerV2FeedPackage serverV2FeedPackage4 = packagesOrderedByVersion[index];
          if (serverV2FeedPackage2 == null)
            serverV2FeedPackage2 = serverV2FeedPackage4;
          if (serverV2FeedPackage1 == null && !serverV2FeedPackage4.PackageIdentity.Version.NuGetVersion.IsPrerelease)
          {
            serverV2FeedPackage1 = serverV2FeedPackage4;
            break;
          }
        }
      }
      if (serverV2FeedPackage1 != null)
        serverV2FeedPackage1.IsLatestVersion = true;
      if (serverV2FeedPackage2 == null)
        return;
      serverV2FeedPackage2.IsAbsoluteLatestVersion = true;
    }
  }
}
