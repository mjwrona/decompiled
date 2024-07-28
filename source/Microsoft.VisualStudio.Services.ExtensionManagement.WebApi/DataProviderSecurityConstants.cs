// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.DataProviderSecurityConstants
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  public static class DataProviderSecurityConstants
  {
    public static readonly Guid NamespaceId = new Guid("7FFA7CF4-317C-4FEA-8F1D-CFDA50CFA956");
    public const string RootMetadataToken = "/DataProviderMetadata";
    public const int ReadPermission = 1;
  }
}
