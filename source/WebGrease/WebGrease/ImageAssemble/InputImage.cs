// Decompiled with JetBrains decompiler
// Type: WebGrease.ImageAssemble.InputImage
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections.Generic;

namespace WebGrease.ImageAssemble
{
  internal class InputImage
  {
    private readonly List<string> duplicateImagePaths = new List<string>();

    internal InputImage() => this.Position = ImagePosition.Left;

    internal InputImage(string imagePath)
    {
      this.AbsoluteImagePath = imagePath;
      this.Position = ImagePosition.Left;
    }

    internal string AbsoluteImagePath { get; set; }

    internal string OriginalImagePath { get; set; }

    internal ImagePosition Position { get; set; }

    internal IList<string> DuplicateImagePaths => (IList<string>) this.duplicateImagePaths;
  }
}
