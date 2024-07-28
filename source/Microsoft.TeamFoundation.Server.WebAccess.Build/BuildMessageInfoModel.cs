// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildMessageInfoModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  public class BuildMessageInfoModel : InformationNodeModel
  {
    public string Status { get; private set; }

    public BuildMessageInfoModel(BuildInformationNode node)
      : base(node)
    {
      if (string.Equals(InformationTypes.BuildError, node.Type, StringComparison.OrdinalIgnoreCase))
      {
        this.Status = "error";
        this.Text = this.GetMessage();
      }
      else if (string.Equals(InformationTypes.BuildWarning, node.Type, StringComparison.OrdinalIgnoreCase))
      {
        this.Status = "warning";
        this.Text = this.GetMessage();
      }
      else
        this.Text = this.GetFieldValue(InformationFields.Message);
    }

    private string GetMessage()
    {
      string fieldValue1 = this.GetFieldValue(InformationFields.Message);
      string fieldValue2 = this.GetFieldValue(InformationFields.File);
      int fieldValueInt = this.GetFieldValueInt(InformationFields.LineNumber);
      if (string.IsNullOrWhiteSpace(fieldValue2))
        return fieldValue1;
      return fieldValueInt <= 0 ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, BuildResources.BuildDetailViewErrorWarningDetailNoLineNumber, (object) fieldValue2, (object) fieldValue1) : string.Format((IFormatProvider) CultureInfo.CurrentCulture, BuildResources.BuildDetailViewErrorWarningDetail, (object) fieldValue2, (object) fieldValueInt, (object) fieldValue1);
    }

    protected override void ContributeToJson(JsObject json)
    {
      if (!string.IsNullOrWhiteSpace(this.Status))
        json["status"] = (object) this.Status;
      json["message"] = (object) true;
      string fieldValue1 = this.GetFieldValue(InformationFields.ErrorType);
      if (!string.IsNullOrWhiteSpace(fieldValue1))
        json["errorType"] = (object) fieldValue1;
      string fieldValue2 = this.GetFieldValue(InformationFields.WarningType);
      if (string.IsNullOrWhiteSpace(fieldValue2))
        return;
      json["warningType"] = (object) fieldValue2;
    }
  }
}
