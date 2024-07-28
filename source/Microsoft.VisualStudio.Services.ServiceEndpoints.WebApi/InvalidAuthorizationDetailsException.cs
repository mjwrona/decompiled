// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.InvalidAuthorizationDetailsException
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  [ExceptionMapping("0.0", "3.0", "InvalidAuthorizationDetailsException", "Microsoft.TeamFoundation.DistributedTask.WebApi.InvalidAuthorizationDetailsException, Microsoft.TeamFoundation.DistributedTask.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [ExceptionMapping("3.0", "5.0", "InvalidAuthorizationDetailsException", "Microsoft.TeamFoundation.DistributedTask.WebApi.InvalidAuthorizationDetailsException, Microsoft.TeamFoundation.DistributedTask.WebApi, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class InvalidAuthorizationDetailsException : ServiceEndpointExceptionType
  {
    public InvalidAuthorizationDetailsException(string message)
      : base(message)
    {
    }

    public InvalidAuthorizationDetailsException(string message, Exception ex)
      : base(message, ex)
    {
    }

    protected InvalidAuthorizationDetailsException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
