// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates.PredicateLessThanOperator
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates
{
  [XmlType("less-than")]
  public class PredicateLessThanOperator : PredicateFieldComparisonOperator
  {
    protected override string WiqlOperator => "<";

    protected override string InverseWiqlOperator => ">=";

    public override bool Evaluate(IPredicateEvaluationHelper helper) => this.Compare(helper) < 0;
  }
}
