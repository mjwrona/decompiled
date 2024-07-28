// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.SecurityChecksService.ByteArrayComparer
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Query.SecurityChecksService
{
  internal class ByteArrayComparer : IEqualityComparer<byte[]>
  {
    public bool Equals(byte[] left, byte[] right) => left == null || right == null ? left == right : ((IEnumerable<byte>) left).SequenceEqual<byte>((IEnumerable<byte>) right);

    public int GetHashCode(byte[] key)
    {
      if (key == null)
        throw new ArgumentNullException(nameof (key));
      return key.Length >= 1 ? (int) key[0] : throw new ArgumentException("key cannot be empty");
    }
  }
}
