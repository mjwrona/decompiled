// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Symbol.WebApi.InvalidEndpointParameterException
// Assembly: Microsoft.VisualStudio.Services.Symbol.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52CDDA61-EF2D-4AD8-A25B-09A8F04FE8C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Symbol.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Symbol.WebApi
{
  [Serializable]
  public class InvalidEndpointParameterException : SymbolException
  {
    public InvalidEndpointParameterException(string message, Exception ex)
      : base(message, ex)
    {
    }

    public InvalidEndpointParameterException(string message)
      : this(message, (Exception) null)
    {
    }

    public static InvalidEndpointParameterException Create(string message) => new InvalidEndpointParameterException(InvalidEndpointParameterException.MakeMessage(message));

    private static string MakeMessage(string message) => Resources.InvalidEndpointParameterExceptionMessage((object) message);
  }
}
