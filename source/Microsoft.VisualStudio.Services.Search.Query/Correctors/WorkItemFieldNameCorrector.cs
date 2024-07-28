// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.Correctors.WorkItemFieldNameCorrector
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Correctors;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Server.WorkItemFieldCache;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Query.Correctors
{
  internal class WorkItemFieldNameCorrector : TermCorrector
  {
    [StaticSafe]
    private static IReadOnlyDictionary<string, string> s_alternateFieldNames = WorkItemFieldNameCorrector.Inverse(WorkItemAlternateFieldNames.AlternateFieldNames);

    public override IExpression CorrectTerm(
      IVssRequestContext requestContext,
      TermExpression termExpression)
    {
      if (termExpression == null)
        throw new ArgumentNullException(nameof (termExpression));
      if (termExpression.IsOfType("*"))
        return (IExpression) termExpression;
      HashSet<IExpression> source = new HashSet<IExpression>();
      HashSet<string> stringSet = new HashSet<string>();
      IWorkItemFieldCacheService service = requestContext.GetService<IWorkItemFieldCacheService>();
      IEnumerable<WorkItemField> fieldsData;
      if (service.TryGetFieldByName(requestContext, termExpression.Type, out fieldsData))
        stringSet.UnionWith(fieldsData.Select<WorkItemField, string>((Func<WorkItemField, string>) (x => x.ReferenceName)));
      WorkItemField fieldData;
      if (service.TryGetFieldByRefName(requestContext, termExpression.Type, out fieldData))
        stringSet.Add(fieldData.ReferenceName);
      string str1;
      if (WorkItemFieldNameCorrector.s_alternateFieldNames.TryGetValue(termExpression.Type, out str1))
        stringSet.Add(str1);
      foreach (string str2 in stringSet)
        source.Add((IExpression) new TermExpression(str2.ToLowerInvariant(), termExpression.Operator, termExpression.Value));
      return source.Any<IExpression>() ? (source.Count != 1 ? (IExpression) new OrExpression(source.ToArray<IExpression>()) : source.First<IExpression>()) : (IExpression) new TermExpression("*", Operator.Matches, termExpression.ToString().Replace("*", " "));
    }

    private static IReadOnlyDictionary<string, string> Inverse(
      IReadOnlyDictionary<string, ISet<string>> input)
    {
      IDictionary<string, string> dictionary = (IDictionary<string, string>) new FriendlyDictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (KeyValuePair<string, ISet<string>> keyValuePair in (IEnumerable<KeyValuePair<string, ISet<string>>>) input)
      {
        foreach (string key in (IEnumerable<string>) keyValuePair.Value)
          dictionary.Add(key, keyValuePair.Key.ToLowerInvariant());
      }
      return (IReadOnlyDictionary<string, string>) new ReadOnlyDictionary<string, string>(dictionary);
    }
  }
}
