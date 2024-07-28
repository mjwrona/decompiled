// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DELTA_HEADER_INFO
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal struct DELTA_HEADER_INFO
  {
    public DELTA_FILE_TYPE FileTypeSet;
    public DELTA_FILE_TYPE FileType;
    public DELTA_FLAG_TYPE Flags;
    public IntPtr TargetSize;
    public FILETIME TargetFileTime;
    public int TargetHashAlgId;
    public DELTA_HASH TargetHash;
  }
}
