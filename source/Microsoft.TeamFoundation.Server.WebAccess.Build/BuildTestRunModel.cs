// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildTestRunModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  public class BuildTestRunModel
  {
    public int Id { get; private set; }

    public int Passed { get; set; }

    public int Failed { get; set; }

    public int Inconclusive { get; set; }

    public int Total => this.Passed + this.Failed + this.Inconclusive;

    public BuildTestRunModel(int id) => this.Id = id;

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json.Add("id", (object) this.Id);
      json.Add("passed", (object) this.Passed);
      json.Add("failed", (object) this.Failed);
      json.Add("inconclusive", (object) this.Inconclusive);
      json.Add("total", (object) this.Total);
      return json;
    }
  }
}
