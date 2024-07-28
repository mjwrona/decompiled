// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.CustomSummaryInformationSection
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  public class CustomSummaryInformationSection
  {
    public CustomSummaryInformationSection() => this.Messages = new List<string>();

    public string Name { get; set; }

    public string Header { get; set; }

    public int Priority { get; set; }

    public List<string> Messages { get; private set; }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json.Add("key", (object) this.Name);
      json.Add("header", (object) this.Header);
      json.Add("priority", (object) this.Priority);
      json.Add("messages", (object) this.Messages.ToArray());
      return json;
    }
  }
}
