// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ChangeList
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
  [KnownType(typeof (TfsChangeList))]
  [KnownType(typeof (GitCommit))]
  public class ChangeList : Entity
  {
    [DataMember(Name = "version", EmitDefaultValue = false)]
    public string Version { get; set; }

    [DataMember(Name = "comment", EmitDefaultValue = false)]
    public string Comment { get; set; }

    [DataMember(Name = "commentTruncated", EmitDefaultValue = false)]
    public bool CommentTruncated { get; set; }

    [DataMember(Name = "notes", EmitDefaultValue = false)]
    public CheckinNote[] Notes { get; set; }

    [DataMember(Name = "ownerDisplayName", EmitDefaultValue = false)]
    public string OwnerDisplayName { get; set; }

    [DataMember(Name = "owner", EmitDefaultValue = false)]
    public string Owner { get; set; }

    [DataMember(Name = "ownerId", EmitDefaultValue = false)]
    public Guid OwnerId { get; set; }

    [DataMember(Name = "creationDate", EmitDefaultValue = false)]
    public DateTime CreationDate { get; set; }

    [DataMember(Name = "sortDate", EmitDefaultValue = false)]
    public DateTime SortDate { get; set; }

    [DataMember(Name = "allChangesIncluded", EmitDefaultValue = false)]
    public bool AllChangesIncluded { get; set; }

    [DataMember(Name = "changeCounts", EmitDefaultValue = false)]
    public Dictionary<VersionControlChangeType, int> ChangeCounts { get; set; }

    [DataMember(Name = "changes", EmitDefaultValue = false)]
    public IEnumerable<Change> Changes { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      IEnumerable<Change> changes = this.Changes;
      if (changes == null)
        return;
      changes.SetSecuredObject<Change>(securedObject);
    }
  }
}
