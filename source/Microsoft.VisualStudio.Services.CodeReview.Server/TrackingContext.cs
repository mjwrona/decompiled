// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.TrackingContext
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal class TrackingContext
  {
    public TrackingVersion SourceVersion;
    public TrackingVersion TargetVersion;
    public TrackingSide SourceSide;
    public TrackingSide TargetSide;
    public TrackingTarget TrackingTarget;
    public int FirstComparingIteration;
    public int SecondComparingIteration;
    public int SourceIteration;
    public int TargetIteration;
    public int ChangeTrackingId;
    public int OrigStartLine;
    public int OrigStartOffset;
    public int OrigEndLine;
    public int OrigEndOffset;
    public string SourceFilename;
    public string TargetFilename;
    public bool TrackBackward;

    public object FormatCI() => (object) new
    {
      TrackingTarget = this.TrackingTarget.ToString(),
      FirstComparingIteration = this.FirstComparingIteration,
      SecondComparingIteration = this.SecondComparingIteration,
      ChangeTrackingId = this.ChangeTrackingId,
      SourceIteration = this.SourceIteration,
      SourceFilename = this.SourceFilename,
      SourceVersion = this.SourceVersion.ToString(),
      SourceSide = this.SourceSide.ToString(),
      TargetIteration = this.TargetIteration,
      TargetFilename = this.TargetFilename,
      TargetVersion = this.TargetVersion.ToString(),
      TargetSide = this.TargetSide.ToString(),
      OrigStartLine = this.OrigStartLine,
      OrigStartOffset = this.OrigStartOffset,
      OrigEndLine = this.OrigEndLine,
      OrigEndOffset = this.OrigEndOffset,
      TrackBackward = this.TrackBackward
    };
  }
}
