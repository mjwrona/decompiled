// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentTrackingCriteria
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi
{
  [DataContract]
  public class CommentTrackingCriteria
  {
    [DataMember(Name = "firstComparingIteration", EmitDefaultValue = false)]
    public int FirstComparingIteration { get; set; }

    [DataMember(Name = "secondComparingIteration", EmitDefaultValue = false)]
    public int SecondComparingIteration { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string OrigFilePath { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Position OrigLeftFileStart { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Position OrigLeftFileEnd { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Position OrigRightFileStart { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Position OrigRightFileEnd { get; set; }

    public CommentTrackingCriteria()
    {
      this.FirstComparingIteration = 0;
      this.SecondComparingIteration = 0;
    }

    internal CommentTrackingCriteria(int firstIter, int secondIter)
    {
      this.FirstComparingIteration = firstIter;
      this.SecondComparingIteration = secondIter;
    }
  }
}
