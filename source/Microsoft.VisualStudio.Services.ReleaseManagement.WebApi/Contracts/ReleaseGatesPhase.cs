// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseGatesPhase
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
  public class ReleaseGatesPhase : ReleaseDeployPhase
  {
    [DataMember(EmitDefaultValue = false)]
    public DateTime? StabilizationCompletedOn { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<IgnoredGate> IgnoredGates { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? SucceedingSince { get; set; }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      IList<IgnoredGate> ignoredGates = this.IgnoredGates;
      if (ignoredGates == null)
        return;
      ignoredGates.ForEach<IgnoredGate>((Action<IgnoredGate>) (i => i?.SetSecuredObject(token, requiredPermissions)));
    }
  }
}
