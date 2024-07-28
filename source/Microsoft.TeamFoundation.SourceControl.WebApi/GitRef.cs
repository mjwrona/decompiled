// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitRef
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  [KnownType(typeof (GitRefsCollection))]
  public class GitRef : VersionControlSecuredObject
  {
    public GitRef()
    {
    }

    public GitRef(string name) => this.Name = name;

    public GitRef(string name, string objectId)
    {
      this.Name = name;
      this.ObjectId = objectId;
    }

    public GitRef(string name, string objectId, IdentityRef isLockedBy)
    {
      this.Name = name;
      this.ObjectId = objectId;
      this.IsLockedBy = isLockedBy;
      this.IsLocked = isLockedBy != null;
    }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string ObjectId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef IsLockedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsLocked { get; set; }

    [DataMember]
    public IdentityRef Creator { get; set; }

    [DataMember]
    public string Url { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string PeeledObjectId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<GitStatus> Statuses { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "_links")]
    public ReferenceLinks Links { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      IEnumerable<GitStatus> statuses = this.Statuses;
      if (statuses == null)
        return;
      statuses.SetSecuredObject<GitStatus>(securedObject);
    }
  }
}
