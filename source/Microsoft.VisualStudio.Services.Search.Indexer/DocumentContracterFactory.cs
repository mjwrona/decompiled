// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.DocumentContracterFactory
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  public static class DocumentContracterFactory
  {
    public static IDocumentContracter CreateDocumentContracter(
      IndexingExecutionContext indexingExecutionContext,
      IndexInfo indexInfo)
    {
      IEntityType typeForContractType = indexingExecutionContext.ProvisioningContext.ContractType.GetEntityTypeForContractType();
      string name = typeForContractType.Name;
      if (name != null)
      {
        switch (name.Length)
        {
          case 4:
            switch (name[0])
            {
              case 'C':
                if (name == "Code")
                  return (IDocumentContracter) new CodeContracter(indexingExecutionContext, indexInfo);
                break;
              case 'W':
                if (name == "Wiki")
                  return (IDocumentContracter) new WikiContracter(indexingExecutionContext, indexInfo);
                break;
            }
            break;
          case 5:
            if (name == "Board")
              return (IDocumentContracter) new BoardContracter(indexingExecutionContext, indexInfo);
            break;
          case 7:
            switch (name[0])
            {
              case 'P':
                if (name == "Package")
                  return (IDocumentContracter) new PackageVersionContracter(indexingExecutionContext, indexInfo);
                break;
              case 'S':
                if (name == "Setting")
                  return (IDocumentContracter) new SettingContracter(indexingExecutionContext, indexInfo);
                break;
            }
            break;
          case 8:
            if (name == "WorkItem")
              return (IDocumentContracter) new WorkItemContracterV2(indexingExecutionContext, indexInfo);
            break;
          case 11:
            if (name == "ProjectRepo")
              return (IDocumentContracter) new ProjectContracter(indexingExecutionContext, indexInfo);
            break;
        }
      }
      throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Unsupported Entity type '{0}'", (object) typeForContractType.Name)));
    }
  }
}
