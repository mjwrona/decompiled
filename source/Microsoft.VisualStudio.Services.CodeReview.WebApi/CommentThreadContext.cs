// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentThreadContext
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi
{
  [DataContract]
  public class CommentThreadContext
  {
    [DataMember]
    public string FilePath { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IterationContext IterationContext { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public CommentTrackingCriteria TrackingCriteria { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int ChangeTrackingId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Position LeftFileStart { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Position LeftFileEnd { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Position RightFileStart { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Position RightFileEnd { get; set; }

    public bool Equals(CommentThreadContext threadContext)
    {
      if (threadContext == null || string.Compare(this.FilePath, threadContext.FilePath, StringComparison.OrdinalIgnoreCase) != 0 || this.ChangeTrackingId != threadContext.ChangeTrackingId || this.IterationContext != threadContext.IterationContext && (this.IterationContext == null || !this.IterationContext.Equals(threadContext.IterationContext)) || this.LeftFileStart != threadContext.LeftFileStart && (this.LeftFileStart == null || !this.LeftFileStart.Equals(threadContext.LeftFileStart)) || this.LeftFileEnd != threadContext.LeftFileEnd && (this.LeftFileEnd == null || !this.LeftFileEnd.Equals(threadContext.LeftFileEnd)) || this.RightFileStart != threadContext.RightFileStart && (this.RightFileStart == null || !this.RightFileStart.Equals(threadContext.RightFileStart)))
        return false;
      if (this.RightFileEnd == threadContext.RightFileEnd)
        return true;
      return this.RightFileEnd != null && this.RightFileEnd.Equals(threadContext.RightFileEnd);
    }

    public override bool Equals(object obj) => this.Equals(obj as CommentThreadContext);

    public override int GetHashCode() => (this.FilePath == null ? 0 : this.FilePath.GetHashCode()) ^ this.ChangeTrackingId.GetHashCode() ^ (this.IterationContext == null ? 0 : this.IterationContext.GetHashCode()) ^ (this.LeftFileStart == null ? 0 : this.LeftFileStart.GetHashCode()) ^ (this.LeftFileEnd == null ? 0 : this.LeftFileEnd.GetHashCode()) ^ (this.RightFileStart == null ? 0 : this.RightFileStart.GetHashCode()) ^ (this.RightFileEnd == null ? 0 : this.RightFileEnd.GetHashCode());
  }
}
