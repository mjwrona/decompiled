// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.Utils.StreamExtensions
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table.RestExecutor.TableCommand;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor.Utils
{
  internal static class StreamExtensions
  {
    [DebuggerNonUserCode]
    internal static int GetBufferSize(Stream inStream) => inStream.CanSeek && inStream.Length - inStream.Position > 0L ? (int) Math.Min(inStream.Length - inStream.Position, 65536L) : 65536;

    [DebuggerNonUserCode]
    internal static Task WriteToAsync<T>(
      this Stream stream,
      Stream toStream,
      ExecutionState<T> executionState,
      CancellationToken cancellationToken)
    {
      return new AsyncStreamCopier<T>(stream, toStream, executionState, new int?(StreamExtensions.GetBufferSize(stream))).StartCopyStream(new long?(), new long?(), cancellationToken);
    }
  }
}
