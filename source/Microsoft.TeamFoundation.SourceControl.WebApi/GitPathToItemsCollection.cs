// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitPathToItemsCollection
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class GitPathToItemsCollection : VersionControlSecuredObject
  {
    public GitPathToItemsCollection() => this.Items = (Dictionary<string, List<GitItem>>) new SecuredDictionary<string, List<GitItem>>();

    public GitPathToItemsCollection(ISecuredObject securedObject)
      : base(securedObject)
    {
      this.Items = (Dictionary<string, List<GitItem>>) new SecuredDictionary<string, List<GitItem>>(securedObject);
    }

    [DataMember(Name = "items", EmitDefaultValue = false)]
    public Dictionary<string, List<GitItem>> Items { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      if (this.Items == null)
        return;
      ((SecuredDictionary<string, List<GitItem>>) this.Items).SetSecuredObject(securedObject);
      foreach (KeyValuePair<string, List<GitItem>> keyValuePair in this.Items)
        keyValuePair.Value.SetSecuredObject<GitItem>(securedObject);
    }
  }
}
