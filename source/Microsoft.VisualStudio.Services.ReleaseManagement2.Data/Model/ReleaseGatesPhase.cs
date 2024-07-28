// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseGatesPhase
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class ReleaseGatesPhase : ReleaseDeployPhase
  {
    public DateTime? StabilizationCompletedOn { get; set; }

    public DateTime? SucceedingSince { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Need it for deserialization")]
    public IList<IgnoredGate> IgnoredGates { get; set; }

    public override ReleaseDeployPhase DeepClone()
    {
      ReleaseGatesPhase clonedObject = (ReleaseGatesPhase) this.MemberwiseClone();
      this.UpdateClonedObjectReferences((ReleaseDeployPhase) clonedObject);
      return (ReleaseDeployPhase) clonedObject;
    }

    protected override void UpdateClonedObjectReferences(ReleaseDeployPhase clonedObject)
    {
      base.UpdateClonedObjectReferences(clonedObject);
      ReleaseGatesPhase clonedGatesPhase = clonedObject as ReleaseGatesPhase;
      if (clonedGatesPhase == null)
        return;
      clonedGatesPhase.IgnoredGates = (IList<IgnoredGate>) null;
      if (this.IgnoredGates == null || !this.IgnoredGates.Any<IgnoredGate>())
        return;
      clonedGatesPhase.IgnoredGates = (IList<IgnoredGate>) new List<IgnoredGate>();
      this.IgnoredGates.ToList<IgnoredGate>().ForEach((Action<IgnoredGate>) (ignoredGate => clonedGatesPhase.IgnoredGates.Add(ignoredGate.DeepClone())));
    }
  }
}
