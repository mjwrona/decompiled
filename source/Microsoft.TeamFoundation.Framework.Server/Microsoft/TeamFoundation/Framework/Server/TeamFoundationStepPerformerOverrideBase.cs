// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationStepPerformerOverrideBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class TeamFoundationStepPerformerOverrideBase : 
    TeamFoundationStepPerformerBase,
    IStepPerformerOverride,
    IStepPerformer
  {
    private Dictionary<string, string[]> m_stepNameFilters;

    public TeamFoundationStepPerformerOverrideBase() => this.m_stepNameFilters = this.GetStepNameFilters();

    public void SetBaseStepPerformer(IStepPerformer baseStepPerformer) => this.BaseStepPerformer = baseStepPerformer;

    public override IServicingStep GetServicingStep(string stepType) => base.GetServicingStep(stepType) ?? this.BaseStepPerformer.GetServicingStep(stepType);

    public override void PerformStep(
      string servicingOperation,
      ServicingOperationTarget target,
      string stepType,
      string stepData,
      ServicingContext servicingContext)
    {
      if (this.IsValidStepOverride(stepType, servicingContext))
      {
        IServicingStep servicingStep = this.GetServicingStep(stepType);
        this.PerformStep(servicingOperation, target, servicingStep, stepType, stepData, servicingContext);
      }
      else
        this.BaseStepPerformer.PerformStep(servicingOperation, target, stepType, stepData, servicingContext);
    }

    private bool IsValidStepOverride(string stepType, ServicingContext servicingContext)
    {
      if (this.m_stepNameFilters == null || !this.m_stepNameFilters.ContainsKey(stepType))
        return true;
      string token = servicingContext.Tokens[ServicingTokenConstants.CurrentStepName];
      ArgumentUtility.CheckStringForNullOrEmpty(token, "stepName");
      foreach (string a in this.m_stepNameFilters[stepType])
      {
        if (string.Equals(a, token, StringComparison.OrdinalIgnoreCase))
          return true;
      }
      return false;
    }

    protected IStepPerformer BaseStepPerformer { get; set; }

    protected virtual Dictionary<string, string[]> GetStepNameFilters() => (Dictionary<string, string[]>) null;
  }
}
