// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.FileHandle
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using System;
using System.Net;

namespace Microsoft.Azure.Storage.File
{
  public sealed class FileHandle
  {
    public ulong? HandleId { get; internal set; }

    public string Path { get; internal set; }

    public IPAddress ClientIp { get; internal set; }

    public int ClientPort { get; internal set; }

    public DateTimeOffset OpenTime { get; internal set; }

    public DateTimeOffset? LastReconnectTime { get; internal set; }

    public ulong FileId { get; internal set; }

    public ulong ParentId { get; internal set; }

    public ulong SessionId { get; internal set; }
  }
}
