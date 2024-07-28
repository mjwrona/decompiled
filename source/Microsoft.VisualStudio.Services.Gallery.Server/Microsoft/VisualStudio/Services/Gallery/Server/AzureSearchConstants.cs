// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.AzureSearchConstants
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal static class AzureSearchConstants
  {
    public static readonly string ExternalSearchSettingsRootPath = "/Configuration/Service/Gallery/ExternalSearch";
    public static readonly string ExternalSearchServiceNameRootPath = AzureSearchConstants.ExternalSearchSettingsRootPath + "/SearchServiceName";
    public static readonly string AzureSearchServicePath = "AzureSearchService";
    public static readonly string ReadIndexPath = "/ReadIndex";
    public static readonly string WriteIndexPath = "/WriteIndex";
    public static readonly string PageSize = "/PageSize";
    public static readonly string SearchProviderSettingPath = "/SearchProvider";
    public static readonly string SynonymPath = "/Synonyms";
    public static readonly string AzureSearchAdminKey = nameof (AzureSearchAdminKey);
    public static readonly string AzureSearchQueryKey = nameof (AzureSearchQueryKey);
    public static readonly string AscendingSortOrder = "asc";
    public static readonly string DescendingSortOrder = "desc";
    public static readonly string PrimaryPrefix = "Primary";
    public static readonly string SecondaryPrefix = "Secondary";
    public static readonly int RequestRetryCount = 3;
  }
}
