// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitChange
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class GitChange : Change<GitItem>
  {
    [DataMember(EmitDefaultValue = false)]
    public string OriginalPath { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int ChangeId { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(EmitDefaultValue = false)]
    public GitTemplate NewContentTemplate { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      this.NewContentTemplate?.SetSecuredObject(securedObject);
    }
  }
}
