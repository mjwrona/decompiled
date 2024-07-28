// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.PipelineArtifactTypeNotSupportedException
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server
{
  [Serializable]
  public class PipelineArtifactTypeNotSupportedException : Exception
  {
    public PipelineArtifactTypeNotSupportedException()
    {
    }

    public PipelineArtifactTypeNotSupportedException(string message)
      : base(message)
    {
    }

    public PipelineArtifactTypeNotSupportedException(string message, Exception ex)
      : base(message, ex)
    {
    }

    protected PipelineArtifactTypeNotSupportedException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }
  }
}
