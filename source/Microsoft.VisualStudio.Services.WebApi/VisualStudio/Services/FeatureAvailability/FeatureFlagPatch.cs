// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.FeatureAvailability.FeatureFlagPatch
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.FeatureAvailability
{
  [DataContract]
  public class FeatureFlagPatch
  {
    public FeatureFlagPatch(string state) => this.State = state;

    [DataMember]
    public string State { get; private set; }
  }
}
