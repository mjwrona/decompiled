// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CommitLog.Constants.NpmCommitLogSerializationConstants
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

namespace Microsoft.VisualStudio.Services.Npm.Server.CommitLog.Constants
{
  public static class NpmCommitLogSerializationConstants
  {
    public const string NpmPackageDisplayNameKey = "packageName";
    public const string PackageJsonOptionsKey = "packageJsonOptions";
    public const string PackageManifestKey = "packageManifest";
    public const string PackageJsonBytesKey = "packageJsonBytes";
    public const string PackageSha1SumKey = "packageSha1Sum";
    public const string PackageDistTagKey = "packageDistTag";
    public const string PackageIsCachedFromUpstreamKey = "packageIsCachedFromUpstream";
    public const string DeprecateMessage = "deprecateMessage";
  }
}
