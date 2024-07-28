// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.PolicyModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class PolicyModel
  {
    public PolicyModel(string description, bool value, string learnMoreLink)
    {
      this.Description = description;
      this.Value = value;
      this.LearnMoreLink = learnMoreLink;
    }

    public string Description { get; set; }

    public bool Value { get; set; }

    public string LearnMoreLink { get; set; }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json["description"] = (object) this.Description;
      json["value"] = (object) this.Value;
      json["learnMoreLink"] = (object) this.LearnMoreLink;
      return json;
    }
  }
}
