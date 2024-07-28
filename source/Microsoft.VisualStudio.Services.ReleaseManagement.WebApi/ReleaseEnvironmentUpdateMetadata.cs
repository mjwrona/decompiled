// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironmentUpdateMetadata
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  public class ReleaseEnvironmentUpdateMetadata
  {
    private IDictionary<string, ConfigurationVariableValue> variables;

    [DataMember]
    public EnvironmentStatus Status { get; set; }

    [DataMember]
    public DateTime? ScheduledDeploymentTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Comment { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, ConfigurationVariableValue> Variables
    {
      get
      {
        if (this.variables == null)
          this.variables = (IDictionary<string, ConfigurationVariableValue>) new Dictionary<string, ConfigurationVariableValue>();
        return this.variables;
      }
      internal set => this.variables = value;
    }

    public bool IsApprovalScheduledDeploymentUpdate() => this.Status == EnvironmentStatus.Undefined;
  }
}
