// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.TemplateContext
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Events;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating
{
  internal sealed class TemplateContext
  {
    private static readonly Dictionary<Type, Converter<object, ConversionResult>> s_defaultConverters = new Dictionary<Type, Converter<object, ConversionResult>>()
    {
      {
        typeof (LiteralToken),
        (Converter<object, ConversionResult>) (obj =>
        {
          string str = (obj as LiteralToken).Value;
          return new ConversionResult()
          {
            Result = (object) str,
            ResultMemory = new ResultMemory()
            {
              Bytes = new int?(IntPtr.Size)
            }
          };
        })
      },
      {
        typeof (SequenceToken),
        (Converter<object, ConversionResult>) (obj =>
        {
          TemplateTokenReadOnlyList tokenReadOnlyList = new TemplateTokenReadOnlyList(obj as SequenceToken);
          MemoryCounter memoryCounter = new MemoryCounter((ExpressionNode) null, new int?());
          memoryCounter.AddMinObjectSize();
          return new ConversionResult()
          {
            Result = (object) tokenReadOnlyList,
            ResultMemory = new ResultMemory()
            {
              Bytes = new int?(memoryCounter.CurrentBytes)
            }
          };
        })
      },
      {
        typeof (MappingToken),
        (Converter<object, ConversionResult>) (obj =>
        {
          TemplateTokenReadOnlyDictionary readOnlyDictionary = new TemplateTokenReadOnlyDictionary(obj as MappingToken);
          MemoryCounter memoryCounter = new MemoryCounter((ExpressionNode) null, new int?());
          memoryCounter.AddMinObjectSize();
          return new ConversionResult()
          {
            Result = (object) new TemplateTokenReadOnlyDictionary(obj as MappingToken),
            ResultMemory = new ResultMemory()
            {
              Bytes = new int?(memoryCounter.CurrentBytes)
            }
          };
        })
      }
    };
    private PipelineValidationErrors m_errors;
    private IList<IFunctionInfo> m_expressionFunctions;
    private IDictionary<string, object> m_expressionValues;
    private IDictionary<string, int> m_fileIds;
    private List<string> m_fileNames;
    private IDictionary<string, object> m_state;

    internal TemplateContext()
      : this((IDictionary<Type, Converter<object, ConversionResult>>) new Dictionary<Type, Converter<object, ConversionResult>>((IDictionary<Type, Converter<object, ConversionResult>>) TemplateContext.s_defaultConverters), new TemplateEvents())
    {
    }

    private TemplateContext(
      IDictionary<Type, Converter<object, ConversionResult>> expressionConverters,
      TemplateEvents events)
    {
      this.AllowUndeclaredParameters = true;
      this.ExpressionConverters = expressionConverters;
      this.Events = events;
    }

    internal bool AllowUndeclaredParameters { get; set; }

    internal CancellationToken CancellationToken { get; set; }

    internal bool EnableEachExpressions { get; set; }

    internal bool RunJobsWithDemandsOnSingleHostedPool { get; set; }

    internal bool AllowTemplateExpressionsInRef { get; set; }

    internal PipelineValidationErrors Errors
    {
      get
      {
        if (this.m_errors == null)
          this.m_errors = new PipelineValidationErrors();
        return this.m_errors;
      }
      set => this.m_errors = value;
    }

    internal TemplateEvents Events { get; set; }

    internal IDictionary<Type, Converter<object, ConversionResult>> ExpressionConverters { get; private set; }

    internal IList<IFunctionInfo> ExpressionFunctions
    {
      get
      {
        if (this.m_expressionFunctions == null)
          this.m_expressionFunctions = (IList<IFunctionInfo>) new List<IFunctionInfo>();
        return this.m_expressionFunctions;
      }
    }

    internal IDictionary<string, object> ExpressionValues
    {
      get
      {
        if (this.m_expressionValues == null)
          this.m_expressionValues = (IDictionary<string, object>) new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_expressionValues;
      }
    }

    internal TemplateMemory Memory { get; set; }

    internal TemplateSchema Schema { get; set; }

    internal IDictionary<string, object> State
    {
      get
      {
        if (this.m_state == null)
          this.m_state = (IDictionary<string, object>) new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_state;
      }
    }

    internal ITemplateLoader TemplateLoader { get; set; }

    internal ITraceWriter TraceWriter { get; set; }

    private IDictionary<string, int> FileIds
    {
      get
      {
        if (this.m_fileIds == null)
          this.m_fileIds = (IDictionary<string, int>) new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_fileIds;
      }
      set => this.m_fileIds = value;
    }

    private List<string> FileNames
    {
      get
      {
        if (this.m_fileNames == null)
          this.m_fileNames = new List<string>();
        return this.m_fileNames;
      }
      set => this.m_fileNames = value;
    }

    internal bool EvaluateAfterAddingToVariablesMap { get; set; }

    internal void Error(PipelineValidationError error)
    {
      this.Errors.Add(error);
      this.TraceWriter.Error(error.Message);
    }

    internal void Error(TemplateToken value, Exception ex) => this.Error((int?) value?.FileId, (int?) value?.Line, (int?) value?.Column, ex);

    internal void Error(int? fileId, int? line, int? column, Exception ex)
    {
      string errorPrefix = this.GetErrorPrefix(fileId, line, column);
      this.Errors.Add(errorPrefix, ex);
      this.TraceWriter.Error(errorPrefix, (object) ex);
    }

    internal void Error(TemplateToken value, string message) => this.Error((int?) value?.FileId, (int?) value?.Line, (int?) value?.Column, message);

    internal void Error(IReadOnlyTemplateToken value, string message) => this.Error((int?) value?.FileId, (int?) value?.Line, (int?) value?.Column, message);

    internal void Error(int? fileId, int? line, int? column, string message)
    {
      string errorPrefix = this.GetErrorPrefix(fileId, line, column);
      if (!string.IsNullOrEmpty(errorPrefix))
        message = errorPrefix + " " + message;
      this.Errors.Add(message);
      this.TraceWriter.Error(message);
    }

    internal void Error(TemplateToken value, PipelineValidationError error)
    {
      string errorPrefix = this.GetErrorPrefix((int?) value?.FileId, (int?) value?.Line, (int?) value?.Column);
      this.Errors.Add(errorPrefix, error);
      this.TraceWriter.Error(errorPrefix, (object) error);
    }

    internal INamedValueInfo[] GetExpressionNamedValues()
    {
      IDictionary<string, object> expressionValues = this.m_expressionValues;
      return (expressionValues != null ? (expressionValues.Count > 0 ? 1 : 0) : 0) != 0 ? (INamedValueInfo[]) this.m_expressionValues.Keys.Select<string, NamedValueInfo<ContextValueNode>>((Func<string, NamedValueInfo<ContextValueNode>>) (x => new NamedValueInfo<ContextValueNode>(x))).ToArray<NamedValueInfo<ContextValueNode>>() : (INamedValueInfo[]) null;
    }

    internal int GetFileId(string file)
    {
      int fileId;
      if (!this.FileIds.TryGetValue(file, out fileId))
      {
        fileId = this.FileIds.Count + 1;
        this.FileIds.Add(file, fileId);
        this.FileNames.Add(file);
      }
      return fileId;
    }

    internal string GetFileName(int fileId) => this.FileNames[fileId - 1];

    internal TemplateContext NewScope(bool omitExpressionValues = false, bool omitExpressionFunctions = false)
    {
      TemplateContext templateContext = new TemplateContext(this.ExpressionConverters, this.Events)
      {
        CancellationToken = this.CancellationToken,
        EnableEachExpressions = this.EnableEachExpressions,
        RunJobsWithDemandsOnSingleHostedPool = this.RunJobsWithDemandsOnSingleHostedPool,
        Errors = this.Errors,
        FileIds = this.FileIds,
        FileNames = this.FileNames,
        Memory = this.Memory,
        Schema = this.Schema,
        TemplateLoader = this.TemplateLoader,
        TraceWriter = this.TraceWriter,
        EvaluateAfterAddingToVariablesMap = this.EvaluateAfterAddingToVariablesMap,
        AllowTemplateExpressionsInRef = this.AllowTemplateExpressionsInRef
      };
      if (!omitExpressionValues)
      {
        IDictionary<string, object> expressionValues = this.m_expressionValues;
        if ((expressionValues != null ? (expressionValues.Count > 0 ? 1 : 0) : 0) != 0)
        {
          foreach (KeyValuePair<string, object> expressionValue in (IEnumerable<KeyValuePair<string, object>>) this.m_expressionValues)
            templateContext.ExpressionValues[expressionValue.Key] = expressionValue.Value;
        }
      }
      if (!omitExpressionFunctions)
      {
        IList<IFunctionInfo> expressionFunctions = this.m_expressionFunctions;
        if ((expressionFunctions != null ? (expressionFunctions.Count > 0 ? 1 : 0) : 0) != 0)
        {
          foreach (IFunctionInfo expressionFunction in (IEnumerable<IFunctionInfo>) this.m_expressionFunctions)
            templateContext.ExpressionFunctions.Add(expressionFunction);
        }
      }
      IDictionary<string, object> state = this.m_state;
      if ((state != null ? (state.Count > 0 ? 1 : 0) : 0) != 0)
      {
        foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) this.m_state)
          templateContext.State[keyValuePair.Key] = keyValuePair.Value;
      }
      return templateContext;
    }

    private string GetErrorPrefix(int? fileId, int? line, int? column)
    {
      if (fileId.HasValue)
      {
        string fileName = this.GetFileName(fileId.Value);
        return line.HasValue && column.HasValue ? fileName + " " + YamlStrings.LineColumn((object) line, (object) column) + ":" : fileName + ":";
      }
      return line.HasValue && column.HasValue ? YamlStrings.LineColumn((object) line, (object) column) + ":" : string.Empty;
    }
  }
}
