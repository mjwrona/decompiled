// Decompiled with JetBrains decompiler
// Type: Windows.Win32.Storage.FileSystem.FILE_SHARE_MODE
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;

namespace Windows.Win32.Storage.FileSystem
{
  [Flags]
  internal enum FILE_SHARE_MODE : uint
  {
    FILE_SHARE_NONE = 0,
    FILE_SHARE_DELETE = 4,
    FILE_SHARE_READ = 1,
    FILE_SHARE_WRITE = 2,
  }
}
