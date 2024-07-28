// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.DeliveryViewPropertyCollection
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Work.WebApi
{
  [DataContract]
  public class DeliveryViewPropertyCollection : PlanPropertyCollection
  {
    [DataMember(Name = "teamBacklogMappings", EmitDefaultValue = false)]
    public IEnumerable<TeamBacklogMapping> TeamBacklogMappings { get; set; }

    [DataMember(Name = "criteria", EmitDefaultValue = false)]
    public IEnumerable<FilterClause> Criteria { get; set; }

    [DataMember(Name = "cardSettings", EmitDefaultValue = false)]
    public CardSettings CardSettings { get; set; }

    [DataMember(Name = "markers", EmitDefaultValue = false)]
    public IEnumerable<Marker> Markers { get; set; }

    [DataMember(Name = "styleSettings", EmitDefaultValue = false)]
    public IEnumerable<Rule> CardStyleSettings { get; set; }

    [DataMember(Name = "tagStyleSettings", EmitDefaultValue = false)]
    public IEnumerable<Rule> TagStyleSettings { get; set; }
  }
}
