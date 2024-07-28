// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.InvalidDatasourceException
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  [ExceptionMapping("3.0", "5.0", "InvalidDatasourceException", "Microsoft.TeamFoundation.DistributedTask.WebApi.InvalidDatasourceException, Microsoft.TeamFoundation.DistributedTask.WebApi, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class InvalidDatasourceException : ServiceEndpointExceptionType
  {
    public InvalidDatasourceException(string message)
      : base(message)
    {
    }

    public InvalidDatasourceException(string message, Exception ex)
      : base(message, ex)
    {
    }

    protected InvalidDatasourceException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
