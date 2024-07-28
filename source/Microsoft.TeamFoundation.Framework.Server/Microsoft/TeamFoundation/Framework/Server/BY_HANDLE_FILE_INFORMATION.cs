// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BY_HANDLE_FILE_INFORMATION
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal struct BY_HANDLE_FILE_INFORMATION
  {
    public uint dwFileAttributes;
    public FILETIME ftCreationTime;
    public FILETIME ftLastAccessTime;
    public FILETIME ftLastWriteTime;
    public uint dwVolumeSerialNumber;
    public uint nFileSizeHigh;
    public uint nFileSizeLow;
    public uint nNumberOfLinks;
    public uint nFileIndexHigh;
    public uint nFileIndexLow;
  }
}
