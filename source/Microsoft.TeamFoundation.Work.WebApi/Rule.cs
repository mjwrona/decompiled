// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.Rule
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Work.WebApi
{
  [DataContract]
  public class Rule
  {
    [DataMember(Order = 1)]
    public string name { get; set; }

    [DataMember(Order = 2)]
    public string isEnabled { get; set; }

    [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false)]
    public string filter { get; set; }

    [DataMember(Order = 4, Name = "clauses", IsRequired = false, EmitDefaultValue = false)]
    public ICollection<FilterClause> Clauses { get; set; }

    [DataMember(Order = 5)]
    public attribute settings { get; set; }
  }
}
