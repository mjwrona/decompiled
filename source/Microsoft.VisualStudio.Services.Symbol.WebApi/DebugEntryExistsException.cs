// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Symbol.WebApi.DebugEntryExistsException
// Assembly: Microsoft.VisualStudio.Services.Symbol.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52CDDA61-EF2D-4AD8-A25B-09A8F04FE8C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Symbol.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Symbol.WebApi
{
  [Serializable]
  public class DebugEntryExistsException : SymbolException
  {
    public DebugEntryExistsException(string message, Exception ex)
      : base(message, ex)
    {
    }

    public DebugEntryExistsException(string message)
      : this(message, (Exception) null)
    {
    }

    public static DebugEntryExistsException Create(string requestId, string debugEntryId) => new DebugEntryExistsException(DebugEntryExistsException.MakeMessage(requestId, debugEntryId));

    private static string MakeMessage(string requestId, string debugEntryId) => Resources.DebugEntryExistsExceptionMessage((object) requestId, (object) debugEntryId);
  }
}
