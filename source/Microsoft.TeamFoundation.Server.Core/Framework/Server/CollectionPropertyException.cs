// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CollectionPropertyException
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class CollectionPropertyException : TeamFoundationServiceException
  {
    public CollectionPropertyException() => this.EventId = TeamFoundationEventId.CollectionPropertyException;

    public CollectionPropertyException(string message)
      : base(message)
    {
      this.EventId = TeamFoundationEventId.CollectionPropertyException;
    }

    public CollectionPropertyException(string message, Exception innerException)
      : base(message, innerException)
    {
      this.EventId = TeamFoundationEventId.CollectionPropertyException;
    }

    protected CollectionPropertyException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.EventId = TeamFoundationEventId.CollectionPropertyException;
    }
  }
}
