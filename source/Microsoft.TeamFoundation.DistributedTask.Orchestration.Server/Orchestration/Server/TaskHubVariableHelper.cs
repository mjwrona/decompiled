// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskHubVariableHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public class TaskHubVariableHelper
  {
    public const string LogParserSourceSystemQuery = "/Service/Tfs/TestExecution/TestResultLogParser/NoConfig/Source";
    public const string DisableTestResultLogPlugin = "agent.disablelogplugin.TestResultLogPlugin";
    public const string AutoPublishTestResultPatternQuery = "/Service/Tfs/TestExecution/AutoPublishTestResult/NoConfig/Pattern";
    public const string AutoPublishTestResultFoldersQuery = "/Service/Tfs/TestExecution/AutoPublishTestResult/NoConfig/Folders";
    public const string DisableAutoPublishTestResultPlugin = "agent.disablelogplugin.TestFilePublisherPlugin";
    public const string PatternVariable = "agent.testfilepublisher.pattern";
    public const string SearchFolderVariable = "agent.testfilepublisher.searchfolders";

    public static void AddTestResultLogParserVariables(
      IVssRequestContext requestContext,
      List<RepositoryResource> pipelineRepoResources,
      IDictionary<string, VariableValue> variables)
    {
      if (variables.ContainsKey("agent.disablelogplugin.TestResultLogPlugin"))
        return;
      bool flag = false;
      try
      {
        RegistryQuery query = new RegistryQuery("/Service/Tfs/TestExecution/TestResultLogParser/NoConfig/Source");
        string[] supportedSources = requestContext.GetService<ICachedRegistryService>().GetValue(requestContext, in query, true, string.Empty).Split(',');
        if (((IEnumerable<string>) supportedSources).Any<string>())
        {
          if (pipelineRepoResources.Any<RepositoryResource>())
            flag = pipelineRepoResources.Count<RepositoryResource>((Func<RepositoryResource, bool>) (pipelineResource => ((IEnumerable<string>) supportedSources).Any<string>((Func<string, bool>) (supportedSource => supportedSource.Equals(pipelineResource.Type, StringComparison.OrdinalIgnoreCase))))) > 0;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceError(10016200, "TaskHub", string.Format("Failed to get registry value for {0}. Error: {1}", (object) "/Service/Tfs/TestExecution/TestResultLogParser/NoConfig/Source", (object) ex));
      }
      if (flag)
        variables["agent.disablelogplugin.TestResultLogPlugin"] = (VariableValue) "false";
      else
        variables["agent.disablelogplugin.TestResultLogPlugin"] = (VariableValue) "true";
    }

    public static void AddAutoPublishTestResultVariables(
      IVssRequestContext requestContext,
      List<RepositoryResource> pipelineRepoResources,
      IDictionary<string, VariableValue> variables)
    {
      if (variables.ContainsKey("agent.disablelogplugin.TestFilePublisherPlugin") || variables.ContainsKey("agent.testfilepublisher.pattern") || variables.ContainsKey("agent.testfilepublisher.searchfolders"))
        return;
      bool flag = true;
      try
      {
        RegistryQuery query = new RegistryQuery("/Service/Tfs/TestExecution/TestResultLogParser/NoConfig/Source");
        string[] supportedSources = requestContext.GetService<ICachedRegistryService>().GetValue(requestContext, in query, true, string.Empty).Split(',');
        if (((IEnumerable<string>) supportedSources).Any<string>())
        {
          if (pipelineRepoResources.Any<RepositoryResource>())
          {
            if (pipelineRepoResources.Count<RepositoryResource>((Func<RepositoryResource, bool>) (pipelineResource => ((IEnumerable<string>) supportedSources).Any<string>((Func<string, bool>) (supportedSource => supportedSource.Equals(pipelineResource.Type, StringComparison.OrdinalIgnoreCase))))) > 0)
            {
              ICachedRegistryService service1 = requestContext.GetService<ICachedRegistryService>();
              IVssRequestContext requestContext1 = requestContext;
              RegistryQuery registryQuery = new RegistryQuery("/Service/Tfs/TestExecution/AutoPublishTestResult/NoConfig/Pattern");
              ref RegistryQuery local1 = ref registryQuery;
              string empty1 = string.Empty;
              string str1 = service1.GetValue(requestContext1, in local1, true, empty1);
              ICachedRegistryService service2 = requestContext.GetService<ICachedRegistryService>();
              IVssRequestContext requestContext2 = requestContext;
              registryQuery = new RegistryQuery("/Service/Tfs/TestExecution/AutoPublishTestResult/NoConfig/Folders");
              ref RegistryQuery local2 = ref registryQuery;
              string empty2 = string.Empty;
              string str2 = service2.GetValue(requestContext2, in local2, true, empty2);
              if (!string.IsNullOrEmpty(str1))
              {
                if (!string.IsNullOrEmpty(str2))
                {
                  variables["agent.testfilepublisher.pattern"] = (VariableValue) str1;
                  variables["agent.testfilepublisher.searchfolders"] = (VariableValue) str2;
                  flag = false;
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceError(10016200, "TaskHub", string.Format("Failed to get registry value for {0}. Error: {1}", (object) "/Service/Tfs/TestExecution/TestResultLogParser/NoConfig/Source", (object) ex));
      }
      if (flag)
        variables["agent.disablelogplugin.TestFilePublisherPlugin"] = (VariableValue) "true";
      else
        variables["agent.disablelogplugin.TestFilePublisherPlugin"] = (VariableValue) "false";
    }
  }
}
