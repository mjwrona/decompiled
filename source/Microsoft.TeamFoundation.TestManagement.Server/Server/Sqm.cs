// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Sqm
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  internal class Sqm
  {
    internal Sqm()
    {
    }

    internal List<KeyValuePair<TcmProperty, int>> FetchAllProperties(
      TfsTestManagementRequestContext context)
    {
      List<KeyValuePair<TcmProperty, int>> keyValuePairList = new List<KeyValuePair<TcmProperty, int>>();
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create((TestManagementRequestContext) context))
        return planningDatabase.FetchAllProperties();
    }

    internal int GetPropertyValue(TestManagementRequestContext context, TcmProperty property)
    {
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        return planningDatabase.GetPropertyValue(property);
    }

    internal void UpdateProperties(
      TestManagementRequestContext context,
      List<KeyValuePair<TcmProperty, int>> propertyTable)
    {
      this.ValidatePropertyParams(propertyTable);
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        planningDatabase.UpdateProperties(propertyTable);
    }

    private void ValidatePropertyParams(List<KeyValuePair<TcmProperty, int>> propertyTable)
    {
      foreach (KeyValuePair<TcmProperty, int> keyValuePair in propertyTable)
      {
        if (keyValuePair.Value < 0)
          throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "Value"));
      }
    }
  }
}
