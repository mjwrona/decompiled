// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeploymentJob
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  public class DeploymentJob : ReleaseManagementSecuredObject
  {
    [DataMember(EmitDefaultValue = false)]
    public ReleaseTask Job { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "XML serializer cannot serialize collections/interfaces")]
    [DataMember(EmitDefaultValue = false)]
    public IList<ReleaseTask> Tasks { get; set; }

    public DeploymentJob() => this.Tasks = (IList<ReleaseTask>) new List<ReleaseTask>();

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      this.Job?.SetSecuredObject(token, requiredPermissions);
      IList<ReleaseTask> tasks = this.Tasks;
      if (tasks == null)
        return;
      tasks.ForEach<ReleaseTask>((Action<ReleaseTask>) (i => i?.SetSecuredObject(token, requiredPermissions)));
    }
  }
}
