// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Models.JobProgressModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.Models
{
  public class JobProgressModel
  {
    public JobProgressState State { get; set; }

    public int PercentComplete { get; set; }

    public string ResultMessage { get; set; }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json["State"] = (object) (int) this.State;
      json["PercentComplete"] = (object) this.PercentComplete;
      if (!string.IsNullOrEmpty(this.ResultMessage))
        json["ResultMessage"] = (object) this.ResultMessage;
      return json;
    }
  }
}
