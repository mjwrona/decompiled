// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.TfvcShelveset
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class TfvcShelveset : TfvcShelvesetRef
  {
    public TfvcShelveset()
    {
    }

    public TfvcShelveset(TfvcShelvesetRef shelvesetRef)
    {
      this.Name = shelvesetRef.Name;
      this.Id = shelvesetRef.Id;
      this.Owner = shelvesetRef.Owner;
      this.CreatedDate = shelvesetRef.CreatedDate;
      this.Comment = shelvesetRef.Comment;
      this.CommentTruncated = shelvesetRef.CommentTruncated;
    }

    [DataMember(EmitDefaultValue = false)]
    public TfvcPolicyOverrideInfo PolicyOverride { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public CheckinNote[] Notes { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<AssociatedWorkItem> WorkItems { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<TfvcChange> Changes { get; set; }
  }
}
