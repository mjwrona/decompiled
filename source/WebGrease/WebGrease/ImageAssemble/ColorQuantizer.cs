// Decompiled with JetBrains decompiler
// Type: WebGrease.ImageAssemble.ColorQuantizer
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace WebGrease.ImageAssemble
{
  public static class ColorQuantizer
  {
    public static Bitmap Quantize(Image image, PixelFormat bitmapPixelFormat) => ColorQuantizer.Quantize(image, bitmapPixelFormat, true);

    public static Bitmap Quantize(Image image, PixelFormat pixelFormat, bool useDither)
    {
      if (image is Bitmap bitmapSource1 && bitmapSource1.PixelFormat == PixelFormat.Format32bppArgb)
        return ColorQuantizer.DoQuantize(bitmapSource1, pixelFormat, useDither);
      int width = image.Width;
      int height = image.Height;
      Rectangle destRect = Rectangle.FromLTRB(0, 0, width, height);
      using (Bitmap bitmapSource2 = new Bitmap(width, height, PixelFormat.Format32bppArgb))
      {
        using (Graphics graphics = Graphics.FromImage((Image) bitmapSource2))
          graphics.DrawImage(image, destRect, 0, 0, width, height, GraphicsUnit.Pixel);
        return ColorQuantizer.DoQuantize(bitmapSource2, pixelFormat, useDither);
      }
    }

    private static Bitmap DoQuantize(Bitmap bitmapSource, PixelFormat pixelFormat, bool useDither)
    {
      int width = bitmapSource.Width;
      int height = bitmapSource.Height;
      Rectangle rect = Rectangle.FromLTRB(0, 0, width, height);
      Bitmap bitmap = (Bitmap) null;
      try
      {
        bitmap = new Bitmap(width, height, pixelFormat);
        BitmapData bitmapdata1 = bitmapSource.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        try
        {
          ColorQuantizer.Octree octree = new ColorQuantizer.Octree(pixelFormat);
          int num1 = Math.Abs(bitmapdata1.Stride);
          byte[] numArray = new byte[num1 * height];
          Marshal.Copy(bitmapdata1.Scan0, numArray, 0, numArray.Length);
          int rowStart1 = 0;
          for (int index = 0; index < height; ++index)
          {
            for (int col = 0; col < width; ++col)
            {
              ColorQuantizer.Pixel sourcePixel = ColorQuantizer.GetSourcePixel(numArray, rowStart1, col);
              octree.AddColor(sourcePixel);
            }
            rowStart1 += num1;
          }
          Color[] paletteColors = octree.GetPaletteColors();
          ColorPalette palette = bitmap.Palette;
          for (int index = 0; index < palette.Entries.Length; ++index)
            palette.Entries[index] = index < paletteColors.Length ? paletteColors[index] : Color.Transparent;
          bitmap.Palette = palette;
          BitmapData bitmapdata2 = bitmap.LockBits(rect, ImageLockMode.ReadWrite, pixelFormat);
          try
          {
            int num2 = Math.Abs(bitmapdata2.Stride);
            byte[] source = new byte[num2 * height];
            int rowStart2 = 0;
            int num3 = 0;
            for (int index = 0; index < height; ++index)
            {
              for (int col = 0; col < width; ++col)
              {
                ColorQuantizer.Pixel sourcePixel = ColorQuantizer.GetSourcePixel(numArray, rowStart2, col);
                int paletteIndex = octree.GetPaletteIndex(sourcePixel);
                if (useDither && sourcePixel.Alpha != (byte) 0)
                {
                  Color color = paletteColors[paletteIndex];
                  int deltaRed = (int) sourcePixel.Red - (int) color.R;
                  int deltaGreen = (int) sourcePixel.Green - (int) color.G;
                  int deltaBlue = (int) sourcePixel.Blue - (int) color.B;
                  if (col + 1 < width)
                    ColorQuantizer.DitherSourcePixel(numArray, rowStart2, col + 1, deltaRed, deltaGreen, deltaBlue, 7);
                  if (index + 1 < height)
                  {
                    int rowStart3 = rowStart2 + num1;
                    if (col > 0)
                      ColorQuantizer.DitherSourcePixel(numArray, rowStart3, col - 1, deltaRed, deltaGreen, deltaBlue, 3);
                    ColorQuantizer.DitherSourcePixel(numArray, rowStart3, col, deltaRed, deltaGreen, deltaBlue, 5);
                    if (col + 1 < width)
                      ColorQuantizer.DitherSourcePixel(numArray, rowStart3, col + 1, deltaRed, deltaGreen, deltaBlue, 1);
                  }
                }
                switch (pixelFormat)
                {
                  case PixelFormat.Format1bppIndexed:
                    if (paletteIndex != 0)
                    {
                      source[num3 + (col >> 3)] |= (byte) (128 >> (col & 7));
                      break;
                    }
                    break;
                  case PixelFormat.Format4bppIndexed:
                    source[num3 + (col >> 1)] |= (col & 1) == 1 ? (byte) (paletteIndex & 15) : (byte) (paletteIndex << 4);
                    break;
                  case PixelFormat.Format8bppIndexed:
                    source[num3 + col] = (byte) paletteIndex;
                    break;
                }
              }
              rowStart2 += num1;
              num3 += num2;
            }
            Marshal.Copy(source, 0, bitmapdata2.Scan0, source.Length);
          }
          finally
          {
            bitmap.UnlockBits(bitmapdata2);
          }
        }
        finally
        {
          bitmapSource.UnlockBits(bitmapdata1);
        }
      }
      catch (Exception ex)
      {
        bitmap?.Dispose();
        throw;
      }
      return bitmap;
    }

    private static void DitherSourcePixel(
      byte[] buffer,
      int rowStart,
      int col,
      int deltaRed,
      int deltaGreen,
      int deltaBlue,
      int weight)
    {
      int index = rowStart + col * 4;
      buffer[index + 2] = ColorQuantizer.ChannelAdjustment(buffer[index + 2], deltaRed * weight >> 4);
      buffer[index + 1] = ColorQuantizer.ChannelAdjustment(buffer[index + 1], deltaGreen * weight >> 4);
      buffer[index] = ColorQuantizer.ChannelAdjustment(buffer[index], deltaBlue * weight >> 4);
    }

    private static ColorQuantizer.Pixel GetSourcePixel(byte[] buffer, int rowStart, int col)
    {
      int index = rowStart + col * 4;
      return new ColorQuantizer.Pixel()
      {
        Alpha = buffer[index + 3],
        Red = buffer[index + 2],
        Green = buffer[index + 1],
        Blue = buffer[index]
      };
    }

    private static byte ChannelAdjustment(byte current, int offset) => (byte) Math.Min((int) byte.MaxValue, Math.Max(0, (int) current + offset));

    private class Octree
    {
      private readonly int m_maxColors;
      private readonly ColorQuantizer.Octree.OctreeNode[] m_reducibleNodes;
      private int m_colorCount;
      private bool m_hasTransparent;
      private int m_lastArgb;
      private ColorQuantizer.Octree.OctreeNode m_lastNode;
      private Color[] m_palette;
      private ColorQuantizer.Octree.OctreeNode m_root;

      internal Octree(PixelFormat pixelFormat)
      {
        switch (pixelFormat)
        {
          case PixelFormat.Format1bppIndexed:
            this.m_maxColors = 2;
            break;
          case PixelFormat.Format4bppIndexed:
            this.m_maxColors = 16;
            break;
          case PixelFormat.Format8bppIndexed:
            this.m_maxColors = 256;
            break;
          default:
            throw new ArgumentException("Invalid Pixel Format", nameof (pixelFormat));
        }
        this.m_reducibleNodes = new ColorQuantizer.Octree.OctreeNode[7];
        this.m_root = new ColorQuantizer.Octree.OctreeNode(0, this);
      }

      internal void AddColor(ColorQuantizer.Pixel pixel)
      {
        if (pixel.Alpha > (byte) 0)
        {
          if (this.m_lastNode != null && pixel.ARGB == this.m_lastArgb)
            this.m_lastNode.AddColor(pixel);
          else
            this.m_colorCount += this.m_root.AddColor(pixel) ? 1 : 0;
        }
        else
          this.m_hasTransparent = true;
      }

      internal int GetPaletteIndex(ColorQuantizer.Pixel pixel)
      {
        int paletteIndex = 0;
        if (pixel.Alpha > (byte) 0)
        {
          paletteIndex = this.m_root.GetPaletteIndex(pixel);
          if (paletteIndex < 0)
          {
            int num1 = int.MaxValue;
            for (int index = 0; index < this.m_palette.Length; ++index)
            {
              Color color = this.m_palette[index];
              int num2 = (int) pixel.Red - (int) color.R;
              int num3 = (int) pixel.Green - (int) color.G;
              int num4 = (int) pixel.Blue - (int) color.B;
              int num5 = num2 * num2 + num3 * num3 + num4 * num4;
              if (num5 < num1)
              {
                num1 = num5;
                paletteIndex = index;
              }
            }
          }
        }
        return paletteIndex;
      }

      internal Color[] GetPaletteColors()
      {
        if (this.m_palette == null)
        {
          int index1 = this.m_reducibleNodes.Length - 1;
          ColorQuantizer.Octree.OctreeNode reducibleNode;
          for (int index2 = this.m_maxColors - (this.m_hasTransparent ? 1 : 0); this.m_colorCount > index2; this.m_colorCount -= reducibleNode.Reduce() - 1)
          {
            while (index1 > 0 && this.m_reducibleNodes[index1] == null)
              --index1;
            if (this.m_reducibleNodes[index1] != null)
            {
              reducibleNode = this.m_reducibleNodes[index1];
              this.m_reducibleNodes[index1] = reducibleNode.NextReducibleNode;
            }
            else
              break;
          }
          if (index1 == 0 && !this.m_hasTransparent)
          {
            this.m_palette = new Color[2];
            this.m_palette[0] = Color.Black;
            this.m_palette[1] = Color.White;
            this.m_root = new ColorQuantizer.Octree.OctreeNode(0, this);
          }
          else
          {
            int paletteIndex = 0;
            this.m_palette = new Color[this.m_colorCount + (this.m_hasTransparent ? 1 : 0)];
            if (this.m_hasTransparent)
              this.m_palette[paletteIndex++] = Color.Transparent;
            this.m_root.AddColorsToPalette(this.m_palette, ref paletteIndex);
          }
        }
        return this.m_palette;
      }

      private void SetLastNode(ColorQuantizer.Octree.OctreeNode node, int argb)
      {
        this.m_lastNode = node;
        this.m_lastArgb = argb;
      }

      private void AddReducibleNode(ColorQuantizer.Octree.OctreeNode reducibleNode)
      {
        reducibleNode.NextReducibleNode = this.m_reducibleNodes[reducibleNode.Level];
        this.m_reducibleNodes[reducibleNode.Level] = reducibleNode;
      }

      private class OctreeNode
      {
        private static readonly byte[] s_levelMasks = new byte[8]
        {
          (byte) 128,
          (byte) 64,
          (byte) 32,
          (byte) 16,
          (byte) 8,
          (byte) 4,
          (byte) 2,
          (byte) 1
        };
        private readonly int m_level;
        private readonly ColorQuantizer.Octree m_octree;
        private int m_blueSum;
        private ColorQuantizer.Octree.OctreeNode[] m_childNodes;
        private int m_greenSum;
        private bool m_isLeaf;
        private int m_paletteIndex;
        private int m_pixelCount;
        private int m_redSum;

        internal OctreeNode(int level, ColorQuantizer.Octree octree)
        {
          this.m_octree = octree;
          this.m_level = level;
          this.m_isLeaf = level == 7;
          if (this.m_isLeaf)
            return;
          this.m_childNodes = new ColorQuantizer.Octree.OctreeNode[8];
          this.m_octree.AddReducibleNode(this);
        }

        internal int Level => this.m_level;

        internal Color NodeColor => Color.FromArgb(this.m_redSum / this.m_pixelCount, this.m_greenSum / this.m_pixelCount, this.m_blueSum / this.m_pixelCount);

        internal ColorQuantizer.Octree.OctreeNode NextReducibleNode { get; set; }

        internal bool AddColor(ColorQuantizer.Pixel pixel)
        {
          bool flag;
          if (this.m_isLeaf)
          {
            flag = ++this.m_pixelCount == 1;
            this.m_redSum += (int) pixel.Red;
            this.m_greenSum += (int) pixel.Green;
            this.m_blueSum += (int) pixel.Blue;
            this.m_octree.SetLastNode(this, pixel.ARGB);
          }
          else
          {
            int childIndex = this.GetChildIndex(pixel);
            if (this.m_childNodes[childIndex] == null)
              this.m_childNodes[childIndex] = new ColorQuantizer.Octree.OctreeNode(this.m_level + 1, this.m_octree);
            flag = this.m_childNodes[childIndex].AddColor(pixel);
          }
          return flag;
        }

        internal int GetPaletteIndex(ColorQuantizer.Pixel pixel)
        {
          int paletteIndex = -1;
          if (this.m_isLeaf)
          {
            paletteIndex = this.m_paletteIndex;
          }
          else
          {
            int childIndex = this.GetChildIndex(pixel);
            if (this.m_childNodes[childIndex] != null)
              paletteIndex = this.m_childNodes[childIndex].GetPaletteIndex(pixel);
          }
          return paletteIndex;
        }

        internal int Reduce()
        {
          int num = 0;
          if (!this.m_isLeaf)
          {
            for (int index = 0; index < this.m_childNodes.Length; ++index)
            {
              if (this.m_childNodes[index] != null)
              {
                ColorQuantizer.Octree.OctreeNode childNode = this.m_childNodes[index];
                this.m_pixelCount += childNode.m_pixelCount;
                this.m_redSum += childNode.m_redSum;
                this.m_greenSum += childNode.m_greenSum;
                this.m_blueSum += childNode.m_blueSum;
                ++num;
              }
            }
            this.m_childNodes = (ColorQuantizer.Octree.OctreeNode[]) null;
            this.m_isLeaf = true;
          }
          return num;
        }

        internal void AddColorsToPalette(Color[] colorArray, ref int paletteIndex)
        {
          if (this.m_isLeaf)
          {
            this.m_paletteIndex = paletteIndex++;
            colorArray[this.m_paletteIndex] = this.NodeColor;
          }
          else
          {
            for (int index = 0; index < this.m_childNodes.Length; ++index)
            {
              if (this.m_childNodes[index] != null)
                this.m_childNodes[index].AddColorsToPalette(colorArray, ref paletteIndex);
            }
          }
        }

        private int GetChildIndex(ColorQuantizer.Pixel pixel)
        {
          int num = 7 - this.m_level;
          int levelMask = (int) ColorQuantizer.Octree.OctreeNode.s_levelMasks[this.m_level];
          return ((int) pixel.Red & levelMask) >> num - 2 | ((int) pixel.Green & levelMask) >> num - 1 | ((int) pixel.Blue & levelMask) >> num;
        }
      }
    }

    private class Pixel
    {
      public byte Blue { get; set; }

      public byte Green { get; set; }

      public byte Red { get; set; }

      public byte Alpha { get; set; }

      public int ARGB => (int) this.Alpha << 24 | (int) this.Red << 16 | (int) this.Green << 8 | (int) this.Blue;
    }
  }
}
