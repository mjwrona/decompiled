// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.MetricsStreaming.Bloomfilter.JenkinsHash
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;
using System.Text;

namespace Microsoft.Cloud.Metrics.Client.MetricsStreaming.Bloomfilter
{
  internal class JenkinsHash
  {
    public static ulong HashSafe(string str, ulong seed)
    {
      ulong num = JenkinsHash.HashSafe(Encoding.UTF8.GetBytes(str), seed);
      if (false)
        Console.WriteLine("HashDebug: {0}\t{1}", (object) str, (object) num);
      return num;
    }

    public static ulong HashSafe(byte[] bytes, ulong seed) => JenkinsHash.HashSafe(bytes, bytes.Length, seed);

    public static ulong HashSafe(byte[] bytes, int length, ulong seed)
    {
      ulong num1 = (ulong) length;
      ulong b;
      ulong a = b = seed;
      ulong c1 = 11400714819323198483;
      int index = 0;
      for (; num1 >= 24UL; num1 -= 24UL)
      {
        a += (ulong) ((long) bytes[index] + ((long) bytes[index + 1] << 8) + ((long) bytes[index + 2] << 16) + ((long) bytes[index + 3] << 24) + ((long) bytes[index + 4] << 32) + ((long) bytes[index + 5] << 40) + ((long) bytes[index + 6] << 48) + ((long) bytes[index + 7] << 56));
        b += (ulong) ((long) bytes[index + 8] + ((long) bytes[index + 9] << 8) + ((long) bytes[index + 10] << 16) + ((long) bytes[index + 11] << 24) + ((long) bytes[index + 12] << 32) + ((long) bytes[index + 13] << 40) + ((long) bytes[index + 14] << 48) + ((long) bytes[index + 15] << 56));
        c1 += (ulong) ((long) bytes[index + 16] + ((long) bytes[index + 17] << 8) + ((long) bytes[index + 18] << 16) + ((long) bytes[index + 19] << 24) + ((long) bytes[index + 20] << 32) + ((long) bytes[index + 21] << 40) + ((long) bytes[index + 22] << 48) + ((long) bytes[index + 23] << 56));
        JenkinsHash.Mix64(ref a, ref b, ref c1);
        index += 24;
      }
      ulong c2 = c1 + (ulong) length;
      long num2 = (long) num1 - 1L;
      if ((ulong) num2 <= 22UL)
      {
        switch ((uint) num2)
        {
          case 0:
            a += (ulong) bytes[index];
            break;
          case 1:
            a += (ulong) bytes[index + 1] << 8;
            goto case 0;
          case 2:
            a += (ulong) bytes[index + 2] << 16;
            goto case 1;
          case 3:
            a += (ulong) bytes[index + 3] << 24;
            goto case 2;
          case 4:
            a += (ulong) bytes[index + 4] << 32;
            goto case 3;
          case 5:
            a += (ulong) bytes[index + 5] << 40;
            goto case 4;
          case 6:
            a += (ulong) bytes[index + 6] << 48;
            goto case 5;
          case 7:
            a += (ulong) bytes[index + 7] << 56;
            goto case 6;
          case 8:
            b += (ulong) bytes[index + 8];
            goto case 7;
          case 9:
            b += (ulong) bytes[index + 9] << 8;
            goto case 8;
          case 10:
            b += (ulong) bytes[index + 10] << 16;
            goto case 9;
          case 11:
            b += (ulong) bytes[index + 11] << 24;
            goto case 10;
          case 12:
            b += (ulong) bytes[index + 12] << 32;
            goto case 11;
          case 13:
            b += (ulong) bytes[index + 13] << 40;
            goto case 12;
          case 14:
            b += (ulong) bytes[index + 14] << 48;
            goto case 13;
          case 15:
            b += (ulong) bytes[index + 15] << 56;
            goto case 14;
          case 16:
            c2 += (ulong) bytes[index + 16] << 8;
            goto case 15;
          case 17:
            c2 += (ulong) bytes[index + 17] << 16;
            goto case 16;
          case 18:
            c2 += (ulong) bytes[index + 18] << 24;
            goto case 17;
          case 19:
            c2 += (ulong) bytes[index + 19] << 32;
            goto case 18;
          case 20:
            c2 += (ulong) bytes[index + 20] << 40;
            goto case 19;
          case 21:
            c2 += (ulong) bytes[index + 21] << 48;
            goto case 20;
          case 22:
            c2 += (ulong) bytes[index + 22] << 56;
            goto case 21;
        }
      }
      JenkinsHash.Mix64(ref a, ref b, ref c2);
      return c2;
    }

    private static void Mix64(ref ulong a, ref ulong b, ref ulong c)
    {
      a -= b;
      a -= c;
      a ^= c >> 43;
      b -= c;
      b -= a;
      b ^= a << 9;
      c -= a;
      c -= b;
      c ^= b >> 8;
      a -= b;
      a -= c;
      a ^= c >> 38;
      b -= c;
      b -= a;
      b ^= a << 23;
      c -= a;
      c -= b;
      c ^= b >> 5;
      a -= b;
      a -= c;
      a ^= c >> 35;
      b -= c;
      b -= a;
      b ^= a << 49;
      c -= a;
      c -= b;
      c ^= b >> 11;
      a -= b;
      a -= c;
      a ^= c >> 12;
      b -= c;
      b -= a;
      b ^= a << 18;
      c -= a;
      c -= b;
      c ^= b >> 22;
    }
  }
}
