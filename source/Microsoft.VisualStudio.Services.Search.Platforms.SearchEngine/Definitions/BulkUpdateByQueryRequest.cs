// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.BulkUpdateByQueryRequest
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions
{
  public class BulkUpdateByQueryRequest
  {
    public BulkUpdateByQueryRequest(
      IndexInfo indexInfo,
      DocumentContractType contractType,
      AbstractSearchDocumentContract updatedPartialAbstractSearchDocument,
      IExpression query)
      : this(indexInfo, contractType, updatedPartialAbstractSearchDocument, query, TimeSpan.MaxValue)
    {
    }

    public BulkUpdateByQueryRequest(
      IndexInfo indexInfo,
      DocumentContractType contractType,
      AbstractSearchDocumentContract updatedPartialAbstractSearchDocument,
      IExpression query,
      TimeSpan requestTimeOut)
    {
      if (indexInfo == null)
        throw new ArgumentNullException(nameof (indexInfo));
      if (string.IsNullOrWhiteSpace(indexInfo.IndexName))
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Name of the index is invalid.")), nameof (indexInfo));
      if (updatedPartialAbstractSearchDocument == null)
        throw new ArgumentNullException(nameof (updatedPartialAbstractSearchDocument));
      if (query == null)
        throw new ArgumentNullException(nameof (query));
      this.IndexName = indexInfo.IndexName;
      this.Routing = indexInfo.Routing;
      this.ContractType = contractType;
      this.UpdatedPartialAbstractSearchDocument = updatedPartialAbstractSearchDocument;
      this.Query = query;
      this.RequestTimeOut = requestTimeOut;
    }

    public AbstractSearchDocumentContract UpdatedPartialAbstractSearchDocument { get; }

    public IExpression Query { get; }

    public DocumentContractType ContractType { get; }

    public string IndexName { get; }

    public string Routing { get; }

    public TimeSpan RequestTimeOut { get; set; }
  }
}
