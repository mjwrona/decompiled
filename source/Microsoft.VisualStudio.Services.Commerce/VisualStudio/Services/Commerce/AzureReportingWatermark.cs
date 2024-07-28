// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureReportingWatermark
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class AzureReportingWatermark : IEquatable<AzureReportingWatermark>
  {
    private const string WatermarkRegistryPathBase = "/Service/Commerce/Reporting/";
    private const string WatermarkRegistryChildItem = "/Watermark";
    private const char RegistryValueSeparator = '|';

    private AzureReportingWatermark()
    {
    }

    public static AzureReportingWatermark FromRegistryEntry(
      RegistryEntry registryEntry,
      string viewName)
    {
      string[] strArray = registryEntry.Value.Split('|');
      return new AzureReportingWatermark()
      {
        PartitionKey = strArray[0],
        RowKey = strArray[1],
        ViewName = viewName
      };
    }

    public static AzureReportingWatermark FromEntity(ITableEntity tableEntity, string viewName) => new AzureReportingWatermark()
    {
      PartitionKey = tableEntity.PartitionKey,
      RowKey = tableEntity.RowKey,
      ViewName = viewName
    };

    public static string GetRegistryPath(string viewName) => "/Service/Commerce/Reporting/" + viewName + "/Watermark";

    public RegistryEntry ToRegistryEntry() => new RegistryEntry(AzureReportingWatermark.GetRegistryPath(this.ViewName), this.GetRegistryValue());

    public override bool Equals(object obj) => this.Equals(obj as AzureReportingWatermark);

    public override int GetHashCode() => this.PartitionKey.GetHashCode() ^ this.RowKey.GetHashCode() ^ this.ViewName.GetHashCode();

    public bool Equals(AzureReportingWatermark other) => other != null && this.PartitionKey == other.PartitionKey && this.RowKey == other.RowKey && this.ViewName == other.ViewName;

    private string GetRegistryValue() => this.PartitionKey + (object) '|' + this.RowKey;

    public string ViewName { get; private set; }

    public string PartitionKey { get; private set; }

    public string RowKey { get; private set; }
  }
}
