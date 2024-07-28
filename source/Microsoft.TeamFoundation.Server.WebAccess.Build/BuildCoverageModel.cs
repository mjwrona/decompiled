// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildCoverageModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using Microsoft.TeamFoundation.TestManagement.Server;
using System;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  public class BuildCoverageModel
  {
    public BuildCoverage Coverage { get; private set; }

    public BuildCoverageModel(BuildCoverage coverage) => this.Coverage = coverage;

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json.Add("pending", (object) (this.Coverage.State == (byte) 1));
      json.Add("lastError", (object) this.Coverage.LastError);
      json.Add("modules", (object) this.Coverage.Modules.Select<ModuleCoverage, JsObject>((Func<ModuleCoverage, JsObject>) (m => new ModuleCoverageModel(m).ToJson())));
      return json;
    }
  }
}
