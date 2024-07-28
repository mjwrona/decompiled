// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.IVssRequestContextIndexerExtensions
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  public static class IVssRequestContextIndexerExtensions
  {
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "requestContext")]
    public static IndexProvisionerFactory GetIndexProvisionerFactory(
      this IVssRequestContext requestContext,
      IEntityType entityType)
    {
      string name = entityType.Name;
      if (name != null)
      {
        switch (name.Length)
        {
          case 4:
            switch (name[0])
            {
              case 'C':
                if (name == "Code")
                  return (IndexProvisionerFactory) new CodeIndexProvisionerFactory();
                break;
              case 'W':
                if (name == "Wiki")
                  return (IndexProvisionerFactory) new WikiIndexProvisionerFactory();
                break;
            }
            break;
          case 5:
            if (name == "Board")
              return (IndexProvisionerFactory) new BoardIndexProvisionerFactory();
            break;
          case 7:
            switch (name[0])
            {
              case 'P':
                if (name == "Package")
                  return (IndexProvisionerFactory) new PackageIndexProvisionerFactory();
                break;
              case 'S':
                if (name == "Setting")
                  return (IndexProvisionerFactory) new SettingIndexProvisionerFactory();
                break;
            }
            break;
          case 8:
            if (name == "WorkItem")
              return (IndexProvisionerFactory) new WorkItemIndexProvisionerFactory();
            break;
          case 11:
            if (name == "ProjectRepo")
              return (IndexProvisionerFactory) new ProjectIndexProvisionerFactory();
            break;
        }
      }
      throw new ArgumentException("Name");
    }

    public static bool TryGetQueryIndexVersion(
      this IVssRequestContext requestContext,
      EntityType entityType,
      out int? indexVersion)
    {
      IEnumerable<IndexInfo> queryIndexInfo = requestContext.GetService<IDocumentContractTypeService>().GetQueryIndexInfo((IEntityType) entityType);
      if (queryIndexInfo == null || !queryIndexInfo.Any<IndexInfo>())
      {
        indexVersion = new int?();
        return false;
      }
      indexVersion = !queryIndexInfo.Skip<IndexInfo>(1).Any<IndexInfo>() ? queryIndexInfo.First<IndexInfo>().Version : throw new NotSupportedException("QueryIndexInfo list contains more than one item.");
      return true;
    }
  }
}
