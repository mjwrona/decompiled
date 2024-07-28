// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.DisposableEtagValue`1
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public struct DisposableEtagValue<T> : IEquatable<EtagValue<T>>, IDisposable where T : IDisposable
  {
    public readonly T Value;
    public readonly string Etag;

    public DisposableEtagValue(T value, string etag)
    {
      this.Value = value;
      this.Etag = etag;
    }

    public bool Equals(EtagValue<T> other) => this.Etag == other.Etag;

    public void Dispose()
    {
      if ((object) this.Value == null)
        return;
      this.Value.Dispose();
    }
  }
}
