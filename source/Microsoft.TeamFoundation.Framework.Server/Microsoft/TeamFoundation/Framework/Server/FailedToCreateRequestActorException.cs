// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FailedToCreateRequestActorException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class FailedToCreateRequestActorException : VssServiceException
  {
    public FailedToCreateRequestActorException()
    {
    }

    public FailedToCreateRequestActorException(string message)
      : base(message)
    {
    }

    public FailedToCreateRequestActorException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
