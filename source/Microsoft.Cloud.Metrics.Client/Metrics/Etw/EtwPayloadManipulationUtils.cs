// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Metrics.Etw.EtwPayloadManipulationUtils
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Cloud.Metrics.Client.Metrics.Etw
{
  internal static class EtwPayloadManipulationUtils
  {
    public static IntPtr Shift(IntPtr ptr, int offset) => new IntPtr(ptr.ToInt64() + (long) offset);

    public static unsafe IntPtr WriteString(
      string value,
      IntPtr pointerInPayload,
      byte[] bytesBuffer)
    {
      int bytes = Encoding.UTF8.GetBytes(value, 0, value.Length, bytesBuffer, 0);
      *(short*) (void*) pointerInPayload = (short) (ushort) bytes;
      pointerInPayload = new IntPtr(pointerInPayload.ToInt64() + 2L);
      Marshal.Copy(bytesBuffer, 0, pointerInPayload, bytes);
      return new IntPtr(pointerInPayload.ToInt64() + (long) bytes);
    }

    public static unsafe string ReadString(ref IntPtr pointerInPayload)
    {
      ushort length = *(ushort*) (void*) pointerInPayload;
      pointerInPayload = new IntPtr(pointerInPayload.ToInt64() + 2L);
      string str = new string((sbyte*) (void*) pointerInPayload, 0, (int) length, Encoding.UTF8);
      pointerInPayload = new IntPtr(pointerInPayload.ToInt64() + (long) length);
      return str;
    }
  }
}
