// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.TfvcChangeset
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class TfvcChangeset : TfvcChangesetRef
  {
    public TfvcChangeset()
    {
    }

    public TfvcChangeset(TfvcChangesetRef tfvcChangesetRef)
    {
      this.ChangesetId = tfvcChangesetRef.ChangesetId;
      this.Author = tfvcChangesetRef.Author;
      this.CheckedInBy = tfvcChangesetRef.CheckedInBy;
      this.CreatedDate = tfvcChangesetRef.CreatedDate;
      this.Comment = tfvcChangesetRef.Comment;
      this.CommentTruncated = tfvcChangesetRef.CommentTruncated;
    }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<TfvcChange> Changes { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool HasMoreChanges { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public CheckinNote[] CheckinNotes { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TfvcPolicyOverrideInfo PolicyOverride { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<AssociatedWorkItem> WorkItems { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid CollectionId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid AccountId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<Guid> TeamProjectIds { get; set; }
  }
}
