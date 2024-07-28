// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionInputFilterExtensions
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using Microsoft.VisualStudio.Services.FormInput;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  public static class SubscriptionInputFilterExtensions
  {
    public static bool Evaluate(
      this IEnumerable<InputFilter> filters,
      IDictionary<string, string> inputs)
    {
      if (filters == null || !filters.Any<InputFilter>())
        return true;
      foreach (InputFilter filter in filters)
      {
        if (filter.Evaluate(inputs))
          return true;
      }
      return false;
    }

    public static bool Evaluate(this InputFilter filter, IDictionary<string, string> inputs)
    {
      foreach (InputFilterCondition condition in filter.Conditions)
      {
        if (!condition.Evaluate(inputs))
          return false;
      }
      return true;
    }

    public static bool Evaluate(
      this InputFilterCondition condition,
      IDictionary<string, string> inputs)
    {
      string b = (string) null;
      inputs.TryGetValue(condition.InputId, out b);
      return condition.Operator == InputFilterOperator.NotEquals ? !string.Equals(condition.InputValue, b, condition.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) : string.Equals(condition.InputValue, b, condition.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
    }

    public static List<InputFilter> ToFilters(this InputFilterCondition expression) => new List<InputFilter>()
    {
      expression.ToFilter()
    };

    public static InputFilter ToFilter(this InputFilterCondition condition) => new InputFilter()
    {
      Conditions = new List<InputFilterCondition>()
      {
        condition
      }
    };

    public static List<InputFilter> ToFilters(this IEnumerable<InputFilterCondition> conditions) => new List<InputFilter>()
    {
      conditions.ToFilter()
    };

    public static InputFilter ToFilter(this IEnumerable<InputFilterCondition> conditions) => new InputFilter()
    {
      Conditions = new List<InputFilterCondition>(conditions)
    };
  }
}
