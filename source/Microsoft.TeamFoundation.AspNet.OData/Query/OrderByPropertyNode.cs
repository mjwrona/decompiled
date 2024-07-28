// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.OrderByPropertyNode
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace Microsoft.AspNet.OData.Query
{
  public class OrderByPropertyNode : OrderByNode
  {
    public OrderByPropertyNode(OrderByClause orderByClause)
      : base(orderByClause)
    {
      this.OrderByClause = orderByClause != null ? orderByClause : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (orderByClause));
      this.Direction = orderByClause.Direction;
      if (!(orderByClause.Expression is SingleValuePropertyAccessNode expression))
        throw new ODataException(SRResources.OrderByClauseNotSupported);
      this.Property = expression.Property;
    }

    public OrderByPropertyNode(IEdmProperty property, OrderByDirection direction)
      : base(direction)
    {
      this.Property = property != null ? property : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (property));
    }

    public OrderByClause OrderByClause { get; private set; }

    public IEdmProperty Property { get; private set; }
  }
}
