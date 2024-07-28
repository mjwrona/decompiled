// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionSummary
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  public class ReleaseDefinitionSummary : ReleaseManagementSecuredObject
  {
    [Obsolete("Use ReleaseDefinitionReference instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ShallowReference ReleaseDefinition
    {
      get => (ShallowReference) this.ReleaseDefinitionReference;
      set => this.ReleaseDefinitionReference = value.ToReleaseDefinitionShallowReference();
    }

    [DataMember(Name = "ReleaseDefinition")]
    public ReleaseDefinitionShallowReference ReleaseDefinitionReference { get; set; }

    [DataMember]
    public IList<ReleaseDefinitionEnvironmentSummary> Environments { get; private set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "This can be set to null explicitly if no releases present")]
    [DataMember(EmitDefaultValue = false)]
    public IList<Release> Releases { get; set; }

    public ReleaseDefinitionSummary() => this.Environments = (IList<ReleaseDefinitionEnvironmentSummary>) new List<ReleaseDefinitionEnvironmentSummary>();

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      this.ReleaseDefinitionReference?.SetSecuredObject(token, requiredPermissions);
      IList<Release> releases = this.Releases;
      if (releases != null)
        releases.ForEach<Release>((Action<Release>) (i => i.SetSecuredObject(token, requiredPermissions)));
      IList<ReleaseDefinitionEnvironmentSummary> environments = this.Environments;
      if (environments == null)
        return;
      environments.ForEach<ReleaseDefinitionEnvironmentSummary>((Action<ReleaseDefinitionEnvironmentSummary>) (i => i.SetSecuredObject(token, requiredPermissions)));
    }
  }
}
