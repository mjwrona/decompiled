// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.FileImportTrackingInfo
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal readonly struct FileImportTrackingInfo
  {
    public readonly long StartTime;
    public readonly string FileResourceId;
    public readonly long FileId;
    public readonly long FileLength;
    public readonly string HashValue;

    public FileImportTrackingInfo(
      long startTime,
      string fileResourceId,
      long fileId,
      long fileLength,
      string hashValue)
    {
      this.StartTime = startTime;
      this.FileResourceId = fileResourceId;
      this.FileId = fileId;
      this.FileLength = fileLength;
      this.HashValue = hashValue;
    }
  }
}
