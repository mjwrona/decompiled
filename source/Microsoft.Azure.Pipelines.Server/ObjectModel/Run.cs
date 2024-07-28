// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.ObjectModel.Run
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Server.ObjectModel
{
  public class Run
  {
    private Dictionary<string, Variable> m_variables;
    private Dictionary<string, object> m_templateParameters;

    public Run(
      PipelineReference pipeline,
      int runId,
      DateTime createdDate,
      RunResources resources)
    {
      ArgumentUtility.CheckForNull<PipelineReference>(pipeline, nameof (pipeline));
      this.Pipeline = pipeline;
      this.Id = runId;
      this.CreatedDate = createdDate;
      this.Resources = resources;
    }

    public PipelineReference Pipeline { get; private set; }

    public Guid ProjectId => this.Pipeline.ProjectId;

    public int Id { get; private set; }

    public string Name { get; set; }

    public RunState State { get; set; }

    public RunResult? Result { get; set; }

    public DateTime CreatedDate { get; private set; }

    public DateTime? FinishedDate { get; set; }

    public string FinalYaml { get; set; }

    public Dictionary<string, Variable> Variables
    {
      get
      {
        if (this.m_variables == null)
          this.m_variables = new Dictionary<string, Variable>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_variables;
      }
    }

    public Dictionary<string, object> TemplateParameters
    {
      get
      {
        if (this.m_templateParameters == null)
          this.m_templateParameters = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_templateParameters;
      }
      set => this.m_templateParameters = value;
    }

    internal Guid? PlanId { get; set; }

    public RunResources Resources { get; private set; }

    public virtual ISecuredObject ToSecuredObject() => (ISecuredObject) new SecuredObject(Guid.Empty, 1, "run dummy token");

    public void SetVariables(string jsonKeyValues, bool handleVariablesWithSameName = false)
    {
      if (string.IsNullOrEmpty(jsonKeyValues))
        return;
      this.Variables.Clear();
      Dictionary<string, string> collection = JsonUtilities.Deserialize<Dictionary<string, string>>(jsonKeyValues);
      if (collection == null)
        return;
      collection.ForEach<KeyValuePair<string, string>>((Action<KeyValuePair<string, string>>) (kv =>
      {
        if (handleVariablesWithSameName && this.Variables.ContainsKey(kv.Key))
          this.Variables.Remove(kv.Key);
        this.Variables.Add(kv.Key, new Variable()
        {
          IsSecret = false,
          Value = kv.Value
        });
      }));
    }
  }
}
