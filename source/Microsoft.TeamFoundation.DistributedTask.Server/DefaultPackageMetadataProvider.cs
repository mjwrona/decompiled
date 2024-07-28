// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DefaultPackageMetadataProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public class DefaultPackageMetadataProvider : IPackageMetadataProvider
  {
    internal const string DownloadUrlKey = "DownloadUrl";
    internal const string InfoUrlKey = "InfoUrl";
    internal const string HashValueKey = "HashValue";
    internal const string FilenameKey = "Filename";

    public string Key => "Default";

    public void EnsurePackageProperties(
      IVssRequestContext requestContext,
      PackageMetadata package,
      IDictionary<string, string> data)
    {
      if (data == null)
        return;
      string empty1 = string.Empty;
      if (data.TryGetValue("DownloadUrl", out empty1))
        package.DownloadUrl = empty1;
      string empty2 = string.Empty;
      if (data.TryGetValue("InfoUrl", out empty2))
        package.InfoUrl = empty2;
      string empty3 = string.Empty;
      if (data.TryGetValue("HashValue", out empty3))
        package.HashValue = empty3;
      string empty4 = string.Empty;
      if (!data.TryGetValue("Filename", out empty4))
        return;
      package.Filename = empty4;
    }
  }
}
