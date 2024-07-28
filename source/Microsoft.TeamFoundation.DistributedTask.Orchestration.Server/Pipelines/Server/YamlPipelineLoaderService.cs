// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.YamlPipelineLoaderService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.ContainerProviders;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Server
{
  public sealed class YamlPipelineLoaderService : IYamlPipelineLoaderService, IVssFrameworkService
  {
    public YamlPipelineLoadResult Load(
      IVssRequestContext requestContext,
      Guid projectId,
      RepositoryResource repository,
      string filePath,
      PipelineBuilder builder,
      int? defaultQueueId = null,
      CheckoutOptions defaultCheckoutOptions = null,
      WorkspaceOptions defaultWorkspaceOptions = null,
      IYamlNameFormatter nameFormatter = null,
      RetrieveOptions retrieveOptions = RetrieveOptions.All,
      bool validateResources = false,
      bool validateLogicalBoundaries = false,
      string yamlOverride = null,
      IDictionary<string, object> templateParameters = null,
      IDictionary<string, RepositoryResource> repositoryOverrides = null)
    {
      using (new MethodScope(requestContext, "IYamlPipelineLoaderService", nameof (Load)))
      {
        using (new ExcessiveMemoryAllocationTraceScope(requestContext, 10016149, "DistributedTask", "PipelineParser", new Lazy<string>((Func<string>) (() => new
        {
          ProjectId = projectId,
          RepositoryId = repository.Id,
          RepositoryName = repository.Name,
          FilePath = filePath,
          DefaultCheckoutOptions = defaultCheckoutOptions,
          DefaultWorkspaceOptions = defaultWorkspaceOptions,
          RetriveOptions = retrieveOptions,
          ValidateResources = validateResources,
          TemplateParameters = templateParameters
        }.Serialize())), 100, caller: nameof (Load)))
        {
          IDistributedTaskPoolService service1 = requestContext.GetService<IDistributedTaskPoolService>();
          if (retrieveOptions == RetrieveOptions.Default || retrieveOptions.HasFlag((Enum) RetrieveOptions.Process))
          {
            IList<TaskAgentQueue> queues = requestContext.ExecutionEnvironment.IsHostedDeployment ? service1.GetHostedAgentQueues(requestContext.Elevate(), projectId) : (requestContext.ExecutionEnvironment.IsDevFabricDeployment ? service1.GetAgentQueues(requestContext.Elevate(), projectId) : (IList<TaskAgentQueue>) null);
            if (queues != null)
              builder.ResourceStore.Queues.Authorize(queues);
          }
          if (repository.Endpoint != null)
            builder.ResourceStore.Endpoints.Authorize(repository.Endpoint);
          IVssRegistryService service2 = requestContext.GetService<IVssRegistryService>();
          int num = 1048576;
          if (requestContext.IsFeatureEnabled("DistributedTask.IncreaseMaxInitializationLogSize"))
            num = service2.GetValue<int>(requestContext, in RegistryKeys.PipelineParserMaxResultSize, false, 20971520);
          int logMaxLength = num * 10;
          YamlPipelineLoaderService.PipelineTraceWriter trace = new YamlPipelineLoaderService.PipelineTraceWriter(requestContext, logMaxLength);
          YamlPipelineLoaderService.PipelineTraceWriter yamlTrace = new YamlPipelineLoaderService.PipelineTraceWriter(requestContext, logMaxLength);
          List<ConfigurationFile> overrideFiles = (List<ConfigurationFile>) null;
          if (!string.IsNullOrEmpty(yamlOverride))
            overrideFiles = new List<ConfigurationFile>()
            {
              new ConfigurationFile(filePath, yamlOverride)
            };
          IFileProviderFactory fileProviderFactory = (IFileProviderFactory) new RepositoryFileProviderFactory(requestContext, projectId, repository, builder.ResourceStore.Endpoints, (IReadOnlyList<ConfigurationFile>) overrideFiles);
          IList<TaskDefinition> taskDefinitions = service1.GetTaskDefinitions(requestContext);
          ArtifactResolver artifactResolver1 = new ArtifactResolver(requestContext, projectId, builder.ResourceStore.Endpoints);
          ParseOptions parseOptions = ParseOptionsFactory.CreateParseOptions(requestContext, retrieveOptions);
          PipelineParser pipelineParser = new PipelineParser((Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.ITraceWriter) trace, parseOptions, fileProviderFactory, (Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.ITraceWriter) yamlTrace);
          IDictionary<string, object> dictionary = templateParameters ?? (IDictionary<string, object>) new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          string path = filePath;
          IDictionary<string, VariableValue> systemVariables = builder.SystemVariables;
          IDictionary<string, object> parameters = dictionary;
          IDictionary<string, RepositoryResource> repositoryOverrides1 = repositoryOverrides;
          IList<TaskDefinition> tasks = taskDefinitions;
          ArtifactResolver artifactResolver2 = artifactResolver1;
          CancellationToken cancellationToken = requestContext.CancellationToken;
          YamlTemplateComposition templateComposition;
          ref YamlTemplateComposition local = ref templateComposition;
          PipelineTemplate pipelineTemplate = pipelineParser.LoadPipeline(path, systemVariables, parameters, repositoryOverrides1, tasks, (IArtifactResolver) artifactResolver2, cancellationToken, out local);
          pipelineTemplate.InitializationLog = trace.Log();
          pipelineTemplate.ExpandedYaml = yamlTrace.Log();
          YamlPipelineLoaderService.ValidateContainerResources(pipelineTemplate);
          YamlPipelineLoaderService.ValidateRepositoryResources(repository, pipelineTemplate);
          PipelineEnvironment environment = (PipelineEnvironment) null;
          if (retrieveOptions == RetrieveOptions.Default || retrieveOptions.HasFlag((Enum) RetrieveOptions.Process))
          {
            AgentQueueReference agentQueueReference = (AgentQueueReference) null;
            if (defaultQueueId.HasValue)
              agentQueueReference = new AgentQueueReference()
              {
                Id = defaultQueueId.Value
              };
            if (pipelineTemplate.Errors.Count == 0)
            {
              bool flag = requestContext.IsFeatureEnabled("DistributedTask.DisableYamlDemandsLatestAgent");
              BuildOptions options = new BuildOptions()
              {
                DemandLatestAgent = !flag,
                MinimumAgentVersion = flag ? service2.GetValue<string>(requestContext, (RegistryQuery) "/Service/DistributedTask/Pipelines/MinAgentVersion", true, "2.163.1") : (string) null,
                MinimumAgentVersionDemandSource = flag ? AgentFeatureDemands.YamlPipelinesDemandSource() : (DemandSource) null,
                ResolveTaskInputAliases = true,
                ValidateResources = validateResources,
                ValidateStepNames = true,
                ValidatePhaseExpressions = validateLogicalBoundaries,
                EnableResourceExpressions = true
              };
              builder.UserVariables.AddRange<IVariable, IList<IVariable>>((IEnumerable<IVariable>) pipelineTemplate.Variables);
              builder.ResourceStore.Builds.Add((IEnumerable<BuildResource>) pipelineTemplate.Resources.Builds);
              builder.ResourceStore.Containers.Add((IEnumerable<ContainerResource>) pipelineTemplate.Resources.Containers);
              builder.ResourceStore.Pipelines.Add((IEnumerable<PipelineResource>) pipelineTemplate.Resources.Pipelines);
              builder.ResourceStore.Repositories.Add((IEnumerable<RepositoryResource>) pipelineTemplate.Resources.Repositories);
              builder.ResourceStore.Packages.Add((IEnumerable<PackageResource>) pipelineTemplate.Resources.Packages);
              ExpressionValue<string> expressionValue;
              if (ExpressionValue.TryParse<string>(pipelineTemplate.Name, out expressionValue))
              {
                PipelineBuildContext buildContext = builder.CreateBuildContext(options);
                pipelineTemplate.Name = expressionValue.GetValue((IPipelineContext) buildContext).Value;
                if (string.IsNullOrEmpty(pipelineTemplate.Name) && nameFormatter != null)
                  pipelineTemplate.Name = nameFormatter.Format((IPipelineContext) buildContext, "");
              }
              else if (nameFormatter != null)
              {
                PipelineBuildContext buildContext = builder.CreateBuildContext(options);
                pipelineTemplate.Name = nameFormatter.Format((IPipelineContext) buildContext, pipelineTemplate.Name);
              }
              builder.DefaultQueue = agentQueueReference;
              builder.DefaultCheckoutOptions = defaultCheckoutOptions;
              builder.DefaultWorkspaceOptions = defaultWorkspaceOptions;
              try
              {
                PipelineBuildResult pipelineBuildResult = builder.Build(pipelineTemplate.Stages, options);
                pipelineTemplate.Errors.AddRange<PipelineValidationError, IList<PipelineValidationError>>((IEnumerable<PipelineValidationError>) pipelineBuildResult.Errors);
                if (validateLogicalBoundaries)
                  YamlPipelineLoaderService.ValidateLogicalBoundaries(builder, pipelineTemplate);
                environment = pipelineBuildResult.Environment;
              }
              catch (ParseException ex)
              {
                pipelineTemplate.Errors.Add(new PipelineValidationError(ex.Message));
              }
              catch (FormatException ex)
              {
                pipelineTemplate.Errors.Add(new PipelineValidationError(ex.Message));
              }
            }
            if (environment == null)
              environment = new PipelineEnvironment()
              {
                Version = builder.EnvironmentVersion
              };
            if (agentQueueReference != null)
              environment.Resources.Queues.Add(agentQueueReference);
            environment.Resources.MergeWith(builder.ResourceStore.GetAuthorizedResources());
          }
          else
            environment = new PipelineEnvironment()
            {
              Version = builder.EnvironmentVersion
            };
          if (environment == null)
            environment = new PipelineEnvironment();
          environment.Options.EnableResourceExpressions = true;
          environment.YamlTemplateReferences.AddRange<YamlTemplateReference, IList<YamlTemplateReference>>((IEnumerable<YamlTemplateReference>) templateComposition.Files);
          return new YamlPipelineLoadResult(environment, pipelineTemplate);
        }
      }
    }

    private static void ValidateLogicalBoundaries(
      PipelineBuilder builder,
      PipelineTemplate pipelineTemplate)
    {
      foreach (Stage stage1 in (IEnumerable<Stage>) pipelineTemplate.Stages)
      {
        StageInstance stage2 = new StageInstance(stage1);
        builder.CreateStageExecutionContext(stage2);
        foreach (PhaseNode phase1 in (IEnumerable<PhaseNode>) stage1.Phases)
        {
          PhaseInstance phase2 = new PhaseInstance(phase1);
          builder.CreatePhaseExecutionContext(stage2, phase2);
        }
      }
    }

    private static void ValidateRepositoryResources(
      RepositoryResource selfRepository,
      PipelineTemplate pipelineTemplate)
    {
      List<RepositoryResource> list = pipelineTemplate.Resources.Repositories.Where<RepositoryResource>((Func<RepositoryResource, bool>) (r => r.Id == selfRepository.Id && r.Alias != PipelineConstants.SelfAlias)).ToList<RepositoryResource>();
      if (pipelineTemplate.Triggers.OfType<ContinuousIntegrationTrigger>().FirstOrDefault<ContinuousIntegrationTrigger>() != null && list != null && list.Count != 0 && list.Any<RepositoryResource>((Func<RepositoryResource, bool>) (r => r.Trigger != null)))
        pipelineTemplate.Errors.Add(new PipelineValidationError(TaskResources.ConflictingSelfRepositoryCITriggers()));
      if (pipelineTemplate.Triggers.OfType<PullRequestTrigger>().FirstOrDefault<PullRequestTrigger>() == null || list == null || list.Count == 0 || !list.Any<RepositoryResource>((Func<RepositoryResource, bool>) (r => r.PR != null)))
        return;
      pipelineTemplate.Errors.Add(new PipelineValidationError(TaskResources.ConflictingSelfRepositoryPRTriggers()));
    }

    private static void ValidateContainerResources(PipelineTemplate pipelineTemplate) => new ContainerProvider().Validate(pipelineTemplate.Resources.Containers);

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public sealed class PipelineTraceWriter : Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.ITraceWriter
    {
      private StringBuilder m_log = new StringBuilder();
      private IVssRequestContext m_requestContext;
      private readonly int m_logMaxLength;
      internal static readonly string s_logTruncatedToken = "[truncated]";

      public PipelineTraceWriter(IVssRequestContext requestContext, int logMaxLength = 1048576)
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        this.m_requestContext = requestContext;
        this.m_logMaxLength = logMaxLength;
      }

      public void Error(string format, params object[] args)
      {
        this.m_requestContext.TraceError(10015530, "PipelineParser", format, args);
        this.AddToLog("[error]" + format, args);
      }

      public void Warning(string format, params object[] args)
      {
        this.m_requestContext.TraceWarning(10015530, "PipelineParser", format, args);
        this.AddToLog("[warning]" + format, args);
      }

      public void Info(string format, params object[] args)
      {
        this.m_requestContext.TraceInfo(10015530, "PipelineParser", format, args);
        this.AddToLog(format, args);
      }

      public void Verbose(string format, params object[] args) => this.m_requestContext.TraceVerbose(10015530, "PipelineParser", format, args);

      public void Telemetry(TemplateTelemetry telemetry)
      {
        this.AddToLog("Load Time: {0}\nEstimated Memory: {1:N0} bytes (Max: {2:N0})\nParser Event Count: {3:N0} (Max: {4:N0})\nGreatest Parser Depth: {5:N0} (Max: {6:N0})\nFile Count: {7:N0} (Max: {8:N0})\nGreatest File Size: {9:N0} (Max: {10:N0})", new object[11]
        {
          (object) telemetry.ElapsedTime.ToString(),
          (object) telemetry.EstimatedMemory,
          (object) telemetry.MaxMemory,
          (object) telemetry.ParserEventCount,
          (object) telemetry.MaxParserEventCount,
          (object) telemetry.GreatestParserDepth,
          (object) telemetry.MaxParserDepth,
          (object) telemetry.FileCount,
          (object) telemetry.MaxFileCount,
          (object) telemetry.GreatestFileSize,
          (object) telemetry.MaxFileSize
        });
        try
        {
          ClientTraceService service = this.m_requestContext.GetService<ClientTraceService>();
          if (!service.IsTracingEnabled(this.m_requestContext))
            return;
          ClientTraceData properties = new ClientTraceData();
          properties.Add("SchemaType", (object) telemetry.SchemaType);
          properties.Add("ElapsedTimeMs", (object) telemetry.ElapsedTime.TotalMilliseconds);
          properties.Add("ErrorCount", (object) telemetry.ErrorCount);
          properties.Add("ParserEventCount", (object) telemetry.ParserEventCount);
          properties.Add("MaxParserEventCount", (object) telemetry.MaxParserEventCount);
          properties.Add("GreatestParserDepth", (object) telemetry.GreatestParserDepth);
          properties.Add("MaxParserDepth", (object) telemetry.MaxParserDepth);
          properties.Add("EstimatedMemory", (object) telemetry.EstimatedMemory);
          properties.Add("FileCount", (object) telemetry.FileCount);
          properties.Add("MaxFileCount", (object) telemetry.MaxFileCount);
          properties.Add("GreatestFileSize", (object) telemetry.GreatestFileSize);
          properties.Add("MaxFileSize", (object) telemetry.MaxFileSize);
          service.Publish(this.m_requestContext, "DistributedTask", "PipelineParser", properties);
        }
        catch (Exception ex)
        {
          this.m_requestContext.TraceException("ClientTrace", ex);
        }
      }

      private void AddToLog(string format, object[] args)
      {
        int val2 = this.m_logMaxLength - this.m_log.Length;
        if (val2 <= 0)
        {
          if (val2 != 0)
            return;
          this.m_log.Append(YamlPipelineLoaderService.PipelineTraceWriter.s_logTruncatedToken);
        }
        else
        {
          string str = string.Format(format, args) + "\n";
          this.m_log.Append(str.Substring(0, Math.Min(str.Length, val2)));
          if (val2 >= str.Length)
            return;
          this.m_log.Append(YamlPipelineLoaderService.PipelineTraceWriter.s_logTruncatedToken);
        }
      }

      public string Log() => this.m_log.Length == 0 ? (string) null : this.m_log.ToString();
    }
  }
}
