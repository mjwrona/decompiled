// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models.ArtifactTraceabilityResourceInfo
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models
{
  public class ArtifactTraceabilityResourceInfo
  {
    private IDictionary<string, string> _properties;

    public string Id { get; set; }

    public string Type { get; set; }

    public string Name { get; set; }

    public Guid ProjectId { get; set; }

    public string Branch { get; set; }

    public string Version { get; set; }

    public IDictionary<string, string> Properties
    {
      get
      {
        if (this._properties == null)
          this._properties = (IDictionary<string, string>) new Dictionary<string, string>();
        return this._properties;
      }
    }

    public override string ToString() => string.Format("Id : {0}, Type : {1}, Name : {2}, ProjectId : {3}, Branch : {4}, Version : {5}", (object) this.Id, (object) this.Type, (object) this.Name, (object) this.ProjectId.ToString(), (object) this.Branch, (object) this.Version);
  }
}
