// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.UserMapping.UserStatusChangeEvent
// Assembly: Microsoft.VisualStudio.Services.Licensing.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F3070F25-7414-49A0-9C00-005379F04A49
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.UserMapping
{
  public class UserStatusChangeEvent
  {
    public UserStatusChangeEvent(Guid userId, Guid collectionId, UserStatusChangeKind newStatus)
    {
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      ArgumentUtility.CheckForEmptyGuid(collectionId, nameof (collectionId));
      this.UserId = userId;
      this.CollectionId = collectionId;
      this.NewStatus = newStatus;
    }

    public Guid CollectionId { get; }

    public Guid UserId { get; }

    public UserStatusChangeKind NewStatus { get; }
  }
}
