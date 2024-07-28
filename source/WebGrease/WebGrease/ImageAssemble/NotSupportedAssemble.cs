// Decompiled with JetBrains decompiler
// Type: WebGrease.ImageAssemble.NotSupportedAssemble
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections.Generic;
using System.Drawing.Imaging;

namespace WebGrease.ImageAssemble
{
  internal class NotSupportedAssemble : ImageAssembleBase
  {
    public NotSupportedAssemble(IWebGreaseContext context)
      : base(context)
    {
    }

    internal override ImageType Type => ImageType.NotSupported;

    internal override string DefaultExtension => ".bmp";

    protected override ImageFormat Format => ImageFormat.Bmp;

    internal override bool Assemble(List<BitmapContainer> inputImages)
    {
      foreach (BitmapContainer inputImage in inputImages)
        this.ImageXmlMap.AppendToXml(inputImage.InputImage.AbsoluteImagePath, "Not supported");
      return false;
    }
  }
}
