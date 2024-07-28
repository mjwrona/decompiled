// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildServerConstants
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class BuildServerConstants
  {
    internal static string FeatureContributionId = "ms.vss-build.pipelines";
    internal static TimeSpan InfiniteRetentionLease = TimeSpan.FromDays(365000.0);
    public const string DeleteCheckSuitesEvent = "MS.Azure.Pipelines.DeleteCheckSuites";
    public const string DefaultQueueName = "Azure Pipelines";
    internal static int DefaultDeleteDefinitionJobSchedule = 30;
  }
}
