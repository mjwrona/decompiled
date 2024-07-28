// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Symbol.WebApi.DebugEntryNotChunkedException
// Assembly: Microsoft.VisualStudio.Services.Symbol.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52CDDA61-EF2D-4AD8-A25B-09A8F04FE8C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Symbol.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Symbol.WebApi
{
  [Serializable]
  public class DebugEntryNotChunkedException : SymbolException
  {
    public DebugEntryNotChunkedException(string message, Exception ex)
      : base(message, ex)
    {
    }

    public DebugEntryNotChunkedException(string message)
      : this(message, (Exception) null)
    {
    }

    public static DebugEntryNotChunkedException Create(
      string requestName,
      string debugEntryId,
      string reason)
    {
      return new DebugEntryNotChunkedException(DebugEntryNotChunkedException.MakeMessage(requestName, debugEntryId, reason));
    }

    public static DebugEntryNotChunkedException Create(
      string requestName,
      string debugEntryId,
      string reason,
      Exception innerException)
    {
      return new DebugEntryNotChunkedException(DebugEntryNotChunkedException.MakeMessage(requestName, debugEntryId, reason), innerException);
    }

    private static string MakeMessage(string requestName, string debugEntryId, string reason) => Resources.DebugEntryNotChunkedExceptionMessage((object) requestName, (object) debugEntryId, (object) reason);
  }
}
