// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.Common.EndOfLineTerminator
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F16CDF2D-8103-4EAE-A2A8-4FA5B1C1BE58
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Server.Common.dll

namespace Microsoft.VisualStudio.Services.CodeReview.Server.Common
{
  public enum EndOfLineTerminator
  {
    None,
    LineFeed,
    CarriageReturn,
    CarriageReturnLineFeed,
    LineSeparator,
    ParagraphSeparator,
    NextLine,
  }
}
