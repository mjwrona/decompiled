// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.EnabledFeature
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  [ExcludeFromCodeCoverage]
  [Entity("enabledFeatures", new string[] {"Microsoft.WindowsAzure.ActiveDirectory.EnabledFeature", "Microsoft.DirectoryServices.EnabledFeature"})]
  [JsonObject(MemberSerialization.OptIn)]
  public class EnabledFeature : GraphObject
  {
    private string _featureId;
    private string _featureName;

    [JsonProperty("featureId")]
    [Key(true)]
    public string FeatureId
    {
      get => this._featureId;
      set
      {
        this._featureId = value;
        this.ChangedProperties.Add(nameof (FeatureId));
      }
    }

    [JsonProperty("featureName")]
    public string FeatureName
    {
      get => this._featureName;
      set
      {
        this._featureName = value;
        this.ChangedProperties.Add(nameof (FeatureName));
      }
    }
  }
}
