// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.AadMemberAccessStatus.AadMemberStatusException
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.AadMemberAccessStatus
{
  [Serializable]
  public abstract class AadMemberStatusException : VssServiceException
  {
    public AadMemberStatusException() => this.EventId = TeamFoundationEventId.AadMemberStatusException;

    public AadMemberStatusException(string message)
      : base(message)
    {
      this.EventId = TeamFoundationEventId.AadMemberStatusException;
    }

    public AadMemberStatusException(string message, Exception ex)
      : base(message, ex)
    {
      this.EventId = TeamFoundationEventId.AadMemberStatusException;
    }

    protected AadMemberStatusException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.EventId = TeamFoundationEventId.AadMemberStatusException;
    }
  }
}
