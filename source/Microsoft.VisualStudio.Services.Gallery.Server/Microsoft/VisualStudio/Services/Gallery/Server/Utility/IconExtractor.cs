// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Utility.IconExtractor
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Utility
{
  public class IconExtractor
  {
    public Bitmap GetLargestIconImage(Stream iconStream) => iconStream != null ? new IconExtractor.MultipleIcon(iconStream).GetLargestIcon().ToBitmap() : throw new ArgumentNullException(nameof (iconStream));

    private class MultipleIcon
    {
      private readonly Stream iconStream;
      private IconExtractor.MultipleIcon.IconHeader iconHeader;
      private readonly List<IconExtractor.MultipleIcon.IconEntry> iconEntries = new List<IconExtractor.MultipleIcon.IconEntry>();

      public MultipleIcon(Stream iconStream)
      {
        this.iconStream = iconStream;
        this.ReadIconInformation(iconStream);
        if (this.iconEntries.Count == 0)
          throw new ArgumentException("Icon contains no entries");
      }

      public Icon GetLargestIcon() => this.CreateIconFromIconEntry(this.GetLargestIconEntry());

      private Icon CreateIconFromIconEntry(IconExtractor.MultipleIcon.IconEntry iconEntry)
      {
        byte[] buffer = new byte[iconEntry.BytesInRes];
        short num1 = 1;
        int num2 = 22;
        using (MemoryStream output = new MemoryStream())
        {
          using (BinaryWriter binaryWriter = new BinaryWriter((Stream) output))
          {
            binaryWriter.Write(this.iconHeader.Reserved);
            binaryWriter.Write(this.iconHeader.Type);
            binaryWriter.Write(num1);
            binaryWriter.Write(iconEntry.Width);
            binaryWriter.Write(iconEntry.Height);
            binaryWriter.Write(iconEntry.ColorCount);
            binaryWriter.Write(iconEntry.Reserved);
            binaryWriter.Write(iconEntry.Planes);
            binaryWriter.Write(iconEntry.BitCount);
            binaryWriter.Write(iconEntry.BytesInRes);
            binaryWriter.Write(num2);
            this.iconStream.Seek((long) iconEntry.ImageOffset, SeekOrigin.Begin);
            this.iconStream.Read(buffer, 0, iconEntry.BytesInRes);
            binaryWriter.Write(buffer);
            binaryWriter.Flush();
            output.Seek(0L, SeekOrigin.Begin);
            return new Icon((Stream) output, (int) iconEntry.Width, (int) iconEntry.Height);
          }
        }
      }

      private IconExtractor.MultipleIcon.IconEntry GetLargestIconEntry()
      {
        int index1 = 0;
        int num1 = 0;
        int num2 = 0;
        int index2 = -1;
        for (int index3 = 0; index3 < this.iconEntries.Count; ++index3)
        {
          IconExtractor.MultipleIcon.IconEntry iconEntry = this.iconEntries[index3];
          int num3 = (int) iconEntry.Height * (int) iconEntry.Width;
          if (num3 >= num1 && (int) iconEntry.BitCount >= num2 && num3 != 0)
          {
            index1 = index3;
            num1 = num3;
            num2 = (int) iconEntry.BitCount;
            if (num2 == 8)
              index2 = index3;
          }
        }
        if (index2 != -1 && num1 == (int) this.iconEntries[index2].Width * (int) this.iconEntries[index2].Height)
          return this.iconEntries[index2];
        if (index1 == 0 && this.iconEntries.Count > 0 && this.iconEntries[0].Width == (byte) 0 && this.iconEntries[0].Height == (byte) 0)
          throw new ArgumentException(GalleryResources.ErrorIconSize());
        return this.iconEntries[index1];
      }

      private void ReadIconInformation(Stream stream)
      {
        stream.Seek(0L, SeekOrigin.Begin);
        BinaryReader reader = new BinaryReader(stream);
        this.iconHeader = this.ReadIconHeader(reader);
        for (int index = 0; index < (int) this.iconHeader.Count; ++index)
          this.iconEntries.Add(this.ReadIconEntry(reader));
      }

      private IconExtractor.MultipleIcon.IconEntry ReadIconEntry(BinaryReader reader) => new IconExtractor.MultipleIcon.IconEntry()
      {
        Width = reader.ReadByte(),
        Height = reader.ReadByte(),
        ColorCount = reader.ReadByte(),
        Reserved = reader.ReadByte(),
        Planes = reader.ReadInt16(),
        BitCount = reader.ReadInt16(),
        BytesInRes = reader.ReadInt32(),
        ImageOffset = reader.ReadInt32()
      };

      private IconExtractor.MultipleIcon.IconHeader ReadIconHeader(BinaryReader reader) => new IconExtractor.MultipleIcon.IconHeader()
      {
        Reserved = reader.ReadInt16(),
        Type = reader.ReadInt16(),
        Count = reader.ReadInt16()
      };

      private class IconHeader
      {
        public short Reserved { get; set; }

        public short Type { get; set; }

        public short Count { get; set; }
      }

      private class IconEntry
      {
        public byte Width { get; set; }

        public byte Height { get; set; }

        public byte ColorCount { get; set; }

        public byte Reserved { get; set; }

        public short Planes { get; set; }

        public short BitCount { get; set; }

        public int BytesInRes { get; set; }

        public int ImageOffset { get; set; }
      }
    }
  }
}
