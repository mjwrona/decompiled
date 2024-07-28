// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Security.AceExtendedInformation
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Security
{
  [DataContract]
  public sealed class AceExtendedInformation
  {
    public AceExtendedInformation()
    {
    }

    public AceExtendedInformation(
      int inheritedAllow,
      int inheritedDeny,
      int effectiveAllow,
      int effectiveDeny)
    {
      this.InheritedAllow = inheritedAllow;
      this.InheritedDeny = inheritedDeny;
      this.EffectiveAllow = effectiveAllow;
      this.EffectiveDeny = effectiveDeny;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int InheritedAllow { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int InheritedDeny { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int EffectiveAllow { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int EffectiveDeny { get; set; }
  }
}
