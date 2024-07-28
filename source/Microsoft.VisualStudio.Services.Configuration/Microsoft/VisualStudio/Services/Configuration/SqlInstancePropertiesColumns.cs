// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.SqlInstancePropertiesColumns
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Configuration
{
  internal class SqlInstancePropertiesColumns : ObjectBinder<SqlInstanceProperties>
  {
    private SqlColumnBinder m_productVersionColumn = new SqlColumnBinder("ProductVersion");
    private SqlColumnBinder m_productLevelColumn = new SqlColumnBinder("ProductLevel");
    private SqlColumnBinder m_editionColumn = new SqlColumnBinder("Edition");
    private SqlColumnBinder m_engineEditionColumn = new SqlColumnBinder("EngineEdition");
    private SqlColumnBinder m_instanceNameColumn = new SqlColumnBinder("InstanceName");
    private SqlColumnBinder m_isFullTextInstalledColumn = new SqlColumnBinder("IsFullTextInstalled");
    private SqlColumnBinder m_isClusteredColumn = new SqlColumnBinder("IsClustered");
    private SqlColumnBinder m_collationColumn = new SqlColumnBinder("Collation");
    private SqlColumnBinder m_licenseTypeColumn = new SqlColumnBinder("LicenseType");
    private SqlColumnBinder m_numLicensesColumn = new SqlColumnBinder("NumLicenses");
    private SqlColumnBinder m_machineNameColumn = new SqlColumnBinder("MachineName");
    private SqlColumnBinder m_defaultDataPathColumn = new SqlColumnBinder("DefaultDataPath");
    private SqlColumnBinder m_defaultLogPathColumn = new SqlColumnBinder("DefaultLogPath");
    private SqlColumnBinder m_masterDbPathColumn = new SqlColumnBinder("MasterDbPath");
    private SqlColumnBinder m_masterDbLogPathColumn = new SqlColumnBinder("MasterDbLogPath");
    private SqlColumnBinder m_backupDirectoryColumn = new SqlColumnBinder("BackupDirectory");
    private SqlColumnBinder m_backupCompressionDefaultColumn = new SqlColumnBinder("BackupCompressionDefault");
    private SqlColumnBinder m_hostPlatformColumn = new SqlColumnBinder("HostPlatform");
    private SqlColumnBinder m_hostDistributionColumn = new SqlColumnBinder("HostDistribution");
    private SqlColumnBinder m_hostReleaseColumn = new SqlColumnBinder("HostRelease");

    protected override SqlInstanceProperties Bind()
    {
      string version = this.m_productVersionColumn.GetString((IDataReader) this.Reader, false);
      return new SqlInstanceProperties()
      {
        ProductVersionString = version,
        ProductVersion = new Version(version),
        ProductLevel = this.m_productLevelColumn.GetString((IDataReader) this.Reader, false),
        Edition = this.m_editionColumn.GetString((IDataReader) this.Reader, false),
        EngineEdition = this.m_engineEditionColumn.GetInt32((IDataReader) this.Reader),
        InstanceName = this.m_instanceNameColumn.GetString((IDataReader) this.Reader, true),
        IsFullTextInstalled = this.m_isFullTextInstalledColumn.GetInt32((IDataReader) this.Reader) != 0,
        IsClustered = this.m_isClusteredColumn.GetInt32((IDataReader) this.Reader, 0) != 0,
        Collation = this.m_collationColumn.GetString((IDataReader) this.Reader, true),
        LicenseType = this.m_licenseTypeColumn.GetString((IDataReader) this.Reader, false),
        NumLicenses = this.m_numLicensesColumn.GetInt32((IDataReader) this.Reader, int.MaxValue),
        MachineName = this.m_machineNameColumn.GetString((IDataReader) this.Reader, true),
        DefaultDataPath = this.m_defaultDataPathColumn.GetString((IDataReader) this.Reader, false),
        DefaultLogPath = this.m_defaultLogPathColumn.GetString((IDataReader) this.Reader, false),
        MasterDbPath = this.m_masterDbPathColumn.GetString((IDataReader) this.Reader, false),
        MasterDbLogPath = this.m_masterDbLogPathColumn.GetString((IDataReader) this.Reader, false),
        BackupDirectory = this.m_backupDirectoryColumn.GetString((IDataReader) this.Reader, false),
        BackupCompressionDefault = (BackupCompressionOptions) this.m_backupCompressionDefaultColumn.GetInt32((IDataReader) this.Reader),
        HostPlatform = this.m_hostPlatformColumn.GetString((IDataReader) this.Reader, false),
        HostDistribution = this.m_hostDistributionColumn.GetString((IDataReader) this.Reader, true),
        HostRelease = this.m_hostReleaseColumn.GetString((IDataReader) this.Reader, true)
      };
    }
  }
}
