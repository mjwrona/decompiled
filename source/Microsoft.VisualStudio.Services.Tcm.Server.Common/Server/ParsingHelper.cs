// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ParsingHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class ParsingHelper
  {
    public static List<TestCaseResultIdentifier> ParseTestCaseResultIdentifierList(
      string resultIdString,
      char separator)
    {
      List<TestCaseResultIdentifier> resultIdentifierList = new List<TestCaseResultIdentifier>();
      if (string.IsNullOrEmpty(resultIdString))
        return resultIdentifierList;
      string str = resultIdString;
      char[] chArray = new char[1]{ separator };
      foreach (string json in ((IEnumerable<string>) str.Split(chArray)).ToList<string>())
        resultIdentifierList.Add(JsonUtilities.Deserialize<TestCaseResultIdentifier>(json, true));
      return resultIdentifierList;
    }

    public static List<string> ParseCommaSeparatedString(string strings)
    {
      if (string.IsNullOrEmpty(strings))
        return new List<string>();
      return ((IEnumerable<string>) strings.Split(',')).ToList<string>();
    }

    public static List<int> ParseIds(string ids, bool isPositive = true)
    {
      List<int> ids1 = new List<int>();
      if (ids != null)
      {
        string str = ids;
        char[] chArray = new char[1]{ ',' };
        foreach (string s in str.Split(chArray))
        {
          int result;
          if (int.TryParse(s, out result) && (isPositive && result > 0 || !isPositive))
            ids1.Add(result);
        }
      }
      return ids1;
    }

    public static Dictionary<string, string> ParseODataOrderBy(string orderBy)
    {
      Dictionary<string, string> odataOrderBy = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (string.IsNullOrEmpty(orderBy))
        return (Dictionary<string, string>) null;
      foreach (string str in ParsingHelper.ParseCommaSeparatedString(orderBy))
      {
        List<string> list = ((IEnumerable<string>) str.Trim().Split(new string[1]
        {
          " "
        }, StringSplitOptions.None)).ToList<string>().Select<string, string>((Func<string, string>) (x => x.Trim())).ToList<string>();
        switch (list.Count)
        {
          case 1:
            if (string.IsNullOrEmpty(list[0]) || !TestResultsConstants.TestResultProperties.Contains(list[0]))
              throw new InvalidPropertyException(string.Format(ServerResources.InvalidArgument, (object) nameof (orderBy), (object) orderBy));
            list.Add(ODataQueryConstants.OrderByAsc);
            break;
          case 2:
            if (string.IsNullOrEmpty(list[0]) || string.IsNullOrEmpty(list[1]) || !list[1].Equals(ODataQueryConstants.OrderByAsc, StringComparison.OrdinalIgnoreCase) && !list[1].Equals(ODataQueryConstants.OrderByDesc, StringComparison.OrdinalIgnoreCase) || !TestResultsConstants.TestResultProperties.Contains(list[0]))
              throw new InvalidPropertyException(string.Format(ServerResources.InvalidArgument, (object) nameof (orderBy), (object) orderBy));
            break;
          default:
            throw new InvalidPropertyException(string.Format(ServerResources.InvalidArgument, (object) nameof (orderBy), (object) orderBy));
        }
        if (odataOrderBy.ContainsKey(list[0]))
          throw new InvalidPropertyException(string.Format(ServerResources.InvalidArgument, (object) nameof (orderBy), (object) orderBy));
        odataOrderBy.Add(list[0], list[1]);
      }
      return odataOrderBy;
    }

    public static Dictionary<string, Tuple<string, List<string>>> ParseODataQueryFilter(
      string filter,
      out bool isRerunOnPassedFilter)
    {
      isRerunOnPassedFilter = false;
      Dictionary<string, Tuple<string, List<string>>> parsedFilter = new Dictionary<string, Tuple<string, List<string>>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (string.IsNullOrEmpty(filter))
        return (Dictionary<string, Tuple<string, List<string>>>) null;
      string str1 = filter;
      string[] separator = new string[1]
      {
        ODataQueryConstants.Operator_AND
      };
      foreach (string str2 in str1.Split(separator, StringSplitOptions.None))
      {
        string cond = str2.Trim();
        if (string.IsNullOrEmpty(cond))
          throw new InvalidPropertyException(string.Format(ServerResources.InvalidArgument, (object) nameof (filter), (object) filter));
        bool isRerunOnPassedFilter1;
        ParsingHelper.ParseODataQueryCondition(cond, parsedFilter, out isRerunOnPassedFilter1);
        isRerunOnPassedFilter |= isRerunOnPassedFilter1;
      }
      return !isRerunOnPassedFilter || ParsingHelper.isParsedFilterValid(parsedFilter) ? parsedFilter : throw new InvalidPropertyException(string.Format(ServerResources.InvalidArgument, (object) nameof (filter), (object) filter));
    }

    private static bool isParsedFilterValid(
      Dictionary<string, Tuple<string, List<string>>> parsedFilter)
    {
      foreach (KeyValuePair<string, Tuple<string, List<string>>> keyValuePair in parsedFilter)
      {
        if (!string.Equals(keyValuePair.Key, TestResultsConstants.OutcomeColumnName, StringComparison.InvariantCultureIgnoreCase) && !string.Equals(keyValuePair.Key, TestResultsConstants.OwnerFilterName, StringComparison.InvariantCultureIgnoreCase) && !string.Equals(keyValuePair.Key, TestResultsConstants.ContainerFilterName, StringComparison.InvariantCultureIgnoreCase))
          return false;
      }
      return true;
    }

    public static void ParseODataQueryCondition(
      string cond,
      Dictionary<string, Tuple<string, List<string>>> parsedFilter,
      out bool isRerunOnPassedFilter)
    {
      isRerunOnPassedFilter = false;
      string[] source = cond.Split(new string[1]
      {
        ODataQueryConstants.Operator_Equal
      }, StringSplitOptions.None);
      string str = ((IEnumerable<string>) source).Count<string>() == 2 ? source[0].Trim() : throw new InvalidPropertyException(string.Format(ServerResources.InvalidArgument, (object) "filter", (object) cond));
      if (string.IsNullOrEmpty(str) || !TestResultsConstants.TestResultProperties.Contains<string>(str, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase))
        throw new InvalidPropertyException(string.Format(ServerResources.InvalidArgument, (object) "filter", (object) cond));
      parsedFilter[str] = new Tuple<string, List<string>>(SQLConstants.Operator_IN, new List<string>());
      foreach (string a in ((IEnumerable<string>) source[1].Trim().Split(new string[1]
      {
        ","
      }, StringSplitOptions.None)).Select<string, string>((Func<string, string>) (v => v.Trim())))
      {
        if (string.Equals(str, TestResultsConstants.OutcomeColumnName, StringComparison.InvariantCultureIgnoreCase))
        {
          if (string.Equals(a, TestResultsConstants.PassedOnRerun, StringComparison.InvariantCultureIgnoreCase))
          {
            isRerunOnPassedFilter = true;
          }
          else
          {
            Microsoft.TeamFoundation.TestManagement.Client.TestOutcome result;
            if (!Enum.TryParse<Microsoft.TeamFoundation.TestManagement.Client.TestOutcome>(a, true, out result))
              throw new InvalidPropertyException(string.Format(ServerResources.InvalidArgument, (object) "filter", (object) cond));
            parsedFilter[str].Item2.Add(((byte) result).ToString());
          }
        }
        else
          parsedFilter[str].Item2.Add(a);
      }
      if (parsedFilter[str].Item2.Count != 0)
        return;
      parsedFilter.Remove(str);
    }

    public static List<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome> ParseOutcomes(
      string outcomes)
    {
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome> outcomes1 = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome>();
      if (string.IsNullOrEmpty(outcomes))
        return outcomes1;
      foreach (string str in (IEnumerable<string>) outcomes.Split(new string[1]
      {
        ","
      }, StringSplitOptions.RemoveEmptyEntries) ?? Enumerable.Empty<string>())
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome result;
        if (!Enum.TryParse<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome>(str, true, out result))
          throw new InvalidPropertyException(string.Format(ServerResources.InvalidArgument, (object) "outcome", (object) str));
        outcomes1.Add(result);
      }
      return outcomes1;
    }

    public static void ParseContinuationTokenResultId(
      string continuationToken,
      out int continuationTokenRunId,
      out int continuationTokenResultId)
    {
      continuationTokenRunId = 0;
      continuationTokenResultId = 0;
      if (string.IsNullOrEmpty(continuationToken))
        return;
      List<string> list = ((IEnumerable<string>) continuationToken.Split('_')).ToList<string>();
      if (list.Count != 2)
        return;
      int.TryParse(list[0], out continuationTokenRunId);
      int.TryParse(list[1], out continuationTokenResultId);
    }
  }
}
