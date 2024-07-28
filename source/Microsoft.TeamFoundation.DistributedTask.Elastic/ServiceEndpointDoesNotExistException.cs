// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.ServiceEndpointDoesNotExistException
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using System;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic
{
  internal class ServiceEndpointDoesNotExistException : ElasticPoolException
  {
    [Obsolete("All exceptions are supposed to have an empty constructor, but they're useless. Use one of the other constructors.")]
    public ServiceEndpointDoesNotExistException()
    {
    }

    public ServiceEndpointDoesNotExistException(string message)
      : base(message)
    {
    }

    public ServiceEndpointDoesNotExistException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public ServiceEndpointDoesNotExistException(Guid serviceEndpointId, Guid serviceEndpointScope)
      : base(string.Format("No Service Endpoint found with Id {0} and Scope {1}", (object) serviceEndpointId, (object) serviceEndpointScope))
    {
    }
  }
}
