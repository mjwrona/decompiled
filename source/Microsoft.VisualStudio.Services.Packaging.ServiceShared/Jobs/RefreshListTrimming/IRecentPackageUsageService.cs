// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.RefreshListTrimming.IRecentPackageUsageService
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.RefreshListTrimming
{
  public interface IRecentPackageUsageService
  {
    Task<IEnumerable<(Guid FeedId, int Count)>> GetCountsOfRecentlyUsedPackageNamesPerFeedAsync();

    Task<IEnumerable<(Guid FeedId, string ProtocolName, string PackageName)>> GetRecentlyUsedPackageNamesForFeedsAsync(
      IEnumerable<Guid> feedIds);
  }
}
