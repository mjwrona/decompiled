// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.CollectionBulkIndexTypes
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Indexer;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal static class CollectionBulkIndexTypes
  {
    public static List<string> GetSupportedTypes(
      IndexingExecutionContext indexingExecutionContext,
      CollectionCodeBulkIndexOperationType opType)
    {
      IEntityType entityType = indexingExecutionContext.IndexingUnit.EntityType;
      switch (entityType.Name)
      {
        case "Code":
          List<string> supportedTypes = new List<string>();
          if (indexingExecutionContext.RequestContext.IsCodeIndexingEnabled())
          {
            supportedTypes.Add("Git_Repository");
            supportedTypes.Add("TFVC_Repository");
          }
          if (indexingExecutionContext.RequestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableCustomRepository", TeamFoundationHostType.ProjectCollection) && (opType == CollectionCodeBulkIndexOperationType.Finalize || opType == CollectionCodeBulkIndexOperationType.MetadataCrawl))
            supportedTypes.Add("CustomRepository");
          return supportedTypes;
        case "Wiki":
          return new List<string>() { "Git_Repository" };
        default:
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("EntityType : {0} is not supported by {1}", (object) entityType.Name, (object) nameof (CollectionBulkIndexTypes))));
      }
    }
  }
}
