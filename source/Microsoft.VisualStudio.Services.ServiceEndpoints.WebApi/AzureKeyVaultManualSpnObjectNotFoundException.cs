// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AzureKeyVaultManualSpnObjectNotFoundException
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  [Serializable]
  public class AzureKeyVaultManualSpnObjectNotFoundException : ServiceEndpointExceptionType
  {
    public AzureKeyVaultManualSpnObjectNotFoundException(string message)
      : base(message)
    {
    }

    public AzureKeyVaultManualSpnObjectNotFoundException(string message, Exception ex)
      : base(message, ex)
    {
    }

    protected AzureKeyVaultManualSpnObjectNotFoundException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }
  }
}
