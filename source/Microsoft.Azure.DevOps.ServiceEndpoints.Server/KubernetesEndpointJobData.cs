// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.KubernetesEndpointJobData
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Kubernetes;
using Microsoft.VisualStudio.Services.Identity;
using System;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server
{
  public class KubernetesEndpointJobData
  {
    public KubernetesEndpointOperation OperationType { get; set; }

    public IdentityDescriptor UserContext { get; set; }

    public Guid ScopeIdentifier { get; set; }

    public Guid EndpointId { get; set; }
  }
}
