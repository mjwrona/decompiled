// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.PreUploadFileResult
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal struct PreUploadFileResult
  {
    public Guid TempFileDataspaceId;
    public int TempFileId;
    public int ExistingFileId;
    public byte[] ExistingHashValue;

    public PreUploadFileResult(int tempFileId)
    {
      this.TempFileId = tempFileId;
      this.TempFileDataspaceId = Guid.Empty;
      this.ExistingFileId = 0;
      this.ExistingHashValue = (byte[]) null;
    }

    public PreUploadFileResult(int tempFileId, int existingFileId, byte[] existingHashValue)
    {
      this.TempFileId = tempFileId;
      this.TempFileDataspaceId = Guid.Empty;
      this.ExistingFileId = existingFileId;
      this.ExistingHashValue = existingHashValue;
    }

    public PreUploadFileResult(
      int tempFileId,
      Guid tempFileDataspaceId,
      int existingFileId,
      byte[] existingHashValue)
    {
      this.TempFileId = tempFileId;
      this.TempFileDataspaceId = tempFileDataspaceId;
      this.ExistingFileId = existingFileId;
      this.ExistingHashValue = existingHashValue;
    }
  }
}
