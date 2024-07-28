// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.ReSearch.Core.Stores.StorageEndpoint.HashedBlob
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace Microsoft.VisualStudio.Services.Search.ReSearch.Core.Stores.StorageEndpoint
{
  public abstract class HashedBlob
  {
    [StaticSafe]
    [ThreadStatic]
    private static WeakReference<SHA1> s_hasher;
    protected const uint BlobMagic = 3138453691;

    [StaticSafe]
    protected static int HashLength { get; private set; }

    static HashedBlob() => HashedBlob.HashLength = HashedBlob.GetHasher().HashSize / 8;

    protected static byte[] ComputeHash(byte[] blob) => HashedBlob.GetHasher().ComputeHash(blob);

    protected static byte[] ComputeHash(ArraySegment<byte> blob) => HashedBlob.GetHasher().ComputeHash(blob.Array, blob.Offset, blob.Count);

    [SuppressMessage("Microsoft.Cryptographic.Standard", "CA5354:SHA1CannotBeUsed")]
    private static SHA1 GetHasher()
    {
      SHA1 target;
      if (HashedBlob.s_hasher == null || !HashedBlob.s_hasher.TryGetTarget(out target))
      {
        target = (SHA1) new SHA1Managed();
        if (HashedBlob.s_hasher == null)
          HashedBlob.s_hasher = new WeakReference<SHA1>(target);
        else
          HashedBlob.s_hasher.SetTarget(target);
      }
      return target;
    }
  }
}
