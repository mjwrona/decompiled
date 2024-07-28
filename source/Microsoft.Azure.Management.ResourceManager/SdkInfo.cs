// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.SdkInfo
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Management.ResourceManager
{
  internal static class SdkInfo
  {
    public static readonly string AutoRestVersion = "latest";
    public static readonly string AutoRestBootStrapperVersion = "autorest@2.0.4283";
    public static readonly string AutoRestCmdExecuted = "cmd.exe /c autorest.cmd https://github.com/Azure/azure-rest-api-specs/blob/master/specification/resources/resource-manager/readme.md --csharp --version=latest --reflect-api-versions --tag=package-policy-2019-09 --csharp.output-folder=C:\\code\\azure-sdk-for-net\\sdk\\resources\\Microsoft.Azure.Management.Resource\\src\\Generated";
    public static readonly string GithubForkName = "Azure";
    public static readonly string GithubBranchName = "master";
    public static readonly string GithubCommidId = "15ce9805b7d33225605476aadb8a6338ae26dda5";
    public static readonly string CodeGenerationErrors = "";
    public static readonly string GithubRepoName = "azure-rest-api-specs";

    public static IEnumerable<Tuple<string, string, string>> ApiInfo_DeploymentScriptsClient => ((IEnumerable<Tuple<string, string, string>>) new Tuple<string, string, string>[1]
    {
      new Tuple<string, string, string>("Resources", "DeploymentScripts", "2019-10-01-preview")
    }).AsEnumerable<Tuple<string, string, string>>();

    public static IEnumerable<Tuple<string, string, string>> ApiInfo_FeatureClient => ((IEnumerable<Tuple<string, string, string>>) new Tuple<string, string, string>[1]
    {
      new Tuple<string, string, string>("Features", "Features", "2015-12-01")
    }).AsEnumerable<Tuple<string, string, string>>();

    public static IEnumerable<Tuple<string, string, string>> ApiInfo_ManagementLinkClient => ((IEnumerable<Tuple<string, string, string>>) new Tuple<string, string, string>[2]
    {
      new Tuple<string, string, string>("ManagementLinkClient", "ResourceLinks", "2016-09-01"),
      new Tuple<string, string, string>("Resources", "ResourceLinks", "2016-09-01")
    }).AsEnumerable<Tuple<string, string, string>>();

    public static IEnumerable<Tuple<string, string, string>> ApiInfo_ManagementLockClient => ((IEnumerable<Tuple<string, string, string>>) new Tuple<string, string, string>[1]
    {
      new Tuple<string, string, string>("Authorization", "ManagementLocks", "2016-09-01")
    }).AsEnumerable<Tuple<string, string, string>>();

    public static IEnumerable<Tuple<string, string, string>> ApiInfo_PolicyClient => ((IEnumerable<Tuple<string, string, string>>) new Tuple<string, string, string>[6]
    {
      new Tuple<string, string, string>("Authorization", "PolicyAssignments", "2019-09-01"),
      new Tuple<string, string, string>("Authorization", "PolicyDefinitions", "2019-09-01"),
      new Tuple<string, string, string>("Authorization", "PolicySetDefinitions", "2019-09-01"),
      new Tuple<string, string, string>("Management", "PolicyDefinitions", "2019-09-01"),
      new Tuple<string, string, string>("Management", "PolicySetDefinitions", "2019-09-01"),
      new Tuple<string, string, string>("PolicyClient", "PolicyAssignments", "2019-09-01")
    }).AsEnumerable<Tuple<string, string, string>>();

    public static IEnumerable<Tuple<string, string, string>> ApiInfo_ResourceManagementClient => ((IEnumerable<Tuple<string, string, string>>) new Tuple<string, string, string>[11]
    {
      new Tuple<string, string, string>("Management", "DeploymentOperations", "2019-10-01"),
      new Tuple<string, string, string>("Management", "Deployments", "2019-10-01"),
      new Tuple<string, string, string>("ResourceManagementClient", "DeploymentOperations", "2019-10-01"),
      new Tuple<string, string, string>("ResourceManagementClient", "Providers", "2019-10-01"),
      new Tuple<string, string, string>("ResourceManagementClient", "ResourceGroups", "2019-10-01"),
      new Tuple<string, string, string>("ResourceManagementClient", "Resources", "2019-10-01"),
      new Tuple<string, string, string>("ResourceManagementClient", "Tags", "2019-10-01"),
      new Tuple<string, string, string>("Resources", "DeploymentOperations", "2019-10-01"),
      new Tuple<string, string, string>("Resources", "Deployments", "2019-10-01"),
      new Tuple<string, string, string>("Resources", "Operations", "2019-10-01"),
      new Tuple<string, string, string>("Resources", "Tags", "2019-10-01")
    }).AsEnumerable<Tuple<string, string, string>>();

    public static IEnumerable<Tuple<string, string, string>> ApiInfo_SubscriptionClient => ((IEnumerable<Tuple<string, string, string>>) new Tuple<string, string, string>[2]
    {
      new Tuple<string, string, string>("SubscriptionClient", "Subscriptions", "2016-06-01"),
      new Tuple<string, string, string>("SubscriptionClient", "Tenants", "2016-06-01")
    }).AsEnumerable<Tuple<string, string, string>>();
  }
}
