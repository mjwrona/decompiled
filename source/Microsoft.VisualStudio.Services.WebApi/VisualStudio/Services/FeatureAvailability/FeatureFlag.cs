// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.FeatureAvailability.FeatureFlag
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.FeatureAvailability
{
  [DataContract]
  public class FeatureFlag
  {
    public FeatureFlag(
      string name,
      string description,
      string uri,
      string effectiveState,
      string explicitState)
    {
      this.EffectiveState = effectiveState;
      this.Uri = uri;
      this.Name = name;
      this.Description = description;
      this.ExplicitState = explicitState;
    }

    public FeatureFlag()
    {
    }

    [DataMember]
    public string Name { get; private set; }

    [DataMember]
    public string Description { get; private set; }

    [DataMember]
    public string Uri { get; private set; }

    [DataMember]
    public string EffectiveState { get; private set; }

    [DataMember]
    public string ExplicitState { get; private set; }
  }
}
