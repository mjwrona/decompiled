// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.CardRule
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  [DataContract]
  public class CardRule
  {
    [DataMember(Name = "name")]
    public string Name;
    [DataMember(Name = "isEnabled")]
    public bool IsEnabled;
    [DataMember(Name = "style")]
    public List<KeyValuePair<string, string>> StyleAttributes;
    public string QueryText;
    [DataMember(Name = "criteria")]
    public FilterModel QueryExpression;

    public CardRule(
      string name,
      string type,
      bool isEnabled,
      List<KeyValuePair<string, string>> ruleAttributes,
      string queryText,
      FilterModel queryExpression)
    {
      this.Name = name;
      this.Type = type;
      this.IsEnabled = isEnabled;
      this.StyleAttributes = ruleAttributes;
      this.QueryText = queryText;
      this.QueryExpression = queryExpression;
    }

    [DataMember(Name = "type")]
    public string Type { get; set; }
  }
}
