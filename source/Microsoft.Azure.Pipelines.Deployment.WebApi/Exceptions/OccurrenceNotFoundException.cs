// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.WebApi.Exceptions.OccurrenceNotFoundException
// Assembly: Microsoft.Azure.Pipelines.Deployment.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8505F8FB-8448-4469-A2DD-E74F64B77053
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.Azure.Pipelines.Deployment.WebApi.Exceptions
{
  [ClientInternalUseOnly(true)]
  [Serializable]
  public class OccurrenceNotFoundException : ArtifactMetadataException
  {
    public OccurrenceNotFoundException(string message)
      : base(message)
    {
    }

    public OccurrenceNotFoundException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
