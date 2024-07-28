// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels.VelocityChartViewModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels
{
  [DataContract]
  public class VelocityChartViewModel
  {
    [DataMember(Name = "teamId", EmitDefaultValue = true)]
    public Guid TeamId { get; set; }

    [DataMember(Name = "title", EmitDefaultValue = true)]
    public string Title { get; set; }

    [DataMember(Name = "iterationsNumber", EmitDefaultValue = true)]
    public int IterationsNumber { get; set; }

    [DataMember(Name = "errors", EmitDefaultValue = true)]
    public string[] Errors { get; set; }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json["teamId"] = (object) this.TeamId;
      json["title"] = (object) this.Title;
      json["iterationsNumber"] = (object) this.IterationsNumber;
      json["errors"] = (object) this.Errors;
      return json;
    }
  }
}
