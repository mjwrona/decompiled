// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Traceability.Server.BaseArtifactSourceVersionNullException
// Assembly: Microsoft.TeamFoundation.Traceability.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C62AF110-A283-470F-B32B-FE03F2A1E0D2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Traceability.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Traceability.Server
{
  [Serializable]
  public sealed class BaseArtifactSourceVersionNullException : VssServiceException
  {
    public BaseArtifactSourceVersionNullException(string message)
      : base(message)
    {
    }

    public BaseArtifactSourceVersionNullException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    private BaseArtifactSourceVersionNullException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
