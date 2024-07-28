// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ParseOptions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ParseOptions
  {
    public ParseOptions()
    {
    }

    internal ParseOptions(ParseOptions copy)
    {
      this.EnableDynamicVariables = copy.EnableDynamicVariables;
      this.EnableEachExpressions = copy.EnableEachExpressions;
      this.EnableParameterReferenceErrors = copy.EnableParameterReferenceErrors;
      this.EnableStages = copy.EnableStages;
      this.EnableTelemetry = copy.EnableTelemetry;
      this.MaxFiles = copy.MaxFiles;
      this.MaxFileSize = copy.MaxFileSize;
      this.MaxResultSize = copy.MaxResultSize;
      this.MaxParseEvents = copy.MaxParseEvents;
      this.RetrieveOptions = copy.RetrieveOptions;
      this.RunJobsWithDemandsOnSingleHostedPool = copy.RunJobsWithDemandsOnSingleHostedPool;
      this.MaxDepth = copy.MaxDepth;
      this.EvaluateAfterAddingToVariablesMap = copy.EvaluateAfterAddingToVariablesMap;
      this.AllowTemplateExpressionsInRef = copy.AllowTemplateExpressionsInRef;
    }

    public bool EnableDynamicVariables { get; set; }

    public bool EnableEachExpressions { get; set; }

    public bool EnableStages { get; set; }

    public bool EnableTelemetry { get; set; }

    public bool EnableParameterReferenceErrors { get; set; }

    public bool RunJobsWithDemandsOnSingleHostedPool { get; set; }

    public bool EvaluateAfterAddingToVariablesMap { get; set; }

    public bool AllowTemplateExpressionsInRef { get; set; }

    public int MaxDepth { get; set; } = 100;

    public int MaxErrorMessageLength => 500;

    public int MaxErrors => 10;

    public int MaxFileSize { get; set; } = 1048576;

    public int MaxFiles { get; set; }

    public int MaxParseEvents { get; set; } = 1000000;

    public int MaxResultSize { get; set; } = 20971520;

    public RetrieveOptions RetrieveOptions { get; set; }

    internal string GetCacheKey(string templateType, string fileUuid)
    {
      List<string> values = new List<string>();
      values.Add(string.Format("ModuleVersionId={0}", (object) this.GetType().GetTypeInfo().Module.ModuleVersionId));
      values.Add("TemplateType=" + Uri.EscapeDataString(templateType));
      if (!this.EnableDynamicVariables)
        values.Add("EnableDynamicVariables=false");
      if (!this.EnableEachExpressions)
        values.Add("EnableEachExpressions=false");
      if (this.EnableStages)
        values.Add("EnableStages");
      values.Add(string.Format("{0}={1}", (object) "MaxParseEvents", (object) this.MaxParseEvents));
      values.Add("RetrieveOptions=" + Uri.EscapeDataString(this.RetrieveOptions.ToString()));
      values.Add("FileUuid=" + Uri.EscapeDataString(fileUuid));
      values.Add(string.Format("{0}={1}", (object) "MaxDepth", (object) this.MaxDepth));
      return string.Join("/", (IEnumerable<string>) values);
    }
  }
}
