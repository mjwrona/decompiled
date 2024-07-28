// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure.DbExpressionHelpers
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure
{
  public static class DbExpressionHelpers
  {
    public static bool HasProperty(this DbExpression expression, string propertyName) => expression.ResultType.EdmType.HasProperty(propertyName);

    public static bool HasProperty(this EdmType edmType, string propertyName)
    {
      ReadOnlyMetadataCollection<EdmProperty> typeProperties = edmType.GetTypeProperties();
      return typeProperties != null && typeProperties.Any<EdmProperty>((Func<EdmProperty, bool>) (p => p.Name == propertyName));
    }

    public static ReadOnlyMetadataCollection<EdmProperty> GetTypeProperties(this EdmType edmType)
    {
      ReadOnlyMetadataCollection<EdmProperty> typeProperties = (ReadOnlyMetadataCollection<EdmProperty>) null;
      if (edmType is RowType rowType)
        typeProperties = rowType.DeclaredProperties;
      if (edmType is EntityType entityType)
        typeProperties = entityType.DeclaredProperties;
      return typeProperties;
    }
  }
}
