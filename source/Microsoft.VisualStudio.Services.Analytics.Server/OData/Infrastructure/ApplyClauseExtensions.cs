// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.ApplyClauseExtensions
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.OData.UriParser.Aggregation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  public static class ApplyClauseExtensions
  {
    public static bool IsAggregated(this ApplyClause apply)
    {
      bool? nullable;
      if (apply == null)
      {
        nullable = new bool?();
      }
      else
      {
        IEnumerable<TransformationNode> transformations = apply.Transformations;
        nullable = transformations != null ? new bool?(transformations.Any<TransformationNode>((Func<TransformationNode, bool>) (t => t.Kind == TransformationNodeKind.Aggregate || t.Kind == TransformationNodeKind.GroupBy))) : new bool?();
      }
      return nullable.GetValueOrDefault();
    }
  }
}
