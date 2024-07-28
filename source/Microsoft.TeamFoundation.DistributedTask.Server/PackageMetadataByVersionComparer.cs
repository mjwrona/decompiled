// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.PackageMetadataByVersionComparer
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public class PackageMetadataByVersionComparer : IEqualityComparer<PackageMetadata>
  {
    public bool Equals(PackageMetadata first, PackageMetadata second)
    {
      if (first == second)
        return true;
      return first != null && second != null && string.Equals(first.Type, second.Type, StringComparison.OrdinalIgnoreCase) && string.Equals(first.Platform, second.Platform, StringComparison.OrdinalIgnoreCase) && first.Version.Equals(second.Version);
    }

    public int GetHashCode(PackageMetadata obj) => ((17 * 23 + (obj.Type == null ? 0 : obj.Type.GetHashCode())) * 23 + (obj.Platform == null ? 0 : obj.Platform.GetHashCode())) * 23 + (obj.Version == null ? 0 : obj.Version.GetHashCode());
  }
}
