// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Native.Mac.MacFoundation
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Telemetry.Native.Mac
{
  internal static class MacFoundation
  {
    public static class NSBundle
    {
      private static readonly IntPtr class_ptr = MacRuntime.Class.GetHandle(nameof (NSBundle));
      private const string selMainBundle = "mainBundle";
      private static readonly IntPtr selMainBundleHandle = MacRuntime.Selector.GetHandle("mainBundle");
      private const string selInfoDictionary = "infoDictionary";
      private static readonly IntPtr selInfoDictionaryHandle = MacRuntime.Selector.GetHandle("infoDictionary");

      public static string GetVersion()
      {
        IntPtr mainBundle = MacFoundation.NSBundle.GetMainBundle();
        if (mainBundle == IntPtr.Zero)
          return (string) null;
        IntPtr infoDictionary = MacFoundation.NSBundle.GetInfoDictionary(mainBundle);
        if (infoDictionary == IntPtr.Zero)
          return (string) null;
        IntPtr native = MacFoundation.NSString.CreateNative("CFBundleShortVersionString", false);
        IntPtr usrhandle = MacFoundation.NSDictionary.ObjectForKey(infoDictionary, native);
        MacFoundation.NSString.ReleaseNative(native);
        return MacFoundation.NSString.FromHandle(usrhandle);
      }

      public static string GetBundleName()
      {
        IntPtr mainBundle = MacFoundation.NSBundle.GetMainBundle();
        if (mainBundle == IntPtr.Zero)
          return (string) null;
        IntPtr infoDictionary = MacFoundation.NSBundle.GetInfoDictionary(mainBundle);
        if (infoDictionary == IntPtr.Zero)
          return (string) null;
        IntPtr native = MacFoundation.NSString.CreateNative("CFBundleName", false);
        IntPtr usrhandle = MacFoundation.NSDictionary.ObjectForKey(infoDictionary, native);
        MacFoundation.NSString.ReleaseNative(native);
        return MacFoundation.NSString.FromHandle(usrhandle);
      }

      private static IntPtr GetMainBundle() => MacRuntime.Messaging.IntPtr_objc_msgSend(MacFoundation.NSBundle.class_ptr, MacFoundation.NSBundle.selMainBundleHandle);

      private static IntPtr GetInfoDictionary(IntPtr bundle) => MacRuntime.Messaging.IntPtr_objc_msgSend(bundle, MacFoundation.NSBundle.selInfoDictionaryHandle);
    }

    public static class CoreGraphics
    {
      private const string CoreGraphicsLib = "/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics";

      [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
      private static extern uint CGMainDisplayID();

      [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
      private static extern uint CGDisplayPixelsHigh(uint id);

      [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
      private static extern uint CGDisplayPixelsWide(uint id);

      [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
      private static extern int CGGetActiveDisplayList(
        uint max,
        IntPtr activeDisplays,
        IntPtr displayCount);

      public static void GetDisplayInfo(
        ref MacFoundation.CoreGraphics.DisplayInformation info)
      {
        uint id = MacFoundation.CoreGraphics.CGMainDisplayID();
        info.MainDisplayHeight = MacFoundation.CoreGraphics.CGDisplayPixelsHigh(id);
        info.MainDisplayWidth = MacFoundation.CoreGraphics.CGDisplayPixelsWide(id);
        IntPtr num = Marshal.AllocHGlobal(4);
        try
        {
          Marshal.WriteInt32(num, 0);
          if (MacFoundation.CoreGraphics.CGGetActiveDisplayList(0U, IntPtr.Zero, num) != 0)
            return;
          info.DisplayCount = (uint) Marshal.ReadInt32(num);
        }
        finally
        {
          Marshal.FreeHGlobal(num);
        }
      }

      public struct DisplayInformation
      {
        public uint DisplayCount;
        public uint MainDisplayHeight;
        public uint MainDisplayWidth;
      }
    }

    public static class NSFileManager
    {
      private static readonly IntPtr class_ptr = MacRuntime.Class.GetHandle(nameof (NSFileManager));
      private static readonly IntPtr selDefaultManagerHandle = MacRuntime.Selector.GetHandle("defaultManager");
      private static readonly IntPtr selAttributesOfFileSystemForPath_Error_Handle = MacRuntime.Selector.GetHandle("attributesOfFileSystemForPath:error:");

      public static MacFoundation.NSFileManager.NSFileSystemAttributes GetFileSystemAttributesForRoot()
      {
        IntPtr systemAttributes = MacFoundation.NSFileManager.GetFileSystemAttributes(MacFoundation.NSFileManager.GetDefaultFileManager(), "/");
        MacFoundation.NSFileManager.NSFileSystemAttributes attributesForRoot = (MacFoundation.NSFileManager.NSFileSystemAttributes) null;
        if (systemAttributes != IntPtr.Zero)
          attributesForRoot = MacFoundation.NSFileManager.NSFileSystemAttributes.FromDictionary(systemAttributes);
        return attributesForRoot;
      }

      private static IntPtr GetDefaultFileManager() => MacRuntime.Messaging.IntPtr_objc_msgSend(MacFoundation.NSFileManager.class_ptr, MacFoundation.NSFileManager.selDefaultManagerHandle);

      private static IntPtr GetFileSystemAttributes(
        IntPtr nsFileManagerHandle,
        string path,
        out IntPtr error)
      {
        if (nsFileManagerHandle == IntPtr.Zero)
          throw new ArgumentNullException(nameof (nsFileManagerHandle));
        if (path == null)
          throw new ArgumentNullException(nameof (path));
        error = IntPtr.Zero;
        IntPtr native = MacFoundation.NSString.CreateNative(path, false);
        IntPtr systemAttributes = MacRuntime.Messaging.IntPtr_objc_msgSend_IntPtr_ref_IntPtr(nsFileManagerHandle, MacFoundation.NSFileManager.selAttributesOfFileSystemForPath_Error_Handle, native, ref error);
        MacFoundation.NSString.ReleaseNative(native);
        return systemAttributes;
      }

      private static IntPtr GetFileSystemAttributes(IntPtr nsFileManagerHandle, string path)
      {
        IntPtr error = IntPtr.Zero;
        IntPtr systemAttributes = MacFoundation.NSFileManager.GetFileSystemAttributes(nsFileManagerHandle, path, out error);
        return !(error == IntPtr.Zero) ? IntPtr.Zero : systemAttributes;
      }

      public class NSFileSystemAttributes
      {
        internal NSFileSystemAttributes()
        {
        }

        public ulong Size { get; internal set; }

        public ulong FreeSize { get; internal set; }

        internal static MacFoundation.NSFileManager.NSFileSystemAttributes FromDictionary(
          IntPtr dict)
        {
          if (dict == IntPtr.Zero)
            return (MacFoundation.NSFileManager.NSFileSystemAttributes) null;
          MacFoundation.NSFileManager.NSFileSystemAttributes systemAttributes = new MacFoundation.NSFileManager.NSFileSystemAttributes();
          IntPtr native1 = MacFoundation.NSString.CreateNative("NSFileSystemSize", false);
          ulong? nullable = MacFoundation.NSFileManager.NSFileSystemAttributes.Fetch_ulong(dict, native1);
          systemAttributes.Size = nullable.GetValueOrDefault();
          MacFoundation.NSString.ReleaseNative(native1);
          IntPtr native2 = MacFoundation.NSString.CreateNative("NSFileSystemFreeSize", false);
          nullable = MacFoundation.NSFileManager.NSFileSystemAttributes.Fetch_ulong(dict, native2);
          systemAttributes.FreeSize = nullable.GetValueOrDefault();
          MacFoundation.NSString.ReleaseNative(native2);
          return systemAttributes;
        }

        internal static ulong? Fetch_ulong(IntPtr dict, IntPtr key)
        {
          IntPtr handle = MacFoundation.NSDictionary.ObjectForKey(dict, key);
          return !(handle == IntPtr.Zero) ? new ulong?(MacFoundation.NSNumber.UInt64Value(handle)) : new ulong?();
        }
      }
    }

    private static class NSNumber
    {
      private const string selUnsignedLongLongValue = "unsignedLongLongValue";
      private static readonly IntPtr selUnsignedLongLongValueHandle = MacRuntime.Selector.GetHandle("unsignedLongLongValue");

      public static ulong UInt64Value(IntPtr handle) => MacRuntime.Messaging.UInt64_objc_msgSend(handle, MacFoundation.NSNumber.selUnsignedLongLongValueHandle);
    }

    private static class NSDictionary
    {
      private const string selObjectForKey_ = "objectForKey:";
      private static readonly IntPtr selObjectForKey_Handle = MacRuntime.Selector.GetHandle("objectForKey:");

      public static IntPtr ObjectForKey(IntPtr dict, IntPtr key)
      {
        if (dict == IntPtr.Zero)
          throw new ArgumentNullException(nameof (dict));
        if (key == IntPtr.Zero)
          throw new ArgumentNullException(nameof (key));
        return MacRuntime.Messaging.IntPtr_objc_msgSend_IntPtr(dict, MacFoundation.NSDictionary.selObjectForKey_Handle, key);
      }
    }

    private static class NSObject
    {
      internal static void DangerousAutorelease(IntPtr handle) => MacRuntime.Messaging.Void_objc_msgSend(handle, MacRuntime.Selector.AutoreleaseHandle);

      internal static void DangerousRelease(IntPtr handle)
      {
        if (handle == IntPtr.Zero)
          return;
        MacRuntime.Messaging.Void_objc_msgSend(handle, MacRuntime.Selector.ReleaseHandle);
      }
    }

    private static class NSString
    {
      private static readonly IntPtr class_ptr = MacRuntime.Class.GetHandle(nameof (NSString));
      private const string selInitWithCharactersLength = "initWithCharacters:length:";
      private static IntPtr selInitWithCharactersLengthHandle = MacRuntime.Selector.GetHandle("initWithCharacters:length:");
      private const string selUTF8String = "UTF8String";
      private static IntPtr selUTF8StringHandle = MacRuntime.Selector.GetHandle("UTF8String");

      [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
      private static extern IntPtr IntPtr_objc_msgSend_IntPtr_nint(
        IntPtr receiver,
        IntPtr selector,
        IntPtr arg1,
        long arg2);

      public static IntPtr CreateNative(string str, bool autorelease) => str != null ? MacFoundation.NSString.CreateWithCharacters(MacRuntime.Messaging.IntPtr_objc_msgSend(MacFoundation.NSString.class_ptr, MacRuntime.Selector.AllocHandle), str, autorelease) : IntPtr.Zero;

      public static void ReleaseNative(IntPtr handle) => MacFoundation.NSObject.DangerousRelease(handle);

      public static string FromHandle(IntPtr usrhandle) => !(usrhandle == IntPtr.Zero) ? Marshal.PtrToStringAuto(MacRuntime.Messaging.IntPtr_objc_msgSend(usrhandle, MacFoundation.NSString.selUTF8StringHandle)) : (string) null;

      private static unsafe IntPtr CreateWithCharacters(
        IntPtr handle,
        string str,
        bool autorelease = false)
      {
        fixed (char* chPtr = str)
        {
          handle = MacFoundation.NSString.IntPtr_objc_msgSend_IntPtr_nint(handle, MacFoundation.NSString.selInitWithCharactersLengthHandle, (IntPtr) (void*) chPtr, (long) str.Length);
          if (autorelease)
            MacFoundation.NSObject.DangerousAutorelease(handle);
          return handle;
        }
      }
    }
  }
}
