// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ReassignedApprovalChangeDetails
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public class ReassignedApprovalChangeDetails : ReleaseRevisionChangeDetails
  {
    public ReassignedApprovalChangeDetails() => this.Id = ReleaseHistoryMessageId.ReassignedApprovalChange;

    public EnvironmentStepType StepType { get; set; }

    public IdentityRef AssignedTo { get; set; }

    public string EnvironmentName { get; set; }

    public override string ToString()
    {
      Dictionary<EnvironmentStepType, string> dictionary = new Dictionary<EnvironmentStepType, string>()
      {
        {
          EnvironmentStepType.PreDeploy,
          Resources.ReleaseApprovalHistoryReassignedPreApproval
        },
        {
          EnvironmentStepType.PostDeploy,
          Resources.ReleaseApprovalHistoryReassignedPostApproval
        }
      };
      return dictionary.ContainsKey(this.StepType) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, dictionary[this.StepType], (object) this.EnvironmentName, (object) (this.AssignedTo.DisplayName ?? this.AssignedTo.Id)) : Resources.ReleaseHistoryChangeDetailsUnknownMessage;
    }
  }
}
