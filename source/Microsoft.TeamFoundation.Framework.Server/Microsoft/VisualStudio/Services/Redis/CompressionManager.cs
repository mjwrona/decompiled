// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.CompressionManager
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.IO;
using System.IO.Compression;

namespace Microsoft.VisualStudio.Services.Redis
{
  internal class CompressionManager
  {
    private readonly bool m_enableCompression;

    public CompressionManager(bool enableCompression) => this.m_enableCompression = enableCompression;

    public Stream CreateInputStream<T>(Stream stream) => this.m_enableCompression && this.IsCompressable(typeof (T)) ? (Stream) new GZipStream(stream, CompressionMode.Decompress) : stream;

    public Stream CreateOutputStream<T>(Stream stream) => this.m_enableCompression && this.IsCompressable(typeof (T)) ? (Stream) new GZipStream(stream, CompressionMode.Compress) : stream;

    protected virtual bool IsCompressable(Type type) => !type.IsValueType;
  }
}
