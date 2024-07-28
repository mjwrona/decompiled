// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointNotFoundException
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  [ExceptionMapping("0.0", "3.0", "ServiceEndpointNotFoundException", "Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointNotFoundException, Microsoft.TeamFoundation.DistributedTask.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [ExceptionMapping("3.0", "5.0", "ServiceEndpointNotFoundException", "Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointNotFoundException, Microsoft.TeamFoundation.DistributedTask.WebApi, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class ServiceEndpointNotFoundException : ServiceEndpointExceptionType
  {
    public ServiceEndpointNotFoundException(string message)
      : base(message)
    {
    }

    public ServiceEndpointNotFoundException(string message, Exception ex)
      : base(message, ex)
    {
    }

    protected ServiceEndpointNotFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
