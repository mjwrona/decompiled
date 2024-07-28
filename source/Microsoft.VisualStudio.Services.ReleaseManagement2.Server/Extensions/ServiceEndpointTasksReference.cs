// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ServiceEndpointTasksReference
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions
{
  public class ServiceEndpointTasksReference
  {
    private IList<string> tasksUsingEndpoint;

    public Guid EndpointId { get; set; }

    public string EndpointName { get; set; }

    public string EndpointType { get; set; }

    public string EndpointReference { get; set; }

    public IList<string> TasksUsingEndpoint
    {
      get
      {
        if (this.tasksUsingEndpoint == null)
          this.tasksUsingEndpoint = (IList<string>) new List<string>();
        return this.tasksUsingEndpoint;
      }
      internal set => this.tasksUsingEndpoint = value;
    }
  }
}
