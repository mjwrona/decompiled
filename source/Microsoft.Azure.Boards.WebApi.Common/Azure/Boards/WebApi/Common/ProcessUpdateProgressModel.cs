// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.ProcessUpdateProgressModel
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Promote;

namespace Microsoft.Azure.Boards.WebApi.Common
{
  public class ProcessUpdateProgressModel
  {
    public bool IsSuccessful { get; set; }

    public int TotalProjectsCount { get; set; }

    public int ProcessedProjectsCount { get; set; }

    public int RemainingRetries { get; set; }

    public PromoteProjectInfo FailedProject { get; set; }

    public int Complete { get; set; }

    public int Remaining { get; set; }
  }
}
