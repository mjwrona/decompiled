// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.TemplateParametersBuilder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating
{
  internal class TemplateParametersBuilder
  {
    private readonly TemplateContext m_context;
    private readonly Dictionary<string, TemplateParametersBuilder.TemplateParameterState> m_states;
    private int m_startingBytes;
    private Dictionary<string, JToken> m_jTokens;

    internal TemplateParametersBuilder(TemplateContext context)
    {
      this.m_context = context;
      this.m_states = new Dictionary<string, TemplateParametersBuilder.TemplateParameterState>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_startingBytes = context.Memory.CurrentBytes;
    }

    internal bool TryGetParameter(string name, out TemplateParameter parameter)
    {
      TemplateParametersBuilder.TemplateParameterState templateParameterState;
      int num = this.m_states.TryGetValue(name, out templateParameterState) ? 1 : 0;
      parameter = templateParameterState?.Parameter;
      return num != 0;
    }

    internal void AddParameters(MappingToken parametersMapping)
    {
      this.AssertNoParameters();
      foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in parametersMapping)
      {
        LiteralToken literalToken = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "template parameters key");
        TemplateParameter parameter = new TemplateParameter();
        parameter.Name = literalToken.Value;
        parameter.Type = TemplateParameterType.LegacyObject;
        TemplateParametersBuilder.TemplateParameterState templateParameterState = new TemplateParametersBuilder.TemplateParameterState(this.m_context, parameter, keyValuePair.Value);
        this.m_states[parameter.Name] = templateParameterState;
      }
    }

    internal void AddParameters(SequenceToken parametersSequence)
    {
      this.AssertNoParameters();
      foreach (TemplateToken templateToken in parametersSequence)
      {
        TemplateToken defaultToken;
        TemplateParameter templateParameter = TemplateResultConverter.ConvertToTemplateParameter(this.m_context, templateToken, out defaultToken);
        if (this.m_states.ContainsKey(templateParameter.Name))
        {
          this.AddError(templateToken, YamlStrings.DuplicateTemplateParameter((object) templateParameter.Name));
        }
        else
        {
          TemplateParametersBuilder.TemplateParameterState templateParameterState = new TemplateParametersBuilder.TemplateParameterState(this.m_context, templateParameter, defaultToken);
          this.m_states[templateParameter.Name] = templateParameterState;
        }
      }
    }

    internal void AddParameter(TemplateParameter parameter)
    {
      if (this.m_states.ContainsKey(parameter.Name))
      {
        this.AddError(YamlStrings.DuplicateTemplateParameter((object) parameter.Name));
      }
      else
      {
        TemplateParametersBuilder.TemplateParameterState templateParameterState = new TemplateParametersBuilder.TemplateParameterState(this.m_context, parameter);
        this.m_states[parameter.Name] = templateParameterState;
      }
    }

    internal void AddValues(IEnumerable<KeyValuePair<string, object>> values)
    {
      foreach (KeyValuePair<string, object> keyValuePair in values)
        this.AddValue(keyValuePair.Key, keyValuePair.Value);
    }

    internal void AddValue(string name, object value)
    {
      TemplateParametersBuilder.TemplateParameterState state;
      if (this.TryGetState(name, out state))
        state.SetValue(value);
      else
        this.AddError(YamlStrings.UnexpectedTemplateParameter((object) name));
    }

    internal void AddValue(string name, TemplateToken token)
    {
      TemplateParametersBuilder.TemplateParameterState state;
      if (this.TryGetState(name, out state))
        state.SetValue(token);
      else
        this.AddError(token, YamlStrings.UnexpectedTemplateParameter((object) name));
    }

    internal IDictionary<string, JToken> Build()
    {
      this.m_jTokens = this.m_jTokens == null ? new Dictionary<string, JToken>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : throw new InvalidOperationException("Template parameter values already validated and copied");
      foreach (TemplateParametersBuilder.TemplateParameterState templateParameterState in this.m_states.Values)
      {
        string name = templateParameterState.Parameter.Name;
        if (templateParameterState.Value == null && templateParameterState.Errors.Count == 0)
        {
          if (templateParameterState.DefaultToken != null)
            templateParameterState.SetValue(templateParameterState.DefaultToken);
          else if (templateParameterState.Parameter.Default != null)
            templateParameterState.SetValue(templateParameterState.Parameter.Default);
        }
        if (templateParameterState.Errors.Count == 0)
        {
          if (templateParameterState.Value != null)
            this.m_jTokens.Add(name, templateParameterState.Value);
          else
            templateParameterState.Errors.Add(new PipelineValidationError(YamlStrings.MissingTemplateParameterValue((object) name)));
        }
        this.m_context.Errors.Add((IEnumerable<PipelineValidationError>) templateParameterState.Errors);
      }
      int bytes = this.m_context.Memory.CurrentBytes - this.m_startingBytes;
      if (bytes > 0)
        this.m_context.Memory.SubtractBytes(bytes);
      return (IDictionary<string, JToken>) this.m_jTokens;
    }

    private void AddError(TemplateToken token, string error) => this.m_context.Error(token, error);

    private void AddError(string error) => this.m_context.Error(new int?(), new int?(), new int?(), error);

    private void AssertNoParameters()
    {
      if (this.m_states.Count > 0)
        throw new InvalidOperationException("Template parameters already declared");
    }

    private bool TryGetState(
      string name,
      out TemplateParametersBuilder.TemplateParameterState state)
    {
      if (this.m_states.TryGetValue(name, out state))
        return true;
      if (!this.m_context.AllowUndeclaredParameters)
        return false;
      state = new TemplateParametersBuilder.TemplateParameterState(this.m_context, new TemplateParameter()
      {
        Name = name,
        Type = TemplateParameterType.LegacyObject
      });
      this.m_states[name] = state;
      return true;
    }

    private class TemplateParameterState
    {
      internal readonly TemplateParameter Parameter;
      internal readonly TemplateToken DefaultToken;
      internal JToken Value;
      internal PipelineValidationErrors Errors;
      private readonly TemplateContext m_context;

      internal TemplateParameterState(
        TemplateContext context,
        TemplateParameter parameter,
        TemplateToken defaultToken = null)
      {
        this.Parameter = parameter;
        this.DefaultToken = defaultToken;
        this.Value = (JToken) null;
        this.Errors = new PipelineValidationErrors();
        this.m_context = context;
      }

      internal void AddError(string error) => this.Errors.Add(new PipelineValidationError(error));

      internal void SetValue(object value)
      {
        switch (value)
        {
          case string str:
            this.SetValue(str);
            break;
          case JToken jtoken:
            this.SetValue(jtoken);
            break;
          case TemplateToken token:
            this.SetValue(token);
            break;
          default:
            this.AddError(YamlStrings.InvalidTemplateParameterType((object) this.Parameter.Name, value, (object) this.Parameter.Type));
            break;
        }
      }

      internal void SetValue(string value)
      {
        TemplateToken token;
        if (!this.TryReadYaml(value, out token))
          return;
        this.SetValue(token);
      }

      internal void SetValue(TemplateToken token)
      {
        if (!this.TryEvaluate(token, out token))
          return;
        TemplateContext templateContext = this.NewContext();
        JToken jToken;
        if (TemplateResultConverter.TryConvertParameterValueToJToken(this.Parameter.Type, token, out jToken))
        {
          this.Value = jToken;
          if (this.Parameter.Values.Count > 0 && !this.Parameter.Values.Any<JToken>((Func<JToken, bool>) (val => jToken.Equals((object) val))))
            templateContext.Error(token, YamlStrings.InvalidTemplateParameterValue((object) this.Parameter.Name, (object) token));
        }
        else if (token is LiteralToken)
          templateContext.Error(token, YamlStrings.InvalidTemplateParameterType((object) this.Parameter.Name, (object) token, (object) this.Parameter.Type));
        else
          templateContext.Error(token, YamlStrings.InvalidTemplateParameter((object) this.Parameter.Name, (object) this.Parameter.Type));
        this.Errors.Add((IEnumerable<PipelineValidationError>) templateContext.Errors);
      }

      internal void SetValue(JToken value)
      {
        bool flag = false;
        switch (this.Parameter.Type)
        {
          case TemplateParameterType.String:
            flag = value.Type == JTokenType.String;
            break;
          case TemplateParameterType.Number:
            flag = value.Type == JTokenType.Integer || value.Type == JTokenType.Float;
            break;
          case TemplateParameterType.Boolean:
            flag = value.Type == JTokenType.Boolean;
            break;
          case TemplateParameterType.Object:
            flag = true;
            break;
        }
        if (flag)
        {
          this.Value = value;
          if (this.Parameter.Values.Count <= 0 || this.Parameter.Values.Any<JToken>((Func<JToken, bool>) (val => value.Equals((object) val))))
            return;
          this.Errors.Add(new PipelineValidationError(YamlStrings.InvalidTemplateParameterValue((object) this.Parameter.Name, (object) value)));
        }
        else
          this.Errors.Add(new PipelineValidationError(YamlStrings.InvalidTemplateParameter((object) this.Parameter.Name, (object) this.Parameter.Type)));
      }

      private bool TryReadYaml(string value, out TemplateToken token)
      {
        if (string.IsNullOrEmpty(value))
        {
          this.Errors.Add(new PipelineValidationError(YamlStrings.InvalidTemplateParameter((object) this.Parameter.Name, (object) this.Parameter.Type)));
          token = (TemplateToken) null;
          return false;
        }
        if (this.Parameter.Type == TemplateParameterType.Boolean || this.Parameter.Type == TemplateParameterType.Number || this.Parameter.Type == TemplateParameterType.String)
        {
          token = (TemplateToken) new LiteralToken(new int?(), new int?(), new int?(), value);
          return true;
        }
        using (StringReader input = new StringReader(value))
        {
          TemplateContext context = this.NewContext();
          YamlObjectReader yamlObjectReader = new YamlObjectReader((TextReader) input);
          token = TemplateReader.Read(context, "any", (IObjectReader) yamlObjectReader, new int?(), true, out int _);
          if (context.Errors.Count <= 0)
            return true;
          this.Errors.Add(new PipelineValidationError(YamlStrings.InvalidTemplateParameter((object) this.Parameter.Name, (object) this.Parameter.Type)));
          this.Errors.Add((IEnumerable<PipelineValidationError>) context.Errors);
          return false;
        }
      }

      private bool TryEvaluate(TemplateToken input, out TemplateToken output)
      {
        string type = (string) null;
        switch (this.Parameter.Type)
        {
          case TemplateParameterType.Step:
            type = "step";
            break;
          case TemplateParameterType.StepList:
            type = "steps";
            break;
          case TemplateParameterType.Job:
          case TemplateParameterType.Deployment:
            type = "job";
            break;
          case TemplateParameterType.JobList:
          case TemplateParameterType.DeploymentList:
            type = "jobs";
            break;
          case TemplateParameterType.Stage:
            type = "stage";
            break;
          case TemplateParameterType.StageList:
            type = "stages";
            break;
          case TemplateParameterType.Container:
            type = "containerResource";
            break;
          case TemplateParameterType.ContainerList:
            type = "containerResources";
            break;
        }
        if (type == null)
        {
          output = input;
          return true;
        }
        TemplateContext context = this.NewContext();
        output = TemplateEvaluator.Evaluate(context, type, input, 0, input.FileId);
        if (context.Errors.Count <= 0)
          return true;
        this.Errors.Add(new PipelineValidationError(YamlStrings.InvalidTemplateParameter((object) this.Parameter.Name, (object) this.Parameter.Type)));
        this.Errors.Add((IEnumerable<PipelineValidationError>) context.Errors);
        return false;
      }

      private TemplateContext NewContext()
      {
        TemplateContext templateContext = this.m_context.NewScope();
        templateContext.TraceWriter = (ITraceWriter) new EmptyTraceWriter();
        templateContext.Errors = new PipelineValidationErrors(this.m_context.Errors.MaxErrors, this.m_context.Errors.MaxMessageLength);
        return templateContext;
      }
    }
  }
}
