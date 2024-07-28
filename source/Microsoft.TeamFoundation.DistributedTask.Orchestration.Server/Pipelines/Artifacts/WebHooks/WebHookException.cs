// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.WebHooks.WebHookException
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.WebHooks
{
  [Serializable]
  public class WebHookException : TeamFoundationServiceException
  {
    public WebHookException()
    {
    }

    public WebHookException(string message)
      : base(message)
    {
    }

    public WebHookException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected WebHookException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
