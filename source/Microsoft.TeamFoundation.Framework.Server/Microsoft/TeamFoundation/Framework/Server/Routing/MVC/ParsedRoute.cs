// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Routing.MVC.ParsedRoute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server.Routing.MVC
{
  internal sealed class ParsedRoute
  {
    public ParsedRoute(IList<PathSegment> pathSegments) => this.PathSegments = pathSegments;

    private IList<PathSegment> PathSegments { get; set; }

    public bool Match(
      IDictionary<string, object> matchedValues,
      IList<string> requestPathSegments,
      IDictionary<string, object> defaultValues)
    {
      bool flag1 = false;
      bool flag2 = false;
      for (int index = 0; index < this.PathSegments.Count; ++index)
      {
        PathSegment pathSegment = this.PathSegments[index];
        if (requestPathSegments.Count <= index)
          flag1 = true;
        string requestPathSegment = flag1 ? (string) null : requestPathSegments[index];
        switch (pathSegment)
        {
          case SeparatorPathSegment _:
            if (!flag1 && !string.Equals(requestPathSegment, "/", StringComparison.Ordinal))
              return false;
            break;
          case ContentPathSegment contentPathSegment:
            if (contentPathSegment.IsCatchAll)
            {
              this.MatchCatchAll(contentPathSegment, requestPathSegments.Skip<string>(index), defaultValues, matchedValues);
              flag2 = true;
              break;
            }
            if (!this.MatchContentPathSegment(contentPathSegment, requestPathSegment, defaultValues, matchedValues))
              return false;
            break;
        }
      }
      if (!flag2 && this.PathSegments.Count < requestPathSegments.Count)
      {
        for (int count = this.PathSegments.Count; count < requestPathSegments.Count; ++count)
        {
          if (!RouteParser.IsSeparator(requestPathSegments[count]))
            return false;
        }
      }
      if (defaultValues != null)
      {
        foreach (KeyValuePair<string, object> defaultValue in (IEnumerable<KeyValuePair<string, object>>) defaultValues)
        {
          if (!matchedValues.ContainsKey(defaultValue.Key))
            matchedValues.Add(defaultValue.Key, defaultValue.Value);
        }
      }
      return true;
    }

    private void MatchCatchAll(
      ContentPathSegment contentPathSegment,
      IEnumerable<string> remainingRequestSegments,
      IDictionary<string, object> defaultValues,
      IDictionary<string, object> matchedValues)
    {
      string str = string.Join(string.Empty, remainingRequestSegments.ToArray<string>());
      ParameterSubsegment subsegment = contentPathSegment.Subsegments[0] as ParameterSubsegment;
      object obj = (object) null;
      if (str.Length > 0)
        obj = (object) str;
      else
        defaultValues?.TryGetValue(subsegment.ParameterName, out obj);
      matchedValues.Add(subsegment.ParameterName, obj);
    }

    private bool MatchContentPathSegment(
      ContentPathSegment routeSegment,
      string requestPathSegment,
      IDictionary<string, object> defaultValues,
      IDictionary<string, object> matchedValues)
    {
      if (string.IsNullOrEmpty(requestPathSegment))
      {
        object obj;
        if (routeSegment.Subsegments.Count > 1 || !(routeSegment.Subsegments[0] is ParameterSubsegment subsegment) || defaultValues == null || !defaultValues.TryGetValue(subsegment.ParameterName, out obj))
          return false;
        matchedValues.Add(subsegment.ParameterName, obj);
        return true;
      }
      int num1 = requestPathSegment.Length;
      int index = routeSegment.Subsegments.Count - 1;
      ParameterSubsegment parameterSubsegment = (ParameterSubsegment) null;
      LiteralSubsegment literalSubsegment = (LiteralSubsegment) null;
      for (; index >= 0; --index)
      {
        int num2 = num1;
        if (routeSegment.Subsegments[index] is ParameterSubsegment subsegment2)
          parameterSubsegment = subsegment2;
        else if (routeSegment.Subsegments[index] is LiteralSubsegment subsegment1)
        {
          literalSubsegment = subsegment1;
          int startIndex = num1 - 1;
          if (parameterSubsegment != null)
            --startIndex;
          if (startIndex < 0)
            return false;
          int num3 = requestPathSegment.LastIndexOf(subsegment1.Literal, startIndex, StringComparison.OrdinalIgnoreCase);
          if (num3 == -1 || index == routeSegment.Subsegments.Count - 1 && num3 + subsegment1.Literal.Length != requestPathSegment.Length)
            return false;
          num2 = num3;
        }
        if (parameterSubsegment != null && (literalSubsegment != null && subsegment2 == null || index == 0))
        {
          int startIndex;
          int length;
          if (literalSubsegment == null)
          {
            startIndex = index != 0 ? num2 : 0;
            length = num1;
          }
          else if (index == 0 && subsegment2 != null)
          {
            startIndex = 0;
            length = num1;
          }
          else
          {
            startIndex = num2 + literalSubsegment.Literal.Length;
            length = num1 - startIndex;
          }
          string str = startIndex > 0 || length != requestPathSegment.Length ? requestPathSegment.Substring(startIndex, length) : requestPathSegment;
          if (string.IsNullOrEmpty(str))
            return false;
          matchedValues.Add(parameterSubsegment.ParameterName, (object) str);
          parameterSubsegment = (ParameterSubsegment) null;
          literalSubsegment = (LiteralSubsegment) null;
        }
        num1 = num2;
      }
      return num1 == 0 || routeSegment.Subsegments[0] is ParameterSubsegment;
    }
  }
}
