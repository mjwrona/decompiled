// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.BulkGetByQueryRequest
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions
{
  public class BulkGetByQueryRequest
  {
    public BulkGetByQueryRequest(
      IndexInfo indexInfo,
      IExpression query,
      DocumentContractType contractType,
      IEnumerable<string> fields,
      int maxScrollSize,
      TimeSpan scrollTime)
    {
      if (indexInfo == null)
        throw new ArgumentNullException(nameof (indexInfo));
      if (DocumentContractType.Unsupported.Equals((object) contractType))
        throw new ArgumentException("contractType is Unsupported");
      if (string.IsNullOrWhiteSpace(indexInfo.IndexName))
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Name of the index is invalid.")), nameof (indexInfo));
      if (maxScrollSize <= 0)
        throw new ArgumentException("maxScrollSize should be greater than 0.");
      if (query == null || query is EmptyExpression)
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Request query is either null or EmptyExpression.")), nameof (query));
      if (scrollTime <= TimeSpan.Zero || scrollTime > TimeSpan.FromMinutes(5.0))
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("{0} should be in range (0,5m]", (object) nameof (scrollTime))));
      this.IndexName = indexInfo.IndexName;
      this.Routing = indexInfo.Routing;
      this.ContractType = contractType;
      this.Query = query;
      this.Fields = fields;
      this.MaxScrollSize = maxScrollSize;
      this.ScrollTime = scrollTime;
    }

    public string IndexName { get; }

    public string Routing { get; }

    public IExpression Query { get; }

    public DocumentContractType ContractType { get; }

    public IEnumerable<string> Fields { get; }

    public int MaxScrollSize { get; }

    public TimeSpan ScrollTime { get; }
  }
}
