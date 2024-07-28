// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.Client.Policy
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Organization.Client
{
  [DataContract]
  [ClientIncludeModel]
  public sealed class Policy
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public object Value { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public object EffectiveValue { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool Enforce { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool IsValueUndefined { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Policy ParentPolicy { get; set; }
  }
}
