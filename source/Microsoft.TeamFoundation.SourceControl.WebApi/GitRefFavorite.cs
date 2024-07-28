// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitRefFavorite
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class GitRefFavorite : VersionControlSecuredObject
  {
    public GitRefFavorite()
    {
    }

    public GitRefFavorite(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public Guid RepositoryId { get; set; }

    [DataMember]
    public Guid IdentityId { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public GitRefFavorite.RefFavoriteType Type { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "_links")]
    public ReferenceLinks Links { get; set; }

    public enum RefFavoriteType
    {
      Invalid,
      Folder,
      Ref,
    }
  }
}
