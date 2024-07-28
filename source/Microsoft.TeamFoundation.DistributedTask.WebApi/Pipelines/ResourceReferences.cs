// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.ResourceReferences
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  public class ResourceReferences
  {
    [DataMember(Name = "Repositories", EmitDefaultValue = false)]
    private HashSet<string> m_repositories;
    [DataMember(Name = "Queues", EmitDefaultValue = false)]
    private HashSet<string> m_queues;

    public ISet<string> Repositories
    {
      get
      {
        if (this.m_repositories == null)
          this.m_repositories = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return (ISet<string>) this.m_repositories;
      }
    }

    public ISet<string> Queues
    {
      get
      {
        if (this.m_queues == null)
          this.m_queues = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return (ISet<string>) this.m_queues;
      }
    }
  }
}
