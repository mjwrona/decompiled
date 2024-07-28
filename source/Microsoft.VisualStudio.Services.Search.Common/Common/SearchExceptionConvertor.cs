// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.SearchExceptionConvertor
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public static class SearchExceptionConvertor
  {
    public static Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions.SearchException ConvertLegacyExceptionToCorrectException(
      this Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.SearchException legacySException)
    {
      Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions.SearchException sException;
      switch (legacySException)
      {
        case null:
          throw new ArgumentNullException(nameof (legacySException));
        case Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.InvalidQueryException _:
          sException = (Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions.SearchException) new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions.InvalidQueryException(legacySException.Message, legacySException.InnerException);
          break;
        case Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.InvalidCustomRequestException _:
          sException = (Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions.SearchException) new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions.InvalidCustomRequestException(legacySException.Message, legacySException.InnerException);
          break;
        default:
          sException = new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions.SearchException(legacySException.Message, legacySException.InnerException);
          break;
      }
      SearchExceptionConvertor.CopyExceptionProperties(legacySException, sException);
      return sException;
    }

    private static void CopyExceptionProperties(
      Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.SearchException legacySException,
      Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions.SearchException sException)
    {
      sException.ErrorCode = legacySException.ErrorCode;
      sException.EventId = legacySException.EventId;
      sException.HelpLink = legacySException.HelpLink;
      sException.ReportException = legacySException.ReportException;
      sException.LogLevel = legacySException.LogLevel;
      sException.Source = legacySException.Source;
      sException.LogException = legacySException.LogException;
    }
  }
}
