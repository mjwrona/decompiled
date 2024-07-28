// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.MigrationTransitionException
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class MigrationTransitionException : VssServiceException
  {
    public MigrationTransitionException(IMigrationInstruction instruction, string message)
      : this(instruction.Protocol, instruction.FeedId, instruction.CollectionId.Guid, message)
    {
    }

    public MigrationTransitionException(IMigrationTransition transition, string message)
      : this(transition.Protocol, transition.FeedId, transition.CollectionId.Guid, message)
    {
    }

    public MigrationTransitionException(
      IProtocol protocol,
      Guid feedId,
      Guid collectionId,
      string message)
      : base(message)
    {
      this.Protocol = protocol;
      this.FeedId = feedId;
      this.CollectionId = collectionId;
    }

    public IProtocol Protocol { get; }

    public Guid FeedId { get; }

    public Guid CollectionId { get; }
  }
}
