// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.ByteStreamCompare
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using System;
using System.ComponentModel;
using System.IO;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ByteStreamCompare
  {
    public static bool Equals(Stream stream1, Stream stream2, byte[] buffer1, byte[] buffer2)
    {
      if (stream1 == null)
        throw new ArgumentNullException(nameof (stream1));
      if (stream2 == null)
        throw new ArgumentNullException(nameof (stream2));
      if (buffer1 == null)
        throw new ArgumentNullException(nameof (buffer1));
      if (buffer2 == null)
        throw new ArgumentNullException(nameof (buffer2));
      if (stream1.CanSeek && stream2.CanSeek && stream1.Length != stream2.Length)
        return false;
      int num1 = stream1.Read(buffer1, 0, buffer1.Length);
      int num2 = stream2.Read(buffer2, 0, buffer2.Length);
      int index1 = 0;
      int index2 = 0;
      while (num1 > 0 && num2 > 0)
      {
        for (; index1 < num1 && index2 < num2 && (int) buffer1[index1] == (int) buffer2[index2]; ++index2)
          ++index1;
        if (index1 != num1 && index2 != num2)
          return false;
        if (index1 == num1)
        {
          num1 = stream1.Read(buffer1, 0, buffer1.Length);
          index1 = 0;
        }
        if (index2 == num2)
        {
          num2 = stream2.Read(buffer2, 0, buffer2.Length);
          index2 = 0;
        }
      }
      return num1 == 0 && num2 == 0;
    }

    public static bool Equals(string file1, string file2, int bufferSize)
    {
      byte[] buffer1 = new byte[bufferSize];
      byte[] buffer2 = new byte[bufferSize];
      using (FileStream stream1 = new FileStream(file1, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize))
      {
        using (FileStream stream2 = new FileStream(file2, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize))
          return ByteStreamCompare.Equals((Stream) stream1, (Stream) stream2, buffer1, buffer2);
      }
    }
  }
}
