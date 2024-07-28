// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql.IndexingUnitChangeEventArchiveAccess
// Assembly: Microsoft.VisualStudio.Services.Search.Server.DataAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3B684226-797D-4C9F-9AC1-E10D39E316D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.DataAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation;
using System;

namespace Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql
{
  internal class IndexingUnitChangeEventArchiveAccess : 
    SqlAzureDataAccess,
    IIndexingUnitChangeEventArchiveDataAccess
  {
    private const int DaysDataToBeKept = 90;

    public void DeleteOldChangeEventsFromArchive(IVssRequestContext requestContext)
    {
      using (IndexingUnitChangeEventArchiveComponent component = requestContext.CreateComponent<IndexingUnitChangeEventArchiveComponent>())
        this.InvokeTableOperation<IndexingUnitChangeEventArchiveComponent>((Func<IndexingUnitChangeEventArchiveComponent>) (() =>
        {
          component.DeleteOldArchives(90);
          return (IndexingUnitChangeEventArchiveComponent) null;
        }));
    }
  }
}
