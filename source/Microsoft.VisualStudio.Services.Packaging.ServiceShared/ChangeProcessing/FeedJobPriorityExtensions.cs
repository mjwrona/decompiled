// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing.FeedJobPriorityExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing
{
  public static class FeedJobPriorityExtensions
  {
    public const int MinimumEffectiveJobPriorityForUserJobs = 8;

    public static JobPriorityLevel GetPriorityLevel(this FeedJobPriority feedJobPriority) => feedJobPriority == FeedJobPriority.UserInitiated ? JobPriorityLevel.Normal : JobPriorityLevel.Idle;

    public static bool IsUserInitiatedJob(TeamFoundationJobDefinition jobDefinition) => jobDefinition.PriorityClassValue >= 8;
  }
}
