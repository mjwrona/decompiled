// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.CommentThreadContext
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class CommentThreadContext : VersionControlSecuredObject
  {
    [DataMember]
    public string FilePath { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public CommentPosition LeftFileStart { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public CommentPosition LeftFileEnd { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public CommentPosition RightFileStart { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public CommentPosition RightFileEnd { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      this.LeftFileStart?.SetSecuredObject(securedObject);
      this.LeftFileEnd?.SetSecuredObject(securedObject);
      this.RightFileStart?.SetSecuredObject(securedObject);
      this.RightFileEnd?.SetSecuredObject(securedObject);
    }
  }
}
