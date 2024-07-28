// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.FeatureManagement.ContributedFeatureState
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.FeatureManagement
{
  [DataContract]
  public class ContributedFeatureState
  {
    [DataMember]
    public string FeatureId { get; set; }

    [DataMember]
    public ContributedFeatureSettingScope Scope { get; set; }

    [DataMember]
    public ContributedFeatureEnabledValue State { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Reason { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool Overridden { get; set; }
  }
}
