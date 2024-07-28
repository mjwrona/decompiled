// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.SearchPlatformExceptionLogger
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using System;
using System.Runtime.ExceptionServices;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public static class SearchPlatformExceptionLogger
  {
    public static void LogSearchPlatformException(Exception ex)
    {
      if (ex == null)
        throw new ArgumentNullException(nameof (ex));
      if (ex is SearchPlatformException platformException)
      {
        Tracer.PublishKpiAndCi("SearchServiceErrorCode", "Query Pipeline", (double) platformException.ErrorCode, true);
        Tracer.PublishClientTrace("Query Pipeline", "Query", "SearchPlatformErrorMessage", (object) platformException.ToString(), true);
        if (platformException.ErrorCode == SearchServiceErrorCode.ElasticSearch_Connection_Error)
          throw new SearchException(SearchWebApiResources.ElasticsearchUnavailableErrorMessage);
      }
      if (ex.GetType().Name.Equals("InvalidQueryException") || ex.GetType().Name.Equals("UnsupportedHostTypeException"))
      {
        Tracer.PublishKpiAndCi("SearchServiceErrorCode", "Query Pipeline", 5.0, true);
        ExceptionDispatchInfo.Capture(ex).Throw();
      }
      else
      {
        if (platformException == null)
          Tracer.PublishKpiAndCi("SearchServiceErrorCode", "Query Pipeline", 14.0, true);
        throw new SearchException(SearchWebApiResources.UnexpectedSearchErrorMessage);
      }
    }
  }
}
