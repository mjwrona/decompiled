// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.WikiJobs
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using System;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public static class WikiJobs
  {
    public static readonly Guid WikiNotificationAggregationJobId = Guid.Parse("2FD1B806-E493-4C9A-AAC2-B35FA619DF05");
    public static readonly Guid WikiPageViewAggregationJobId = Guid.Parse("BF933958-2525-4CB3-960A-241CB5385274");
    public static readonly Guid WikiPageViewDailyAggregationJobId = Guid.Parse("75C320E2-2EFF-455D-8C78-BB588C6F6E83");
  }
}
