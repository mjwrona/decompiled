// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitConflictRename2to1
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class GitConflictRename2to1 : GitConflict
  {
    [DataMember(EmitDefaultValue = false)]
    public string SourceOriginalPath { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public GitBlobRef SourceOriginalBlob { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public GitBlobRef SourceNewBlob { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string TargetOriginalPath { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public GitBlobRef TargetOriginalBlob { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public GitBlobRef TargetNewBlob { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public GitResolutionPathConflict Resolution { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      this.SourceOriginalBlob?.SetSecuredObject(securedObject);
      this.SourceNewBlob?.SetSecuredObject(securedObject);
      this.TargetOriginalBlob?.SetSecuredObject(securedObject);
      this.TargetNewBlob?.SetSecuredObject(securedObject);
      this.Resolution?.SetSecuredObject(securedObject);
    }
  }
}
