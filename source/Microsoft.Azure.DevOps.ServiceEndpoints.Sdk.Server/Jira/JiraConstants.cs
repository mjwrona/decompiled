// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Jira.JiraConstants
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using System;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Jira
{
  public static class JiraConstants
  {
    public const string JiraSecondaryKeyFormat = "{0}-backup";
    public const string UninstallJobName = "OnJiraAppUninstall";
    public const string UninstallJobClassName = "Microsoft.TeamFoundation.Pipelines.Server.Extensions.JiraAppUninstallHandlerJobExtension";
    public const string JWTParameter = "jwt";
    public const string AuthHeader = "authorization";
    public const string AzurePipelinesJiraConnectAppKey = "com.azure.devops.integration.jira";
    public const string JiraAccountUrl = "jiraAccountUrl";
    public const string JiraAccountName = "jiraAccountName";
    public const string JiraConnectAppKeyVaultRegistryPath = "/Configuration/ConnectedService/PipelinesJiraConnectApp/KeyVault";

    public static class Url
    {
      public static readonly string jiraIssuesRelativeHtmlUrlFormat = "/browse/{0}";
      public static readonly string jiraDeploymentApiRelativeUrl = "/rest/deployments/0.1/bulk";
      public static readonly string jiraSearchIssuesApiRelativeUrl = "/rest/api/2/search";
      public static readonly string jiraAuthValidationRelativeUrl = "/rest/devinfo/0.10/existsByProperties?fakeProperty=1";
    }

    public static class JiraDeploymentStatus
    {
      public static readonly string Pending = "pending";
      public static readonly string InProgress = "in_progress";
      public static readonly string Successful = "successful";
      public static readonly string Cancelled = "cancelled";
      public static readonly string Failed = "failed";
      public static readonly string RolledBack = "rolled_back";
      public static readonly string Unknown = "unknown";
    }

    public static class JiraValidateQuery
    {
      public static readonly string Strict = "strict";
      public static readonly string Warn = "warn";
      public static readonly string None = "none";
    }

    public static class JiraEnvironmentType
    {
      public const string Production = "production";
      public const string Staging = "staging";
      public const string Testing = "testing";
      public const string Development = "development";
      public const string Unmapped = "unmapped";
    }

    public static class JiraLifecycleEvents
    {
      public const string Installed = "installed";
      public const string Uninstalled = "uninstalled";
      public const string DeleteProject = "deleteproject";
      public const string DeleteAccount = "deleteaccount";
    }

    public static class HeaderKey
    {
      public const string HeaderEventType = "X-Jira-Event";
      public const string HeaderDeliveryId = "X-Jira-Delivery";
      public const string HeaderEventSignature = "X-Jira-Signature";
    }

    public static class Security
    {
      public static readonly Guid NamespaceId = new Guid("9A82C708-BFBE-4F31-984C-E960C2196711");
      public const int ReadPermission = 1;
      public const string Token = "JiraConfiguration";
    }
  }
}
