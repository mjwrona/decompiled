// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.QueryBoolField`1
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class QueryBoolField<TQueryItem> : QueryField<TQueryItem>
  {
    public QueryBoolField(
      TestManagerRequestContext testContext,
      string referenceName,
      string displayName,
      bool isQueryable,
      Func<TQueryItem, bool> getDataValueFunc)
      : base(testContext, referenceName, displayName, isQueryable, QueryFieldType.Boolean, (Func<TQueryItem, object>) (item => (object) getDataValueFunc(item)))
    {
      this.DefaultWidth = 80;
    }

    protected override QueryOperator[] GetQueryOperators() => new QueryOperator[2]
    {
      this.OperatorCollection.EqualTo,
      this.OperatorCollection.NotEquals
    };

    public override IEnumerable<string> GetSuggestedValues() => (IEnumerable<string>) new string[2]
    {
      TestManagementServerResources.Boolean_True,
      TestManagementServerResources.Boolean_False
    };

    public override string ConvertRawValueToDisplayValue(object value) => !(bool) value ? TestManagementServerResources.Boolean_False : TestManagementServerResources.Boolean_True;

    public override object ConvertDisplayValueToRawValue(string displayValue) => (object) string.Equals(displayValue, TestManagementServerResources.Boolean_True, StringComparison.CurrentCultureIgnoreCase);

    protected override string GetQueryValueString(object rawValue) => !(bool) rawValue ? "False" : "True";
  }
}
