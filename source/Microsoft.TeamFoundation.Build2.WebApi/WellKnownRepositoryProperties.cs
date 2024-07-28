// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.WellKnownRepositoryProperties
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using System;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [Obsolete("Use RepositoryProperties instead.")]
  public static class WellKnownRepositoryProperties
  {
    public const string ApiUrl = "apiUrl";
    public const string BranchesUrl = "branchesUrl";
    public const string CheckoutNestedSubmodules = "checkoutNestedSubmodules";
    public const string CleanOptions = "cleanOptions";
    public const string CloneUrl = "cloneUrl";
    public const string ConnectedServiceId = "connectedServiceId";
    public const string FetchDepth = "fetchDepth";
    public const string Fullname = "fullName";
    public const string GitLfsSupport = "gitLfsSupport";
    public const string LabelSources = "labelSources";
    public const string LabelSourcesFormat = "labelSourcesFormat";
    public const string Password = "password";
    public const string SkipSyncSource = "skipSyncSource";
    public const string SvnMapping = "svnMapping";
    public const string TfvcMapping = "tfvcMapping";
    public const string TokenType = "tokenType";
    public const string Username = "username";
    public const string ReportBuildStatus = "reportBuildStatus";
    public const string AcceptUntrustedCertificates = "acceptUntrustedCerts";
  }
}
