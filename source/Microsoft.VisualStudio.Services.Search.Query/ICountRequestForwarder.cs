// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.ICountRequestForwarder
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  public interface ICountRequestForwarder
  {
    CountResponse ForwardCountRequest(
      IVssRequestContext requestContext,
      CountRequest countRequest,
      IEnumerable<IndexInfo> indexInfo,
      IExpression scopeFiltersExpression,
      string requestId,
      DocumentContractType contractType);

    CountResponse GetZeroResultResponse(CountRequest countRequest);
  }
}
