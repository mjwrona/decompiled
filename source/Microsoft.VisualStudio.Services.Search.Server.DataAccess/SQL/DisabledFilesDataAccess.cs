// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql.DisabledFilesDataAccess
// Assembly: Microsoft.VisualStudio.Services.Search.Server.DataAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3B684226-797D-4C9F-9AC1-E10D39E316D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.DataAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql
{
  internal class DisabledFilesDataAccess : SqlAzureDataAccess, IDisabledFilesDataAccess
  {
    public DisabledFilesDataAccess()
    {
    }

    internal DisabledFilesDataAccess(ITableAccessPlatform tableAccessPlatform)
      : base(tableAccessPlatform)
    {
    }

    public void AddDisabledFile(IVssRequestContext requestContext, DisabledFile disabledFile)
    {
      this.ValidateNotNull<DisabledFile>(nameof (disabledFile), disabledFile);
      using (ITable<DisabledFile> table = this.m_tableAccessPlatform.GetTable<DisabledFile>(requestContext))
        this.InvokeTableOperation<DisabledFile>((Func<DisabledFile>) (() => table.Insert(disabledFile)));
    }

    public List<DisabledFile> GetDisabledFiles(
      IVssRequestContext requestContext,
      Guid collectionId,
      Guid repositoryId,
      int topCount)
    {
      this.ValidateNotEmptyGuid(nameof (collectionId), collectionId);
      this.ValidateNotEmptyGuid(nameof (repositoryId), repositoryId);
      TableEntityFilterList disabledFileFilters = new TableEntityFilterList();
      disabledFileFilters.Add(new TableEntityFilter("CollectionId", "eq", collectionId.ToString()));
      disabledFileFilters.Add(new TableEntityFilter("RepositoryId", "eq", repositoryId.ToString()));
      List<DisabledFile> disabledFileList = new List<DisabledFile>();
      using (ITable<DisabledFile> table = this.m_tableAccessPlatform.GetTable<DisabledFile>(requestContext))
        return this.InvokeTableOperation<List<DisabledFile>>((Func<List<DisabledFile>>) (() => table.RetriveTableEntityList(topCount, disabledFileFilters)));
    }

    public void UpdateDisabledFile(IVssRequestContext requestContext, DisabledFile disabledFile)
    {
      if (disabledFile == null)
        throw new DataAccessException(DataAccessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentNullException("Null Input File"));
      using (ITable<DisabledFile> table = this.m_tableAccessPlatform.GetTable<DisabledFile>(requestContext))
      {
        if (!(table is DisabledFilesTableV2))
          return;
        this.InvokeTableOperation<DisabledFile>((Func<DisabledFile>) (() => table.Update(disabledFile)));
      }
    }
  }
}
