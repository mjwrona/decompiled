// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.AggregatedCapacityLimitExceededException
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Models
{
  [Serializable]
  internal sealed class AggregatedCapacityLimitExceededException : TeamFoundationServiceException
  {
    public AggregatedCapacityLimitExceededException(int limit, int count)
    {
      this.ReportException = false;
      this.WorkItemLimit = limit;
      this.WorkItemCount = count;
    }

    private AggregatedCapacityLimitExceededException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }

    public int WorkItemLimit { get; private set; }

    public int WorkItemCount { get; private set; }
  }
}
