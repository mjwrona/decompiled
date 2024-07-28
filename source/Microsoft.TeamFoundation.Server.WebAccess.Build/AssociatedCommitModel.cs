// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.AssociatedCommitModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  public class AssociatedCommitModel
  {
    private IDictionary<string, string> FieldDict { get; set; }

    public AssociatedCommitModel(BuildInformationNode node) => this.FieldDict = node.FieldsDict();

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json.Add("id", (object) CommonInformationHelper.GetString(this.FieldDict, InformationFields.CommitId));
      json.Add("by", (object) CommonInformationHelper.GetString(this.FieldDict, InformationFields.Author));
      json.Add("comment", (object) CommonInformationHelper.GetString(this.FieldDict, InformationFields.Message));
      json.Add("artifactData", (object) this.GetArtifactData(CommonInformationHelper.GetString(this.FieldDict, InformationFields.Uri)));
      return json;
    }

    private JsObject GetArtifactData(string uri)
    {
      string toolSpecificId = LinkingUtilities.DecodeUri(uri).ToolSpecificId;
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      string empty3 = string.Empty;
      if (!string.IsNullOrEmpty(toolSpecificId))
      {
        string[] strArray = toolSpecificId.Split('/');
        if (strArray.Length == 3)
        {
          empty1 = strArray[0];
          empty2 = strArray[1];
          empty3 = strArray[2];
        }
      }
      JsObject artifactData = new JsObject();
      artifactData.Add("projectGuid", (object) empty1);
      artifactData.Add("repositoryId", (object) empty2);
      artifactData.Add("commitId", (object) empty3);
      return artifactData;
    }
  }
}
