// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Queryable.PathBox
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Microsoft.Azure.Cosmos.Table.Queryable
{
  internal class PathBox
  {
    private const char EntireEntityMarker = '*';
    private readonly List<StringBuilder> projectionPaths = new List<StringBuilder>();
    private readonly List<StringBuilder> expandPaths = new List<StringBuilder>();
    private readonly Stack<ParameterExpression> parameterExpressions = new Stack<ParameterExpression>();
    private readonly Dictionary<ParameterExpression, string> basePaths = new Dictionary<ParameterExpression, string>((IEqualityComparer<ParameterExpression>) ReferenceEqualityComparer<ParameterExpression>.Instance);

    internal PathBox() => this.projectionPaths.Add(new StringBuilder());

    internal IEnumerable<string> ProjectionPaths => this.projectionPaths.Where<StringBuilder>((Func<StringBuilder, bool>) (s => s.Length > 0)).Select<StringBuilder, string>((Func<StringBuilder, string>) (s => s.ToString())).Distinct<string>();

    internal IEnumerable<string> ExpandPaths => this.expandPaths.Where<StringBuilder>((Func<StringBuilder, bool>) (s => s.Length > 0)).Select<StringBuilder, string>((Func<StringBuilder, string>) (s => s.ToString())).Distinct<string>();

    internal void PushParamExpression(ParameterExpression pe)
    {
      StringBuilder stringBuilder = this.projectionPaths.Last<StringBuilder>();
      this.basePaths.Add(pe, stringBuilder.ToString());
      this.projectionPaths.Remove(stringBuilder);
      this.parameterExpressions.Push(pe);
    }

    internal void PopParamExpression() => this.parameterExpressions.Pop();

    internal ParameterExpression ParamExpressionInScope => this.parameterExpressions.Peek();

    internal void StartNewPath()
    {
      StringBuilder sb = new StringBuilder(this.basePaths[this.ParamExpressionInScope]);
      PathBox.RemoveEntireEntityMarkerIfPresent(sb);
      this.expandPaths.Add(new StringBuilder(sb.ToString()));
      PathBox.AddEntireEntityMarker(sb);
      this.projectionPaths.Add(sb);
    }

    internal void AppendToPath(PropertyInfo pi)
    {
      Type elementType = TypeSystem.GetElementType(pi.PropertyType);
      if (CommonUtil.IsClientType(elementType))
      {
        StringBuilder stringBuilder = this.expandPaths.Last<StringBuilder>();
        if (stringBuilder.Length > 0)
          stringBuilder.Append('/');
        stringBuilder.Append(pi.Name);
      }
      StringBuilder sb = this.projectionPaths.Last<StringBuilder>();
      PathBox.RemoveEntireEntityMarkerIfPresent(sb);
      if (sb.Length > 0)
        sb.Append('/');
      sb.Append(pi.Name);
      if (!CommonUtil.IsClientType(elementType))
        return;
      PathBox.AddEntireEntityMarker(sb);
    }

    private static void RemoveEntireEntityMarkerIfPresent(StringBuilder sb)
    {
      if (sb.Length > 0 && sb[sb.Length - 1] == '*')
        sb.Remove(sb.Length - 1, 1);
      if (sb.Length <= 0 || sb[sb.Length - 1] != '/')
        return;
      sb.Remove(sb.Length - 1, 1);
    }

    private static void AddEntireEntityMarker(StringBuilder sb)
    {
      if (sb.Length > 0)
        sb.Append('/');
      sb.Append('*');
    }
  }
}
