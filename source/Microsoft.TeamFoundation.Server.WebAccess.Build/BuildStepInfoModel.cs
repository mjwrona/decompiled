// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildStepInfoModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  public class BuildStepInfoModel : InformationNodeModel
  {
    public string Status { get; private set; }

    public BuildStepInfoModel(BuildInformationNode node)
      : base(node)
    {
      string fieldValue = this.GetFieldValue(InformationFields.Status);
      if (string.Equals("InProgress", fieldValue, StringComparison.OrdinalIgnoreCase))
        this.Status = "inprogress";
      else if (string.Equals("Succeeded", fieldValue, StringComparison.OrdinalIgnoreCase))
        this.Status = "succeeded";
      else if (string.Equals("Failed", fieldValue, StringComparison.OrdinalIgnoreCase))
        this.Status = "failed";
      this.Text = this.GetFieldValue(InformationFields.Message);
    }

    protected override void ContributeToJson(JsObject result)
    {
      if (string.IsNullOrWhiteSpace(this.Status))
        return;
      result["status"] = (object) this.Status;
    }
  }
}
