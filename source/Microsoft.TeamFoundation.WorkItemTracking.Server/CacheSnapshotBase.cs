// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.CacheSnapshotBase
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public abstract class CacheSnapshotBase
  {
    public CacheSnapshotBase(MetadataDBStamps stamps) => this.Stamps = stamps;

    public MetadataDBStamps Stamps { get; protected set; }

    internal virtual bool IsFresh(
      IVssRequestContext context,
      IEnumerable<MetadataTable> metadataTables)
    {
      return this.Stamps.Compare(context.MetadataDbStamps(metadataTables)) >= 0;
    }

    internal virtual void MarkSnapshotForUse(
      IVssRequestContext context,
      CacheSnapshotBase snapshotToReplace)
    {
    }
  }
}
