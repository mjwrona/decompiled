// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.EditQueuePipelineVarSecurityValidator
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Orchestration.Server;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class EditQueuePipelineVarSecurityValidator : IEditQueuePipelineVarSecurityValidator
  {
    private readonly IBuildSecurityProvider securityProvider;
    private readonly OrchestrationTracer ciao;
    private const string c_teamName = "ciBuild";

    public EditQueuePipelineVarSecurityValidator(
      IBuildSecurityProvider securityProvider,
      OrchestrationTracer ciao)
    {
      this.securityProvider = securityProvider;
      this.ciao = ciao;
    }

    public void ValidatePermissions(
      IVssRequestContext requestContext,
      BuildData build,
      BuildDefinition definition)
    {
      string orchestrationId = build.OrchestrationPlan.PlanId.ToString("D");
      BuildProcess process = definition.Process;
      if ((process != null ? (process.Type == 2 ? 1 : 0) : 0) == 0 || this.securityProvider.HasDefinitionPermission(requestContext, definition.ProjectId, (MinimalBuildDefinition) definition, BuildPermissions.EditPipelineQueueConfigurationPermission))
        return;
      if (!string.IsNullOrEmpty(build.Parameters))
      {
        Dictionary<string, string> dictionary = JsonUtilities.Deserialize<Dictionary<string, string>>(build.Parameters, true);
        foreach (KeyValuePair<string, BuildDefinitionVariable> variable in definition.Variables)
        {
          string str;
          if (dictionary.TryGetValue(variable.Key, out str) && variable.Value.Value != str)
          {
            this.ciao.TraceCompletedWithError(orchestrationId, "ciBuild", "QueueBuild", "BuildEventPermissionException", BuildServerResources.EditPipelineQueueConfigurationPermission(), true);
            this.securityProvider.CheckDefinitionPermission(requestContext, definition.ProjectId, (MinimalBuildDefinition) definition, BuildPermissions.EditPipelineQueueConfigurationPermission);
          }
        }
      }
      if (!build.TemplateParameters.Any<KeyValuePair<string, object>>())
        return;
      IDictionary<string, TemplateParameter> templateParams = this.GetTemplateParams(requestContext, build, definition);
      foreach (KeyValuePair<string, object> templateParameter1 in build.TemplateParameters)
      {
        TemplateParameter templateParameter2;
        if (templateParams.TryGetValue(templateParameter1.Key, out templateParameter2))
        {
          bool flag = false;
          if (templateParameter2.Type == TemplateParameterType.Object)
          {
            JToken t1 = templateParameter2.Default;
            JToken jtoken = this.ConvertFromBuildParamsToJToken(templateParameter1.Value.ToString(), t1.Type, requestContext);
            flag = t1 != null && jtoken != null && JToken.DeepEquals(t1, jtoken);
          }
          else if (templateParameter2.Type == TemplateParameterType.String)
          {
            string str1 = (string) templateParameter2.Default;
            string str2 = (string) templateParameter1.Value;
            flag = str1 != null && str2 == str1;
          }
          if (!flag)
          {
            this.ciao.TraceCompletedWithError(orchestrationId, "ciBuild", "QueueBuild", "BuildEventPermissionException", BuildServerResources.EditPipelineQueueConfigurationPermission(), true);
            this.securityProvider.CheckDefinitionPermission(requestContext, definition.ProjectId, (MinimalBuildDefinition) definition, BuildPermissions.EditPipelineQueueConfigurationPermission);
          }
        }
      }
    }

    public virtual IDictionary<string, TemplateParameter> GetTemplateParams(
      IVssRequestContext requestContext,
      BuildData build,
      BuildDefinition definition)
    {
      BuildProcessResources authorizedResources = requestContext.GetService<IBuildResourceAuthorizationService>().GetAuthorizedResources(requestContext, definition.ProjectId, definition.Id);
      return (IDictionary<string, TemplateParameter>) definition.LoadYamlPipeline(requestContext, build.SourceBranch, build.SourceVersion, false, authorizedResources, RetrieveOptions.PipelineParameters).Template.Parameters.Where<TemplateParameter>((Func<TemplateParameter, bool>) (templateParam =>
      {
        if (templateParam.Values.Any<JToken>())
          return false;
        return templateParam.Type == TemplateParameterType.String || templateParam.Type == TemplateParameterType.Object;
      })).ToDictionary<TemplateParameter, string, TemplateParameter>((Func<TemplateParameter, string>) (templateParam => templateParam.Name), (Func<TemplateParameter, TemplateParameter>) (templateParam => templateParam));
    }

    private JToken ConvertFromBuildParamsToJToken(
      string buildParamsValue,
      JTokenType expectedType,
      IVssRequestContext requestContext)
    {
      IDeserializer deserializer = new DeserializerBuilder().Build();
      switch (expectedType)
      {
        case JTokenType.Object:
          try
          {
            object o = deserializer.Deserialize<object>(buildParamsValue);
            if (o != null)
              return JToken.FromObject(o);
            break;
          }
          catch (YamlException ex)
          {
            requestContext.Trace(12030396, TraceLevel.Warning, "ciBuild", "QueueBuild", ex.Message);
            break;
          }
        case JTokenType.Array:
          try
          {
            List<object> o = deserializer.Deserialize<List<object>>(buildParamsValue);
            if (o != null)
              return JToken.FromObject((object) o);
            break;
          }
          catch (YamlException ex)
          {
            requestContext.Trace(12030396, TraceLevel.Warning, "ciBuild", "QueueBuild", ex.Message);
            break;
          }
        case JTokenType.String:
          return !(buildParamsValue == "''") ? (JToken) new JValue(buildParamsValue) : JToken.Parse(buildParamsValue);
      }
      return (JToken) null;
    }
  }
}
