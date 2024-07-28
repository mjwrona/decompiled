// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.TfvcBranch
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class TfvcBranch : TfvcBranchRef
  {
    public TfvcBranch()
    {
    }

    public TfvcBranch(TfvcBranchRef branchRef)
    {
      this.Path = branchRef.Path;
      this.Description = branchRef.Description;
      this.CreatedDate = branchRef.CreatedDate;
      this.Owner = branchRef.Owner;
      this.IsDeleted = branchRef.IsDeleted;
    }

    [DataMember(EmitDefaultValue = false, Order = 5)]
    public TfvcShallowBranchRef Parent { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 6)]
    public List<TfvcShallowBranchRef> RelatedBranches { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 7)]
    public List<TfvcBranchMapping> Mappings { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 8)]
    public List<TfvcBranch> Children { get; set; }
  }
}
