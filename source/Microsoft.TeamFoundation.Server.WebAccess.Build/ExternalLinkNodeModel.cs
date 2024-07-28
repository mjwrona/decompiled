// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.ExternalLinkNodeModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  public class ExternalLinkNodeModel : InformationNodeModel
  {
    private BuildExternalLinkModel Link { get; set; }

    public ExternalLinkNodeModel(BuildInformationNode node)
      : base(node)
    {
      this.Link = new BuildExternalLinkModel(this.GetFieldValue(InformationFields.Url), this.GetFieldValue(InformationFields.DisplayText));
    }

    protected override void ContributeToJson(JsObject result) => result["link"] = (object) this.Link.ToJson();
  }
}
