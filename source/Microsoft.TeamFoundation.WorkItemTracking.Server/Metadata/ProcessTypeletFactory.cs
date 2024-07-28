// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ProcessTypeletFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class ProcessTypeletFactory
  {
    public static IReadOnlyCollection<ProcessTypelet> Create(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemTypeletRecord> databaseRecords,
      WorkItemTrackingFieldService fieldService)
    {
      List<ProcessTypelet> processTypeletList = new List<ProcessTypelet>();
      foreach (WorkItemTypeletRecord databaseRecord in databaseRecords)
      {
        if (databaseRecord.TypeletType == 1)
        {
          processTypeletList.Add((ProcessTypelet) ProcessWorkItemType.Create(requestContext, databaseRecord, fieldService));
        }
        else
        {
          if (databaseRecord.TypeletType != 2)
            throw new InvalidOperationException(string.Format("Trying to convert record of type that is not understood type : {0}", (object) databaseRecord.TypeletType));
          processTypeletList.Add((ProcessTypelet) Behavior.Create(requestContext, databaseRecord, fieldService));
        }
      }
      return (IReadOnlyCollection<ProcessTypelet>) processTypeletList;
    }

    public static IReadOnlyCollection<ProcessTypelet> TransformIdentitiyRulesInTyplets(
      IVssRequestContext requestContext,
      IReadOnlyCollection<ProcessTypelet> typelets)
    {
      IReadOnlyCollection<ProcessTypelet> typelets1 = new IdentityDefaultsTransformer(requestContext).PopulateDisplayNames((IEnumerable<ProcessTypelet>) typelets);
      return new IdentityScopeTransformer(requestContext).PopulateIdentityAllowedValueRules((IEnumerable<ProcessTypelet>) typelets1);
    }
  }
}
