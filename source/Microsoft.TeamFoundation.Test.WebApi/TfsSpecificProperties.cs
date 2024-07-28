// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Test.WebApi.TfsSpecificProperties
// Assembly: Microsoft.TeamFoundation.Test.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17829F78-DAC0-47C1-AC4C-95D401C54448
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Test.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Test.WebApi
{
  [DataContract]
  public class TfsSpecificProperties
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string PhaseName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int PhaseAttempt { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string StageName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int StageAttempt { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string JobName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int JobAttempt { get; set; }
  }
}
