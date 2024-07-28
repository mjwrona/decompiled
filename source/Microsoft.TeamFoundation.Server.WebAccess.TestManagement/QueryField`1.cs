// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.QueryField`1
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class QueryField<TQueryItem> : QueryField
  {
    private Func<TQueryItem, object> m_getDataValueFunc;

    public QueryField(
      TestManagerRequestContext testContext,
      string referenceName,
      string displayName,
      bool isQueryable,
      QueryFieldType type,
      Func<TQueryItem, object> getDataValueFunc)
      : base(testContext, referenceName, displayName, type, isQueryable)
    {
      this.m_getDataValueFunc = getDataValueFunc;
      this.AllowedOperators = (IEnumerable<QueryOperator>) this.GetQueryOperators();
    }

    public virtual object GetDataRawValue(TQueryItem item) => this.m_getDataValueFunc(item);

    public string GetDataDisplayValue(TQueryItem item) => this.ConvertRawValueToDisplayValue(this.GetDataRawValue(item));

    protected virtual QueryOperator[] GetQueryOperators() => new QueryOperator[0];
  }
}
