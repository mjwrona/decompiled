// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.AzureStorageAccessKeyFetchFailedException
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  [Serializable]
  public class AzureStorageAccessKeyFetchFailedException : TeamFoundationServiceException
  {
    public AzureStorageAccessKeyFetchFailedException(string message)
      : base(message)
    {
    }

    public AzureStorageAccessKeyFetchFailedException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected AzureStorageAccessKeyFetchFailedException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }
  }
}
