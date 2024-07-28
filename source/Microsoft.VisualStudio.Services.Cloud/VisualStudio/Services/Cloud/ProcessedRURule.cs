// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ProcessedRURule
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class ProcessedRURule
  {
    public string RuleName;
    public string RuleDefinition;
    public List<RUFilter> Filters;
    public RUResource Resource;
    public List<RURequestProperty> EntityTypes;
    public Dictionary<string, ProcessedRUThreshold> Thresholds = new Dictionary<string, ProcessedRUThreshold>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public ProcessedRURule()
    {
    }

    public ProcessedRURule(ProcessedRURule other)
    {
      this.RuleName = other.RuleName;
      this.RuleDefinition = other.RuleDefinition;
      this.Filters = other.Filters;
      this.Resource = other.Resource;
      this.EntityTypes = other.EntityTypes;
      this.Thresholds = other.Thresholds.ToDictionary<KeyValuePair<string, ProcessedRUThreshold>, string, ProcessedRUThreshold>((Func<KeyValuePair<string, ProcessedRUThreshold>, string>) (entry => entry.Key), (Func<KeyValuePair<string, ProcessedRUThreshold>, ProcessedRUThreshold>) (entry => new ProcessedRUThreshold(entry.Value)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.WhenToCheckIfGoverned = other.WhenToCheckIfGoverned;
      this.HasPublicUserFilter = other.HasPublicUserFilter;
      this.HasIPAddressEntityType = other.HasIPAddressEntityType;
      this.ShouldOutputEntityToTelemetry = other.ShouldOutputEntityToTelemetry;
    }

    public void Compile(
      IVssRequestContext requestContext,
      Dictionary<string, IEnumerable<string>> macros,
      RURule rawRule)
    {
      this.RuleName = rawRule.RuleName;
      this.RuleDefinition = rawRule.RuleDefinition;
      this.EntityTypes = new List<RURequestProperty>();
      this.Filters = new List<RUFilter>();
      try
      {
        string[] array = ((IEnumerable<string>) this.RuleDefinition.Split(';')).SelectMany<string, string>((Func<string, IEnumerable<string>>) (l =>
        {
          IEnumerable<string> strings;
          if (macros.TryGetValue(l.TrimStart(), out strings))
            return strings;
          return (IEnumerable<string>) new List<string>()
          {
            l
          };
        })).ToArray<string>();
        for (int index = 0; index < array.Length - 1; ++index)
        {
          string str1 = array[index].Trim();
          int length1 = str1.IndexOf(' ');
          string property = str1.Substring(0, length1);
          string str2 = str1.Substring(length1 + 1).Trim();
          int length2 = str2.IndexOf(' ');
          string comparison = str2.Substring(0, length2);
          string comparand = str2.Substring(length2 + 1).Trim();
          this.Filters.Add(new RUFilter(property, comparison, comparand));
        }
        if (array.Length != 0)
        {
          string str = ((IEnumerable<string>) array).Last<string>();
          string[] strArray = str.Split(new string[1]
          {
            " by "
          }, StringSplitOptions.RemoveEmptyEntries);
          string resource;
          if (strArray.Length > 1)
          {
            resource = strArray[0].Trim();
            foreach (string property in ((IEnumerable<string>) strArray[1].Split(',')).Select<string, string>((Func<string, string>) (x => x.Trim())))
              this.EntityTypes.Add(RURequestProperty.GetRURequestPropertyFromString(property) ?? throw new Exception("GetRURequestPropertyFromString(" + property + ") failed"));
          }
          else
            resource = str.Trim();
          this.Resource = RUResource.GetRUResourceFromString(resource);
          if (this.Resource == null)
            throw new Exception("GetRUResourceFromString(" + resource + ") failed");
        }
        this.WhenToCheckIfGoverned = this.GetWhenToCheckIfGoverned(requestContext);
        this.HasPublicUserFilter = false;
        this.HasIPAddressEntityType = false;
        this.ShouldOutputEntityToTelemetry = this.EntityTypes.Any<RURequestProperty>((Func<RURequestProperty, bool>) (p => p.ShouldOutputEntityToTelemetry));
      }
      catch (Exception ex)
      {
        requestContext.Trace(522304008, TraceLevel.Error, "ResourceUtilizationService", "Service", "Error compiling rule [" + rawRule.RuleName + "]: " + this.RuleDefinition + " (" + ex.Message + ")");
        this.Filters.Clear();
        this.EntityTypes.Clear();
        this.Resource = (RUResource) null;
      }
    }

    public RUStage WhenToCheckIfGoverned { get; private set; }

    public bool HasPublicUserFilter { get; private set; }

    public bool HasIPAddressEntityType { get; private set; }

    public bool ShouldOutputEntityToTelemetry { get; private set; }

    internal string GetResourceName() => this.Resource.GetName();

    internal string GetFriendlyNamespace() => string.Join(",", this.EntityTypes.Select<RURequestProperty, string>((Func<RURequestProperty, string>) (x => x.GetName())));

    internal bool ShouldGovernRequest(IVssRequestContext requestContext)
    {
      if (!requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.ResourceUtilization2.AccountLicenseTypeBasicTestPlan") && this.RuleName.EndsWith("User.BasicTestPlans"))
        return false;
      foreach (RUFilter filter in this.Filters)
      {
        if (!filter.ShouldGovernRequest(requestContext))
          return false;
      }
      return true;
    }

    internal string GetEntity(IVssRequestContext requestContext) => string.Join(",", this.EntityTypes.Select<RURequestProperty, string>((Func<RURequestProperty, string>) (prop => prop.GetRequestValue(requestContext)?.ToString() ?? string.Empty)));

    internal string GetXEventEntity(XEventObjectBase xeventObject) => string.Join(",", this.EntityTypes.Select<RURequestProperty, string>((Func<RURequestProperty, string>) (prop => prop.GetXEventValue(xeventObject).ToString())));

    internal RUStage GetWhenToCheckIfGoverned(IVssRequestContext requestContext)
    {
      bool flag = requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.ResourceUtilization2.UseResourceUtilization21");
      if (this.Filters.Any<RUFilter>())
      {
        RUStage toCheckIfGoverned = this.Filters.Max<RUFilter, RUStage>((Func<RUFilter, RUStage>) (f => f.KnownAt));
        if (toCheckIfGoverned == RUStage.BeginRequest && !flag)
          toCheckIfGoverned = RUStage.RequestReady;
        return toCheckIfGoverned;
      }
      return flag ? RUStage.BeginRequest : RUStage.RequestReady;
    }
  }
}
