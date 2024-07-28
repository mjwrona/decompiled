// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels.CumulativeFlowDiagramViewModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels
{
  [DataContract]
  public class CumulativeFlowDiagramViewModel
  {
    [DataMember(Name = "title", EmitDefaultValue = true)]
    public string Title { get; set; }

    [DataMember(Name = "backlogLevelId", EmitDefaultValue = true)]
    public string BacklogLevelId { get; set; }

    [DataMember(Name = "errors", EmitDefaultValue = true)]
    public string[] Errors { get; set; }

    [DataMember(Name = "startDate", EmitDefaultValue = true)]
    public string StartDate { get; set; }

    [DataMember(Name = "hideIncoming", EmitDefaultValue = true)]
    public bool HideIncoming { get; set; }

    [DataMember(Name = "hideOutgoing", EmitDefaultValue = true)]
    public bool HideOutgoing { get; set; }

    [DataMember(Name = "teamId", EmitDefaultValue = true)]
    public Guid TeamId { get; set; }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json["teamId"] = (object) this.TeamId;
      json["title"] = (object) this.Title;
      json["backlogLevelId"] = (object) this.BacklogLevelId;
      json["errors"] = (object) this.Errors;
      json["startDate"] = (object) this.StartDate;
      json["hideIncoming"] = (object) this.HideIncoming;
      json["hideOutgoing"] = (object) this.HideOutgoing;
      return json;
    }
  }
}
