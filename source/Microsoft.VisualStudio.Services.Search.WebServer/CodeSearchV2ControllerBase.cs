// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.CodeSearchV2ControllerBase
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Query;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi;
using Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  public abstract class CodeSearchV2ControllerBase : CodeSearchControllerBase
  {
    protected CodeSearchV2ControllerBase()
    {
    }

    internal CodeSearchV2ControllerBase(
      IIndexMapper indexMapper,
      ISearchQueryForwarder<SearchQuery, CodeQueryResponse> codeSearchQueryForwarder)
      : base(indexMapper, codeSearchQueryForwarder)
    {
    }

    internal CodeSearchResponse HandleCodeSearchResults(
      CodeSearchRequest request,
      ProjectInfo projectInfo = null)
    {
      try
      {
        if (request == null)
          throw new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions.InvalidQueryException(Microsoft.VisualStudio.Services.Search.WebApi.SearchWebApiResources.NullQueryMessage);
        this.ValidateQuery(request);
        CodeQueryResponse response = this.HandlePostCodeQueryRequest(this.TfsRequestContext, request.ToOldRequestContract(), this.EnableSecurityChecksInQueryPipeline, projectInfo);
        CodeSearchResponse responseContract = response != null ? response.ToNewResponseContract() : (CodeSearchResponse) null;
        this.PopulateSearchSecuredObjectInResponse(this.TfsRequestContext, (SearchSecuredV2Object) responseContract);
        return responseContract;
      }
      catch (Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.SearchException ex)
      {
        throw ex.ConvertLegacyExceptionToCorrectException();
      }
    }

    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.IFormatProvider,System.String,System.Object,System.Object)", Justification = "CurrentUICulture is to be used by design.")]
    protected virtual void ValidateQuery(CodeSearchRequest request)
    {
      request.ValidateQuery();
      if (request.Skip < 0 || request.Skip > this.SkipResultsCap)
        throw new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions.InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Microsoft.VisualStudio.Services.Search.WebApi.SearchWebApiResources.InvalidSkipMessage, (object) this.SkipResultsCap, (object) request.Skip));
      if (request.Top < 1 || request.Top > this.TakeResultsCap)
        throw new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions.InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Microsoft.VisualStudio.Services.Search.Shared.WebApi.SearchSharedWebApiResources.InvalidTopMessage, (object) this.TakeResultsCap, (object) request.Top));
    }
  }
}
