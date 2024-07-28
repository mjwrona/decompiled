// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.UserMapping.UserRightsTransferEvent
// Assembly: Microsoft.VisualStudio.Services.Licensing.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F3070F25-7414-49A0-9C00-005379F04A49
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.UserMapping
{
  public class UserRightsTransferEvent
  {
    public UserRightsTransferEvent(
      Guid collectionId,
      IEnumerable<KeyValuePair<Guid, Guid>> userIdTransferMap)
    {
      ArgumentUtility.CheckForEmptyGuid(collectionId, nameof (collectionId));
      this.CollectionId = collectionId;
      this.UserIdTransferMap = userIdTransferMap;
    }

    public Guid CollectionId { get; }

    public IEnumerable<KeyValuePair<Guid, Guid>> UserIdTransferMap { get; }
  }
}
