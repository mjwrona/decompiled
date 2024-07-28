// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.IntermediateLogNodeModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  public class IntermediateLogNodeModel : InformationNodeModel
  {
    public IntermediateLogNodeModel(BuildInformationNode node)
      : base(node)
    {
      this.Location = this.GetFieldValue(InformationFields.LogFile);
      this.ErrorMessage = this.GetFieldValue(InformationFields.Message);
      this.RequestedBy = this.GetFieldValue(InformationFields.RequestedBy);
    }

    public string RequestedBy { get; set; }

    public string Location { get; set; }

    public string ErrorMessage { get; set; }

    protected override void ContributeToJson(JsObject json)
    {
      json["text"] = (object) this.Description;
      json["createdOn"] = (object) this.TimeStamp;
      json["requestedBy"] = (object) this.RequestedBy;
      json["location"] = (object) this.Location;
      json["errorMessage"] = (object) this.ErrorMessage;
    }

    public void MakeLocationRelative(string buildContainerLogsUrl)
    {
      string fieldValue = this.GetFieldValue(InformationFields.RelativeServerLogDirectory);
      if (!string.IsNullOrEmpty(buildContainerLogsUrl) && string.IsNullOrEmpty(fieldValue))
        this.Location = "logs\\" + BuildContainerPath.GetFolderName(BuildContainerPath.MakeRelative(buildContainerLogsUrl, this.Location));
      else
        this.Location = fieldValue;
    }

    public string Description
    {
      get
      {
        if (!string.IsNullOrEmpty(this.ErrorMessage))
          return BuildServerResources.BuildDetailViewDiagnosticsRequestFailed;
        return !string.IsNullOrEmpty(this.RequestedBy) ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, BuildServerResources.BuildDetailViewDiagnosticsLogAsOfFor, (object) this.TimeStamp, (object) this.RequestedBy) : string.Format((IFormatProvider) CultureInfo.CurrentCulture, BuildServerResources.BuildDetailViewDiagnosticsLogAsOfFor, (object) this.TimeStamp, (object) BuildServerResources.RequestBy_Unknown);
      }
    }
  }
}
