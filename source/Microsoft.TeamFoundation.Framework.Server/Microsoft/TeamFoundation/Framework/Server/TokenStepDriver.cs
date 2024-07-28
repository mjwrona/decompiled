// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TokenStepDriver
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TokenStepDriver
  {
    private Dictionary<string, IList<ServicingStepPath>> m_tokenMap;
    private HashSet<string> m_skipOperations;
    private HashSet<string> m_skipStepGroups;
    private ServicingContext m_servicingContext;

    public TokenStepDriver(ServicingContext servicingContext)
    {
      this.m_tokenMap = new Dictionary<string, IList<ServicingStepPath>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_skipOperations = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_skipStepGroups = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_servicingContext = servicingContext;
    }

    public ServicingStepState PerformServicingStep(
      ServicingStep step,
      ServicingContext servicingContext,
      ServicingStepGroup group,
      ServicingOperation servicingOperation,
      int stepNumber,
      int totalSteps)
    {
      if (step.StepData != null)
      {
        bool replacedAll = false;
        StringUtil.ReplaceTokens(step.StepData.OuterXml, "$i$", "$i$", (Func<string, string>) (t =>
        {
          ServicingStepPath servicingStepPath = new ServicingStepPath(servicingOperation.Name, group.Name, step.Name);
          if (!this.m_tokenMap.ContainsKey(t))
            this.m_tokenMap.Add(t, (IList<ServicingStepPath>) new List<ServicingStepPath>()
            {
              servicingStepPath
            });
          else if (!this.m_tokenMap[t].Contains(servicingStepPath))
            this.m_tokenMap[t].Add(servicingStepPath);
          return (string) null;
        }), out replacedAll);
      }
      return ServicingStepState.Passed;
    }

    public ServicingStepGroupExecutionDecision StartStepGroup(
      ServicingOperation servicingOperation,
      ServicingStepGroup stepGroup)
    {
      this.m_servicingContext.StartStepGroup(servicingOperation.Name, stepGroup);
      return this.m_skipOperations.Contains(servicingOperation.Name) || this.m_skipStepGroups.Contains(stepGroup.Name) ? ServicingStepGroupExecutionDecision.Skip : ServicingStepGroupExecutionDecision.Execute;
    }

    public void FinishStepGroup(
      ServicingOperation servicingOperation,
      ServicingStepGroup stepGroup,
      ServicingStepState groupResolution)
    {
      this.m_servicingContext.FinishStepGroup();
    }

    public IDictionary<string, IList<ServicingStepPath>> RequiredTokenMap => (IDictionary<string, IList<ServicingStepPath>>) this.m_tokenMap;

    public void SetSkipOperations(params string[] skipOperations)
    {
      foreach (string skipOperation in skipOperations)
        this.m_skipOperations.Add(skipOperation);
    }

    public void SetSkipStepGroups(params string[] skipStepGroups)
    {
      foreach (string skipStepGroup in skipStepGroups)
        this.m_skipStepGroups.Add(skipStepGroup);
    }
  }
}
