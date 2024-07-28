// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Native.Mac.MacRuntime
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Telemetry.Native.Mac
{
  internal static class MacRuntime
  {
    public const string LIBOBJC_DYLIB = "/usr/lib/libobjc.dylib";

    internal static class Messaging
    {
      [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
      public static extern void Void_objc_msgSend(IntPtr receiver, IntPtr selector);

      [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
      public static extern IntPtr IntPtr_objc_msgSend(IntPtr receiver, IntPtr selector);

      [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
      public static extern IntPtr IntPtr_objc_msgSend_IntPtr_ref_IntPtr(
        IntPtr receiver,
        IntPtr selector,
        IntPtr arg1,
        ref IntPtr arg2);

      [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
      public static extern IntPtr IntPtr_objc_msgSend_IntPtr(
        IntPtr receiver,
        IntPtr selector,
        IntPtr arg1);

      [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
      public static extern ulong UInt64_objc_msgSend(IntPtr receiver, IntPtr selector);

      [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
      public static extern double Double_objc_msgSend(IntPtr receiver, IntPtr selector);
    }

    internal static class Selector
    {
      private const string Alloc = "alloc";
      internal static IntPtr AllocHandle = MacRuntime.Selector.GetHandle("alloc");
      private const string Release = "release";
      internal static IntPtr ReleaseHandle = MacRuntime.Selector.GetHandle("release");
      private const string Autorelease = "autorelease";
      internal static IntPtr AutoreleaseHandle = MacRuntime.Selector.GetHandle("autorelease");

      [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "sel_registerName")]
      public static extern IntPtr GetHandle(string name);
    }

    internal static class Class
    {
      [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_getClass")]
      private static extern IntPtr GetObjcClass(string name);

      public static IntPtr GetHandle(string name) => MacRuntime.Class.GetObjcClass(name);
    }
  }
}
