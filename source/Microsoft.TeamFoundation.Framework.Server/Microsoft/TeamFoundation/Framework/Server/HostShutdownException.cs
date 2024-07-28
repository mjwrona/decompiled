// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostShutdownException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class HostShutdownException : RequestCanceledException
  {
    public HostShutdownException()
      : base(FrameworkResources.HostShutdownError())
    {
      this.LogException = true;
      this.EventId = TeamFoundationEventId.ForcefullyShuttingDown;
      this.LogLevel = EventLogEntryType.Error;
    }

    public HostShutdownException(string message)
      : base(message)
    {
    }

    public HostShutdownException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected HostShutdownException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
