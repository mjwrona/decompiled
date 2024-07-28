// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.FunctionParameterAliasToken
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;

namespace Microsoft.OData.UriParser
{
  internal sealed class FunctionParameterAliasToken : QueryToken
  {
    public FunctionParameterAliasToken(string alias)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(alias, nameof (alias));
      this.Alias = alias;
    }

    public string Alias { get; private set; }

    public override QueryTokenKind Kind => QueryTokenKind.FunctionParameterAlias;

    internal IEdmTypeReference ExpectedParameterType { get; set; }

    public override T Accept<T>(ISyntacticTreeVisitor<T> visitor) => throw new NotImplementedException();
  }
}
