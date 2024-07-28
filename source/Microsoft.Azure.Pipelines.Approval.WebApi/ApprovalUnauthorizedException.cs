// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.WebApi.ApprovalUnauthorizedException
// Assembly: Microsoft.Azure.Pipelines.Approval.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CD73FA2-1519-4C73-96D3-90013EEE8275
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Approval.WebApi
{
  [Serializable]
  public sealed class ApprovalUnauthorizedException : VssServiceException
  {
    public ApprovalUnauthorizedException(string message)
      : base(message)
    {
    }

    public ApprovalUnauthorizedException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    private ApprovalUnauthorizedException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
