// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Utilities.TeamProjectUtilAzureBoards
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Utilities
{
  internal static class TeamProjectUtilAzureBoards
  {
    private static readonly string s_area = nameof (TeamProjectUtilAzureBoards);
    private static readonly string s_layer = nameof (TeamProjectUtilAzureBoards);

    public static bool IsTpcOptedOutOfPromote(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(1000002, TeamProjectUtilAzureBoards.s_area, TeamProjectUtilAzureBoards.s_layer, nameof (IsTpcOptedOutOfPromote));
      ArtifactSpec artifactSpec = new ArtifactSpec(ArtifactKinds.ProcessTemplate, 0, 0);
      using (TeamFoundationDataReader properties = requestContext.GetService<TeamFoundationPropertyService>().GetProperties(requestContext, artifactSpec, (IEnumerable<string>) new string[1]
      {
        ProcessTemplateVersionPropertyNames.IsOptOutOfPromote
      }))
      {
        IEnumerator enumerator1 = properties.GetEnumerator();
        try
        {
          if (enumerator1.MoveNext())
          {
            using (IEnumerator<PropertyValue> enumerator2 = ((ArtifactPropertyValue) enumerator1.Current).PropertyValues.GetEnumerator())
            {
              if (enumerator2.MoveNext())
              {
                PropertyValue current = enumerator2.Current;
                bool result = false;
                bool.TryParse(current.Value as string, out result);
                requestContext.TraceLeave(1000003, TeamProjectUtilAzureBoards.s_area, TeamProjectUtilAzureBoards.s_layer, nameof (IsTpcOptedOutOfPromote));
                return result;
              }
            }
          }
        }
        finally
        {
          if (enumerator1 is IDisposable disposable)
            disposable.Dispose();
        }
      }
      requestContext.TraceLeave(1000003, TeamProjectUtilAzureBoards.s_area, TeamProjectUtilAzureBoards.s_layer, nameof (IsTpcOptedOutOfPromote));
      return false;
    }
  }
}
