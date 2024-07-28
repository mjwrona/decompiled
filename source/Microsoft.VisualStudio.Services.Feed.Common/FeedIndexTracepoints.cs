// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.FeedIndexTracepoints
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

namespace Microsoft.VisualStudio.Services.Feed.Common
{
  public static class FeedIndexTracepoints
  {
    public const string Area = "FeedIndex";
    public const string ServiceTraceLayer = "Service";
    public const int ServiceStartEnter = 10019200;
    public const int ServiceStartLeave = 10019201;
    public const int ServiceStartException = 10019202;
    public const int ServiceEndEnter = 10019203;
    public const int ServiceEndLeave = 10019204;
    public const int ServiceEndException = 10019205;
    public const int GetPackageEnter = 10019206;
    public const int GetPackageLeave = 10019207;
    public const int GetPackageException = 10019208;
    public const int GetPackagesEnter = 10019209;
    public const int GetPackagesLeave = 10019210;
    public const int GetPackagesException = 10019211;
    public const int GetPackageVersionEnter = 10019212;
    public const int GetPackageVersionLeave = 10019213;
    public const int GetPackageVersionException = 10019214;
    public const int GetPackageVersionsEnter = 10019215;
    public const int GetPackageVersionsLeave = 10019216;
    public const int GetPackageVersionsException = 10019217;
    public const int SetIndexEntryEnter = 10019218;
    public const int SetIndexEntryLeave = 10019219;
    public const int SetIndexEntryException = 10019220;
    public const int CheckAddToPackageIndexPermissionsEnter = 10019221;
    public const int CheckAddToPackageIndexPermissionsLeave = 10019222;
    public const int CheckAddToPackageIndexPermissionsException = 10019223;
    public const int UpdatePackageVersionEnter = 10019224;
    public const int UpdatePackageVersionLeave = 10019225;
    public const int UpdatePackageVersionException = 10019226;
    public const int RetryHelperEnter = 10019227;
    public const int RetryHelperLeave = 10019228;
    public const int RetryHelperException = 10019229;
    public const int RetryHelperInfo = 10019230;
    public const int UpdatePackageVersionsEnter = 10019231;
    public const int UpdatePackageVersionsLeave = 10019232;
    public const int UpdatePackageVersionsException = 10019233;
    public const int SetMissingSourceIdEnter = 10019234;
    public const int SetMissingSourceIdLeave = 10019235;
    public const int SetMissingSourceIdException = 10019236;
    public const int ValidateProvenanceEnter = 10019237;
    public const int ValidateProvenanceLeave = 10019238;
    public const int ValidateProvenanceException = 10019239;
    public const int DeletePackageVersionEnter = 10019243;
    public const int DeletePackageVersionLeave = 10019244;
    public const int DeletePackageVersionException = 10019245;
    public const int ClearCachedPackagesEnter = 10019246;
    public const int ClearCachedPackagesLeave = 10019247;
    public const int ClearCachedPackagesException = 10019248;
    public const int GetPackageVersionDependenciesEnter = 10019249;
    public const int GetPackageVersionDependenciesLeave = 10019250;
    public const int GetPackageVersionDependenciesException = 10019251;
  }
}
