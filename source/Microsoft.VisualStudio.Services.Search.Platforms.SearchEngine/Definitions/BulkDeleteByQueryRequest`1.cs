// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.BulkDeleteByQueryRequest`1
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using System;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions
{
  public class BulkDeleteByQueryRequest<T> where T : AbstractSearchDocumentContract
  {
    public BulkDeleteByQueryRequest()
    {
    }

    public BulkDeleteByQueryRequest(
      IExpression query,
      DocumentContractType contractType,
      string[] routingIds = null)
      : this(query, contractType, TimeSpan.MaxValue, routingIds)
    {
    }

    public BulkDeleteByQueryRequest(
      IExpression query,
      DocumentContractType contractType,
      TimeSpan requestTimeOut,
      string[] routingIds = null)
    {
      this.Query = query;
      this.ContractType = contractType;
      this.RequestTimeOut = requestTimeOut;
      this.RoutingIds = routingIds;
    }

    public IExpression Query { get; set; }

    public string[] RoutingIds { get; set; }

    public DocumentContractType ContractType { get; set; }

    public bool Lenient { get; set; }

    public TimeSpan RequestTimeOut { get; set; }
  }
}
