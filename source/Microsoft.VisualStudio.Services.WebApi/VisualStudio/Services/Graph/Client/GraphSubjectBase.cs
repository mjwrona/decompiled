// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.Client.GraphSubjectBase
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Xml;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Graph.Client
{
  [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
  [XmlSerializableDataContract]
  public abstract class GraphSubjectBase : IXmlSerializable
  {
    public SubjectDescriptor Descriptor { get; protected set; }

    [DataMember(Name = "Descriptor", EmitDefaultValue = false)]
    [JsonProperty(PropertyName = "Descriptor", DefaultValueHandling = DefaultValueHandling.Ignore)]
    private string DescriptorString
    {
      get => this.Descriptor.ToString();
      set => this.Descriptor = SubjectDescriptor.FromString(value);
    }

    [DataMember]
    [JsonProperty]
    public string DisplayName { get; protected set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    [JsonProperty(PropertyName = "_links", DefaultValueHandling = DefaultValueHandling.Ignore)]
    [XmlIgnore]
    public ReferenceLinks Links { get; protected set; }

    [DataMember(EmitDefaultValue = false)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Url { get; protected set; }

    protected GraphSubjectBase()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected GraphSubjectBase(
      SubjectDescriptor descriptor,
      string displayName,
      ReferenceLinks links,
      string url)
    {
      this.Descriptor = descriptor;
      this.DisplayName = displayName;
      this.Links = links;
      this.Url = url;
    }

    XmlSchema IXmlSerializable.GetSchema() => (XmlSchema) null;

    void IXmlSerializable.ReadXml(XmlReader reader) => reader.ReadDataMemberXml<GraphSubjectBase>(this);

    void IXmlSerializable.WriteXml(XmlWriter writer) => writer.WriteDataMemberXml((object) this);
  }
}
