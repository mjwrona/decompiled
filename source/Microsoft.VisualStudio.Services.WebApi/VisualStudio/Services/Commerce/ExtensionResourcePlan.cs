// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ExtensionResourcePlan
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class ExtensionResourcePlan
  {
    public string name { get; set; }

    public string publisher { get; set; }

    public string product { get; set; }

    public string promotionCode { get; set; }

    public string version { get; set; }

    public override string ToString() => "Publisher: " + this.publisher + ", Product: " + this.product + ", Name: " + this.name;
  }
}
