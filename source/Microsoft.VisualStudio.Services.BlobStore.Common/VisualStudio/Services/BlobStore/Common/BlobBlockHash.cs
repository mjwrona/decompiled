// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using Microsoft.VisualStudio.Services.Content.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  [JsonObject(MemberSerialization.OptIn)]
  public sealed class BlobBlockHash : IEquatable<BlobBlockHash>
  {
    [JsonProperty(PropertyName = "HashBytes")]
    [JsonConverter(typeof (ByteArrayAsNumberArrayJsonConvertor))]
    public readonly byte[] HashBytes;

    public BlobBlockHash(byte[] hashValue) => this.HashBytes = hashValue;

    public BlobBlockHash(string valueString) => this.HashBytes = valueString.ToByteArray();

    private BlobBlockHash()
    {
    }

    public string HashString => this.HashBytes.ToHexString();

    public static bool operator ==(BlobBlockHash x, BlobBlockHash y) => (object) x == null ? (object) y == null : x.Equals(y);

    public static bool operator !=(BlobBlockHash x, BlobBlockHash y) => !(x == y);

    public override bool Equals(object obj) => this.Equals(obj as BlobBlockHash);

    public bool Equals(BlobBlockHash other)
    {
      if ((object) this == (object) other)
        return true;
      return (object) other != null && ((IEnumerable<byte>) this.HashBytes).SequenceEqual<byte>((IEnumerable<byte>) other.HashBytes);
    }

    public override int GetHashCode() => BitConverter.ToInt32(this.HashBytes, 0);

    public override string ToString() => this.HashString;
  }
}
