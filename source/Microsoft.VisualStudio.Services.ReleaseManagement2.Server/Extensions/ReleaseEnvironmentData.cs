// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ReleaseEnvironmentData
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions
{
  public class ReleaseEnvironmentData
  {
    private IList<DeployPhase> deployPhases;
    private IDictionary<string, string> variables;
    private IList<ServiceEndpointTasksReference> serviceEndpointTasksReferences;

    public int EnvironmentId { get; set; }

    public string EnvironmentName { get; set; }

    public IList<DeployPhase> DeployPhases
    {
      get
      {
        if (this.deployPhases == null)
          this.deployPhases = (IList<DeployPhase>) new List<DeployPhase>();
        return this.deployPhases;
      }
      internal set => this.deployPhases = value;
    }

    public IDictionary<string, string> Variables
    {
      get
      {
        if (this.variables == null)
          this.variables = (IDictionary<string, string>) new Dictionary<string, string>();
        return this.variables;
      }
      internal set => this.variables = value;
    }

    public IList<ServiceEndpointTasksReference> ServiceEndpointTasksReferences
    {
      get
      {
        if (this.serviceEndpointTasksReferences == null)
          this.serviceEndpointTasksReferences = (IList<ServiceEndpointTasksReference>) new List<ServiceEndpointTasksReference>();
        return this.serviceEndpointTasksReferences;
      }
      internal set => this.serviceEndpointTasksReferences = value;
    }
  }
}
