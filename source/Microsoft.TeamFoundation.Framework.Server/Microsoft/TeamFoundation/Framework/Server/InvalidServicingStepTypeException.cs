// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.InvalidServicingStepTypeException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class InvalidServicingStepTypeException : TeamFoundationServicingException
  {
    public InvalidServicingStepTypeException() => this.EventId = TeamFoundationEventId.ServicingError;

    public InvalidServicingStepTypeException(string message)
      : base(message)
    {
      this.EventId = TeamFoundationEventId.ServicingError;
    }

    public InvalidServicingStepTypeException(string message, Exception innerException)
      : base(message, innerException)
    {
      this.EventId = TeamFoundationEventId.ServicingError;
    }

    protected InvalidServicingStepTypeException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.EventId = TeamFoundationEventId.ServicingError;
    }

    public InvalidServicingStepTypeException(
      string servicingOperation,
      IStepPerformer performer,
      string stepType)
      : base(FrameworkResources.InvalidServicingStepTypeException((object) stepType, (object) performer.Name, (object) servicingOperation))
    {
      this.EventId = TeamFoundationEventId.ServicingError;
    }
  }
}
