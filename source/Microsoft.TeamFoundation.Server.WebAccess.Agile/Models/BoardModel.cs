// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.BoardModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Models
{
  [DataContract]
  [ClientIncludeModel]
  public class BoardModel
  {
    [DataMember]
    public IBoard Board { get; set; }

    [DataMember]
    public BoardSettings BoardSettings { get; set; }

    [DataMember]
    public BoardCardSettings BoardCardSettings { get; set; }

    [DataMember]
    public BoardFilterSettings BoardFilterSettings { get; set; }

    [DataMember(Name = "itemTypes")]
    public IEnumerable<string> WorkItemTypes { get; set; }

    [DataMember]
    public IItemSource ItemSource { get; set; }

    [DataMember]
    public bool ReconciliationComplete { get; set; }

    public JsObject ToJson(IVssRequestContext requestContext, bool isBoardSettingsValid)
    {
      JsObject json = new JsObject();
      if (this.ReconciliationComplete)
      {
        json["boardSettings"] = (object) this.BoardSettings.ToJson(requestContext, isBoardSettingsValid);
        json["board"] = (object) this.Board.ToJson(requestContext);
        if (isBoardSettingsValid)
          json["itemSource"] = (object) this.ItemSource.ToJson(this.BoardSettings);
        if (this.BoardCardSettings != null)
          json["boardCardSettings"] = (object) this.BoardCardSettings.ToJson();
        if (this.BoardFilterSettings != null)
          json["boardFilterSettings"] = (object) this.BoardFilterSettings.ToJson();
        json["itemTypes"] = (object) this.WorkItemTypes.ToArray<string>();
      }
      else
        json["notReady"] = (object) true;
      return json;
    }
  }
}
