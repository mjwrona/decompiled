// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.ExpressionFilterClause
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  [DataContract]
  public class ExpressionFilterClause
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

    public bool Validate(bool throwIfInvalid = false)
    {
      int num = string.IsNullOrEmpty(this.FieldName) ? 0 : (!string.IsNullOrEmpty(this.Operator) ? 1 : 0);
      if (!(num == 0 & throwIfInvalid))
        return num != 0;
      throw new ArgumentException(NotificationsWebApiResources.Error_SubscriptionFilterInvalid((object) this.ToString()));
    }

    public override string ToString() => string.Format("LogicalOperator: {0}. FieldName: {1}, Operator: {2}, Value: {3}, Index: {4}", (object) this.LogicalOperator, (object) this.FieldName, (object) this.Operator, (object) this.Value, (object) this.Index);
  }
}
