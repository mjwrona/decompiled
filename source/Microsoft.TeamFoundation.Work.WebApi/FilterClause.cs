// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.FilterClause
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Work.WebApi
{
  [DataContract]
  public class FilterClause
  {
    public FilterClause(
      string name,
      int index,
      string logicalOperator,
      string _operator,
      string val)
    {
      this.FieldName = name;
      this.Index = index;
      this.LogicalOperator = logicalOperator;
      this.Operator = _operator;
      this.Value = val;
    }

    [DataMember(Order = 1, Name = "fieldName", IsRequired = false, EmitDefaultValue = false)]
    public string FieldName { get; set; }

    [DataMember(Order = 2, Name = "index", IsRequired = false, EmitDefaultValue = false)]
    public int Index { get; set; }

    [DataMember(Order = 3, Name = "logicalOperator", IsRequired = false, EmitDefaultValue = false)]
    public string LogicalOperator { get; set; }

    [DataMember(Order = 4, Name = "operator", IsRequired = false, EmitDefaultValue = false)]
    public string Operator { get; set; }

    [DataMember(Order = 5, Name = "value", IsRequired = false, EmitDefaultValue = false)]
    public string Value { get; set; }
  }
}
