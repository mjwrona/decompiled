// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.ItemModel
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  [KnownType(typeof (TfvcItem))]
  [KnownType(typeof (TfvcItemPreviousHash))]
  [KnownType(typeof (GitItem))]
  public abstract class ItemModel : VersionControlSecuredObject
  {
    [DataMember]
    public string Path { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsFolder { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Content { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public FileContentMetadata ContentMetadata { get; set; }

    [DataMember(Name = "isSymLink", EmitDefaultValue = false)]
    public bool IsSymbolicLink { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "_links")]
    public ReferenceLinks Links { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      this.ContentMetadata?.SetSecuredObject(securedObject);
    }
  }
}
