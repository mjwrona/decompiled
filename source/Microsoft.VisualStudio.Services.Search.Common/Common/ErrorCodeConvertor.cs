// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.ErrorCodeConvertor
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public static class ErrorCodeConvertor
  {
    public static bool TryConvertToInfoCode(
      IEnumerable<ErrorData> errorCodes,
      out InfoCodes infoCode)
    {
      infoCode = InfoCodes.Ok;
      if (!errorCodes.Any<ErrorData>())
        return false;
      string errorCode = ErrorCodeConvertor.Collapse(errorCodes).ErrorCode;
      InfoCodes result;
      if (!Enum.TryParse<InfoCodes>(errorCode, out result))
        Tracer.TraceError(1080078, "REST-API", "REST-API", FormattableString.Invariant(FormattableStringFactory.Create("Invalid error code [{0}] cannot be converted to {1} enum.", (object) errorCode, (object) "InfoCodes")));
      infoCode = result;
      return true;
    }

    private static ErrorData Collapse(IEnumerable<ErrorData> errorCodes)
    {
      foreach (ErrorData errorCode in errorCodes)
      {
        if (errorCode.ErrorCode.Equals("AccountIsBeingOnboarded", StringComparison.Ordinal))
          return errorCode;
      }
      return errorCodes.First<ErrorData>();
    }
  }
}
