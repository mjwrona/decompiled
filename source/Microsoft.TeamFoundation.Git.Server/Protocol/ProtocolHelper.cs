// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Protocol.ProtocolHelper
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Git.Server.Protocol
{
  internal static class ProtocolHelper
  {
    private const int c_maxLineLength = 65530;

    public static byte[] GetLine(string line, SidebandChannel channel, bool newLine = true)
    {
      if (line == null)
        return ProtocolHelper.MagicBytes.NullLine;
      int byteCount = GitEncodingUtil.SafeUtf8NoBom.GetByteCount(line);
      int num1 = 65530;
      if (channel != SidebandChannel.None)
        --num1;
      if (byteCount > num1)
        throw new ArgumentException(string.Format("The maximum line length is {0}, but the provided line was {1} characters long.", (object) num1, (object) byteCount), nameof (line));
      int length = byteCount + (4 + (newLine ? 1 : 0));
      if (channel != SidebandChannel.None)
        ++length;
      byte[] bytes = new byte[length];
      for (int index = 0; index < 4; ++index)
      {
        byte num2 = (byte) ((length >> 4 * index & 15) + 48);
        bytes[3 - index] = num2 >= (byte) 58 ? (byte) ((uint) num2 + 39U) : num2;
      }
      if (channel != SidebandChannel.None)
        bytes[4] = (byte) channel;
      GitEncodingUtil.SafeUtf8NoBom.GetBytes(line, 0, line.Length, bytes, channel != SidebandChannel.None ? 5 : 4);
      if (newLine)
        bytes[bytes.Length - 1] = (byte) 10;
      return bytes;
    }

    public static bool IsPrefixMatch(byte[] incomingCommand, byte[] magicBytes) => GitUtils.CompareByteArrays(incomingCommand, 0, magicBytes, 0, magicBytes.Length);

    public static string ReadLine(Stream inputStream)
    {
      byte[] bytes = ProtocolHelper.ReadLineBytes(inputStream);
      return bytes != null ? GitEncodingUtil.SafeUtf8NoBom.GetString(bytes) : (string) null;
    }

    public static byte[] ReadLineBytes(Stream inputStream)
    {
      byte[] buf1 = new byte[4];
      if (buf1.Length != GitStreamUtil.TryReadGreedy(inputStream, buf1, 0, buf1.Length))
        throw new GitProtocolException("Unable to read the 4-byte length header of the message.");
      ushort length = 0;
      for (int index = 0; index < 4; ++index)
      {
        byte num1 = buf1[index];
        byte num2;
        if (num1 >= (byte) 48 && num1 <= (byte) 57)
          num2 = (byte) 48;
        else if (num1 >= (byte) 65 && num1 <= (byte) 70)
          num2 = (byte) 55;
        else if (num1 >= (byte) 97 && num1 <= (byte) 102)
          num2 = (byte) 87;
        else
          throw new GitProtocolException(FormattableString.Invariant(FormattableStringFactory.Create("While decoding a message length, expected to find a hex digit in ASCII form; found {0:x2} instead.", (object) num1)));
        length |= (ushort) ((int) num1 - (int) num2 << 4 * (3 - index));
      }
      if (length == (ushort) 0)
        return (byte[]) null;
      if (length == (ushort) 1)
        return new byte[(int) length];
      byte[] buf2 = length > (ushort) 4 && length <= (ushort) 65520 ? new byte[(int) (ushort) ((uint) length - 4U)] : throw new GitProtocolException(FormattableString.Invariant(FormattableStringFactory.Create("Invalid pkt-line length: {0}", (object) length)));
      if (buf2.Length != GitStreamUtil.TryReadGreedy(inputStream, buf2, 0, buf2.Length))
        throw new GitProtocolException(string.Format("Unable to read all {0} bytes of the message.", (object) buf2.Length));
      return buf2;
    }

    public static void WriteDelimLine(Stream outputStream)
    {
      outputStream.Write(ProtocolHelper.MagicBytes.DelimPkt, 0, ProtocolHelper.MagicBytes.DelimPkt.Length);
      outputStream.Flush();
    }

    public static void WriteLine(Stream outputStream, string line)
    {
      byte[] line1 = ProtocolHelper.GetLine(line, SidebandChannel.None);
      outputStream.Write(line1, 0, line1.Length);
      outputStream.Flush();
    }

    public static void WriteNestedSidebandLine(
      Stream outputStream,
      SidebandChannel channel,
      string line)
    {
      byte[] line1 = ProtocolHelper.GetLine(line, SidebandChannel.None);
      ProtocolHelper.WriteSideband(outputStream, channel, line1, 0, line1.Length);
      outputStream.Flush();
    }

    public static void WriteSideband(Stream outputStream, SidebandChannel channel, string message)
    {
      byte[] line = ProtocolHelper.GetLine(message, channel, false);
      outputStream.Write(line, 0, line.Length);
      outputStream.Flush();
    }

    public static void WriteSideband(
      Stream outputStream,
      SidebandChannel channel,
      byte[] buffer,
      int offset,
      int count)
    {
      int val2 = 8192;
      for (; count > 0; count -= val2)
      {
        int length = Math.Min(count, val2);
        byte[] numArray = new byte[length + 5];
        GitEncodingUtil.SafeUtf8NoBom.GetBytes(numArray.Length.ToString("x4"), 0, 4, numArray, 0);
        numArray[4] = (byte) channel;
        Array.Copy((Array) buffer, offset, (Array) numArray, 5, length);
        outputStream.Write(numArray, 0, numArray.Length);
      }
      outputStream.Flush();
    }

    public static void WriteSidebandLine(Stream outputStream, SidebandChannel channel, string line)
    {
      byte[] line1 = ProtocolHelper.GetLine(line, channel);
      outputStream.Write(line1, 0, line1.Length);
      outputStream.Flush();
    }

    public static class MagicBytes
    {
      public static readonly byte[] Agent = new byte[6]
      {
        (byte) 97,
        (byte) 103,
        (byte) 101,
        (byte) 110,
        (byte) 116,
        (byte) 61
      };
      public static readonly byte[] Command = new byte[8]
      {
        (byte) 99,
        (byte) 111,
        (byte) 109,
        (byte) 109,
        (byte) 97,
        (byte) 110,
        (byte) 100,
        (byte) 61
      };
      public static readonly byte[] DeepenPrefix = new byte[7]
      {
        (byte) 100,
        (byte) 101,
        (byte) 101,
        (byte) 112,
        (byte) 101,
        (byte) 110,
        (byte) 32
      };
      public static readonly byte[] FilterPrefix = new byte[7]
      {
        (byte) 102,
        (byte) 105,
        (byte) 108,
        (byte) 116,
        (byte) 101,
        (byte) 114,
        (byte) 32
      };
      public static readonly byte[] DelimPkt = new byte[4]
      {
        (byte) 48,
        (byte) 48,
        (byte) 48,
        (byte) 49
      };
      public static readonly byte[] Done = new byte[5]
      {
        (byte) 100,
        (byte) 111,
        (byte) 110,
        (byte) 101,
        (byte) 10
      };
      public static readonly byte[] Fetch = new byte[5]
      {
        (byte) 102,
        (byte) 101,
        (byte) 116,
        (byte) 99,
        (byte) 104
      };
      public static readonly byte[] HavePrefix = new byte[5]
      {
        (byte) 104,
        (byte) 97,
        (byte) 118,
        (byte) 101,
        (byte) 32
      };
      public static readonly byte[] IncludeTag = new byte[11]
      {
        (byte) 105,
        (byte) 110,
        (byte) 99,
        (byte) 108,
        (byte) 117,
        (byte) 100,
        (byte) 101,
        (byte) 45,
        (byte) 116,
        (byte) 97,
        (byte) 103
      };
      public static readonly byte[] LsRefs = new byte[7]
      {
        (byte) 108,
        (byte) 115,
        (byte) 45,
        (byte) 114,
        (byte) 101,
        (byte) 102,
        (byte) 115
      };
      public static readonly byte[] NoProgress = new byte[11]
      {
        (byte) 110,
        (byte) 111,
        (byte) 45,
        (byte) 112,
        (byte) 114,
        (byte) 111,
        (byte) 103,
        (byte) 114,
        (byte) 101,
        (byte) 115,
        (byte) 115
      };
      public static readonly byte[] NullLine = new byte[4]
      {
        (byte) 48,
        (byte) 48,
        (byte) 48,
        (byte) 48
      };
      public static readonly byte[] OfsDelta = new byte[9]
      {
        (byte) 111,
        (byte) 102,
        (byte) 115,
        (byte) 45,
        (byte) 100,
        (byte) 101,
        (byte) 108,
        (byte) 116,
        (byte) 97
      };
      public static readonly byte[] Peel = new byte[4]
      {
        (byte) 112,
        (byte) 101,
        (byte) 101,
        (byte) 108
      };
      public static readonly byte[] RefPrefix = new byte[11]
      {
        (byte) 114,
        (byte) 101,
        (byte) 102,
        (byte) 45,
        (byte) 112,
        (byte) 114,
        (byte) 101,
        (byte) 102,
        (byte) 105,
        (byte) 120,
        (byte) 32
      };
      public static readonly byte[] Shallow = new byte[7]
      {
        (byte) 115,
        (byte) 104,
        (byte) 97,
        (byte) 108,
        (byte) 108,
        (byte) 111,
        (byte) 119
      };
      public static readonly byte[] ShallowPrefix = new byte[8]
      {
        (byte) 115,
        (byte) 104,
        (byte) 97,
        (byte) 108,
        (byte) 108,
        (byte) 111,
        (byte) 119,
        (byte) 32
      };
      public static readonly byte[] Symrefs = new byte[7]
      {
        (byte) 115,
        (byte) 121,
        (byte) 109,
        (byte) 114,
        (byte) 101,
        (byte) 102,
        (byte) 115
      };
      public static readonly byte[] ThinPack = new byte[9]
      {
        (byte) 116,
        (byte) 104,
        (byte) 105,
        (byte) 110,
        (byte) 45,
        (byte) 112,
        (byte) 97,
        (byte) 99,
        (byte) 107
      };
      public static readonly byte[] WantPrefix = new byte[5]
      {
        (byte) 119,
        (byte) 97,
        (byte) 110,
        (byte) 116,
        (byte) 32
      };
    }
  }
}
