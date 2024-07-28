// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.FeedChangeTracepoints
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

namespace Microsoft.VisualStudio.Services.Feed.Common
{
  public static class FeedChangeTracepoints
  {
    public const string Area = "FeedChange";
    public const string ServiceTraceLayer = "Service";
    public const int ServiceStartEnter = 10019400;
    public const int ServiceStartLeave = 10019401;
    public const int ServiceStartException = 10019402;
    public const int ServiceEndEnter = 10019403;
    public const int ServiceEndLeave = 10019404;
    public const int ServiceEndException = 10019405;
    public const int GetFeedChangesEnter = 10019406;
    public const int GetFeedChangesLeave = 10019407;
    public const int GetFeedChangesException = 10019408;
    public const int GetFeedChangeEnter = 100192409;
    public const int GetFeedChangeLeave = 10019410;
    public const int GetFeedChangeException = 10019411;
    public const int GetPackageChangesEnter = 10019412;
    public const int GetPackageChangesLeave = 10019413;
    public const int GetPackageChangesException = 10019414;
  }
}
