// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.AgentScopeActivityTrackingInfoModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  public class AgentScopeActivityTrackingInfoModel : InformationNodeModel
  {
    public AgentScopeActivityTrackingInfoModel(BuildInformationNode node)
      : base(node)
    {
      string fieldValue1 = this.GetFieldValue(InformationFields.DisplayText);
      string fieldValue2 = this.GetFieldValue(InformationFields.ReservedAgentName);
      string fieldValue3 = this.GetFieldValue(InformationFields.ReservationStatus);
      if (string.IsNullOrWhiteSpace(fieldValue2) || string.IsNullOrWhiteSpace(fieldValue3) || string.Equals(fieldValue3, "AgentRequested", StringComparison.OrdinalIgnoreCase))
        this.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, BuildServerResources.BuildDetailViewAgentScopeWaitingFormat, (object) fieldValue1);
      else
        this.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, BuildServerResources.BuildDetailViewAgentScopeExecutingFormat, (object) fieldValue1, (object) fieldValue2);
    }
  }
}
