// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent49
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent49 : WorkItemTrackingMetadataComponent48
  {
    public override ProvisionAllOobLinkTypesRequestResult ProvisionAllOOBLinkTypes(
      IDictionary<string, string> refNameToForwardNameMappings,
      IDictionary<string, string> refNameToReverseNameMappings)
    {
      this.PrepareStoredProcedure("prc_ProvisionOobLinkTypes");
      this.BindKeyValuePairStringTable("@forwardNamesByRefName", (IEnumerable<KeyValuePair<string, string>>) refNameToForwardNameMappings);
      this.BindKeyValuePairStringTable("@reverseNamesByRefName", (IEnumerable<KeyValuePair<string, string>>) refNameToReverseNameMappings);
      this.ExecuteNonQuery();
      return ProvisionAllOobLinkTypesRequestResult.Provisioned;
    }
  }
}
