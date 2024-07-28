// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.IDocumentContractTypeService
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  [DefaultServiceImplementation(typeof (DocumentContractTypeService))]
  public interface IDocumentContractTypeService : IVssFrameworkService
  {
    DocumentContractType GetSupportedQueryDocumentContractType(
      IVssRequestContext requestContext,
      IEntityType entityType);

    DocumentContractType GetSupportedIndexDocumentContractType(
      IVssRequestContext requestContext,
      IEntityType entityType);

    DocumentContractType GetSupportedIndexDocumentContractTypeDuringZLRI(
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit,
      bool isShadow);

    IEnumerable<IndexInfo> GetQueryIndexInfo(IEntityType entityType);

    string GetIndexESConnectionString(IVssRequestContext requestContext, IEntityType entityType);

    string GetIndexESConnectionStringDuringZLRI(
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit);

    string GetQueryESConnectionString(IEntityType entityType);
  }
}
