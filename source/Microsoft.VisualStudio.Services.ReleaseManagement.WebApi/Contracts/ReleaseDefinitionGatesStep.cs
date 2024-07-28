// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionGatesStep
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  public class ReleaseDefinitionGatesStep : ReleaseManagementSecuredObject
  {
    public ReleaseDefinitionGatesStep() => this.Gates = (IList<ReleaseDefinitionGate>) new List<ReleaseDefinitionGate>();

    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public ReleaseDefinitionGatesOptions GatesOptions { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<ReleaseDefinitionGate> Gates { get; set; }

    public override int GetHashCode() => this.Id.GetHashCode();

    public override bool Equals(object obj) => obj is ReleaseDefinitionGatesStep definitionGatesStep && this.Id == definitionGatesStep.Id && ReleaseDefinitionGatesStep.AreReleaseDefintionGatesStepsEqual(this.Gates, definitionGatesStep.Gates) && ReleaseDefinitionGatesStep.AreReleaseDefintionGatesOptionsEqual(this.GatesOptions, definitionGatesStep.GatesOptions);

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      this.GatesOptions?.SetSecuredObject(token, requiredPermissions);
      IList<ReleaseDefinitionGate> gates = this.Gates;
      if (gates == null)
        return;
      gates.ForEach<ReleaseDefinitionGate>((Action<ReleaseDefinitionGate>) (i => i?.SetSecuredObject(token, requiredPermissions)));
    }

    private static bool AreReleaseDefintionGatesStepsEqual(
      IList<ReleaseDefinitionGate> existingGateSteps,
      IList<ReleaseDefinitionGate> newGateSteps)
    {
      if (existingGateSteps == null && newGateSteps == null)
        return true;
      return existingGateSteps != null && newGateSteps != null && existingGateSteps.Count == newGateSteps.Count && !existingGateSteps.Where<ReleaseDefinitionGate>((Func<ReleaseDefinitionGate, int, bool>) ((t, i) => !t.Equals((object) newGateSteps[i]))).Any<ReleaseDefinitionGate>();
    }

    private static bool AreReleaseDefintionGatesOptionsEqual(
      ReleaseDefinitionGatesOptions existingGatesOptions,
      ReleaseDefinitionGatesOptions newGatesOptions)
    {
      if (existingGatesOptions == null && newGatesOptions == null)
        return true;
      return existingGatesOptions != null && newGatesOptions != null && existingGatesOptions.Equals((object) newGatesOptions);
    }
  }
}
