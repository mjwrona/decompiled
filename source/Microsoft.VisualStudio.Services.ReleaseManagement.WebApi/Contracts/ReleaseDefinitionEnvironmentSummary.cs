// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionEnvironmentSummary
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  public class ReleaseDefinitionEnvironmentSummary : ReleaseManagementSecuredObject
  {
    [DataMember]
    public int Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember]
    public IList<ReleaseShallowReference> LastReleases { get; private set; }

    public ReleaseDefinitionEnvironmentSummary() => this.LastReleases = (IList<ReleaseShallowReference>) new List<ReleaseShallowReference>();

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      IList<ReleaseShallowReference> lastReleases = this.LastReleases;
      if (lastReleases == null)
        return;
      lastReleases.ForEach<ReleaseShallowReference>((Action<ReleaseShallowReference>) (i => i.SetSecuredObject(token, requiredPermissions)));
    }
  }
}
