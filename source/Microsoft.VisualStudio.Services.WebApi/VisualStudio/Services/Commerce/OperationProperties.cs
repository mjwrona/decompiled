// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.OperationProperties
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

namespace Microsoft.VisualStudio.Services.Commerce
{
  public struct OperationProperties
  {
    public string provider;
    public string resource;
    public string operation;
    public string description;

    public OperationProperties(
      string providerStr,
      string resourceStr,
      string operationStr,
      string descriptionStr)
    {
      this.provider = providerStr;
      this.resource = resourceStr;
      this.operation = operationStr;
      this.description = descriptionStr;
    }
  }
}
