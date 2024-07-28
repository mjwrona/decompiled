// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.InternalAuthConfigurationConstants
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using System;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public static class InternalAuthConfigurationConstants
  {
    public static readonly Uri GitHubUri = new Uri("https://github.com");
    public static readonly Uri BitbucketUri = new Uri("https://bitbucket.org");
    public static readonly Guid AzurePipelinesOAuthAppId = new Guid("9b079bb9-758f-452e-a3e8-8deed3054af2");
    public static readonly string AzurePipelinesOAuthAppName = "AzurePipelines";
    public static readonly Guid AzureBoardsOAuthAppId = new Guid("ac0ed9a0-e28c-424a-a9d1-3b99526cac70");
    public static readonly string AzureBoardsOAuthAppName = "AzureBoards";
    public static readonly Guid AzurePipelinesMarketplaceAppId = new Guid("5d80f9d6-8df1-458e-bc11-c34089a907ff");
    public static readonly string AzurePipelinesMarketplaceAppName = "AzurePipelinesMarketplace";
    public static readonly Guid AzureBoardsMarketplaceAppId = new Guid("b0a09990-07f8-e97c-a45f-33d5ad3eaa90");
    public static readonly string AzureBoardsMarketplaceAppName = "AzureBoardsMarketplace";
    public static readonly Guid BitbucketVstsAppId = new Guid("59ca0dba-ffff-4694-b3ef-46b52db4fa89");
    public static readonly string BitbucketVstsAppName = "Bitbucket";
    public static readonly Guid BitbucketAzurePipelinesOAuthAppId = new Guid("744a24d8-54a8-4947-9320-d63745191082");
    public static readonly string BitbucketAzurePipelinesOAuthAppName = "BitbucketAzurePipelines";
    public static readonly Guid BitbucketAzurePipelinesBackupOAuthAppId = new Guid("70af9a3d-1ecd-442f-a68b-a1141cedd6e6");
    public static readonly string BitbucketAzurePipelinesBackupOAuthAppName = "BitbucketAzurePipelines";
  }
}
