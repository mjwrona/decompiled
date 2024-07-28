// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Models.MetadataFilterItem
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

namespace Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Models
{
  public class MetadataFilterItem
  {
    public string Key { get; set; }

    public string Value { get; set; }

    public MetadataFilterItem.ComparisonOperator Operator { get; set; }

    public MetadataFilterItem(string key, string value)
      : this(key, value, MetadataFilterItem.ComparisonOperator.Equal)
    {
    }

    public MetadataFilterItem(string key, string value, MetadataFilterItem.ComparisonOperator op)
    {
      this.Key = key;
      this.Value = value;
      this.Operator = op;
    }

    public enum ComparisonOperator
    {
      Equal,
      NotEqual,
    }
  }
}
