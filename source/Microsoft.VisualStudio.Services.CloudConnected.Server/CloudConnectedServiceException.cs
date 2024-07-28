// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CloudConnected.CloudConnectedServiceException
// Assembly: Microsoft.VisualStudio.Services.CloudConnected.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6AAFC756-39E6-4247-9102-7DC33B035E4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CloudConnected.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Net;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.CloudConnected
{
  [Serializable]
  public class CloudConnectedServiceException : TeamFoundationServiceException
  {
    public CloudConnectedServiceException(string message)
      : base(message)
    {
    }

    public CloudConnectedServiceException(string message, HttpStatusCode httpStatusCode)
      : base(message, httpStatusCode)
    {
    }

    public CloudConnectedServiceException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
