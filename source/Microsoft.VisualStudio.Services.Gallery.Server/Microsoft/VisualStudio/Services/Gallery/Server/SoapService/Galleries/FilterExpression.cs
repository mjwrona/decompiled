// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Galleries.FilterExpression
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

namespace Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Galleries
{
  public class FilterExpression
  {
    public FilterExpression(string filterName, string comparison, string value)
    {
      this.FilterName = filterName;
      this.Comparison = comparison;
      this.Value = value;
    }

    public string FilterName { get; set; }

    public string Comparison { get; set; }

    public string Value { get; set; }
  }
}
