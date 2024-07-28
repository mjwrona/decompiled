// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ReleaseEnvironmentStatusChangeDetails
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public class ReleaseEnvironmentStatusChangeDetails : ReleaseRevisionChangeDetails
  {
    public ReleaseEnvironmentStatusChangeDetails() => this.Id = ReleaseHistoryMessageId.EnvironmentStatusChange;

    public string EnvironmentName { get; set; }

    public ReleaseEnvironmentStatus EnvironmentStatus { get; set; }

    public override string ToString()
    {
      Dictionary<ReleaseEnvironmentStatus, string> dictionary = new Dictionary<ReleaseEnvironmentStatus, string>()
      {
        {
          ReleaseEnvironmentStatus.InProgress,
          Resources.ReleaseEnvironmentHistoryTriggeredDeployment
        },
        {
          ReleaseEnvironmentStatus.Succeeded,
          Resources.ReleaseEnvironmentHistorySucceededDeployment
        },
        {
          ReleaseEnvironmentStatus.Canceled,
          Resources.ReleaseEnvironmentHistoryCancelledDeployment
        },
        {
          ReleaseEnvironmentStatus.Rejected,
          Resources.ReleaseEnvironmentHistoryRejectedDeployment
        },
        {
          ReleaseEnvironmentStatus.Queued,
          Resources.ReleaseEnvironmentHistoryQueuedDeployment
        },
        {
          ReleaseEnvironmentStatus.Scheduled,
          Resources.ReleaseEnvironmentHistoryScheduledDeployment
        },
        {
          ReleaseEnvironmentStatus.NotStarted,
          Resources.ReleaseEnvironmentHistoryResetDeployment
        },
        {
          ReleaseEnvironmentStatus.PartiallySucceeded,
          Resources.ReleaseEnvironmentHistoryPartiallySucceededDeployment
        }
      };
      return dictionary.ContainsKey(this.EnvironmentStatus) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, dictionary[this.EnvironmentStatus], (object) this.EnvironmentName) : Resources.ReleaseHistoryChangeDetailsUnknownReleaseEnvironmentStatus;
    }

    public override string GetStageName() => this.EnvironmentName;

    public override ReleaseEnvironmentStatus GetReleaseEnvironmentStatus() => this.EnvironmentStatus;
  }
}
