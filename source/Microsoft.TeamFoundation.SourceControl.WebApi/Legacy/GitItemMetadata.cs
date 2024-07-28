// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitItemMetadata
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi.Legacy
{
  [DataContract]
  public class GitItemMetadata
  {
    [DataMember(Name = "item", EmitDefaultValue = false)]
    public GitItem Item { get; set; }

    [DataMember(Name = "comment", EmitDefaultValue = false)]
    public string Comment { get; set; }

    [DataMember(Name = "owner", EmitDefaultValue = false)]
    public string Owner { get; set; }

    [DataMember(Name = "ownerDisplayName", EmitDefaultValue = false)]
    public string OwnerDisplayName { get; set; }

    [DataMember(Name = "commitId", EmitDefaultValue = false)]
    public GitObjectId CommitId { get; set; }
  }
}
