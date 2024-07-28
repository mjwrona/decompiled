// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.CommentTrackingCriteria
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class CommentTrackingCriteria : VersionControlSecuredObject
  {
    [DataMember(EmitDefaultValue = false)]
    public int FirstComparingIteration { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int SecondComparingIteration { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string OrigFilePath { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public CommentPosition OrigLeftFileStart { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public CommentPosition OrigLeftFileEnd { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public CommentPosition OrigRightFileStart { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public CommentPosition OrigRightFileEnd { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      this.OrigLeftFileStart?.SetSecuredObject(securedObject);
      this.OrigLeftFileEnd?.SetSecuredObject(securedObject);
      this.OrigRightFileStart?.SetSecuredObject(securedObject);
      this.OrigRightFileEnd?.SetSecuredObject(securedObject);
    }
  }
}
