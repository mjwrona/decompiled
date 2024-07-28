// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Bitmap.IsolationBitmapProvider
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Git.Server.Bitmap.Roaring;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Bitmap
{
  internal class IsolationBitmapProvider : IIsolationBitmapProvider
  {
    private readonly Func<ITwoWayReadOnlyList<Sha1Id>> m_odbObjectListFactory;
    private readonly IRepoBitmapFileProvider m_filePrv;
    private bool m_odbBitmapInitialized;
    private Guid? m_odbBitmapId;
    private RoaringBitmap<Sha1Id> m_odbBitmap;

    public IsolationBitmapProvider(
      Func<ITwoWayReadOnlyList<Sha1Id>> odbObjectListFactory,
      IRepoBitmapFileProvider filePrv)
    {
      this.m_odbObjectListFactory = odbObjectListFactory;
      this.m_filePrv = filePrv;
    }

    public IReadOnlyBitmap<Sha1Id> GetOdb()
    {
      this.EnsureOdbBitmapLoaded();
      return (IReadOnlyBitmap<Sha1Id>) this.m_odbBitmap;
    }

    public void AddOdbObjectsAndSerialize(IEnumerable<Sha1Id> objectIds)
    {
      RoaringBitmap<Sha1Id> bitmap1 = objectIds as RoaringBitmap<Sha1Id>;
      ITwoWayReadOnlyList<Sha1Id> objectList = this.m_odbObjectListFactory();
      if (bitmap1 == null)
      {
        bitmap1 = new RoaringBitmap<Sha1Id>(objectList);
        foreach (Sha1Id objectId in objectIds)
          bitmap1.Add(objectId);
      }
      this.EnsureOdbBitmapLoaded();
      RoaringBitmap<Sha1Id> bitmap2 = RoaringBitmapCombiner<Sha1Id>.Union(bitmap1, this.m_odbBitmap);
      bitmap2.OptimizeAsReadOnly();
      this.m_filePrv.Update<Sha1Id>(IsolationBitmapType.Odb, bitmap2, this.m_odbBitmapId);
      this.m_odbBitmapInitialized = false;
    }

    private void EnsureOdbBitmapLoaded()
    {
      if (this.m_odbBitmapInitialized)
        return;
      this.m_odbBitmapId = this.m_filePrv.GetFileId(IsolationBitmapType.Odb);
      ITwoWayReadOnlyList<Sha1Id> objectList = this.m_odbObjectListFactory();
      this.m_odbBitmap = !this.m_odbBitmapId.HasValue ? new RoaringBitmap<Sha1Id>(objectList) : this.m_filePrv.GetBitmap<Sha1Id>(IsolationBitmapType.Odb, this.m_odbBitmapId.Value, objectList);
      this.m_odbBitmapInitialized = true;
    }
  }
}
