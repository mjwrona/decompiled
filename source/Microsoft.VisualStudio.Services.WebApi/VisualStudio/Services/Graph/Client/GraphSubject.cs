// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.Client.GraphSubject
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Graph.Client
{
  [DataContract]
  [JsonConverter(typeof (GraphSubjectJsonConverter))]
  public abstract class GraphSubject : GraphSubjectBase
  {
    [ClientInternalUseOnly(true)]
    internal bool ShoudSerializeInternals;

    [DataMember]
    public abstract string SubjectKind { get; }

    [DataMember]
    public string Origin { get; private set; }

    [DataMember]
    public string OriginId { get; private set; }

    [ClientInternalUseOnly(true)]
    internal IdentityDescriptor LegacyDescriptor { get; private set; }

    [DataMember(Name = "LegacyDescriptor", IsRequired = false, EmitDefaultValue = false)]
    private string LegacyDescriptorString
    {
      get => this.LegacyDescriptor?.ToString();
      set => this.LegacyDescriptor = IdentityDescriptor.FromString(value);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual bool ShouldSerializeLegacyDescriptorString() => this.ShoudSerializeInternals;

    protected GraphSubject()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected GraphSubject(
      string origin,
      string originId,
      SubjectDescriptor descriptor,
      IdentityDescriptor legacyDescriptor,
      string displayName,
      ReferenceLinks links,
      string url)
      : base(descriptor, displayName, links, url)
    {
      this.Origin = origin;
      this.OriginId = originId;
      this.LegacyDescriptor = legacyDescriptor;
      this.ShoudSerializeInternals = false;
    }
  }
}
