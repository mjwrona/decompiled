// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata.WorkItemTrackingOutOfBoxHelpTextCache
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata
{
  internal class WorkItemTrackingOutOfBoxHelpTextCache : 
    WorkItemTrackingOutOfBoxValuesCacheBase<HelpTextDescriptor>
  {
    public bool TryGetOutOfBoxHelpTexts(
      IVssRequestContext requestContext,
      ProcessDescriptor systemDescriptor,
      string workItemTypeReferenceName,
      out IReadOnlyCollection<HelpTextDescriptor> helpTexts)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return this.TryGetOutOfBoxValues(requestContext, systemDescriptor, workItemTypeReferenceName, ProcessMetadataResourceType.HelpTextMetadata, "help-texts", WorkItemTrackingOutOfBoxHelpTextCache.\u003C\u003EO.\u003C0\u003E__ResolveHelpTexts ?? (WorkItemTrackingOutOfBoxHelpTextCache.\u003C\u003EO.\u003C0\u003E__ResolveHelpTexts = new Func<IVssRequestContext, IEnumerable<HelpTextDescriptor>, IEnumerable<HelpTextDescriptor>>(WorkItemRulesService.ResolveHelpTexts)), out helpTexts);
    }
  }
}
