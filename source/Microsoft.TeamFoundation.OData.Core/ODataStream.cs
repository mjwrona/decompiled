// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataStream
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.IO;

namespace Microsoft.OData
{
  internal abstract class ODataStream : Stream
  {
    private IODataStreamListener listener;

    internal ODataStream(IODataStreamListener listener) => this.listener = listener;

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.listener != null)
      {
        this.listener.StreamDisposed();
        this.listener = (IODataStreamListener) null;
      }
      base.Dispose(disposing);
    }

    protected void ValidateNotDisposed()
    {
      if (this.listener == null)
        throw new ObjectDisposedException((string) null, Strings.ODataBatchOperationStream_Disposed);
    }
  }
}
