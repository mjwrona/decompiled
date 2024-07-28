// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.OrderByNode
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.AspNet.OData.Query
{
  public abstract class OrderByNode
  {
    protected OrderByNode(OrderByDirection direction)
    {
      this.Direction = direction;
      this.PropertyPath = string.Empty;
    }

    protected OrderByNode(OrderByClause orderByClause)
    {
      this.Direction = orderByClause != null ? orderByClause.Direction : throw Error.ArgumentNull(nameof (orderByClause));
      this.PropertyPath = OrderByNode.RestorePropertyPath(orderByClause.Expression);
    }

    internal OrderByNode()
    {
    }

    public OrderByDirection Direction { get; internal set; }

    internal string PropertyPath { get; set; }

    public static IList<OrderByNode> CreateCollection(OrderByClause orderByClause)
    {
      List<OrderByNode> collection = new List<OrderByNode>();
      for (OrderByClause orderByClause1 = orderByClause; orderByClause1 != null; orderByClause1 = orderByClause1.ThenBy)
      {
        if (orderByClause1.Expression is CountNode)
          collection.Add((OrderByNode) new OrderByCountNode(orderByClause1));
        else if (orderByClause1.Expression is NonResourceRangeVariableReferenceNode || orderByClause1.Expression is ResourceRangeVariableReferenceNode)
          collection.Add((OrderByNode) new OrderByItNode(orderByClause1.Direction));
        else if (orderByClause1.Expression is SingleValueOpenPropertyAccessNode)
          collection.Add((OrderByNode) new OrderByOpenPropertyNode(orderByClause1));
        else
          collection.Add((OrderByNode) new OrderByPropertyNode(orderByClause1));
      }
      return (IList<OrderByNode>) collection;
    }

    internal static string RestorePropertyPath(SingleValueNode expression)
    {
      if (expression == null)
        return string.Empty;
      string str1 = string.Empty;
      SingleValueNode expression1 = (SingleValueNode) null;
      if (expression is SingleValuePropertyAccessNode propertyAccessNode)
      {
        str1 = propertyAccessNode.Property.Name;
        expression1 = propertyAccessNode.Source;
      }
      else if (expression is SingleComplexNode singleComplexNode)
      {
        str1 = singleComplexNode.Property.Name;
        expression1 = (SingleValueNode) singleComplexNode.Source;
      }
      else if (expression is SingleNavigationNode singleNavigationNode)
      {
        str1 = singleNavigationNode.NavigationProperty.Name;
        expression1 = (SingleValueNode) singleNavigationNode.Source;
      }
      string str2 = OrderByNode.RestorePropertyPath(expression1);
      if (string.IsNullOrEmpty(str2))
        return str1;
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}/{1}", new object[2]
      {
        (object) str2,
        (object) str1
      });
    }
  }
}
