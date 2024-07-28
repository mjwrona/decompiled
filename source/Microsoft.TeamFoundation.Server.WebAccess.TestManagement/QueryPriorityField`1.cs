// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.QueryPriorityField`1
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class QueryPriorityField<TQueryItem> : QueryStringField<TQueryItem>
  {
    public QueryPriorityField(
      TestManagerRequestContext testContext,
      string referenceName,
      string displayName,
      bool isQueryable,
      Func<TQueryItem, string> getDataValueFunc,
      Dictionary<string, string> valuesMap)
      : base(testContext, referenceName, displayName, isQueryable, getDataValueFunc, valuesMap)
    {
    }

    protected override QueryOperator[] GetQueryOperators() => new QueryOperator[8]
    {
      this.OperatorCollection.EqualTo,
      this.OperatorCollection.NotEquals,
      this.OperatorCollection.In,
      this.OperatorCollection.NotIn,
      this.OperatorCollection.GT,
      this.OperatorCollection.GTE,
      this.OperatorCollection.LT,
      this.OperatorCollection.LTE
    };
  }
}
