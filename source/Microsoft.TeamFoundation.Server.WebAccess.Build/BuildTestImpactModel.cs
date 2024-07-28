// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildTestImpactModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using Microsoft.TeamFoundation.TestImpact.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  public class BuildTestImpactModel
  {
    public IEnumerable<IGrouping<string, TestImpactModel>> TestsByType { get; private set; }

    public BuildTestImpactModel(BuildImpactedTests impactedTests)
    {
      if (impactedTests == null || impactedTests.Tests == null)
        return;
      this.TestsByType = (IEnumerable<IGrouping<string, TestImpactModel>>) impactedTests.Tests.Select<Test, TestImpactModel>((Func<Test, TestImpactModel>) (t => new TestImpactModel(t))).GroupBy<TestImpactModel, string>((Func<TestImpactModel, string>) (t => t.Test.TestType)).OrderBy<IGrouping<string, TestImpactModel>, string>((Func<IGrouping<string, TestImpactModel>, string>) (g => g.Key), (IComparer<string>) StringComparer.CurrentCultureIgnoreCase);
    }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      if (this.TestsByType != null)
      {
        List<JsObject> jsObjectList1 = new List<JsObject>();
        foreach (IGrouping<string, TestImpactModel> source in this.TestsByType)
        {
          List<JsObject> jsObjectList2 = jsObjectList1;
          JsObject jsObject = new JsObject();
          jsObject.Add("type", (object) source.Key);
          jsObject.Add("tests", (object) source.OrderBy<TestImpactModel, string>((Func<TestImpactModel, string>) (t => t.Test.TestName), (IComparer<string>) StringComparer.CurrentCultureIgnoreCase).Select<TestImpactModel, JsObject>((Func<TestImpactModel, JsObject>) (t => t.ToJson())));
          jsObjectList2.Add(jsObject);
        }
        json["impactedTests"] = (object) jsObjectList1;
      }
      return json;
    }
  }
}
