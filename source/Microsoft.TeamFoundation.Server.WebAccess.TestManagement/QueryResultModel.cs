// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.QueryResultModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class QueryResultModel
  {
    public IEnumerable<string> ItemIds { get; set; }

    public string Error { get; set; }

    public IEnumerable<QueryResultDataRowModel> DataRows { get; set; }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      if (this.ItemIds != null)
        json["itemIds"] = (object) this.ItemIds;
      if (!string.IsNullOrEmpty(this.Error))
        json["error"] = (object) this.Error;
      if (this.DataRows != null)
        json["dataRows"] = (object) this.DataRows.Select<QueryResultDataRowModel, JsObject>((Func<QueryResultDataRowModel, JsObject>) (row => row.ToJson()));
      return json;
    }
  }
}
