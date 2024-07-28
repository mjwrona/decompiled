// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitSubmoduleItem
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi.Legacy
{
  [DataContract]
  public class GitSubmoduleItem : GitItem
  {
    public GitSubmoduleItem(GitItem item)
      : base(item.ObjectId)
    {
      this.ServerItem = item.ServerItem;
      this.IsFolder = item.IsFolder;
      this.GitObjectType = item.GitObjectType;
      this.IsSymbolicLink = item.IsSymbolicLink;
      this.VersionString = item.VersionString;
      this.Url = item.Url;
    }

    [DataMember(Name = "name", EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(Name = "newObjectId", EmitDefaultValue = false)]
    public GitObjectId NewObjectId { get; set; }

    [DataMember(Name = "oldObjectId", EmitDefaultValue = false)]
    public GitObjectId OldObjectId { get; set; }

    [DataMember(Name = "repositoryUrl", EmitDefaultValue = false)]
    public string RepositoryUrl { get; set; }
  }
}
