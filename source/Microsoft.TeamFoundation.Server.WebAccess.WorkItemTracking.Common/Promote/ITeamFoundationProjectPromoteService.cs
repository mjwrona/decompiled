// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Promote.ITeamFoundationProjectPromoteService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Promote
{
  [DefaultServiceImplementation(typeof (TeamFoundationProjectPromoteService))]
  public interface ITeamFoundationProjectPromoteService : IVssFrameworkService
  {
    void RenameFields(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<string, string>> fields);

    void QueuePromoteJob(
      IVssRequestContext requestContext,
      Guid processTemplateTypeId,
      bool isXmlToInheritedPromote = false,
      Guid? projectIdToPromote = null);

    PromoteProjectInfo Promote(
      IVssRequestContext requestContext,
      Guid projectId,
      bool isXmlToInheritedPromote,
      Guid targetProcessTemplateTypeIdForXmlToInheritedPromote,
      out string promoteLog);

    void PromoteImportedProjectToOOBProcess(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid oobProcessTypeId,
      bool skipProcessStamp = false);
  }
}
