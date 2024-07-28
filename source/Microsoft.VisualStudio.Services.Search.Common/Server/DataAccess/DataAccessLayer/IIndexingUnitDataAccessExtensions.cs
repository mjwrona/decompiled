// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.IIndexingUnitDataAccessExtensions
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;

namespace Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer
{
  internal static class IIndexingUnitDataAccessExtensions
  {
    internal static IndexingUnit AddIndexingUnitAndNotify(
      this IIndexingUnitDataAccess indexingUnitDataAccess,
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit)
    {
      IndexingUnit indexingUnit1 = indexingUnitDataAccess.AddIndexingUnit(requestContext, indexingUnit);
      IIndexingUnitDataAccessExtensions.NotifyDocumentContractTypeChange(requestContext);
      return indexingUnit1;
    }

    internal static IndexingUnit UpdateIndexingUnitAndNotify(
      this IIndexingUnitDataAccess indexingUnitDataAccess,
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit)
    {
      IndexingUnit indexingUnit1 = indexingUnitDataAccess.UpdateIndexingUnit(requestContext, indexingUnit);
      IIndexingUnitDataAccessExtensions.NotifyDocumentContractTypeChange(requestContext);
      return indexingUnit1;
    }

    private static void NotifyDocumentContractTypeChange(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, Microsoft.VisualStudio.Services.Search.Common.SqlNotificationEventClasses.DocumentContractTypeChanged, (string) null);
  }
}
