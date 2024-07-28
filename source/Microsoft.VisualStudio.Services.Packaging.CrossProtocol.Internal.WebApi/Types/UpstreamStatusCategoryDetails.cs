// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types.UpstreamStatusCategoryDetails
// Assembly: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 208E7E0C-C249-4CB0-B738-E2A4534A31E8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.dll

namespace Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types
{
  public class UpstreamStatusCategoryDetails
  {
    public UpstreamStatusCategoryDetails(
      UpstreamStatusCategory category,
      CategoryType categoryType,
      bool isUserResponsibility,
      bool shouldTryOtherPackagesAfterFailure)
    {
      this.Category = category;
      this.CategoryType = categoryType;
      this.IsUserResponsibility = isUserResponsibility;
      this.ShouldTryOtherPackagesAfterFailure = shouldTryOtherPackagesAfterFailure;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public UpstreamStatusCategory Category { get; }

    public CategoryType CategoryType { get; }

    public bool IsUserResponsibility { get; }

    public bool ShouldTryOtherPackagesAfterFailure { get; }
  }
}
