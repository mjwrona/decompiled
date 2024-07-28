// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.SecureStringHelper
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Azure.NotificationHubs
{
  internal static class SecureStringHelper
  {
    public static unsafe SecureString ToSecureString(this string unsecureString)
    {
      if (unsecureString == null)
        return (SecureString) null;
      fixed (char* chPtr = unsecureString)
      {
        SecureString secureString = new SecureString(chPtr, unsecureString.Length);
        secureString.MakeReadOnly();
        return secureString;
      }
    }

    public static string ToUnsecureString(this SecureString secureString)
    {
      if (secureString == null)
        return (string) null;
      IntPtr num = IntPtr.Zero;
      try
      {
        num = Marshal.SecureStringToGlobalAllocUnicode(secureString);
        return Marshal.PtrToStringUni(num);
      }
      finally
      {
        Marshal.ZeroFreeGlobalAllocUnicode(num);
      }
    }
  }
}
