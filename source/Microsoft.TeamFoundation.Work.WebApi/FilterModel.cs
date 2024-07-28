// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.FilterModel
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Work.WebApi
{
  [DataContract]
  [Obsolete]
  public class FilterModel
  {
    public FilterModel()
    {
      this.Clauses = (ICollection<FilterClause>) new List<FilterClause>();
      this.Groups = (ICollection<FilterGroup>) new List<FilterGroup>();
    }

    [DataMember(Order = 1, Name = "clauses", EmitDefaultValue = false)]
    public ICollection<FilterClause> Clauses { get; set; }

    [DataMember(Order = 2, Name = "groups", EmitDefaultValue = false)]
    public ICollection<FilterGroup> Groups { get; set; }

    [DataMember(Order = 3, Name = "maxGroupLevel")]
    public int MaxGroupLevel { get; set; }
  }
}
