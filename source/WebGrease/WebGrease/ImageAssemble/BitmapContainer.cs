// Decompiled with JetBrains decompiler
// Type: WebGrease.ImageAssemble.BitmapContainer
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Drawing;
using System.IO;

namespace WebGrease.ImageAssemble
{
  internal class BitmapContainer
  {
    private Bitmap bitmap;

    internal BitmapContainer(InputImage inputImage) => this.InputImage = inputImage;

    internal InputImage InputImage { get; private set; }

    internal Bitmap Bitmap
    {
      get => this.bitmap;
      set
      {
        this.bitmap = value;
        if (value != null)
        {
          this.Width = value.Width;
          this.Height = value.Height;
        }
        else
        {
          this.Width = 0;
          this.Height = 0;
        }
      }
    }

    internal int Width { get; private set; }

    internal int Height { get; private set; }

    public void BitmapAction(Action<Bitmap> action) => Safe.FileLock((FileSystemInfo) new FileInfo(this.InputImage.AbsoluteImagePath), int.MaxValue, (Action) (() => action(this.Bitmap)));
  }
}
