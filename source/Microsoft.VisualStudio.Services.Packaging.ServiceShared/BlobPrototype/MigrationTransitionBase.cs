// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.MigrationTransitionBase
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public abstract class MigrationTransitionBase : IMigrationTransition, IMigrationEdit
  {
    protected MigrationTransitionBase(
      string destinationMigration,
      CollectionId collectionId,
      Guid feedId,
      IProtocol protocol)
    {
      this.DestinationMigration = destinationMigration;
      this.CollectionId = collectionId;
      this.FeedId = feedId;
      this.Protocol = protocol;
    }

    public string DestinationMigration { get; }

    public CollectionId CollectionId { get; }

    public Guid FeedId { get; }

    public IProtocol Protocol { get; }
  }
}
