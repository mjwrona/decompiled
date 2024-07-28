// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.WellKnownEndpointData
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using System;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [Obsolete("Use EndpointData instead.")]
  public static class WellKnownEndpointData
  {
    public const string CheckoutNestedSubmodules = "checkoutNestedSubmodules";
    public const string CheckoutSubmodules = "checkoutSubmodules";
    public const string Clean = "clean";
    public const string CleanOptions = "cleanOptions";
    public const string DefaultBranch = "defaultBranch";
    public const string FetchDepth = "fetchDepth";
    public const string GitLfsSupport = "gitLfsSupport";
    public const string JenkinsAcceptUntrustedCertificates = "acceptUntrustedCerts";
    public const string OnPremTfsGit = "onpremtfsgit";
    public const string Password = "password";
    public const string RepositoryId = "repositoryId";
    public const string RootFolder = "rootFolder";
    public const string SkipSyncSource = "skipSyncSource";
    public const string SvnAcceptUntrustedCertificates = "acceptUntrustedCerts";
    public const string SvnRealmName = "realmName";
    public const string SvnWorkspaceMapping = "svnWorkspaceMapping";
    public const string TfvcWorkspaceMapping = "tfvcWorkspaceMapping";
    public const string Username = "username";
    public const string AcceptUntrustedCertificates = "acceptUntrustedCerts";
  }
}
