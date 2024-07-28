// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.InvalidContributionInputTypeException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  public class InvalidContributionInputTypeException : Exception
  {
    public string ContributionId { get; private set; }

    public string ContributionInputId { get; private set; }

    public string ContributionInputType { get; private set; }

    public InvalidContributionInputTypeException(
      string contributionId,
      string contributionInputId,
      string contributionInputType)
    {
      this.ContributionId = contributionId;
      this.ContributionInputId = contributionInputId;
      this.ContributionInputType = contributionInputType;
    }
  }
}
