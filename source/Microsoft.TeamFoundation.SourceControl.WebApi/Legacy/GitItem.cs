// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitItem
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi.Legacy
{
  [DataContract]
  public class GitItem : ItemModel
  {
    private const int S_IFMT = 61440;
    private const int S_IFDIR = 16384;
    private const int S_IFLNK = 40960;

    public GitItem()
    {
    }

    public GitItem(byte[] objectId)
    {
      if (objectId == null)
        return;
      this.ObjectId = new GitObjectId(objectId);
    }

    public GitItem(GitObjectId objectId)
    {
      if (objectId == null)
        return;
      this.ObjectId = objectId;
    }

    public GitItem(
      string itemPath,
      byte[] objectId,
      GitObjectType objectType,
      string version,
      int mode)
      : this(objectId)
    {
      this.ServerItem = "/" + itemPath.Trim('/');
      this.IsFolder = objectType == GitObjectType.Tree;
      this.GitObjectType = objectType;
      this.IsSymbolicLink = (mode & 61440) == 40960;
      this.VersionString = version;
    }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      this.ObjectId?.SetSecuredObject(securedObject);
      this.ContentMetadata?.SetSecuredObject(securedObject);
    }

    [DataMember(Name = "objectId", EmitDefaultValue = false)]
    public GitObjectId ObjectId { get; private set; }

    [DataMember(Name = "gitObjectType", EmitDefaultValue = false)]
    public GitObjectType GitObjectType { get; set; }

    [DataMember(Name = "commitId", EmitDefaultValue = true)]
    public GitObjectId CommitId { get; set; }
  }
}
