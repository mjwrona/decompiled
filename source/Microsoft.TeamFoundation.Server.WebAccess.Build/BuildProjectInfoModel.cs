// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildProjectInfoModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  public class BuildProjectInfoModel : InformationNodeModel
  {
    public BuildProjectInfoModel(BuildInformationNode node)
      : base(node)
    {
      string fileName = this.GetFileName();
      string fieldValue = this.GetFieldValue(InformationFields.TargetNames);
      if (!string.IsNullOrWhiteSpace(fieldValue))
        this.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, BuildResources.BuildDetailViewProjectNodeWithTargetsFormat, (object) fileName, (object) fieldValue);
      else
        this.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, BuildResources.BuildDetailViewProjectNodeFormat, (object) fileName);
    }

    private string GetFileName()
    {
      string fieldValue = this.GetFieldValue(InformationFields.ServerPath);
      if (string.IsNullOrWhiteSpace(fieldValue))
        fieldValue = this.GetFieldValue(InformationFields.LocalPath);
      return fieldValue;
    }

    protected override void ContributeToJson(JsObject json)
    {
      json["platform"] = (object) this.GetFieldValue(InformationFields.Platform);
      json["flavor"] = (object) this.GetFieldValue(InformationFields.Flavor);
      json["serverPath"] = (object) this.GetFieldValue(InformationFields.ServerPath);
      json["localPath"] = (object) this.GetFieldValue(InformationFields.LocalPath);
      json["targetNames"] = (object) this.GetFieldValue(InformationFields.TargetNames);
      int fieldValueInt1 = this.GetFieldValueInt(InformationFields.CompilationErrors);
      int fieldValueInt2 = this.GetFieldValueInt(InformationFields.StaticAnalysisErrors);
      int fieldValueInt3 = this.GetFieldValueInt(InformationFields.CompilationWarnings);
      int fieldValueInt4 = this.GetFieldValueInt(InformationFields.StaticAnalysisWarnings);
      json["errors"] = (object) (fieldValueInt1 + fieldValueInt2);
      json["warnings"] = (object) (fieldValueInt3 + fieldValueInt4);
      json["total"] = (object) (fieldValueInt1 + fieldValueInt2 + fieldValueInt3 + fieldValueInt4);
    }
  }
}
