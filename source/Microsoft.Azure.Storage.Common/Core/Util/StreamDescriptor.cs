// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.StreamDescriptor
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System.Threading;

namespace Microsoft.Azure.Storage.Core.Util
{
  internal class StreamDescriptor
  {
    private long length;
    private volatile string md5;
    private volatile string crc64;
    private volatile ChecksumWrapper checksumWrapper;

    public long Length
    {
      get => Interlocked.Read(ref this.length);
      set => Interlocked.Exchange(ref this.length, value);
    }

    public string Md5
    {
      get => this.md5;
      set => this.md5 = value;
    }

    public string Crc64
    {
      get => this.crc64;
      set => this.crc64 = value;
    }

    public ChecksumWrapper ChecksumWrapper
    {
      get => this.checksumWrapper;
      set => this.checksumWrapper = value;
    }
  }
}
