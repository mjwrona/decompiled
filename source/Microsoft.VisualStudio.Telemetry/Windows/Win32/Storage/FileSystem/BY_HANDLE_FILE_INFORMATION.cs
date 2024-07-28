// Decompiled with JetBrains decompiler
// Type: Windows.Win32.Storage.FileSystem.BY_HANDLE_FILE_INFORMATION
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.Runtime.InteropServices.ComTypes;

namespace Windows.Win32.Storage.FileSystem
{
  internal struct BY_HANDLE_FILE_INFORMATION
  {
    internal uint dwFileAttributes;
    internal FILETIME ftCreationTime;
    internal FILETIME ftLastAccessTime;
    internal FILETIME ftLastWriteTime;
    internal uint dwVolumeSerialNumber;
    internal uint nFileSizeHigh;
    internal uint nFileSizeLow;
    internal uint nNumberOfLinks;
    internal uint nFileIndexHigh;
    internal uint nFileIndexLow;
  }
}
