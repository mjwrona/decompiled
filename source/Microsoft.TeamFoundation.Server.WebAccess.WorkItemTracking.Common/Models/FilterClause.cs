// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterClause
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  [DataContract]
  public class FilterClause
  {
    [DataMember(Name = "logicalOperator", EmitDefaultValue = false)]
    public string LogicalOperator { get; set; }

    [DataMember(Name = "fieldName", EmitDefaultValue = false)]
    public string FieldName { get; set; }

    [DataMember(Name = "operator", EmitDefaultValue = false)]
    public string Operator { get; set; }

    [DataMember(Name = "value", EmitDefaultValue = false)]
    public string Value { get; set; }

    [DataMember(Name = "index", EmitDefaultValue = false)]
    public int Index { get; set; }

    public bool IsValid() => !string.IsNullOrEmpty(this.FieldName) && !string.IsNullOrEmpty(this.Operator);

    public override string ToString() => string.Format("LogicalOperator: {0}. FieldName: {1}, Operator: {2}, Value: {3}, Index: {4}", (object) this.LogicalOperator, (object) this.FieldName, (object) this.Operator, (object) this.Value, (object) this.Index);
  }
}
