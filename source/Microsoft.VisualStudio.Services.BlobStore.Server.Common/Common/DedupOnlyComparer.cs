// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupOnlyComparer
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public class DedupOnlyComparer : IEqualityComparer<DedupMetadataEntry>
  {
    public static readonly DedupOnlyComparer Instance = new DedupOnlyComparer();

    private DedupOnlyComparer()
    {
    }

    public bool Equals(DedupMetadataEntry x, DedupMetadataEntry y) => x.DedupId.Equals(y.DedupId);

    public int GetHashCode(DedupMetadataEntry entry) => entry.DedupId.GetHashCode();
  }
}
