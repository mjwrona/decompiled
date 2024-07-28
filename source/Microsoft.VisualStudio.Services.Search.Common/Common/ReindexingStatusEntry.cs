// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.ReindexingStatusEntry
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class ReindexingStatusEntry : IEquatable<ReindexingStatusEntry>
  {
    public ReindexingStatusEntry()
    {
    }

    public ReindexingStatusEntry(Guid collectionId, IEntityType entityType)
    {
      this.CollectionId = collectionId;
      this.EntityType = entityType;
    }

    public Guid CollectionId { get; }

    public IEntityType EntityType { get; }

    public ReindexingStatus Status { get; set; }

    public bool Equals(ReindexingStatusEntry other) => other != null && this.CollectionId.Equals(other.CollectionId) && this.EntityType.Equals((object) other.EntityType) && this.Status == other.Status && (int) this.Priority == (int) other.Priority;

    public DateTime LastUpdatedTimeStamp { get; set; }

    public short Priority { get; set; }

    public override string ToString() => FormattableString.Invariant(FormattableStringFactory.Create("{0}: {1}, {2}: {3}, {4}: {5}", (object) "CollectionId", (object) this.CollectionId, (object) "EntityType", (object) this.EntityType.Name, (object) "Status", (object) this.Status));
  }
}
