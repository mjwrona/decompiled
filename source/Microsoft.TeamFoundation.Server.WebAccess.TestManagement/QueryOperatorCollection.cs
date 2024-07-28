// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.QueryOperatorCollection
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class QueryOperatorCollection
  {
    public QueryOperator EqualTo;
    public QueryOperator NotEquals;
    public QueryOperator Contains;
    public QueryOperator NotContains;
    public QueryOperator GT;
    public QueryOperator GTE;
    public QueryOperator LT;
    public QueryOperator LTE;
    public QueryOperator In;
    public QueryOperator NotIn;
    public QueryOperator Under;
    private Dictionary<string, QueryOperator> m_allOperators = new Dictionary<string, QueryOperator>();

    public QueryOperatorCollection()
    {
      this.EqualTo = this.RegisterOperator("=", TestManagementResources.QueryEqualTo);
      this.NotEquals = this.RegisterOperator("<>", TestManagementServerResources.QueryNotEqualTo);
      this.Contains = this.RegisterOperator("CONTAINS", TestManagementServerResources.QueryContains);
      this.NotContains = this.RegisterOperator("NOT CONTAINS", TestManagementServerResources.QueryNotContains);
      this.GT = this.RegisterOperator(">", TestManagementServerResources.QueryGreaterThan);
      this.GTE = this.RegisterOperator(">=", TestManagementServerResources.QueryGreaterThanOrEqualTo);
      this.LT = this.RegisterOperator("<", TestManagementServerResources.QueryLessThan);
      this.LTE = this.RegisterOperator("<=", TestManagementServerResources.QueryLessThanOrEqualTo);
      this.In = this.RegisterOperator(nameof (In), TestManagementServerResources.QueryIn);
      this.NotIn = this.RegisterOperator("Not In", TestManagementServerResources.QueryNotIn);
      this.Under = this.RegisterOperator(nameof (Under), TestManagementServerResources.QueryUnder);
    }

    public QueryOperator GetOperatorByDisplayName(string operatorDisplayName) => this.m_allOperators.Values.FirstOrDefault<QueryOperator>((Func<QueryOperator, bool>) (op => string.Equals(op.DisplayName, operatorDisplayName, StringComparison.CurrentCultureIgnoreCase)));

    public QueryOperator GetOperatorByRawValue(string operatorRawValue)
    {
      QueryOperator queryOperator;
      return this.m_allOperators.TryGetValue(operatorRawValue, out queryOperator) ? queryOperator : (QueryOperator) null;
    }

    private QueryOperator RegisterOperator(string rawValue, string getDisplayName)
    {
      QueryOperator queryOperator = new QueryOperator(rawValue, getDisplayName);
      this.m_allOperators[queryOperator.RawValue] = queryOperator;
      return queryOperator;
    }
  }
}
