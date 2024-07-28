// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.BufferProvider
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Buffers;

namespace Microsoft.Azure.Documents
{
  internal sealed class BufferProvider
  {
    private readonly ArrayPool<byte> arrayPool;

    public BufferProvider() => this.arrayPool = ArrayPool<byte>.Create();

    public BufferProvider.DisposableBuffer GetBuffer(int desiredLength) => new BufferProvider.DisposableBuffer(this, desiredLength);

    public struct DisposableBuffer : IDisposable
    {
      private readonly BufferProvider provider;

      public DisposableBuffer(byte[] buffer)
      {
        this.provider = (BufferProvider) null;
        this.Buffer = new ArraySegment<byte>(buffer, 0, buffer.Length);
      }

      public DisposableBuffer(BufferProvider provider, int desiredLength)
      {
        this.provider = provider;
        this.Buffer = new ArraySegment<byte>(provider.arrayPool.Rent(desiredLength), 0, desiredLength);
      }

      public ArraySegment<byte> Buffer { get; private set; }

      public void Dispose()
      {
        if (this.Buffer.Array == null)
          return;
        this.provider?.arrayPool.Return(this.Buffer.Array);
        this.Buffer = new ArraySegment<byte>();
      }
    }
  }
}
