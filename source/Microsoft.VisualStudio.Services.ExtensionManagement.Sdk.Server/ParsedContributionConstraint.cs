// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ParsedContributionConstraint
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  internal class ParsedContributionConstraint
  {
    private object m_parsedProperties;
    private IContributionFilter m_plugin;
    private Exception m_parseException;
    private ContributionConstraint m_sharedConstraint;
    private const string s_area = "ContributionService";
    private const string s_layer = "ContributionFilterService";

    public ParsedContributionConstraint(
      ContributionConstraint constraint,
      ContributionConstraint sharedConstraint,
      IContributionFilter plugin)
    {
      this.Constraint = constraint;
      this.m_sharedConstraint = sharedConstraint;
      this.m_plugin = plugin;
      if (constraint.Relationships != null && constraint.Relationships.Count > 0)
        this.Relationships = new HashSet<string>((IEnumerable<string>) constraint.Relationships, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      string str;
      if (constraint.Properties != null && !constraint.Properties.TryGetValue<string>("phase", out str) || sharedConstraint != null && sharedConstraint.Properties != null && !sharedConstraint.Properties.TryGetValue<string>("phase", out str))
        this.Phase = str;
      if (plugin == null)
      {
        this.Evaluate = new Func<IVssRequestContext, IDictionary<string, bool>, Contribution, EvaluatedConstraint, bool>(this.EvaluateError);
      }
      else
      {
        JObject properties = (sharedConstraint != null ? sharedConstraint.Properties : constraint.Properties) ?? new JObject();
        try
        {
          this.m_parsedProperties = plugin.ProcessProperties(properties);
          if (sharedConstraint == null)
            this.Evaluate = new Func<IVssRequestContext, IDictionary<string, bool>, Contribution, EvaluatedConstraint, bool>(this.EvaluateConstraint);
          else
            this.Evaluate = new Func<IVssRequestContext, IDictionary<string, bool>, Contribution, EvaluatedConstraint, bool>(this.EvaluateSharedConstraint);
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(10025002, "ContributionService", "ContributionFilterService", ex);
          this.m_parseException = ex;
          this.Evaluate = new Func<IVssRequestContext, IDictionary<string, bool>, Contribution, EvaluatedConstraint, bool>(this.EvaluateError);
        }
      }
    }

    private bool EvaluateConstraint(
      IVssRequestContext requestContext,
      IDictionary<string, bool> sharedConstraintValues,
      Contribution contribution,
      EvaluatedConstraint evaluatedConstraint)
    {
      try
      {
        bool flag = this.m_plugin.ApplyFilter(requestContext, this.m_parsedProperties, contribution);
        return this.Constraint.Inverse ? !flag : flag;
      }
      catch (Exception ex)
      {
        if (evaluatedConstraint != null)
          evaluatedConstraint.Message = ex.Message;
        requestContext.TraceException(10025004, "ContributionService", "ContributionFilterService", ex);
        return false;
      }
    }

    private bool EvaluateSharedConstraint(
      IVssRequestContext requestContext,
      IDictionary<string, bool> sharedConstraintValues,
      Contribution contribution,
      EvaluatedConstraint evaluatedConstraint)
    {
      try
      {
        bool flag1;
        if (sharedConstraintValues != null && sharedConstraintValues.TryGetValue(this.Constraint.Id, out flag1))
          return this.Constraint.Inverse ? !flag1 : flag1;
        bool flag2 = this.m_plugin.ApplyFilter(requestContext, this.m_parsedProperties, (Contribution) null);
        bool flag3 = this.m_sharedConstraint.Inverse ? !flag2 : flag2;
        if (sharedConstraintValues != null)
          sharedConstraintValues[this.Constraint.Id] = flag3;
        return this.Constraint.Inverse ? !flag3 : flag3;
      }
      catch (Exception ex)
      {
        if (evaluatedConstraint != null)
          evaluatedConstraint.Message = ex.Message;
        requestContext.TraceException(10025005, "ContributionService", "ContributionFilterService", ex);
        return false;
      }
    }

    private bool EvaluateError(
      IVssRequestContext requestContext,
      IDictionary<string, bool> sharedConstraintValues,
      Contribution contribution,
      EvaluatedConstraint evaluatedConstraint)
    {
      if (evaluatedConstraint != null)
      {
        if (this.m_parseException != null)
          evaluatedConstraint.Message = string.Format("Failed to parse constraint properties: {0}.", (object) this.m_parseException.Message);
        else if (this.m_plugin == null)
          evaluatedConstraint.Message = "Unable to find constraint implmentation";
      }
      return false;
    }

    public Func<IVssRequestContext, IDictionary<string, bool>, Contribution, EvaluatedConstraint, bool> Evaluate { get; private set; }

    public ContributionConstraint Constraint { get; private set; }

    public HashSet<string> Relationships { get; private set; }

    public string Phase { get; private set; }
  }
}
