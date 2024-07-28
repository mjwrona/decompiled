// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.CheckoutOptions
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class CheckoutOptions
  {
    [JsonConstructor]
    public CheckoutOptions()
    {
    }

    private CheckoutOptions(CheckoutOptions optionsToCopy)
    {
      this.Clean = optionsToCopy.Clean;
      this.FetchDepth = optionsToCopy.FetchDepth;
      this.Lfs = optionsToCopy.Lfs;
      this.Submodules = optionsToCopy.Submodules;
      this.PersistCredentials = optionsToCopy.PersistCredentials;
      this.FetchTags = optionsToCopy.FetchTags;
    }

    [DataMember(EmitDefaultValue = false)]
    public string Clean { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string FetchDepth { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Lfs { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Submodules { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string PersistCredentials { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string FetchTags { get; set; }

    public CheckoutOptions Clone() => new CheckoutOptions(this);
  }
}
