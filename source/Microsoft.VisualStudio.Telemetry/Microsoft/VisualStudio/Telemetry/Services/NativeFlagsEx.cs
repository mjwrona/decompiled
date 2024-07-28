// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Services.NativeFlagsEx
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.IO;
using Windows.Win32.Storage.FileSystem;

namespace Microsoft.VisualStudio.Telemetry.Services
{
  internal static class NativeFlagsEx
  {
    internal static FILE_ACCESS_FLAGS ToFILE_ACCESS_FLAGS(this FileAccess fileAccess)
    {
      switch (fileAccess)
      {
        case FileAccess.Read:
          return FILE_ACCESS_FLAGS.FILE_GENERIC_READ;
        case FileAccess.Write:
          return FILE_ACCESS_FLAGS.FILE_GENERIC_WRITE;
        case FileAccess.ReadWrite:
          return FILE_ACCESS_FLAGS.FILE_GENERIC_WRITE | FILE_ACCESS_FLAGS.FILE_READ_DATA | FILE_ACCESS_FLAGS.FILE_READ_EA | FILE_ACCESS_FLAGS.FILE_READ_ATTRIBUTES;
        default:
          throw new NotImplementedException();
      }
    }

    internal static FILE_SHARE_MODE ToFILE_SHARE_MODE(this FileShare fileShare)
    {
      switch (fileShare)
      {
        case FileShare.None:
          return FILE_SHARE_MODE.FILE_SHARE_NONE;
        case FileShare.Read:
          return FILE_SHARE_MODE.FILE_SHARE_READ;
        case FileShare.Write:
          return FILE_SHARE_MODE.FILE_SHARE_WRITE;
        case FileShare.ReadWrite:
          return FILE_SHARE_MODE.FILE_SHARE_READ | FILE_SHARE_MODE.FILE_SHARE_WRITE;
        case FileShare.Delete:
          return FILE_SHARE_MODE.FILE_SHARE_DELETE;
        default:
          throw new NotImplementedException();
      }
    }
  }
}
