// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controllers.ImageExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Controllers
{
  internal static class ImageExtensions
  {
    public static string GetMimeType(this ImageFormat imageFormat) => ((IEnumerable<ImageCodecInfo>) ImageCodecInfo.GetImageEncoders()).First<ImageCodecInfo>((Func<ImageCodecInfo, bool>) (codec => codec.FormatID == imageFormat.Guid)).MimeType;

    public static string GetMimeType(this Image image) => image.RawFormat.GetMimeType();
  }
}
