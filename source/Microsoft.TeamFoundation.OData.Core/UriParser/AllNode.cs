// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.AllNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System.Collections.ObjectModel;

namespace Microsoft.OData.UriParser
{
  public sealed class AllNode : LambdaNode
  {
    public AllNode(Collection<RangeVariable> rangeVariables)
      : this(rangeVariables, (RangeVariable) null)
    {
    }

    public AllNode(Collection<RangeVariable> rangeVariables, RangeVariable currentRangeVariable)
      : base(rangeVariables, currentRangeVariable)
    {
    }

    public override IEdmTypeReference TypeReference => (IEdmTypeReference) EdmCoreModel.Instance.GetBoolean(true);

    internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.All;

    public override T Accept<T>(QueryNodeVisitor<T> visitor)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryNodeVisitor<T>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }
  }
}
