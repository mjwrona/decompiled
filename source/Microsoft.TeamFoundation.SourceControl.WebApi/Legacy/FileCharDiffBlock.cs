// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.FileCharDiffBlock
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi.Legacy
{
  [DataContract]
  public class FileCharDiffBlock : VersionControlSecuredObject
  {
    [DataMember(Name = "lineChange")]
    public FileDiffBlock LineChange { get; set; }

    [DataMember(Name = "charChange")]
    public List<FileDiffBlock> CharChange { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      List<FileDiffBlock> charChange = this.CharChange;
      if (charChange == null)
        return;
      charChange.SetSecuredObject<FileDiffBlock>(securedObject);
    }
  }
}
