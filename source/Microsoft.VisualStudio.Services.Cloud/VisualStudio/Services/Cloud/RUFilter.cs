// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.RUFilter
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
  public class RUFilter
  {
    private readonly RURequestProperty Property;
    private readonly RUFilter.RUComparison Comparison;
    private readonly bool NegatedComparison;
    private readonly HashSet<object> Comparands;

    public RUFilter(string property, string comparison, string comparand)
    {
      this.Property = RURequestProperty.GetRURequestPropertyFromString(property);
      if (this.Property == null)
        throw new Exception("GetRURequestPropertyFromString(" + property + ") failed");
      if (comparison.StartsWith("!"))
      {
        this.NegatedComparison = true;
        comparison = comparison.Substring(1);
        if (comparison == "=")
          comparison = "==";
      }
      switch (comparison.ToLower())
      {
        case "==":
          this.Comparison = RUFilter.RUComparison.Equals;
          break;
        case "startswith":
          this.Comparison = RUFilter.RUComparison.StringStartsWith;
          break;
        case "endswith":
          this.Comparison = RUFilter.RUComparison.StringEndsWith;
          break;
        case "contains":
          this.Comparison = RUFilter.RUComparison.StringContains;
          break;
        case "is":
          this.Comparison = RUFilter.RUComparison.Satisfies;
          break;
        default:
          throw new Exception("Can't parse");
      }
      List<string> list = ((IEnumerable<string>) comparand.Split(',')).Select<string, string>((Func<string, string>) (x => x.Trim())).ToList<string>();
      if (this.Comparison == RUFilter.RUComparison.Equals)
        this.Comparands = this.Property is RURequestProperty_String ? new HashSet<object>(list.Select<string, object>((Func<string, object>) (s => this.Property.ConvertType(s))), (IEqualityComparer<object>) new RUFilter.StringCaseInvariantComparer()) : new HashSet<object>(list.Select<string, object>((Func<string, object>) (s => this.Property.ConvertType(s))));
      else
        this.Comparands = new HashSet<object>((IEnumerable<object>) list);
    }

    public RUStage KnownAt => this.Property.PropertyKnownAt;

    public bool ShouldGovernRequest(IVssRequestContext requestContext)
    {
      if (!requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.ResourceUtilization2.AccountLicenseTypeBasicTestPlan") && this.Property is RURequestProperty_AccountLicenseType)
        return true;
      object requestValue = this.Property.GetRequestValue(requestContext);
      bool flag1 = false;
      switch (this.Comparison)
      {
        case RUFilter.RUComparison.Equals:
          flag1 = this.Comparands.Contains(requestValue);
          break;
        case RUFilter.RUComparison.StringStartsWith:
          string requestValueString1 = requestValue?.ToString() ?? string.Empty;
          flag1 = this.Comparands.Any<object>((Func<object, bool>) (c => requestValueString1.StartsWith(c.ToString(), StringComparison.OrdinalIgnoreCase)));
          break;
        case RUFilter.RUComparison.StringEndsWith:
          string requestValueString2 = requestValue?.ToString() ?? string.Empty;
          flag1 = this.Comparands.Any<object>((Func<object, bool>) (c => requestValueString2.EndsWith(c.ToString(), StringComparison.OrdinalIgnoreCase)));
          break;
        case RUFilter.RUComparison.StringContains:
          string requestValueString3 = requestValue?.ToString() ?? string.Empty;
          flag1 = this.Comparands.Any<object>((Func<object, bool>) (c => requestValueString3.IndexOf(c.ToString(), StringComparison.OrdinalIgnoreCase) >= 0));
          break;
        case RUFilter.RUComparison.Satisfies:
          flag1 = this.Comparands.Any<object>((Func<object, bool>) (c => this.Property.EvaluateSpecialProperty(requestValue, c.ToString())));
          break;
      }
      bool flag2 = this.NegatedComparison ? !flag1 : flag1;
      requestContext.Trace(522304019, TraceLevel.Info, "ResourceUtilizationService", "Service", "Filter {0} returned {1}. RequestValue: {2}", (object) this.Property, (object) flag2, requestValue);
      return flag2;
    }

    private enum RUComparison
    {
      Equals,
      StringStartsWith,
      StringEndsWith,
      StringContains,
      Satisfies,
    }

    private class StringCaseInvariantComparer : IEqualityComparer<object>
    {
      public bool Equals(object comparand1, object comparand2) => string.Equals(comparand1 as string, comparand2 as string, StringComparison.OrdinalIgnoreCase);

      public int GetHashCode(object obj) => (obj as string).ToLowerInvariant().GetHashCode();
    }
  }
}
