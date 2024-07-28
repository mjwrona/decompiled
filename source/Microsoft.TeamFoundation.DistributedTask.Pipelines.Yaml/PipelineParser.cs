// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.PipelineParser
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PipelineParser
  {
    private static TemplateSchema s_schema;
    private static int s_schemaFileId;
    private static TemplateToken s_systemPreSteps;
    private static int s_systemPreStepsFileId;
    private readonly IFileProviderFactory m_fileProviderFactory;
    private readonly ParseOptions m_parseOptions;
    private readonly Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.ITraceWriter m_trace;
    private readonly Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.ITraceWriter m_yamlTrace;
    private const string c_typeRepoUnreachableError = "ExternalSourceProviderException";
    private const string c_repoUnreachable = "RepoNotReachable";

    static PipelineParser()
    {
      TemplateContext context = new TemplateContext()
      {
        CancellationToken = CancellationToken.None,
        EnableEachExpressions = true,
        RunJobsWithDemandsOnSingleHostedPool = true,
        Errors = new PipelineValidationErrors(10, 500),
        Memory = new TemplateMemory(50, 1000000, 1048576),
        TraceWriter = (Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.ITraceWriter) new EmptyTraceWriter()
      };
      Assembly executingAssembly = Assembly.GetExecutingAssembly();
      using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream("Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.schema.yml"))
      {
        using (StreamReader input = new StreamReader(manifestResourceStream))
        {
          YamlObjectReader yamlObjectReader = new YamlObjectReader((TextReader) input);
          PipelineParser.s_schemaFileId = context.GetFileId("__built-in-schema.yml");
          PipelineParser.s_schema = TemplateSchema.Load(context, (IObjectReader) yamlObjectReader, new int?(PipelineParser.s_schemaFileId));
          context.Schema = PipelineParser.s_schema;
        }
      }
      if (context.Errors.Count > 0)
        throw new PipelineValidationException((IEnumerable<PipelineValidationError>) context.Errors);
      using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream("Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.system-pre-steps.yml"))
      {
        using (StreamReader input = new StreamReader(manifestResourceStream))
        {
          YamlObjectReader yamlObjectReader = new YamlObjectReader((TextReader) input);
          PipelineParser.s_systemPreStepsFileId = context.GetFileId("system-pre-steps.yml");
          PipelineParser.s_systemPreSteps = TemplateReader.Read(context, "jobDecoratorSteps", (IObjectReader) yamlObjectReader, new int?(PipelineParser.s_systemPreStepsFileId), true, out int _);
        }
      }
      if (context.Errors.Count > 0)
        throw new PipelineValidationException((IEnumerable<PipelineValidationError>) context.Errors);
    }

    public PipelineParser(
      Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.ITraceWriter trace,
      ParseOptions options,
      IFileProviderFactory fileProviderFactory,
      Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.ITraceWriter yamlTrace = null)
    {
      this.m_trace = trace ?? throw new ArgumentNullException(nameof (trace));
      this.m_fileProviderFactory = fileProviderFactory;
      this.m_parseOptions = new ParseOptions(options ?? throw new ArgumentNullException(nameof (options)));
      if (this.m_parseOptions.RetrieveOptions == RetrieveOptions.Default)
        this.m_parseOptions.RetrieveOptions = RetrieveOptions.All;
      this.m_yamlTrace = yamlTrace;
    }

    public static object GetPipelineSchema(
      IList<TaskDefinition> majorVersionTasks,
      Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.ITraceWriter writer,
      ParseOptions parseOptions,
      bool validateTaskNames = true,
      IList<TaskDefinition> allTasks = null,
      Dictionary<YamlTemplateLocation, IList<TemplateParameter>> templates = null)
    {
      return new TemplateSchemaConverter().GetYamlSchema(writer, PipelineParser.s_schema, majorVersionTasks, parseOptions, validateTaskNames, allTasks, templates);
    }

    public PipelineTemplate LoadPipeline(
      string path,
      IDictionary<string, VariableValue> systemVariables,
      IDictionary<string, object> parameters,
      IDictionary<string, RepositoryResource> repositoryOverrides,
      IList<TaskDefinition> tasks,
      IArtifactResolver artifactResolver,
      CancellationToken cancellationToken,
      out YamlTemplateComposition composition)
    {
      ParseResult result;
      if (this.m_parseOptions.EnableTelemetry)
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        result = this.LoadPipelineInternal(path, systemVariables, parameters, repositoryOverrides, cancellationToken);
        stopwatch.Stop();
        this.TraceParseResult(result, stopwatch.Elapsed);
      }
      else
        result = this.LoadPipelineInternal(path, systemVariables, parameters, repositoryOverrides, cancellationToken);
      composition = result.Composition;
      return TemplateResultConverter.ConvertToPipelineTemplate(result.Context, result.Resources, tasks, artifactResolver, result.Value);
    }

    public PipelineStepsTemplate LoadStepsContribution(
      string path,
      IDictionary<string, VariableValue> variables,
      IResourceStore resources,
      IReadOnlyList<JobStep> steps,
      CancellationToken cancellationToken)
    {
      ParseResult parseResult = this.LoadStepsContributionInternal(path, variables, resources, steps, cancellationToken);
      return TemplateResultConverter.ConvertToStepsTemplate(parseResult.Context, parseResult.Value);
    }

    public PipelineStepsTemplate LoadSystemPreSteps(
      IDictionary<string, VariableValue> variables,
      IResourceStore resources,
      IReadOnlyList<JobStep> steps,
      CancellationToken cancellationToken)
    {
      ParseResult parseResult = this.LoadSystemPreStepsInternal(variables, resources, steps, cancellationToken);
      return TemplateResultConverter.ConvertToStepsTemplate(parseResult.Context, parseResult.Value);
    }

    public PipelineStepsTemplate LoadPublishMetadataPostSteps(
      IDictionary<string, VariableValue> variables,
      IResourceStore resources,
      IReadOnlyList<JobStep> steps,
      CancellationToken cancellationToken)
    {
      ParseResult parseResult = this.LoadPublishMetadataPostStepsInternal(variables, resources, steps, cancellationToken);
      return TemplateResultConverter.ConvertToStepsTemplate(parseResult.Context, parseResult.Value);
    }

    internal ParseResult LoadPipelineInternal(
      string path,
      IDictionary<string, VariableValue> systemVariables,
      IDictionary<string, object> parameters,
      IDictionary<string, RepositoryResource> repositoryOverrides,
      CancellationToken cancellationToken)
    {
      PipelineResources resources = new PipelineResources();
      YamlTemplateLoader yamlTemplateLoader = new YamlTemplateLoader(new ParseOptions(this.m_parseOptions), this.m_fileProviderFactory, resources);
      TemplateContext context = this.CreateContext((ITemplateLoader) yamlTemplateLoader, (IResourceStore) null, (IReadOnlyList<JobStep>) null, cancellationToken, this.m_parseOptions.EvaluateAfterAddingToVariablesMap);
      this.SetupPipelineVariables(context, systemVariables);
      if (this.m_parseOptions.EnableDynamicVariables)
        PipelineTemplateEvents.Setup(context);
      string b = (string) null;
      bool flag = false;
      string str;
      switch (this.m_parseOptions.RetrieveOptions)
      {
        case RetrieveOptions.ContinuousIntegrationTrigger:
          str = "pipelineTrigger";
          b = "trigger";
          flag = true;
          break;
        case RetrieveOptions.PullRequestTrigger:
          str = "pipelinePR";
          flag = true;
          b = "pr";
          break;
        case RetrieveOptions.PipelineSchedules:
          str = "pipelineSchedules";
          b = "schedules";
          break;
        case RetrieveOptions.PipelineParameters:
          str = "pipelineParameters";
          b = nameof (parameters);
          break;
        case RetrieveOptions.All:
          str = "pipeline";
          break;
        default:
          throw new Exception(string.Format("Unexpected retrieve options combination '{0}'", (object) this.m_parseOptions.RetrieveOptions));
      }
      resources.Repositories.UnionWith(this.m_fileProviderFactory.GetRepositories());
      TemplateToken templateToken1 = (TemplateToken) null;
      try
      {
        LoadTemplateResult loadTemplateResult;
        try
        {
          loadTemplateResult = yamlTemplateLoader.Load(context, PipelineConstants.SelfAlias, path, str);
        }
        catch (FileNotFoundException ex)
        {
          throw new YamlFileNotFoundException(ex.Message, (Exception) ex);
        }
        if (context.Errors.Count == 0)
        {
          MappingToken mappingToken = TemplateUtil.AssertMapping(loadTemplateResult.Value, "pipeline");
          TemplateToken templateToken2 = (TemplateToken) null;
          TemplateParametersBuilder parametersBuilder = new TemplateParametersBuilder(context);
          context.AllowUndeclaredParameters = flag;
          for (int index = mappingToken.Count - 1; index >= 0; --index)
          {
            KeyValuePair<ScalarToken, TemplateToken> keyValuePair = mappingToken[index];
            LiteralToken literalToken = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "pipeline key");
            if ((string.Equals(str, "pipeline", StringComparison.Ordinal) || string.Equals(str, "pipelineTrigger", StringComparison.Ordinal) || string.Equals(str, "pipelinePR", StringComparison.Ordinal)) && string.Equals(literalToken.Value, "resources", StringComparison.Ordinal))
              templateToken2 = keyValuePair.Value;
            if (string.Equals(str, "pipeline", StringComparison.Ordinal) && string.Equals(literalToken.Value, nameof (parameters), StringComparison.Ordinal))
              parametersBuilder.AddParameters(TemplateUtil.AssertSequence(keyValuePair.Value, "template parameter definitions"));
            if (b != null && !string.Equals(literalToken.Value, b, StringComparison.Ordinal))
              mappingToken.RemoveAt(index);
          }
          if (templateToken2 != null)
          {
            TemplateToken webhookResourceToken;
            foreach (WebhookResource webhookResource in (IEnumerable<WebhookResource>) (TemplateResultConverter.ConvertToWebhookResources(context, templateToken2, out webhookResourceToken)?.Webhooks ?? (ISet<WebhookResource>) new HashSet<WebhookResource>()))
            {
              TemplateParameter parameter1 = new TemplateParameter()
              {
                Name = webhookResource.Alias,
                Type = TemplateParameterType.Object,
                DisplayName = "Webhook $" + webhookResource.Alias + " payload",
                Default = (JToken) new JObject()
              };
              TemplateParameter parameter2;
              if (parametersBuilder.TryGetParameter(webhookResource.Alias, out parameter2))
              {
                if (parameter2.Type != TemplateParameterType.Object)
                {
                  PipelineValidationError error = new PipelineValidationError(YamlStrings.FailedToAddParameterForWebhook((object) parameter2.Name, (object) webhookResource.Alias));
                  if (webhookResourceToken != null)
                    context.Error(webhookResourceToken, error);
                  else
                    context.Error(error);
                }
              }
              else
                parametersBuilder.AddParameter(parameter1);
            }
          }
          if (parameters != null)
          {
            parametersBuilder.AddValues((IEnumerable<KeyValuePair<string, object>>) parameters);
            this.m_trace.Info("Template Parameters:");
            foreach (KeyValuePair<string, object> parameter in (IEnumerable<KeyValuePair<string, object>>) parameters)
              this.m_trace.Info("- {0}: {1}", (object) parameter.Key, (object) (parameter.Value?.ToString() ?? ""));
          }
          context.ExpressionValues[nameof (parameters)] = (object) parametersBuilder.Build();
          if (templateToken2 != null && context.Errors.Count == 0)
          {
            TemplateToken resourceTemplate = this.EvaluateResourceTemplate(context, templateToken2, loadTemplateResult.FileId);
            PipelineResources repositoryResources = TemplateResultConverter.ConvertToPipelineRepositoryResources(context, resourceTemplate, true);
            foreach (RepositoryResource repository in (IEnumerable<RepositoryResource>) repositoryResources.Repositories)
            {
              repository.IsRoot = true;
              RepositoryResource valueOrDefault = repositoryOverrides != null ? repositoryOverrides.GetValueOrDefault<string, RepositoryResource>(repository.Alias) : (RepositoryResource) null;
              if (valueOrDefault != null && valueOrDefault.Ref != null)
              {
                repository.Ref = valueOrDefault.Ref;
                repository.Version = valueOrDefault.Version;
              }
              this.m_fileProviderFactory.AddRepository(repository);
            }
            resources.MergeWith(repositoryResources);
          }
          if (context.Errors.Count == 0)
            templateToken1 = TemplateEvaluator.Evaluate(context, str, loadTemplateResult.Value, loadTemplateResult.ValueBytes, loadTemplateResult.FileId);
        }
      }
      catch (Exception ex) when (!(ex is YamlFileNotFoundException))
      {
        if (ex.GetType().Name == "ExternalSourceProviderException")
          context.Errors.Add(new PipelineValidationError("RepoNotReachable", ex.Message));
        else
          context.Errors.Add(ex);
      }
      ParseResult parseResult = new ParseResult()
      {
        SchemaType = str,
        Composition = yamlTemplateLoader.Composition,
        Context = context,
        Resources = resources,
        Value = templateToken1
      };
      if (context.Errors.Count == 0)
      {
        if (this.m_yamlTrace != null)
        {
          this.m_yamlTrace.Info("{0}", (object) parseResult.ToYaml());
        }
        else
        {
          this.m_trace.Info("********************************************************************************");
          this.m_trace.Info(YamlStrings.CompiledYamlDocument());
          this.m_trace.Info("{0}", (object) parseResult.ToYaml());
        }
      }
      return parseResult;
    }

    internal ParseResult LoadStepsContributionInternal(
      string path,
      IDictionary<string, VariableValue> variables,
      IResourceStore resources,
      IReadOnlyList<JobStep> steps,
      CancellationToken cancellationToken)
    {
      YamlTemplateLoader yamlTemplateLoader = new YamlTemplateLoader(new ParseOptions(this.m_parseOptions), this.m_fileProviderFactory);
      TemplateContext context = this.CreateContext((ITemplateLoader) null, resources, steps ?? (IReadOnlyList<JobStep>) new List<JobStep>(), cancellationToken);
      context.ExpressionValues[nameof (variables)] = (object) this.GetPublicVariables(variables);
      TemplateToken templateToken = (TemplateToken) null;
      try
      {
        LoadTemplateResult loadTemplateResult = yamlTemplateLoader.Load(context, PipelineConstants.SelfAlias, path, "jobDecoratorSteps");
        templateToken = TemplateEvaluator.Evaluate(context, "jobDecoratorSteps", loadTemplateResult.Value, loadTemplateResult.ValueBytes, loadTemplateResult.FileId);
      }
      catch (Exception ex)
      {
        context.Errors.Add(ex);
      }
      ParseResult parseResult = new ParseResult()
      {
        SchemaType = "jobDecoratorSteps",
        Context = context,
        Value = templateToken
      };
      if (context.Errors.Count == 0)
      {
        if (this.m_yamlTrace != null)
        {
          this.m_yamlTrace.Info("{0}", (object) parseResult.ToYaml());
        }
        else
        {
          this.m_trace.Info("********************************************************************************");
          this.m_trace.Info(YamlStrings.CompiledYamlDocument());
          this.m_trace.Info("{0}", (object) parseResult.ToYaml());
        }
      }
      return parseResult;
    }

    internal ParseResult LoadSystemPreStepsInternal(
      IDictionary<string, VariableValue> variables,
      IResourceStore resources,
      IReadOnlyList<JobStep> steps,
      CancellationToken cancellationToken)
    {
      TemplateContext context = this.CreateContext((ITemplateLoader) null, resources ?? (IResourceStore) new ResourceStore(), steps ?? (IReadOnlyList<JobStep>) new List<JobStep>(), cancellationToken);
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      VariableValue variableValue;
      if (variables != null && variables.TryGetValue("system.debugContext", out variableValue) && string.Equals(variableValue.Value, "true", StringComparison.OrdinalIgnoreCase))
        dictionary["system.debugContext"] = "true";
      context.ExpressionValues[nameof (variables)] = (object) dictionary;
      int fileId = context.GetFileId("system-pre-steps.yml");
      if (fileId != PipelineParser.s_systemPreStepsFileId)
        throw new Exception(string.Format("Expected system steps file ID '{0}'; actual '{1}'", (object) PipelineParser.s_systemPreStepsFileId, (object) fileId));
      ParseResult parseResult = new ParseResult()
      {
        SchemaType = "jobDecoratorSteps",
        Context = context
      };
      try
      {
        parseResult.Value = TemplateEvaluator.Evaluate(context, "jobDecoratorSteps", PipelineParser.s_systemPreSteps, 0, new int?(fileId));
      }
      catch (Exception ex)
      {
        context.Errors.Add(ex);
      }
      if (context.Errors.Count == 0)
      {
        if (this.m_yamlTrace != null)
        {
          this.m_yamlTrace.Info("{0}", (object) parseResult.ToYaml());
        }
        else
        {
          this.m_trace.Info("********************************************************************************");
          this.m_trace.Info(YamlStrings.CompiledYamlDocument());
          this.m_trace.Info("{0}", (object) parseResult.ToYaml());
        }
      }
      return parseResult;
    }

    internal ParseResult LoadPublishMetadataPostStepsInternal(
      IDictionary<string, VariableValue> variables,
      IResourceStore resources,
      IReadOnlyList<JobStep> steps,
      CancellationToken cancellationToken)
    {
      TemplateContext context = this.CreateContext((ITemplateLoader) null, resources ?? (IResourceStore) new ResourceStore(), steps ?? (IReadOnlyList<JobStep>) new List<JobStep>(), cancellationToken);
      int fileId;
      TemplateToken template;
      using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.publish-metadata-post-steps.yml"))
      {
        using (StreamReader input = new StreamReader(manifestResourceStream))
        {
          YamlObjectReader yamlObjectReader = new YamlObjectReader((TextReader) input);
          fileId = context.GetFileId("publish-metadata-post-steps.yml");
          template = TemplateReader.Read(context, "jobDecoratorSteps", (IObjectReader) yamlObjectReader, new int?(fileId), true, out int _);
        }
      }
      ParseResult parseResult = new ParseResult()
      {
        SchemaType = "jobDecoratorSteps",
        Context = context
      };
      try
      {
        parseResult.Value = TemplateEvaluator.Evaluate(context, "jobDecoratorSteps", template, 0, new int?(fileId));
      }
      catch (Exception ex)
      {
        context.Errors.Add(ex);
      }
      if (context.Errors.Count == 0)
      {
        context.TraceWriter.Info("********************************************************************************");
        context.TraceWriter.Info(YamlStrings.CompiledYamlDocument());
        context.TraceWriter.Info("{0}", (object) parseResult.ToYaml());
      }
      return parseResult;
    }

    private void TraceParseResult(ParseResult result, TimeSpan elapsed)
    {
      TemplateMemory memory = result.Context.Memory;
      TemplateTelemetry telemetry = new TemplateTelemetry();
      telemetry.SchemaType = result.SchemaType;
      telemetry.ElapsedTime = elapsed;
      telemetry.ErrorCount = result.Context.Errors.Count;
      telemetry.ParserEventCount = memory.EventCount;
      telemetry.MaxParserEventCount = this.m_parseOptions.MaxParseEvents;
      telemetry.GreatestParserDepth = memory.GreatestDepth;
      telemetry.MaxParserDepth = this.m_parseOptions.MaxDepth;
      telemetry.EstimatedMemory = memory.GreatestBytes;
      telemetry.MaxMemory = this.m_parseOptions.MaxResultSize;
      telemetry.GreatestFileSize = memory.GreatestFileSize;
      telemetry.MaxFileSize = this.m_parseOptions.MaxFileSize;
      YamlTemplateComposition composition = result.Composition;
      telemetry.FileCount = composition != null ? composition.Files.Count : 0;
      telemetry.MaxFileCount = this.m_parseOptions.MaxFiles;
      this.m_trace.Telemetry(telemetry);
    }

    private TemplateToken EvaluateResourceTemplate(
      TemplateContext context,
      TemplateToken resourceToken,
      int? fileId)
    {
      int currentBytes = context.Memory.CurrentBytes;
      TemplateToken resourceTemplate = TemplateEvaluator.Evaluate(context, "resources", resourceToken, 0, fileId);
      int bytes = context.Memory.CurrentBytes - currentBytes;
      if (bytes <= 0)
        return resourceTemplate;
      context.Memory.SubtractBytes(bytes);
      return resourceTemplate;
    }

    private TemplateContext CreateContext(
      ITemplateLoader templateLoader,
      IResourceStore resources,
      IReadOnlyList<JobStep> steps,
      CancellationToken cancellationToken,
      bool evaluateAfterAddingToVariablesMap = false)
    {
      TemplateContext context = new TemplateContext()
      {
        CancellationToken = cancellationToken,
        EnableEachExpressions = this.m_parseOptions.EnableEachExpressions,
        RunJobsWithDemandsOnSingleHostedPool = this.m_parseOptions.RunJobsWithDemandsOnSingleHostedPool,
        Errors = new PipelineValidationErrors(this.m_parseOptions.MaxErrors, this.m_parseOptions.MaxErrorMessageLength),
        Memory = new TemplateMemory(this.m_parseOptions.MaxDepth, this.m_parseOptions.MaxParseEvents, this.m_parseOptions.MaxResultSize),
        Schema = PipelineParser.s_schema,
        TemplateLoader = templateLoader,
        TraceWriter = this.m_trace,
        EvaluateAfterAddingToVariablesMap = evaluateAfterAddingToVariablesMap,
        AllowTemplateExpressionsInRef = this.m_parseOptions.AllowTemplateExpressionsInRef
      };
      int fileId = context.GetFileId("__built-in-schema.yml");
      if (fileId != PipelineParser.s_schemaFileId)
        throw new Exception(string.Format("Expected schema file ID '{0}'; actual '{1}'", (object) PipelineParser.s_schemaFileId, (object) fileId));
      context.ExpressionConverters.Add(typeof (Guid), (Converter<object, ConversionResult>) (obj => new ConversionResult()
      {
        Result = (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", obj)
      }));
      if (resources != null)
      {
        PipelineResources pipelineResources1 = new PipelineResources();
        context.ExpressionValues[nameof (resources)] = (object) pipelineResources1;
        IBuildStore builds = resources.Builds;
        if (builds != null)
        {
          foreach (BuildResource buildResource in builds.GetAll())
            pipelineResources1.Builds.Add(buildResource);
        }
        IPackageResourceStore packages = resources.Packages;
        if (packages != null)
        {
          foreach (PackageResource packageResource in packages.GetAll())
            pipelineResources1.Packages.Add(packageResource);
        }
        IRepositoryStore repositories = resources.Repositories;
        if (repositories != null)
        {
          foreach (RepositoryResource repositoryResource in repositories.GetAll())
            pipelineResources1.Repositories.Add(repositoryResource);
        }
        IPipelineStore pipelines = resources.Pipelines;
        if (pipelines != null)
        {
          foreach (PipelineResource pipelineResource in pipelines.GetAll())
            pipelineResources1.Pipelines.Add(pipelineResource);
        }
        context.ExpressionConverters[typeof (PipelineResources)] = (Converter<object, ConversionResult>) (obj =>
        {
          PipelineResources pipelineResources2 = obj as PipelineResources;
          Dictionary<string, object> dictionary1 = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          MemoryCounter memoryCounter = new MemoryCounter((ExpressionNode) null, new int?());
          memoryCounter.AddMinObjectSize();
          Dictionary<string, object> dictionary2 = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          dictionary1.Add("builds", (object) dictionary2);
          memoryCounter.Add(IntPtr.Size);
          memoryCounter.AddMinObjectSize();
          foreach (BuildResource build in (IEnumerable<BuildResource>) pipelineResources2.Builds)
          {
            dictionary2[build.Alias] = (object) build;
            memoryCounter.Add(IntPtr.Size);
            memoryCounter.Add(IntPtr.Size);
          }
          Dictionary<string, object> dictionary3 = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          dictionary1.Add("packages", (object) dictionary3);
          memoryCounter.Add(IntPtr.Size);
          memoryCounter.AddMinObjectSize();
          foreach (PackageResource package in (IEnumerable<PackageResource>) pipelineResources2.Packages)
          {
            dictionary3[package.Alias] = (object) package;
            memoryCounter.Add(IntPtr.Size);
            memoryCounter.Add(IntPtr.Size);
          }
          Dictionary<string, object> dictionary4 = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          dictionary1.Add("repositories", (object) dictionary4);
          memoryCounter.Add(IntPtr.Size);
          memoryCounter.AddMinObjectSize();
          foreach (RepositoryResource repository in (IEnumerable<RepositoryResource>) pipelineResources2.Repositories)
          {
            dictionary4[repository.Alias] = (object) repository;
            memoryCounter.Add(IntPtr.Size);
            memoryCounter.Add(IntPtr.Size);
          }
          Dictionary<string, object> dictionary5 = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          dictionary1.Add("pipelines", (object) dictionary5);
          memoryCounter.Add(IntPtr.Size);
          memoryCounter.AddMinObjectSize();
          foreach (PipelineResource pipeline in (IEnumerable<PipelineResource>) pipelineResources2.Pipelines)
          {
            dictionary5[pipeline.Alias] = (object) pipeline;
            memoryCounter.Add(IntPtr.Size);
            memoryCounter.Add(IntPtr.Size);
          }
          return new ConversionResult()
          {
            Result = (object) dictionary1,
            ResultMemory = new ResultMemory()
            {
              Bytes = new int?(memoryCounter.CurrentBytes)
            }
          };
        });
        context.ExpressionConverters[typeof (BuildResource)] = new Converter<object, ConversionResult>(this.ConvertResource);
        context.ExpressionConverters[typeof (RepositoryResource)] = new Converter<object, ConversionResult>(this.ConvertResource);
        context.ExpressionConverters[typeof (PipelineResource)] = new Converter<object, ConversionResult>(this.ConvertResource);
      }
      if (steps != null)
      {
        context.ExpressionValues["job"] = (object) new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
        {
          {
            nameof (steps),
            (object) steps
          }
        };
        context.ExpressionConverters[typeof (TaskStep)] = new Converter<object, ConversionResult>(this.ConvertTaskStep);
      }
      return context;
    }

    private void SetupPipelineVariables(
      TemplateContext context,
      IDictionary<string, VariableValue> systemVariables)
    {
      IDictionary<string, string> publicVariables = this.GetPublicVariables(systemVariables);
      UserVariables userVariables = new UserVariables();
      context.ExpressionValues["variables"] = (object) new CompositeVariables(publicVariables, (IDictionary<string, string>) userVariables);
      ParserUtil.SetPublicSystemVariables(context, publicVariables);
      ParserUtil.SetSystemVariables(context, systemVariables);
      ParserUtil.SetUserVariables(context, userVariables);
    }

    private IDictionary<string, string> GetPublicVariables(
      IDictionary<string, VariableValue> variables)
    {
      Dictionary<string, string> publicVariables = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (variables != null)
      {
        foreach (KeyValuePair<string, VariableValue> keyValuePair in variables.Where<KeyValuePair<string, VariableValue>>((Func<KeyValuePair<string, VariableValue>, bool>) (x =>
        {
          VariableValue variableValue = x.Value;
          return variableValue != null && !variableValue.IsSecret;
        })))
          publicVariables[keyValuePair.Key] = keyValuePair.Value.Value;
      }
      return (IDictionary<string, string>) publicVariables;
    }

    private ConversionResult ConvertTaskStep(object obj)
    {
      TaskStep taskStep = obj as TaskStep;
      JsonUtility.CreateJsonSerializer();
      Dictionary<string, object> dictionary1 = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      MemoryCounter memoryCounter = new MemoryCounter((ExpressionNode) null, new int?());
      memoryCounter.AddMinObjectSize();
      if (taskStep.Reference != null)
      {
        Dictionary<string, object> dictionary2 = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        dictionary1.Add("task", (object) dictionary2);
        memoryCounter.Add("task");
        memoryCounter.AddMinObjectSize();
        dictionary2.Add("id", (object) taskStep.Reference.Id);
        memoryCounter.Add("id");
        memoryCounter.AddMinObjectSize();
        dictionary2.Add("name", (object) taskStep.Reference.Name);
        memoryCounter.Add("name");
        memoryCounter.AddMinObjectSize();
        if (taskStep.Reference.Version != null)
        {
          dictionary2.Add("version", (object) new Version(taskStep.Reference.Version));
          memoryCounter.Add("version");
          memoryCounter.AddMinObjectSize();
        }
      }
      dictionary1.Add("env", (object) taskStep.Environment);
      memoryCounter.Add("env");
      memoryCounter.Add(IntPtr.Size);
      dictionary1.Add("inputs", (object) taskStep.Inputs);
      memoryCounter.Add("inputs");
      memoryCounter.Add(IntPtr.Size);
      dictionary1.Add("condition", (object) taskStep.Condition);
      memoryCounter.Add("condition");
      memoryCounter.Add(taskStep.Condition);
      dictionary1.Add("continueOnError", (object) taskStep.ContinueOnError);
      memoryCounter.Add("continueOnError");
      memoryCounter.Add(1);
      dictionary1.Add("name", (object) taskStep.Name);
      memoryCounter.Add("name");
      memoryCounter.Add(taskStep.Name);
      dictionary1.Add("displayName", (object) taskStep.DisplayName);
      memoryCounter.Add("displayName");
      memoryCounter.Add(taskStep.DisplayName);
      dictionary1.Add("enabled", (object) taskStep.Enabled);
      memoryCounter.Add("enabled");
      memoryCounter.Add(1);
      return new ConversionResult()
      {
        Result = (object) dictionary1,
        ResultMemory = new ResultMemory()
        {
          Bytes = new int?(memoryCounter.CurrentBytes)
        }
      };
    }

    private ConversionResult ConvertResource(object obj)
    {
      RepositoryResource o = obj as RepositoryResource;
      JsonSerializer jsonSerializer1 = JsonUtility.CreateJsonSerializer();
      jsonSerializer1.Converters.Add((JsonConverter) new ResourceJsonConverter());
      JsonSerializer jsonSerializer2 = jsonSerializer1;
      JToken jtoken = JToken.FromObject((object) o, jsonSerializer2);
      MemoryCounter memoryCounter = new MemoryCounter((ExpressionNode) null, new int?());
      memoryCounter.Add(jtoken, true);
      return new ConversionResult()
      {
        Result = (object) jtoken,
        ResultMemory = new ResultMemory()
        {
          Bytes = new int?(memoryCounter.CurrentBytes)
        }
      };
    }
  }
}
