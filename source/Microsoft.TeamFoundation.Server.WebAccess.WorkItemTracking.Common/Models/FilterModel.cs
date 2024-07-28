// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  [DataContract]
  public class FilterModel
  {
    public FilterModel()
    {
      this.Clauses = (ICollection<FilterClause>) new List<FilterClause>();
      this.Groups = (ICollection<FilterGroup>) new List<FilterGroup>();
    }

    [DataMember(Name = "clauses", EmitDefaultValue = false)]
    public ICollection<FilterClause> Clauses { get; set; }

    [DataMember(Name = "groups", EmitDefaultValue = false)]
    public ICollection<FilterGroup> Groups { get; set; }

    [DataMember(Name = "maxGroupLevel")]
    public int MaxGroupLevel { get; set; }
  }
}
