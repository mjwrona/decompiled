// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ReleaseApprovalStatusChangeDetails
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public class ReleaseApprovalStatusChangeDetails : ReleaseRevisionChangeDetails
  {
    public ReleaseApprovalStatusChangeDetails() => this.Id = ReleaseHistoryMessageId.ApprovalStatusChange;

    public ApprovalStatus ApprovalStatus { get; set; }

    public string EnvironmentName { get; set; }

    public EnvironmentStepType ApprovalType { get; set; }

    public override string ToString()
    {
      Dictionary<EnvironmentStepType, Func<ApprovalStatus, string>> dictionary = new Dictionary<EnvironmentStepType, Func<ApprovalStatus, string>>()
      {
        {
          EnvironmentStepType.PreDeploy,
          (Func<ApprovalStatus, string>) (approvalStatus => !approvalStatus.Equals((object) ApprovalStatus.Approved) ? Resources.ReleaseApprovalHistoryRejectedPreApproval : Resources.ReleaseApprovalHistoryApprovedPreApproval)
        },
        {
          EnvironmentStepType.PostDeploy,
          (Func<ApprovalStatus, string>) (approvalStatus => !approvalStatus.Equals((object) ApprovalStatus.Approved) ? Resources.ReleaseApprovalHistoryRejectedPostApproval : Resources.ReleaseApprovalHistoryApprovedPostApproval)
        }
      };
      return dictionary.ContainsKey(this.ApprovalType) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, dictionary[this.ApprovalType](this.ApprovalStatus), (object) this.EnvironmentName) : Resources.ReleaseHistoryChangeDetailsUnknownApprovalStatus;
    }
  }
}
