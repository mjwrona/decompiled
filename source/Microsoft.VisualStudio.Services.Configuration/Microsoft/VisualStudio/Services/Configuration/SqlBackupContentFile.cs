// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.SqlBackupContentFile
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class SqlBackupContentFile
  {
    public string LogicalName { get; set; }

    public string PhysicalName { get; set; }

    public DatabaseFileType FileType { get; set; }

    public string FileGroupName { get; set; }

    public long Size { get; set; }

    public long MaxSize { get; set; }

    public long FileId { get; set; }

    public Guid UniqueId { get; set; }

    public long BackupSizeInBytes { get; set; }

    public int SourceBlockSize { get; set; }

    public int FileGroupId { get; set; }

    public bool IsReadOnly { get; set; }

    public bool IsPresent { get; set; }
  }
}
