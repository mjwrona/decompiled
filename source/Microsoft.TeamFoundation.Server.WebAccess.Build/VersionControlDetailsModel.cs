// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.VersionControlDetailsModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  public class VersionControlDetailsModel
  {
    public int ChangesetId { get; set; }

    public string ShelvesetName { get; set; }

    public string Version { get; set; }

    public string VersionText { get; set; }

    public int SuccessChangesetCount { get; set; }

    public int FailChangesetCount { get; set; }

    public AssociatedChangesetModel[] AssociatedChangesets { get; set; }

    public JsObject ToJson()
    {
      JsObject jsObject = new JsObject();
      jsObject.Add("csId", (object) this.ChangesetId);
      jsObject.Add("shelvesetName", (object) this.ShelvesetName);
      jsObject.Add("version", (object) this.Version);
      jsObject.Add("versionText", (object) this.VersionText);
      jsObject.Add("successCs", (object) this.SuccessChangesetCount);
      jsObject.Add("failCs", (object) this.FailChangesetCount);
      JsObject json = jsObject;
      if (this.AssociatedChangesets != null)
        json["associatedChangesets"] = (object) ((IEnumerable<AssociatedChangesetModel>) this.AssociatedChangesets).Select<AssociatedChangesetModel, JsObject>((Func<AssociatedChangesetModel, JsObject>) (ac => ac.ToJson()));
      return json;
    }
  }
}
