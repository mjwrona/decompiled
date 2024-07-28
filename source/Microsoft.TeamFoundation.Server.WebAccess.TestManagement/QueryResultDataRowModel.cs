// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.QueryResultDataRowModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class QueryResultDataRowModel
  {
    public string Id { get; set; }

    public string Title { get; set; }

    public IEnumerable<string> Data { get; set; }

    public virtual JsObject ToJson()
    {
      JsObject json = new JsObject();
      json.Add("id", (object) this.Id);
      json.Add("title", (object) this.Title);
      json.Add("data", (object) this.Data);
      return json;
    }
  }
}
