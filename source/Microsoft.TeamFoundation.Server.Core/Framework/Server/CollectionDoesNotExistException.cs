// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CollectionDoesNotExistException
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class CollectionDoesNotExistException : TeamFoundationServiceException
  {
    public CollectionDoesNotExistException(Guid collectionId)
      : base(TFCommonResources.CollectionDoesNotExist((object) collectionId.ToString()))
    {
      this.EventId = TeamFoundationEventId.CollectionDoesNotExistException;
    }

    public CollectionDoesNotExistException(Guid collectionId, Exception innerException)
      : base(TFCommonResources.CollectionDoesNotExist((object) collectionId.ToString()), innerException)
    {
      this.EventId = TeamFoundationEventId.CollectionDoesNotExistException;
    }

    public CollectionDoesNotExistException(string collectionName)
      : base(TFCommonResources.CollectionDoesNotExist((object) collectionName))
    {
      this.EventId = TeamFoundationEventId.CollectionDoesNotExistException;
    }
  }
}
