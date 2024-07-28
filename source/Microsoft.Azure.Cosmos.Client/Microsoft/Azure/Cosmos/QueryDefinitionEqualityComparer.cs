// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.QueryDefinitionEqualityComparer
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Query.Core;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class QueryDefinitionEqualityComparer : IEqualityComparer<QueryDefinition>
  {
    public static readonly QueryDefinitionEqualityComparer Instance = new QueryDefinitionEqualityComparer();

    private QueryDefinitionEqualityComparer()
    {
    }

    public bool Equals(QueryDefinition x, QueryDefinition y)
    {
      if (x == y)
        return true;
      return x != null && y != null && x.Parameters.Count == y.Parameters.Count && string.Equals(x.QueryText, y.QueryText, StringComparison.Ordinal) && QueryDefinitionEqualityComparer.ParameterEquals(x.Parameters, y.Parameters);
    }

    public int GetHashCode(QueryDefinition queryDefinition)
    {
      int num = 23;
      foreach (SqlParameter parameter in (IEnumerable<SqlParameter>) queryDefinition.Parameters)
        num ^= parameter.GetHashCode();
      return num * 16777619 + queryDefinition.QueryText.GetHashCode();
    }

    private static bool ParameterEquals(
      IReadOnlyList<SqlParameter> parameters,
      IReadOnlyList<SqlParameter> otherParameters)
    {
      return parameters.Count == otherParameters.Count && new HashSet<SqlParameter>((IEnumerable<SqlParameter>) parameters).SetEquals((IEnumerable<SqlParameter>) new HashSet<SqlParameter>((IEnumerable<SqlParameter>) otherParameters));
    }
  }
}
