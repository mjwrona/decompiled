// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestAuthoringWiqlHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class TestAuthoringWiqlHelper
  {
    private const string c_selectStatementFormat = "SELECT {0} FROM  WorkItems \r\n                                                         WHERE [System.WorkItemType] IN GROUP 'Microsoft.TestCaseCategory' AND \r\n                                                               [System.Id] IN ({1})\r\n                                                         ORDER BY [System.ChangedDate] DESC";

    internal static string GetWiqlQuery(
      IList<string> fieldReferenceNames,
      string testCaseIdListString)
    {
      ArgumentUtility.CheckForNull<IList<string>>(fieldReferenceNames, nameof (fieldReferenceNames));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(testCaseIdListString, nameof (testCaseIdListString));
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SELECT {0} FROM  WorkItems \r\n                                                         WHERE [System.WorkItemType] IN GROUP 'Microsoft.TestCaseCategory' AND \r\n                                                               [System.Id] IN ({1})\r\n                                                         ORDER BY [System.ChangedDate] DESC", (object) TestAuthoringWiqlHelper.FormatWiqlSelectFields(fieldReferenceNames), (object) testCaseIdListString);
    }

    private static string FormatWiqlSelectFields(IList<string> fieldReferenceNames)
    {
      if (fieldReferenceNames.Count == 0)
        return "[System.Id]";
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < fieldReferenceNames.Count; ++index)
      {
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "[{0}]", (object) fieldReferenceNames[index]);
        if (index < fieldReferenceNames.Count - 1)
          stringBuilder.AppendFormat(",");
      }
      return stringBuilder.ToString();
    }
  }
}
