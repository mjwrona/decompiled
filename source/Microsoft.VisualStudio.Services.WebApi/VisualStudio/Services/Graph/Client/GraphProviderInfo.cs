// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.Client.GraphProviderInfo
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Graph.Client
{
  [DataContract]
  public class GraphProviderInfo
  {
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string Domain { get; private set; }

    [DataMember]
    public string Origin { get; private set; }

    [DataMember]
    public string OriginId { get; private set; }

    public SubjectDescriptor Descriptor { get; private set; }

    [DataMember(Name = "Descriptor", EmitDefaultValue = false)]
    [JsonProperty(PropertyName = "Descriptor", DefaultValueHandling = DefaultValueHandling.Ignore)]
    private string DescriptorString
    {
      get => this.Descriptor.ToString();
      set => this.Descriptor = SubjectDescriptor.FromString(value);
    }

    internal GraphProviderInfo(
      string origin,
      string originId,
      SubjectDescriptor descriptor,
      string domain)
    {
      this.Origin = origin;
      this.OriginId = originId;
      this.Descriptor = descriptor;
      this.Domain = domain;
    }

    protected GraphProviderInfo()
    {
    }
  }
}
