// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.DeploymentGateUpdateChangeDetails
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
  public class DeploymentGateUpdateChangeDetails : ReleaseRevisionChangeDetails
  {
    public DeploymentGateUpdateChangeDetails() => this.Id = ReleaseHistoryMessageId.DeploymentGateChange;

    public string GateName { get; set; }

    public EnvironmentStepType GateType { get; set; }

    public string EnvironmentName { get; set; }

    public bool IsProcessing { get; set; }

    public override string ToString()
    {
      Dictionary<EnvironmentStepType, string> dictionary1 = new Dictionary<EnvironmentStepType, string>()
      {
        {
          EnvironmentStepType.PreGate,
          Resources.ReleaseHistoryPreDeploymentGateUpdate
        },
        {
          EnvironmentStepType.PostGate,
          Resources.ReleaseHistoryPostDeploymentGateUpdate
        }
      };
      Dictionary<EnvironmentStepType, string> dictionary2 = new Dictionary<EnvironmentStepType, string>()
      {
        {
          EnvironmentStepType.PreGate,
          Resources.ReleaseHistoryPreDeploymentGateUpdateInprogress
        },
        {
          EnvironmentStepType.PostGate,
          Resources.ReleaseHistoryPostDeploymentGateUpdateInprogress
        }
      };
      if (!dictionary1.ContainsKey(this.GateType))
        return Resources.ReleaseHistoryChangeDetailsUnknownMessage;
      return this.IsProcessing ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, dictionary2[this.GateType], (object) this.GateName, (object) this.EnvironmentName) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, dictionary1[this.GateType], (object) this.GateName, (object) this.EnvironmentName);
    }
  }
}
