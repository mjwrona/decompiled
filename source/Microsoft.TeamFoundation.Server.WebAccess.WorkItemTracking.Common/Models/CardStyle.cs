// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.CardStyle
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  [DataContract]
  public class CardStyle
  {
    [DataMember(Name = "styleType")]
    public string StyleType;
    [DataMember(Name = "properties")]
    public List<KeyValuePair<string, string>> Properties;

    public CardStyle(string styleType, params KeyValuePair<string, string>[] properties)
    {
      this.StyleType = styleType;
      this.Properties = new List<KeyValuePair<string, string>>();
      foreach (KeyValuePair<string, string> property in properties)
        this.Properties.Add(property);
    }
  }
}
