// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.OAuthConfigurationExistsException
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  [ExceptionMapping("3.0", "5.0", "OAuthConfigurationExistsException", "Microsoft.TeamFoundation.DistributedTask.WebApi.OAuthConfigurationExistsException, Microsoft.TeamFoundation.DistributedTask.WebApi, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public sealed class OAuthConfigurationExistsException : ServiceEndpointExceptionType
  {
    public OAuthConfigurationExistsException(string message)
      : base(message)
    {
    }

    public OAuthConfigurationExistsException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    private OAuthConfigurationExistsException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
