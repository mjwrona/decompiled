// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.WorkItemTrackingQueryOptimizationConfiguration
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  public class WorkItemTrackingQueryOptimizationConfiguration
  {
    public WorkItemTrackingQueryOptimizationConfiguration(RegistryEntryCollection queryOptimizations)
    {
      Dictionary<string, QueryOptimization> dictionary1 = new Dictionary<string, QueryOptimization>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<Guid, QueryOptimization> dictionary2 = new Dictionary<Guid, QueryOptimization>();
      foreach (RegistryEntry queryOptimization1 in queryOptimizations)
      {
        string str = queryOptimization1.Path.Substring("/Service/WorkItemTracking/Settings/QueryOptimizations/".Length);
        if (str != null)
        {
          QueryOptimization queryOptimization2 = WorkItemTrackingQueryOptimizationConfiguration.ParseQueryOptimization(queryOptimization1.GetValue<string>());
          Guid result;
          if (Guid.TryParse(str, out result))
            dictionary2[result] = queryOptimization2;
          else
            dictionary1[str] = queryOptimization2;
        }
      }
      this.QueryOptimizationsById = (IReadOnlyDictionary<Guid, QueryOptimization>) new ReadOnlyDictionary<Guid, QueryOptimization>((IDictionary<Guid, QueryOptimization>) dictionary2);
      this.QueryOptimizationsByHash = (IReadOnlyDictionary<string, QueryOptimization>) new ReadOnlyDictionary<string, QueryOptimization>((IDictionary<string, QueryOptimization>) dictionary1);
    }

    public IReadOnlyDictionary<string, QueryOptimization> QueryOptimizationsByHash { get; }

    public IReadOnlyDictionary<Guid, QueryOptimization> QueryOptimizationsById { get; }

    public static QueryOptimization ParseQueryOptimization(string queryOptimizations)
    {
      if (string.IsNullOrWhiteSpace(queryOptimizations))
        return QueryOptimization.None;
      QueryOptimization queryOptimization = QueryOptimization.None;
      string str1 = queryOptimizations;
      char[] chArray = new char[1]{ ';' };
      foreach (string str2 in str1.Split(chArray))
      {
        string lower = str2.Trim().ToLower();
        if (lower != null)
        {
          switch (lower.Length)
          {
            case 10:
              if (lower == "forceorder")
              {
                queryOptimization |= QueryOptimization.ForceOrder;
                continue;
              }
              continue;
            case 14:
              if (lower == "moveorclauseup")
              {
                queryOptimization |= QueryOptimization.MoveOrClauseUp;
                continue;
              }
              continue;
            case 18:
              switch (lower[5])
              {
                case 'c':
                  if (lower == "forcecustomtablepk")
                  {
                    queryOptimization |= QueryOptimization.ForceCustomTablePK;
                    continue;
                  }
                  continue;
                case 'f':
                  if (lower == "forcefulltextindex")
                  {
                    queryOptimization |= QueryOptimization.ForceFullTextIndex;
                    continue;
                  }
                  continue;
                default:
                  continue;
              }
            case 22:
              if (lower == "fulltextjoinforceorder")
              {
                queryOptimization |= QueryOptimization.FullTextJoinForceOrder;
                continue;
              }
              continue;
            case 23:
              if (lower == "donotforcefulltextindex")
              {
                queryOptimization |= QueryOptimization.DoNotForceFullTextIndex;
                continue;
              }
              continue;
            case 31:
              if (lower == "fulltextsearchresultintemptable")
              {
                queryOptimization |= QueryOptimization.FullTextSearchResultInTempTable;
                continue;
              }
              continue;
            case 35:
              if (lower == "disablenonclusteredcolumnstoreindex")
              {
                queryOptimization |= QueryOptimization.DisableNonClusteredColumnstoreIndex;
                continue;
              }
              continue;
            default:
              continue;
          }
        }
      }
      return queryOptimization;
    }
  }
}
