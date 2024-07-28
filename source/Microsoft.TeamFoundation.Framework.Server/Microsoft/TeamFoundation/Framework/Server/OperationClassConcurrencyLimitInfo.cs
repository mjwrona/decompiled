// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OperationClassConcurrencyLimitInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class OperationClassConcurrencyLimitInfo
  {
    public readonly int MaxConcurrency;
    public readonly int RetryTime;
    public readonly JobPriorityLevel PriorityLevel;

    public OperationClassConcurrencyLimitInfo(
      int maxConcurrency,
      int retryTime,
      JobPriorityLevel priorityLevel)
    {
      ArgumentUtility.CheckForNonPositiveInt(maxConcurrency, nameof (maxConcurrency));
      ArgumentUtility.CheckForNonPositiveInt(retryTime, nameof (retryTime));
      ArgumentUtility.CheckForDefinedEnum<JobPriorityLevel>(priorityLevel, nameof (priorityLevel));
      if (priorityLevel == JobPriorityLevel.None)
        throw new ArgumentOutOfRangeException("priorityLevel must be higher than JobPriorityLevel.None");
      this.MaxConcurrency = maxConcurrency;
      this.RetryTime = retryTime;
      this.PriorityLevel = priorityLevel;
    }
  }
}
