// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.ObjectModel.RunPipelineParameters
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Server.ObjectModel
{
  public class RunPipelineParameters
  {
    private HashSet<string> m_stagesToSkip;
    private Dictionary<string, string> m_variables;
    private Dictionary<string, string> m_templateParameters;

    public Guid? OrchestrationIdentifier { get; set; }

    public JustInTimeConfiguration Configuration { get; set; }

    public JustInTimeContext Context { get; set; }

    public ISet<string> StagesToSkip
    {
      get
      {
        if (this.m_stagesToSkip == null)
          this.m_stagesToSkip = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return (ISet<string>) this.m_stagesToSkip;
      }
    }

    public RunResourcesParameters Resources { get; set; }

    public Dictionary<string, string> Secrets { get; } = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, string> TemplateParameters
    {
      get
      {
        if (this.m_templateParameters == null)
          this.m_templateParameters = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_templateParameters;
      }
      set => this.m_templateParameters = value;
    }

    public Dictionary<string, string> Variables
    {
      get
      {
        if (this.m_variables == null)
          this.m_variables = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_variables;
      }
      set => this.m_variables = value;
    }

    public string YamlOverride { get; set; }

    public bool PreviewRun { get; set; }
  }
}
