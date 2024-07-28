// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureRegion
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class AzureRegion
  {
    public string Id { get; set; }

    public string DisplayName { get; set; }

    public string RegionCode { get; set; }

    public override int GetHashCode() => base.GetHashCode();

    public override bool Equals(object obj)
    {
      if (obj is AzureRegion)
      {
        AzureRegion azureRegion = obj as AzureRegion;
        if (obj != null)
          return azureRegion.Id.Equals(this.Id, StringComparison.OrdinalIgnoreCase) && azureRegion.DisplayName.Equals(this.DisplayName, StringComparison.OrdinalIgnoreCase) && azureRegion.RegionCode.Equals(this.RegionCode, StringComparison.OrdinalIgnoreCase);
      }
      return base.Equals(obj);
    }
  }
}
