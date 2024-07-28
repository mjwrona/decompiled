// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi.Legacy
{
  [DataContract]
  [KnownType(typeof (TfsItem))]
  [KnownType(typeof (GitItem))]
  [KnownType(typeof (GitSubmoduleItem))]
  public abstract class ItemModel : Entity
  {
    [DataMember(Name = "serverItem", EmitDefaultValue = false)]
    public string ServerItem { get; set; }

    [DataMember(Name = "version", EmitDefaultValue = false)]
    public string VersionString { get; set; }

    [DataMember(Name = "isFolder", EmitDefaultValue = false)]
    public bool IsFolder { get; set; }

    [DataMember(Name = "changeDate", EmitDefaultValue = false)]
    public DateTime ChangeDate { get; set; }

    [DataMember(Name = "childItems", EmitDefaultValue = false)]
    public IList<ItemModel> ChildItems { get; set; }

    [DataMember(Name = "contentMetadata", EmitDefaultValue = false)]
    public FileContentMetadata ContentMetadata { get; set; }

    [DataMember(Name = "versionDescription", EmitDefaultValue = false)]
    public string VersionDescription { get; set; }

    [DataMember(Name = "isSymLink", EmitDefaultValue = false)]
    public bool IsSymbolicLink { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      this.ContentMetadata?.SetSecuredObject(securedObject);
      if (this.ChildItems == null)
        return;
      foreach (VersionControlSecuredObject childItem in (IEnumerable<ItemModel>) this.ChildItems)
        childItem.SetSecuredObject(securedObject);
    }
  }
}
