// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using WebGrease.Css.Ast;

namespace WebGrease.Css.ImageAssemblyAnalysis.LogModel
{
  internal class ImageAssemblyAnalysis
  {
    internal WebGrease.Css.ImageAssemblyAnalysis.LogModel.FailureReason? FailureReason { get; set; }

    internal AstNode AstNode { get; set; }

    internal string Image { get; set; }

    internal WebGrease.ImageAssemble.ImageType? ImageType { get; set; }

    internal string SpritedImage { get; set; }
  }
}
