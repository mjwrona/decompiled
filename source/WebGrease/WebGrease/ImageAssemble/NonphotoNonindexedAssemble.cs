// Decompiled with JetBrains decompiler
// Type: WebGrease.ImageAssemble.NonphotoNonindexedAssemble
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace WebGrease.ImageAssemble
{
  internal class NonphotoNonindexedAssemble : ImageAssembleBase
  {
    public NonphotoNonindexedAssemble(IWebGreaseContext context)
      : base(context)
    {
    }

    internal override ImageType Type => ImageType.NonphotoNonindexed;

    internal override string DefaultExtension => ".png";

    protected override ImageFormat Format => ImageFormat.Png;

    protected override void SaveImage(Bitmap newImage)
    {
      if (File.Exists(this.AssembleFileName))
        return;
      base.SaveImage(newImage);
      this.OptimizeImage();
    }
  }
}
