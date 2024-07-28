// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.Aggregation.AggregateExpressionBase
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData.UriParser.Aggregation
{
  public abstract class AggregateExpressionBase
  {
    protected AggregateExpressionBase(AggregateExpressionKind kind, string alias)
    {
      this.AggregateKind = kind;
      this.Alias = alias;
    }

    public AggregateExpressionKind AggregateKind { get; private set; }

    public string Alias { get; private set; }
  }
}
