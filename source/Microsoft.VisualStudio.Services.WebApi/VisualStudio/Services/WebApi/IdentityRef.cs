// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.IdentityRef
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.WebApi.Xml;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
  [XmlSerializableDataContract(EnableCamelCaseNameCompat = true)]
  public class IdentityRef : GraphSubjectBase, ISecuredObject
  {
    public new SubjectDescriptor Descriptor
    {
      get => base.Descriptor;
      set => base.Descriptor = value;
    }

    public new string DisplayName
    {
      get => base.DisplayName;
      set => base.DisplayName = value;
    }

    public new string Url
    {
      get => base.Url;
      set => base.Url = value;
    }

    public new ReferenceLinks Links
    {
      get => base.Links;
      set => base.Links = value;
    }

    [DataMember(Name = "id")]
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [DataMember(Name = "uniqueName", EmitDefaultValue = false)]
    [JsonProperty(PropertyName = "uniqueName", DefaultValueHandling = DefaultValueHandling.Ignore)]
    [JsonConverter(typeof (DefaultValueOnPublicAccessJsonConverter<string>))]
    public string UniqueName { get; set; }

    [DataMember(Name = "directoryAlias", EmitDefaultValue = false)]
    [JsonProperty(PropertyName = "directoryAlias", DefaultValueHandling = DefaultValueHandling.Ignore)]
    [JsonConverter(typeof (DefaultValueOnPublicAccessJsonConverter<string>))]
    public string DirectoryAlias { get; set; }

    [DataMember(Name = "profileUrl", EmitDefaultValue = false)]
    [JsonProperty(PropertyName = "profileUrl", DefaultValueHandling = DefaultValueHandling.Ignore)]
    [JsonConverter(typeof (DefaultValueOnPublicAccessJsonConverter<string>))]
    public string ProfileUrl { get; set; }

    [DataMember(Name = "imageUrl", EmitDefaultValue = false)]
    [JsonProperty(PropertyName = "imageUrl", DefaultValueHandling = DefaultValueHandling.Ignore)]
    [JsonConverter(typeof (DefaultValueOnPublicAccessJsonConverter<string>))]
    public string ImageUrl { get; set; }

    [DataMember(Name = "isContainer", EmitDefaultValue = false)]
    [JsonProperty(PropertyName = "isContainer", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool IsContainer { get; set; }

    [DataMember(Name = "isAadIdentity", EmitDefaultValue = false)]
    [JsonProperty(PropertyName = "isAadIdentity", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool IsAadIdentity { get; set; }

    [DataMember(Name = "inactive", EmitDefaultValue = false)]
    [JsonProperty(PropertyName = "inactive", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool Inactive { get; set; }

    [DataMember(Name = "isDeletedInOrigin", EmitDefaultValue = false)]
    [JsonProperty(PropertyName = "isDeletedInOrigin", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool IsDeletedInOrigin { get; set; }

    [DataMember(Name = "displayName", EmitDefaultValue = false)]
    [JsonIgnore]
    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string DisplayNameForXmlSerialization
    {
      get => base.DisplayName;
      set => base.DisplayName = value;
    }

    [DataMember(Name = "url", EmitDefaultValue = false)]
    [JsonIgnore]
    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string UrlForXmlSerialization
    {
      get => base.Url;
      set => base.Url = value;
    }

    Guid ISecuredObject.NamespaceId => GraphSecurityConstants.NamespaceId;

    int ISecuredObject.RequiredPermissions => 1;

    string ISecuredObject.GetToken() => GraphSecurityConstants.RefsToken;
  }
}
