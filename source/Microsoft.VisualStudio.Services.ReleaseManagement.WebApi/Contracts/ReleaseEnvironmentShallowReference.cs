// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseEnvironmentShallowReference
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  public class ReleaseEnvironmentShallowReference : ShallowReference
  {
    [DataMember]
    public new int Id
    {
      get => base.Id;
      set => base.Id = value;
    }

    [DataMember(EmitDefaultValue = false)]
    public new string Name
    {
      get => base.Name;
      set => base.Name = value;
    }

    [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "The name Url is appropariate name for this property and it signifies REST URL of resource")]
    [DataMember(EmitDefaultValue = false)]
    public new string Url
    {
      get => base.Url;
      set => base.Url = value;
    }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public new ReferenceLinks Links
    {
      get => base.Links;
      set => base.Links = value;
    }

    public ReleaseEnvironmentShallowReference() => this.Links = new ReferenceLinks();
  }
}
