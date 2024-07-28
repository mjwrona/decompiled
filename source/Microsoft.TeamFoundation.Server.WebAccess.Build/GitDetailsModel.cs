// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.GitDetailsModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  public class GitDetailsModel
  {
    public AssociatedCommitModel[] AssociatedCommits { get; set; }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      if (this.AssociatedCommits != null)
        json["associatedCommits"] = (object) ((IEnumerable<AssociatedCommitModel>) this.AssociatedCommits).Select<AssociatedCommitModel, JsObject>((Func<AssociatedCommitModel, JsObject>) (ac => ac.ToJson()));
      return json;
    }
  }
}
