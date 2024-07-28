// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.FeedRecycleBinTracepoints
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

namespace Microsoft.VisualStudio.Services.Feed.Common
{
  public static class FeedRecycleBinTracepoints
  {
    public const string Area = "FeedRecycleBin";
    public const string TraceLayer = "Service";
    public const int PermanentDeleteFeedEnter = 10019700;
    public const int PermanentDeleteFeedLeave = 10019701;
    public const int PermanentDeleteFeedException = 10019702;
    public const int RestoreDeletedFeedEnter = 10019703;
    public const int RestoreDeletedFeedLeave = 10019704;
    public const int RestoreDeletedFeedException = 10019705;
    public const int GetFeedsInRecycleBinEnter = 10019706;
    public const int GetFeedsInRecycleBinLeave = 10019707;
    public const int GetFeedsInRecycleBinException = 10019708;
  }
}
