// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.ODataQueryOptionHelpers
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser.Aggregation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  public static class ODataQueryOptionHelpers
  {
    public static IEnumerable<IEdmStructuralProperty> GetAvailableOrderByProperties(
      ODataQueryContext context)
    {
      return context.ElementType is IEdmEntityType elementType ? (IEnumerable<IEdmStructuralProperty>) (elementType.Key().Any<IEdmStructuralProperty>() ? elementType.Key() : elementType.StructuralProperties().Where<IEdmStructuralProperty>((Func<IEdmStructuralProperty, bool>) (property => property.Type.IsPrimitive()))).OrderBy<IEdmStructuralProperty, string>((Func<IEdmStructuralProperty, string>) (property => property.Name)) : Enumerable.Empty<IEdmStructuralProperty>();
    }

    public static bool ContainsRawComputeAndFilter(this ODataQueryOptions options)
    {
      bool flag1 = options.Compute != null;
      bool flag2 = options.Filter != null;
      if (options.Apply != null)
      {
        bool flag3 = false;
        foreach (TransformationNode transformation in options.Apply.ApplyClause.Transformations)
        {
          switch (transformation.Kind)
          {
            case TransformationNodeKind.Aggregate:
            case TransformationNodeKind.GroupBy:
              flag3 = true;
              continue;
            case TransformationNodeKind.Filter:
              flag2 = true;
              continue;
            case TransformationNodeKind.Compute:
              flag1 = true;
              continue;
            default:
              continue;
          }
        }
        if (flag3)
          return false;
      }
      return flag1 & flag2;
    }
  }
}
