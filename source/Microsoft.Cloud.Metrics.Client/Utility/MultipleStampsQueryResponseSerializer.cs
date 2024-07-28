// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Utility.MultipleStampsQueryResponseSerializer
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.Cloud.Metrics.Client.Utility
{
  public static class MultipleStampsQueryResponseSerializer
  {
    private const byte MultipleQueryResponseFormatVersion = 1;
    private static readonly byte[] MergedResponseIdentifierByteSequence = new byte[4]
    {
      byte.MaxValue,
      (byte) 175,
      (byte) 171,
      (byte) 250
    };

    public static void MergeResponses(
      IEnumerable<Stream> allResponses,
      int numberOfResponses,
      Stream destination)
    {
      using (BinaryWriter binaryWriter = new BinaryWriter(destination, Encoding.UTF8, true))
      {
        binaryWriter.Write(MultipleStampsQueryResponseSerializer.MergedResponseIdentifierByteSequence, 0, MultipleStampsQueryResponseSerializer.MergedResponseIdentifierByteSequence.Length);
        binaryWriter.Write((byte) 1);
        binaryWriter.Write(numberOfResponses);
        int num = 0;
        foreach (Stream allResponse in allResponses)
        {
          binaryWriter.Write((int) allResponse.Length);
          allResponse.CopyTo(destination);
          ++num;
        }
        if (num != numberOfResponses)
          throw new ArgumentException(string.Format("Number of elements in {0} should be same as {1}={2}", (object) nameof (allResponses), (object) nameof (numberOfResponses), (object) numberOfResponses));
      }
      destination.Position = 0L;
    }

    internal static bool IsMergedResponseStream(Stream streamToCheck)
    {
      if (streamToCheck == null)
        throw new ArgumentNullException(nameof (streamToCheck));
      if (streamToCheck.Length < (long) MultipleStampsQueryResponseSerializer.MergedResponseIdentifierByteSequence.Length)
        return false;
      long position = streamToCheck.Position;
      for (int index = 0; index < MultipleStampsQueryResponseSerializer.MergedResponseIdentifierByteSequence.Length; ++index)
      {
        if ((int) MultipleStampsQueryResponseSerializer.MergedResponseIdentifierByteSequence[index] != streamToCheck.ReadByte())
        {
          streamToCheck.Position = position;
          return false;
        }
      }
      return true;
    }

    internal static IEnumerable<Stream> EnumerateMergeResponses(BinaryReader reader)
    {
      byte num1 = reader.ReadByte();
      if (num1 != (byte) 1)
        throw new ArgumentException(string.Format("Unknown version found. Version:{0}, Expected:{1}", (object) num1, (object) (byte) 1));
      Stream baseStream = reader.BaseStream;
      byte[] tempArray = new byte[85000];
      int numberOfResponses = reader.ReadInt32();
      for (int i = 0; i < numberOfResponses; ++i)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          int num2 = reader.ReadInt32();
          int val1;
          int count;
          for (val1 = num2; val1 > 0; val1 -= count)
          {
            count = baseStream.Read(tempArray, 0, Math.Min(val1, tempArray.Length));
            if (count == 0)
              throw new EndOfStreamException(string.Format("{0} end of stream reached before reading {1} bytes.", (object) nameof (reader), (object) num2));
            memoryStream.Write(tempArray, 0, count);
          }
          if (val1 != 0)
            throw new ArgumentException(string.Format("Unexpected error encountered during deserialization of merged responses, expected {0}={1} to be 0", (object) "dataLeftToRead", (object) val1));
          memoryStream.Position = 0L;
          yield return (Stream) memoryStream;
          Array.Clear((Array) tempArray, 0, tempArray.Length);
        }
      }
    }
  }
}
