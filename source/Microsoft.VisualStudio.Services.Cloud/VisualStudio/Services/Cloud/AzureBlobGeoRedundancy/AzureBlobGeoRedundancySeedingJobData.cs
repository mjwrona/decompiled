// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy.AzureBlobGeoRedundancySeedingJobData
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy
{
  public class AzureBlobGeoRedundancySeedingJobData
  {
    private string m_drawerName;

    public string DrawerName
    {
      get => this.m_drawerName ?? "ConfigurationSecrets";
      set => this.m_drawerName = value;
    }

    public string PrimaryLookupKey { get; set; }

    public string SecondaryLookupKey { get; set; }

    public bool OverwriteExisting { get; set; }

    public bool ContinueOnError { get; set; }

    public int MaxRetries { get; set; } = 1;
  }
}
