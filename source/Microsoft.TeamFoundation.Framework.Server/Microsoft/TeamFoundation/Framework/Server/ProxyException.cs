// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ProxyException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class ProxyException : TeamFoundationServiceException
  {
    public ProxyException()
      : this(FrameworkResources.UnknownProxyException())
    {
      this.EventId = TeamFoundationEventId.ProxyException;
    }

    public ProxyException(string message)
      : this(message, (Exception) null)
    {
      this.EventId = TeamFoundationEventId.ProxyException;
    }

    public ProxyException(Exception innerException)
      : this(FrameworkResources.UnknownProxyException(), innerException)
    {
      this.EventId = TeamFoundationEventId.ProxyException;
    }

    public ProxyException(string message, Exception innerException)
      : base(message, innerException)
    {
      this.EventId = TeamFoundationEventId.ProxyException;
    }

    protected ProxyException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
