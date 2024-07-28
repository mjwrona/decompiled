// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.TestImpactModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using Microsoft.TeamFoundation.TestImpact.Server.Common;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  public class TestImpactModel
  {
    public Test Test { get; private set; }

    public TestImpactModel(Test test) => this.Test = test;

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json.Add("type", (object) this.Test.TestType);
      json.Add("name", (object) this.Test.TestName);
      json.Add("isTestCase", (object) this.Test.IsTestCase);
      json.Add("testCaseId", (object) this.Test.TestCaseId);
      json.Add("associationIndexes", (object) (this.Test.AssociationIndexes != null ? this.Test.AssociationIndexes.Count : 0));
      return json;
    }
  }
}
