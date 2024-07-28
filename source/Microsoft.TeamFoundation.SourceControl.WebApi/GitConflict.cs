// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitConflict
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  [JsonConverter(typeof (GitConflictJsonConverter))]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class GitConflict : VersionControlSecuredObject
  {
    [DataMember(EmitDefaultValue = false)]
    public GitMergeOriginRef MergeOrigin { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int ConflictId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public GitCommitRef MergeBaseCommit { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public GitCommitRef MergeSourceCommit { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public GitCommitRef MergeTargetCommit { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public GitConflictType ConflictType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ConflictPath { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public GitResolutionStatus ResolutionStatus { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public GitResolutionError ResolutionError { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef ResolvedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime ResolvedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "_links")]
    public ReferenceLinks Links { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      this.MergeOrigin?.SetSecuredObject(securedObject);
      this.MergeBaseCommit?.SetSecuredObject(securedObject);
      this.MergeSourceCommit.SetSecuredObject(securedObject);
      this.MergeTargetCommit.SetSecuredObject(securedObject);
    }
  }
}
