// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.KeepUntilReceipt
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public class KeepUntilReceipt
  {
    public static readonly byte[] NullSignature = new byte[32];
    public readonly byte[] Signature;
    public readonly KeepUntilBlobReference KeepUntil;

    public KeepUntilReceipt(KeepUntilBlobReference keepUntil, byte[] signature)
    {
      this.KeepUntil = keepUntil;
      this.Signature = signature;
    }

    public override bool Equals(object obj) => (object) (obj as KeepUntilReceipt) != null && this.Equals((KeepUntilReceipt) obj);

    public static bool operator ==(KeepUntilReceipt lhs, KeepUntilReceipt rhs) => object.Equals((object) lhs, (object) rhs);

    public static bool operator !=(KeepUntilReceipt lhs, KeepUntilReceipt rhs) => !(lhs == rhs);

    public bool Equals(KeepUntilReceipt obj) => this.KeepUntil == obj.KeepUntil && ((IEnumerable<byte>) this.Signature).SequenceEqual<byte>((IEnumerable<byte>) obj.Signature);

    public override int GetHashCode() => Convert.ToBase64String(this.Signature).GetHashCode();

    public static KeepUntilReceipt Create(
      string secret,
      Guid serviceHost,
      DedupIdentifier dedupId,
      KeepUntilBlobReference keepuntil)
    {
      byte[] signature = KeepUntilReceipt.ComputeSignature(secret, serviceHost, dedupId, keepuntil);
      return new KeepUntilReceipt(keepuntil, signature);
    }

    internal static byte[] ComputeSignature(
      string secret,
      Guid serviceHost,
      DedupIdentifier dedupId,
      KeepUntilBlobReference keepuntil)
    {
      using (MemoryStream output = new MemoryStream())
      {
        using (BinaryWriter binaryWriter = new BinaryWriter((Stream) output))
        {
          binaryWriter.Write(serviceHost.ToByteArray());
          binaryWriter.Write(dedupId.Value);
          binaryWriter.Write(keepuntil.KeepUntil.ToFileTimeUtc());
          binaryWriter.Flush();
          byte[] bytes = Encoding.UTF8.GetBytes(secret);
          return KeepUntilReceipt.HMACSHA256Encode(output.ToArray(), bytes);
        }
      }
    }

    private static byte[] HMACSHA256Encode(byte[] input, byte[] key) => new HMACSHA256(key).ComputeHash(input);
  }
}
