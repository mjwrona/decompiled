// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi.BitbucketConstants
// Assembly: Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 223C9BE7-A3E9-431B-86B7-A81B8A6447FF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi.dll

namespace Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi
{
  public static class BitbucketConstants
  {
    public static class ContentType
    {
      public static readonly string File = "commit_file";
      public static readonly string Directory = "commit_directory";
    }

    public static class StrongBox
    {
      public static readonly string OAuthCallbackDrawerName = "BitbucketOauthCallback";
      public static readonly string PrimaryVstsRegistrationLookupKey = "BitbucketClientSecretPrimary";
      public static readonly string PrimaryAzurePipelinesRegistrationLookupKey = "BitbucketAzurePipelinesClientSecretPrimary";
      public static readonly string PipelineUserTokenDrawer = "BitbucketPipelineUserTokenDrawer";
      public static readonly string BackupAzurePipelinesRegistrationLookupKey = "BitbucketAzurePipelinesClientSecretBackup";
    }
  }
}
