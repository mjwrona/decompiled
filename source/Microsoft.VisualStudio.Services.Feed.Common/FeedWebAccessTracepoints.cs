// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.FeedWebAccessTracepoints
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

namespace Microsoft.VisualStudio.Services.Feed.Common
{
  public static class FeedWebAccessTracepoints
  {
    public const string Area = "FeedWebAccess";
    public const string TraceLayer = "Service";
    public const int WebAccessExceptionEaten = 10019600;
    public const int PackageUnfollowDataProvider = 10019601;
    public const int PackageHubDataProviderGetDataEnter = 10019602;
    public const int PackageHubDataProviderGetDataLeave = 10019603;
    public const int PackageHubDataProviderGetDataException = 10019604;
    public const int FeedPickerDataProviderGetDataException = 10019607;
  }
}
