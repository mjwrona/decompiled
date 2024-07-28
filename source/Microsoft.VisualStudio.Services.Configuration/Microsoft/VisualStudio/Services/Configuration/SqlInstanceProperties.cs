// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.SqlInstanceProperties
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Configuration
{
  [DebuggerDisplay("Name: {InstanceName}, Machine: {MachineName}")]
  public class SqlInstanceProperties
  {
    private const string c_expressEdition = "Express Edition";
    private const string c_enterpriseEdition = "Enterprise Edition";
    private const string c_enterpriseEvaluationEdition = "Enterprise Evaluation Edition";
    private const string c_dataCenterEdition = "Data Center Edition";
    private const string c_developerEdition = "Developer Edition";

    public string ProductVersionString { get; set; }

    public Version ProductVersion { get; set; }

    public string ProductLevel { get; set; }

    public string Edition { get; set; }

    public int EngineEdition { get; set; }

    public string InstanceName { get; set; }

    public bool IsFullTextInstalled { get; set; }

    public bool IsClustered { get; set; }

    public string Collation { get; set; }

    public string LicenseType { get; set; }

    public int NumLicenses { get; set; }

    public string MachineName { get; set; }

    public string DefaultDataPath { get; set; }

    public string DefaultLogPath { get; set; }

    public string MasterDbPath { get; set; }

    public string MasterDbLogPath { get; set; }

    public string BackupDirectory { get; set; }

    public BackupCompressionOptions BackupCompressionDefault { get; set; }

    public string HostPlatform { get; set; }

    public string HostDistribution { get; set; }

    public string HostRelease { get; set; }

    public bool SupportsBackupCompression => this.BackupCompressionDefault != BackupCompressionOptions.NotSupported;

    public bool IsExpressEdition => this.Edition.StartsWith("Express Edition", StringComparison.OrdinalIgnoreCase);

    public bool IsEnterpriseEdition => this.Edition.StartsWith("Enterprise Edition", StringComparison.OrdinalIgnoreCase) || this.Edition.StartsWith("Enterprise Evaluation Edition", StringComparison.OrdinalIgnoreCase);

    public bool IsDataCenterEdition => this.Edition.StartsWith("Data Center Edition", StringComparison.OrdinalIgnoreCase);

    public bool IsDeveloperEdition => this.Edition.StartsWith("Developer Edition", StringComparison.OrdinalIgnoreCase);

    public bool SupportsPageCompression
    {
      get
      {
        if (this.ProductVersion >= new Version("13.0.4001.0"))
          return true;
        return (this.IsEnterpriseEdition || this.IsDeveloperEdition || this.IsDataCenterEdition) && this.ProductVersion > new Version("10.00");
      }
    }

    public bool SupportsOnlineIndexing => (this.IsEnterpriseEdition || this.IsDeveloperEdition || this.IsDataCenterEdition) && this.ProductVersion > new Version("10.00");

    public bool IsLocal => ComputerInfo.IsLocalMachine(this.MachineName);

    public bool IsLinux => string.Equals(this.HostPlatform, "Linux", StringComparison.OrdinalIgnoreCase);

    public bool Is64Bit => this.Edition.IndexOf("(64-bit)", StringComparison.OrdinalIgnoreCase) >= 0;
  }
}
