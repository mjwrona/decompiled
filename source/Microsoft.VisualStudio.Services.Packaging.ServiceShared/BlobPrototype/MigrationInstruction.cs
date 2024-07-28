// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.MigrationInstruction
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public abstract class MigrationInstruction : IMigrationInstruction, IMigrationEdit
  {
    protected MigrationInstruction(
      MigrationInstructionEnum instructionName,
      string destinationMigration,
      CollectionId collectionId,
      Guid feedId,
      IProtocol protocol)
    {
      this.InstructionName = instructionName;
      this.DestinationMigration = destinationMigration;
      this.CollectionId = collectionId;
      this.FeedId = feedId;
      this.Protocol = protocol;
    }

    public string DestinationMigration { get; }

    public CollectionId CollectionId { get; }

    public Guid FeedId { get; }

    public IProtocol Protocol { get; }

    public MigrationInstructionEnum InstructionName { get; }

    public static MigrationInstruction Create(
      MigrationInstructionEnum instructionName,
      string destinationMigration,
      CollectionId collectionId,
      Guid feedId,
      IProtocol protocol)
    {
      switch (instructionName)
      {
        case MigrationInstructionEnum.Compute:
          return (MigrationInstruction) new ComputeInstruction(destinationMigration, collectionId, feedId, protocol);
        case MigrationInstructionEnum.LockStep:
          return (MigrationInstruction) new LockStepInstruction(destinationMigration, collectionId, feedId, protocol);
        case MigrationInstructionEnum.ReadVNext:
          return (MigrationInstruction) new ReadVNextInstruction(destinationMigration, collectionId, feedId, protocol);
        case MigrationInstructionEnum.Rollback:
          return (MigrationInstruction) new RollbackInstruction(destinationMigration, collectionId, feedId, protocol);
        case MigrationInstructionEnum.Complete:
          return (MigrationInstruction) new CompleteInstruction(destinationMigration, collectionId, feedId, protocol);
        case MigrationInstructionEnum.RollbackToLockStep:
          return (MigrationInstruction) new RollbackToLockStepInstruction(destinationMigration, collectionId, feedId, protocol);
        default:
          throw new ArgumentException(Resources.Error_InstructionNameNotSupported((object) instructionName), nameof (instructionName));
      }
    }
  }
}
