// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Symbol.WebApi.RequestNotFoundException
// Assembly: Microsoft.VisualStudio.Services.Symbol.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52CDDA61-EF2D-4AD8-A25B-09A8F04FE8C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Symbol.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Symbol.WebApi
{
  [Serializable]
  public class RequestNotFoundException : SymbolException
  {
    public RequestNotFoundException(string message, Exception ex)
      : base(message, ex)
    {
    }

    public RequestNotFoundException(string message)
      : this(message, (Exception) null)
    {
    }

    public static RequestNotFoundException Create(string requestName) => new RequestNotFoundException(RequestNotFoundException.MakeMessage(requestName));

    private static string MakeMessage(string requestName) => Resources.RequestNotFoundExceptionMessage((object) requestName);
  }
}
