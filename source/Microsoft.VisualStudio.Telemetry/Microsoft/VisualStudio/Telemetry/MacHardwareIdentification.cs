// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.MacHardwareIdentification
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Telemetry
{
  internal static class MacHardwareIdentification
  {
    private const string CoreFoundationLibrary = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";
    private static readonly IntPtr CoreFoundationLibraryHandle = MacHardwareIdentification.dlopen("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation", 0);
    private static readonly IntPtr kCFTypeDictionaryKeyCallBacks = MacHardwareIdentification.dlsym(MacHardwareIdentification.CoreFoundationLibraryHandle, nameof (kCFTypeDictionaryKeyCallBacks));
    private static readonly IntPtr kCFTypeDictionaryValueCallBacks = MacHardwareIdentification.dlsym(MacHardwareIdentification.CoreFoundationLibraryHandle, nameof (kCFTypeDictionaryValueCallBacks));
    private static readonly IntPtr kCFBooleanTrue = Marshal.ReadIntPtr(MacHardwareIdentification.dlsym(MacHardwareIdentification.CoreFoundationLibraryHandle, nameof (kCFBooleanTrue)));
    private static readonly IntPtr kCFAllocatorDefault = Marshal.ReadIntPtr(MacHardwareIdentification.dlsym(MacHardwareIdentification.CoreFoundationLibraryHandle, nameof (kCFAllocatorDefault)));
    private const int KERN_SUCCESS = 0;
    private const string IOKitLibrary = "/System/Library/Frameworks/IOKit.framework/IOKit";
    private static readonly IntPtr IOKitLibraryHandle = MacHardwareIdentification.dlopen("/System/Library/Frameworks/IOKit.framework/IOKit", 0);
    private const string kIOServicePlane = "IOService";
    private const string kIOEthernetInterface = "IOEthernetInterface";
    private const string kIOPrimaryInterface = "IOPrimaryInterface";
    private const string kIOPropertyMatchKey = "IOPropertyMatch";
    private const string kIOMACAddress = "IOMACAddress";
    private static readonly IntPtr kIOMasterPortDefault = Marshal.ReadIntPtr(MacHardwareIdentification.dlsym(MacHardwareIdentification.IOKitLibraryHandle, nameof (kIOMasterPortDefault)));

    public static bool TryGetFirstPrimaryMacAddress(out string macAddress)
    {
      IntPtr interfaceIterator = new IntPtr();
      if (MacHardwareIdentification.TryFindPrimaryEthernetInterfaces(ref interfaceIterator))
        return MacHardwareIdentification.TryGetFirstPrimaryMacAddress(interfaceIterator, out macAddress);
      macAddress = (string) null;
      return false;
    }

    private static bool TryGetFirstPrimaryMacAddress(
      IntPtr interfaceIterator,
      out string macAddress)
    {
      macAddress = (string) null;
      bool primaryMacAddress = false;
      IntPtr entry;
      while (!primaryMacAddress && (entry = MacHardwareIdentification.IOIteratorNext(interfaceIterator)) != IntPtr.Zero)
      {
        IntPtr parent;
        if (MacHardwareIdentification.IORegistryEntryGetParentEntry(entry, "IOService", out parent) != 0)
          return false;
        IntPtr withCstring = MacHardwareIdentification.CFStringCreateWithCString(MacHardwareIdentification.kCFAllocatorDefault, "IOMACAddress", MacHardwareIdentification.CFStringEncoding.kCFStringEncodingASCII);
        IntPtr cfProperty = MacHardwareIdentification.IORegistryEntryCreateCFProperty(parent, withCstring, MacHardwareIdentification.kCFAllocatorDefault, 0U);
        if (cfProperty != IntPtr.Zero)
        {
          byte[] buffer = new byte[6];
          MacHardwareIdentification.CFDataGetBytes(cfProperty, new MacHardwareIdentification.CFRange()
          {
            length = new IntPtr(buffer.Length)
          }, buffer);
          macAddress = string.Format("{0:x2}:{1:x2}:{2:x2}:{3:x2}:{4:x2}:{5:x2}", (object) buffer[0], (object) buffer[1], (object) buffer[2], (object) buffer[3], (object) buffer[4], (object) buffer[5]);
          primaryMacAddress = true;
          MacHardwareIdentification.CFRelease(cfProperty);
        }
        MacHardwareIdentification.CFRelease(withCstring);
        MacHardwareIdentification.IOObjectRelease(parent);
        MacHardwareIdentification.IOObjectRelease(entry);
      }
      return primaryMacAddress;
    }

    private static bool TryFindPrimaryEthernetInterfaces(ref IntPtr interfaceIterator)
    {
      IntPtr num = MacHardwareIdentification.IOServiceMatching("IOEthernetInterface");
      if (num == IntPtr.Zero)
        return false;
      IntPtr mutable = MacHardwareIdentification.CFDictionaryCreateMutable(IntPtr.Zero, IntPtr.Zero, MacHardwareIdentification.kCFTypeDictionaryKeyCallBacks, MacHardwareIdentification.kCFTypeDictionaryValueCallBacks);
      if (num == IntPtr.Zero)
        return false;
      IntPtr key1 = new IntPtr();
      IntPtr key2 = new IntPtr();
      try
      {
        key1 = MacHardwareIdentification.CFStringCreateWithCString(MacHardwareIdentification.kCFAllocatorDefault, "IOPrimaryInterface", MacHardwareIdentification.CFStringEncoding.kCFStringEncodingASCII);
        if (key1 == IntPtr.Zero)
          return false;
        MacHardwareIdentification.CFDictionarySetValue(mutable, key1, MacHardwareIdentification.kCFBooleanTrue);
        key2 = MacHardwareIdentification.CFStringCreateWithCString(MacHardwareIdentification.kCFAllocatorDefault, "IOPropertyMatch", MacHardwareIdentification.CFStringEncoding.kCFStringEncodingASCII);
        if (key2 == IntPtr.Zero)
          return false;
        MacHardwareIdentification.CFDictionarySetValue(num, key2, mutable);
        MacHardwareIdentification.CFRelease(mutable);
      }
      finally
      {
        MacHardwareIdentification.CFRelease(key1);
        MacHardwareIdentification.CFRelease(key2);
      }
      return MacHardwareIdentification.IOServiceGetMatchingServices(MacHardwareIdentification.kIOMasterPortDefault, num, ref interfaceIterator) == 0;
    }

    [DllImport("libc")]
    private static extern IntPtr dlopen(string path, int mode);

    [DllImport("libc")]
    private static extern IntPtr dlsym(IntPtr handle, string symbol);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    private static extern void CFRelease(IntPtr obj);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    private static extern IntPtr CFStringCreateWithCString(
      IntPtr alloc,
      [MarshalAs(UnmanagedType.LPStr)] string str,
      MacHardwareIdentification.CFStringEncoding encoding);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    private static extern IntPtr CFDictionaryCreateMutable(
      IntPtr allocator,
      IntPtr capacity,
      IntPtr keyCallBacks,
      IntPtr valueCallBacks);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    private static extern void CFDictionarySetValue(IntPtr theDict, IntPtr key, IntPtr value);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    private static extern void CFDataGetBytes(
      IntPtr theData,
      MacHardwareIdentification.CFRange range,
      byte[] buffer);

    [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    private static extern IntPtr IOServiceMatching([MarshalAs(UnmanagedType.LPStr)] string serviceName);

    [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    private static extern int IOServiceGetMatchingServices(
      IntPtr masterPort,
      IntPtr matching,
      ref IntPtr existing);

    [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    private static extern void IOObjectRelease(IntPtr obj);

    [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    private static extern IntPtr IOIteratorNext(IntPtr iter);

    [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    private static extern int IORegistryEntryGetParentEntry(
      IntPtr entry,
      [MarshalAs(UnmanagedType.LPStr)] string plane,
      out IntPtr parent);

    [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    private static extern IntPtr IORegistryEntryCreateCFProperty(
      IntPtr entry,
      IntPtr key,
      IntPtr allocator,
      uint options);

    private enum CFStringEncoding : uint
    {
      kCFStringEncodingASCII = 1536, // 0x00000600
    }

    private struct CFRange
    {
      public IntPtr location;
      public IntPtr length;
    }
  }
}
