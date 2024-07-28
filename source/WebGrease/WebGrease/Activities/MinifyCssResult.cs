// Decompiled with JetBrains decompiler
// Type: WebGrease.Activities.MinifyCssResult
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections.Generic;

namespace WebGrease.Activities
{
  internal class MinifyCssResult
  {
    public MinifyCssResult(
      IEnumerable<ContentItem> css,
      IEnumerable<ContentItem> spritedImages,
      IEnumerable<ContentItem> hashedImages)
    {
      this.Css = css;
      this.SpritedImages = spritedImages;
      this.HashedImages = hashedImages;
    }

    internal IEnumerable<ContentItem> Css { get; private set; }

    internal IEnumerable<ContentItem> SpritedImages { get; private set; }

    internal IEnumerable<ContentItem> HashedImages { get; private set; }
  }
}
