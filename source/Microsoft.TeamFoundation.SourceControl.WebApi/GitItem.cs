// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitItem
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  [KnownType(typeof (GitItemsCollection))]
  public class GitItem : ItemModel
  {
    private const int S_IFMT = 61440;
    private const int S_IFDIR = 16384;
    private const int S_IFLNK = 40960;
    private const int S_755 = 493;

    public GitItem()
    {
    }

    public GitItem(
      string itemPath,
      string objectId,
      GitObjectType objectType,
      string commitId,
      int mode)
    {
      this.ObjectId = objectId;
      this.Path = "/" + itemPath.Trim('/');
      this.IsFolder = objectType == GitObjectType.Tree;
      this.GitObjectType = objectType;
      this.IsSymbolicLink = (mode & 61440) == 40960;
      this.IsLinuxExecutable = (mode & 493) == 493;
      this.CommitId = commitId;
    }

    [DataMember(EmitDefaultValue = false)]
    public string ObjectId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string OriginalObjectId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public GitObjectType GitObjectType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string CommitId { get; set; }

    [IgnoreDataMember]
    public bool IsLinuxExecutable { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public GitCommitRef LatestProcessedChange { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      this.LatestProcessedChange?.SetSecuredObject(securedObject);
    }
  }
}
