// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.BiosFirmwareTableParser
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.VisualStudio.Telemetry
{
  internal static class BiosFirmwareTableParser
  {
    private const string BiosSerialNumberNotAvailable = "NotAvailable";
    internal const int MinimumTableLength = 8;

    private static Version MinimumSupportedBiosVersion { get; } = new Version("2.1");

    private static Dictionary<int, Func<BinaryReader, BiosInformation, BiosInformation>> SectionProcessorLookup { get; } = new Dictionary<int, Func<BinaryReader, BiosInformation, BiosInformation>>()
    {
      {
        1,
        new Func<BinaryReader, BiosInformation, BiosInformation>(BiosFirmwareTableParser.ProcessBiosSystemInformation)
      }
    };

    internal static BiosInformation ParseBiosFirmwareTable(byte[] biosFirmwareTable)
    {
      biosFirmwareTable.RequiresArgumentNotNull<byte[]>(nameof (biosFirmwareTable));
      using (MemoryStream biosFirmwareTable1 = new MemoryStream(biosFirmwareTable))
        return BiosFirmwareTableParser.ParseBiosFirmwareTable((Stream) biosFirmwareTable1);
    }

    private static int GoToNextSectionToParse(BinaryReader br)
    {
      int nextSectionToParse = -1;
      while (br.BaseStream.Position != br.BaseStream.Length && !BiosFirmwareTableParser.SectionProcessorLookup.ContainsKey(nextSectionToParse = (int) br.ReadByte()))
      {
        byte num1 = br.ReadByte();
        br.BaseStream.Position += (long) ((int) num1 - 2);
        int num2 = 0;
        while (num2 < 2)
          num2 = br.ReadByte() == (byte) 0 ? num2 + 1 : 0;
      }
      return nextSectionToParse;
    }

    private static IList<string> GetSringsFromStringsSection(BinaryReader br)
    {
      int num = 0;
      StringBuilder stringBuilder = new StringBuilder();
      List<string> fromStringsSection = new List<string>();
      while (num < 2)
      {
        char ch = (char) br.ReadByte();
        num = ch == char.MinValue ? num + 1 : 0;
        if (num == 1)
        {
          fromStringsSection.Add(stringBuilder.ToString());
          stringBuilder.Length = 0;
        }
        else
          stringBuilder.Append(ch);
      }
      return (IList<string>) fromStringsSection;
    }

    private static BiosInformation ProcessBiosSystemInformation(
      BinaryReader br,
      BiosInformation biosInformation)
    {
      byte num1 = br.ReadByte();
      long num2 = br.BaseStream.Position + (long) num1 - 2L;
      br.BaseStream.Position += 5L;
      byte num3 = br.ReadByte();
      biosInformation.UUID = new Guid(br.ReadBytes(16));
      br.BaseStream.Position = num2;
      IList<string> fromStringsSection = BiosFirmwareTableParser.GetSringsFromStringsSection(br);
      if (num3 == (byte) 0)
        biosInformation.SerialNumber = "Not Provided in System Information Section";
      else if ((int) num3 > fromStringsSection.Count)
        biosInformation.Error = BiosFirmwareTableParserError.SerialNumberOrdinalIndexOutOfBound;
      else
        biosInformation.SerialNumber = fromStringsSection[(int) num3 - 1];
      return biosInformation;
    }

    internal static BiosInformation ParseBiosFirmwareTable(Stream biosFirmwareTable)
    {
      BiosInformation biosFirmwareTable1 = new BiosInformation()
      {
        Error = BiosFirmwareTableParserError.Success,
        SerialNumber = "NotAvailable"
      } with
      {
        TableSize = biosFirmwareTable.Length
      };
      if (biosFirmwareTable.Length < 8L)
      {
        biosFirmwareTable1.Error = BiosFirmwareTableParserError.TableTooSmall;
        return biosFirmwareTable1;
      }
      using (BinaryReader br = new BinaryReader(biosFirmwareTable))
      {
        int num1 = (int) br.ReadByte();
        biosFirmwareTable1.SpecVersion = new Version(string.Format("{0}.{1}", (object) br.ReadByte(), (object) br.ReadByte()));
        if (biosFirmwareTable1.SpecVersion < BiosFirmwareTableParser.MinimumSupportedBiosVersion)
        {
          biosFirmwareTable1.Error = BiosFirmwareTableParserError.SpecVersionUnsupported;
          return biosFirmwareTable1;
        }
        int num2 = (int) br.ReadByte();
        br.ReadInt32();
        int nextSectionToParse;
        if ((nextSectionToParse = BiosFirmwareTableParser.GoToNextSectionToParse(br)) > 0)
        {
          if (br.BaseStream.Position >= br.BaseStream.Length - 1L)
            biosFirmwareTable1.Error = BiosFirmwareTableParserError.RequiredSectionNotFound;
          else
            biosFirmwareTable1 = BiosFirmwareTableParser.SectionProcessorLookup[nextSectionToParse](br, biosFirmwareTable1);
        }
        return biosFirmwareTable1;
      }
    }
  }
}
