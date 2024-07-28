// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.CommonStructureSubsystemServiceException
// Assembly: Microsoft.Azure.Boards.CssNodes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D887A041-2C68-42E5-BA83-E261159AB40A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.CssNodes.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server
{
  [Serializable]
  public class CommonStructureSubsystemServiceException : TeamFoundationServiceException
  {
    public CommonStructureSubsystemServiceException() => this.EventId = TeamFoundationEventId.CommonStructureSubsystemServiceException;

    public CommonStructureSubsystemServiceException(string message)
      : base(message)
    {
      this.EventId = TeamFoundationEventId.CommonStructureSubsystemServiceException;
    }

    public CommonStructureSubsystemServiceException(string message, Exception innerException)
      : base(message, innerException)
    {
      this.EventId = TeamFoundationEventId.CommonStructureSubsystemServiceException;
    }

    protected CommonStructureSubsystemServiceException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
      this.EventId = TeamFoundationEventId.CommonStructureSubsystemServiceException;
    }
  }
}
