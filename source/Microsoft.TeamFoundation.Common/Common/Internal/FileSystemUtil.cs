// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Internal.FileSystemUtil
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class FileSystemUtil
  {
    public static string GetUniversalName(string localPath)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(localPath, nameof (localPath));
      IntPtr num = IntPtr.Zero;
      try
      {
        int lpBufferSize = 0;
        int universalName1 = NativeMethods.WNetGetUniversalName(localPath, NativeMethods.UNIVERSAL_NAME_INFO_LEVEL, (IntPtr) IntPtr.Size, ref lpBufferSize);
        if (universalName1 != NativeMethods.ERROR_MORE_DATA)
          throw new Win32Exception(universalName1);
        num = Marshal.AllocCoTaskMem(lpBufferSize);
        int universalName2 = NativeMethods.WNetGetUniversalName(localPath, NativeMethods.UNIVERSAL_NAME_INFO_LEVEL, num, ref lpBufferSize);
        if (universalName2 != 0)
          throw new Win32Exception(universalName2);
        string stringAuto = Marshal.PtrToStringAuto(new IntPtr(num.ToInt64() + (long) IntPtr.Size), lpBufferSize);
        return stringAuto.Substring(0, stringAuto.IndexOf(char.MinValue));
      }
      finally
      {
        Marshal.FreeCoTaskMem(num);
      }
    }
  }
}
