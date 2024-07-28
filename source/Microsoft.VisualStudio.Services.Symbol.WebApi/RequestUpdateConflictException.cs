// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Symbol.WebApi.RequestUpdateConflictException
// Assembly: Microsoft.VisualStudio.Services.Symbol.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52CDDA61-EF2D-4AD8-A25B-09A8F04FE8C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Symbol.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Symbol.WebApi
{
  [Serializable]
  public class RequestUpdateConflictException : SymbolException
  {
    public RequestUpdateConflictException(string message, Exception ex)
      : base(message, ex)
    {
    }

    public RequestUpdateConflictException(string message)
      : this(message, (Exception) null)
    {
    }

    public static RequestUpdateConflictException Create(
      string requestId,
      string currentEtag,
      string expectedEtag)
    {
      return new RequestUpdateConflictException(RequestUpdateConflictException.MakeMessage(requestId, currentEtag, expectedEtag));
    }

    private static string MakeMessage(string requestId, string currentEtag, string expectedEtag) => Resources.RequestUpdateConflictExceptionMessage((object) requestId, (object) currentEtag, (object) expectedEtag);
  }
}
