// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildHistogramModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using Microsoft.TeamFoundation.Build.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  public class BuildHistogramModel
  {
    public IEnumerable<BuildDetail> Builds { get; private set; }

    public BuildHistogramModel(IEnumerable<BuildDetail> builds) => this.Builds = builds;

    public JsObject ToJson()
    {
      List<JsObject> jsObjectList1 = new List<JsObject>();
      JsObject json = new JsObject();
      json["builds"] = (object) jsObjectList1;
      foreach (BuildDetail build in this.Builds)
      {
        List<JsObject> jsObjectList2 = jsObjectList1;
        JsObject jsObject = new JsObject();
        jsObject.Add("uri", (object) build.Uri);
        jsObject.Add("name", (object) build.BuildNumber);
        jsObject.Add("finished", (object) build.BuildFinished());
        jsObject.Add("status", (object) build.Status);
        jsObject.Add("startTime", (object) build.StartTime);
        jsObject.Add("finishTime", (object) build.FinishTime);
        jsObjectList2.Add(jsObject);
      }
      return json;
    }
  }
}
