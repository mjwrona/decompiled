// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.Aggregation.GroupByToken
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Collections.Generic;

namespace Microsoft.OData.UriParser.Aggregation
{
  public sealed class GroupByToken : ApplyTransformationToken
  {
    private readonly IEnumerable<EndPathToken> properties;
    private readonly ApplyTransformationToken child;

    public GroupByToken(IEnumerable<EndPathToken> properties, ApplyTransformationToken child)
    {
      ExceptionUtils.CheckArgumentNotNull<IEnumerable<EndPathToken>>(properties, nameof (properties));
      this.properties = properties;
      this.child = child;
    }

    public override QueryTokenKind Kind => QueryTokenKind.AggregateGroupBy;

    public IEnumerable<EndPathToken> Properties => this.properties;

    public ApplyTransformationToken Child => this.child;

    public override T Accept<T>(ISyntacticTreeVisitor<T> visitor) => visitor.Visit(this);
  }
}
