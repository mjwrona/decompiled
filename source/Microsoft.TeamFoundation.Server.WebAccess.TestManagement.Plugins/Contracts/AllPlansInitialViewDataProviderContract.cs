// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.Contracts.AllPlansInitialViewDataProviderContract
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.Contracts
{
  public class AllPlansInitialViewDataProviderContract : AllPlansViewDataProviderContract
  {
    public AllPlansInitialViewDataProviderContract(IEnumerable<WebApiTeam> teams)
      : base(teams)
    {
    }

    public Dictionary<string, TestPlan> TestPlanMap { get; set; }

    public void AddTestPlans(IList<WorkItem> testPlans)
    {
      this.TestPlanMap = new Dictionary<string, TestPlan>();
      foreach (WorkItem testPlan in (IEnumerable<WorkItem>) testPlans)
      {
        int? id = testPlan.Id;
        if (id.HasValue)
        {
          id = testPlan.Id;
          this.TestPlanMap.Add(id.GetValueOrDefault().ToString((IFormatProvider) CultureInfo.InvariantCulture), new TestPlan()
          {
            Id = Convert.ToInt32(testPlan.Fields["System.Id"]),
            Name = Convert.ToString(testPlan.Fields["System.Title"]),
            Fields = new Fields()
            {
              AssignedTo = this.GetValue<IdentityRef>(testPlan.Fields, "System.AssignedTo", new IdentityRef()),
              AreaPath = this.GetValue<string>(testPlan.Fields, "System.AreaPath", string.Empty),
              IterationPath = this.GetValue<string>(testPlan.Fields, "System.IterationPath", string.Empty),
              State = this.GetValue<string>(testPlan.Fields, "System.State", string.Empty)
            }
          });
        }
      }
    }

    private T GetValue<T>(IDictionary<string, object> Fields, string key, T defaultValue)
    {
      T obj;
      Fields.TryGetValue<string, T>(key, out obj);
      return obj ?? defaultValue;
    }
  }
}
