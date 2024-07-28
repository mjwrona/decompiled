// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionDeployStep
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  public class ReleaseDefinitionDeployStep : ReleaseDefinitionEnvironmentStep
  {
    public ReleaseDefinitionDeployStep() => this.Tasks = new List<WorkflowTask>();

    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "XML serializer cannot serialize collections/interfaces")]
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [DataMember(EmitDefaultValue = false)]
    public List<WorkflowTask> Tasks { get; set; }

    public override bool Equals(object obj) => obj is ReleaseDefinitionDeployStep definitionDeployStep && this.Id == definitionDeployStep.Id;

    public override int GetHashCode() => this.Id.GetHashCode();

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      this.Tasks?.ForEach((Action<WorkflowTask>) (i => i.SetSecuredObject(token, requiredPermissions)));
    }
  }
}
