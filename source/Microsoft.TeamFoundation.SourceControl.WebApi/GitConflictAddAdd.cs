// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitConflictAddAdd
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
  public class GitConflictAddAdd : GitConflict
  {
    [DataMember(EmitDefaultValue = false)]
    public GitBlobRef SourceBlob { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public GitBlobRef TargetBlob { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public GitResolutionMergeContent Resolution { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      this.SourceBlob?.SetSecuredObject(securedObject);
      this.TargetBlob?.SetSecuredObject(securedObject);
      this.Resolution?.SetSecuredObject(securedObject);
    }
  }
}
