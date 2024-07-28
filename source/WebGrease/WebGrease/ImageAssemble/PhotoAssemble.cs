// Decompiled with JetBrains decompiler
// Type: WebGrease.ImageAssemble.PhotoAssemble
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace WebGrease.ImageAssemble
{
  internal class PhotoAssemble : ImageAssembleBase
  {
    private const long DefaultJpegQuality = 100;

    public PhotoAssemble(IWebGreaseContext context)
      : base(context)
    {
    }

    internal override string DefaultExtension => ".jpg";

    internal override ImageType Type => ImageType.Photo;

    protected override ImageFormat Format => ImageFormat.Jpeg;

    protected override void SaveImage(Bitmap newImage)
    {
      ImageCodecInfo encoder = ((IEnumerable<ImageCodecInfo>) ImageCodecInfo.GetImageEncoders()).Where<ImageCodecInfo>((Func<ImageCodecInfo, bool>) (e => e.MimeType == "image/jpeg")).First<ImageCodecInfo>();
      using (EncoderParameter encoderParameter = new EncoderParameter(Encoder.Quality, 100L))
      {
        using (EncoderParameters encoderParams = new EncoderParameters(1))
        {
          encoderParams.Param[0] = encoderParameter;
          newImage.Save(this.AssembleFileName, encoder, encoderParams);
        }
      }
    }
  }
}
