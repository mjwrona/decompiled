// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.ProjectFeedsConversionHelper
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;

namespace Microsoft.VisualStudio.Services.Feed.Common
{
  public class ProjectFeedsConversionHelper
  {
    public static FeedCore ExplicitlyCreateFeedCoreWithoutCallingGetFeed(FeedIdentity feedIdentity) => new FeedCore()
    {
      Id = feedIdentity.Id,
      Project = feedIdentity.ProjectId.ToProjectReference()
    };
  }
}
