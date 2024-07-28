// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ThrowInvalidDataCommitHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class ThrowInvalidDataCommitHandler : FeedIndexCommitHandler
  {
    public override Task ApplyCommitAsync(
      IFeedIndexAggregationContext context,
      FeedCore feed,
      ICommitLogEntry commitLogEntry)
    {
      throw new InvalidDataException(Resources.Error_InvalidCommitEntryType());
    }

    public ThrowInvalidDataCommitHandler()
      : base()
    {
    }
  }
}
