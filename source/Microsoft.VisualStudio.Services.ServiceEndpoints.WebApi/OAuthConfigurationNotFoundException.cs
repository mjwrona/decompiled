// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.OAuthConfigurationNotFoundException
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  [ExceptionMapping("3.0", "5.0", "OAuthConfigurationNotFoundException", "Microsoft.TeamFoundation.DistributedTask.WebApi.OAuthConfigurationNotFoundException, Microsoft.TeamFoundation.DistributedTask.WebApi, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public sealed class OAuthConfigurationNotFoundException : ServiceEndpointExceptionType
  {
    public OAuthConfigurationNotFoundException(string message)
      : base(message)
    {
    }

    public OAuthConfigurationNotFoundException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    private OAuthConfigurationNotFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
