// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.ActivityTrackingInfoModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  public class ActivityTrackingInfoModel : InformationNodeModel
  {
    public ActivityTrackingInfoModel(BuildInformationNode node)
      : base(node)
    {
      this.Text = this.GetFieldValue(InformationFields.DisplayText);
    }
  }
}
