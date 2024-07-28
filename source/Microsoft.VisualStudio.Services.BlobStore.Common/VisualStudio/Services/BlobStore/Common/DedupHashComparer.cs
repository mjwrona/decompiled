// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.DedupHashComparer
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using BuildXL.Cache.ContentStore.Interfaces.Utils;
using Microsoft.DataDeduplication.Interop;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  [CLSCompliant(false)]
  public sealed class DedupHashComparer : IEqualityComparer<DedupHash>, IComparer<DedupHash>
  {
    public static readonly DedupHashComparer Instance = new DedupHashComparer();

    private DedupHashComparer()
    {
    }

    public int Compare(DedupHash x, DedupHash y) => ByteArrayComparer.Instance.Compare(x.Hash, y.Hash);

    public bool Equals(DedupHash x, DedupHash y) => ByteArrayComparer.Instance.Equals(x.Hash, y.Hash);

    public int GetHashCode(DedupHash obj) => ByteArrayComparer.Instance.GetHashCode(obj.Hash);
  }
}
