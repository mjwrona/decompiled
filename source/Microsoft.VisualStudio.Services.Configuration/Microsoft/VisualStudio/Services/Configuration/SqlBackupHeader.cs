// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.SqlBackupHeader
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Configuration
{
  [DebuggerDisplay("DbName: {DatabaseName}, Type: {BackupType}, Size: {BackupSize}")]
  public class SqlBackupHeader
  {
    public string BackupName { get; set; }

    public string BackupDescription { get; set; }

    public BackupType BackupType { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public bool IsCompressed { get; set; }

    public string UserName { get; set; }

    public int Position { get; set; }

    public string ServerName { get; set; }

    public string DatabaseName { get; set; }

    public int DatabaseVersion { get; set; }

    public DateTime DatabaseCreationDate { get; set; }

    public long BackupSize { get; set; }

    public DateTime BackupStartDate { get; set; }

    public DateTime BackupFinishDate { get; set; }

    public DatabaseCompatibilityLevel CompatibilityLevel { get; set; }

    public int SoftwareVersionMajor { get; set; }

    public int SoftwareVersionMinor { get; set; }

    public int SoftwareVersionBuild { get; set; }

    public Version SoftwareVersion => new Version(this.SoftwareVersionMajor, this.SoftwareVersionMinor, this.SoftwareVersionBuild, 0);

    public string MachineName { get; set; }

    public string Collation { get; set; }

    public bool IsSnapshot { get; set; }

    public bool IsReadOnly { get; set; }

    public bool IsSingleUser { get; set; }

    public bool HasBackupChecksums { get; set; }

    public bool IsDamaged { get; set; }

    public bool HasIncompleteMetaData { get; set; }

    public bool IsForceOffline { get; set; }

    public bool IsCopyOnly { get; set; }

    public string BackupTypeDescription { get; set; }

    public Guid BackupSetGUID { get; set; }

    public long CompressedBackupSize { get; set; }
  }
}
