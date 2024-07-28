// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.OrderByOpenPropertyNode
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData;
using Microsoft.OData.UriParser;

namespace Microsoft.AspNet.OData.Query
{
  public class OrderByOpenPropertyNode : OrderByNode
  {
    public OrderByOpenPropertyNode(OrderByClause orderByClause)
      : base(orderByClause)
    {
      this.OrderByClause = orderByClause != null ? orderByClause : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (orderByClause));
      if (!(orderByClause.Expression is SingleValueOpenPropertyAccessNode expression))
        throw new ODataException(SRResources.OrderByClauseNotSupported);
      this.PropertyName = expression.Name;
    }

    public OrderByClause OrderByClause { get; private set; }

    public string PropertyName { get; private set; }
  }
}
