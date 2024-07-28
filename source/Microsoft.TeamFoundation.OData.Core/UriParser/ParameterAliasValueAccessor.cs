// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ParameterAliasValueAccessor
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;

namespace Microsoft.OData.UriParser
{
  internal sealed class ParameterAliasValueAccessor
  {
    public ParameterAliasValueAccessor(
      IDictionary<string, string> parameterAliasValueExpressions)
    {
      ExceptionUtils.CheckArgumentNotNull<IDictionary<string, string>>(parameterAliasValueExpressions, nameof (parameterAliasValueExpressions));
      this.ParameterAliasValueExpressions = (IDictionary<string, string>) new Dictionary<string, string>(parameterAliasValueExpressions, (IEqualityComparer<string>) StringComparer.Ordinal);
      this.ParameterAliasValueNodesCached = (IDictionary<string, SingleValueNode>) new Dictionary<string, SingleValueNode>((IEqualityComparer<string>) StringComparer.Ordinal);
    }

    public IDictionary<string, SingleValueNode> ParameterAliasValueNodesCached { get; private set; }

    internal IDictionary<string, string> ParameterAliasValueExpressions { get; private set; }

    public string GetAliasValueExpression(string alias)
    {
      string str = (string) null;
      return this.ParameterAliasValueExpressions.TryGetValue(alias, out str) ? str : (string) null;
    }
  }
}
