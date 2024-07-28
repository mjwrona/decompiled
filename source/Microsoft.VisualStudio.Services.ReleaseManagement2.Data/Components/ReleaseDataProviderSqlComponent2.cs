// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseDataProviderSqlComponent2
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class ReleaseDataProviderSqlComponent2 : ReleaseDataProviderSqlComponent
  {
    public override AllPipelinesViewData GetAllPipelinesViewData(
      Guid projectId,
      string queriedFolderPath)
    {
      this.PrepareStoredProcedure("Release.prc_GetFoldersAndDefinitions", projectId);
      this.BindString("folderPath", PathHelper.UserToDBPath(queriedFolderPath), 400, false, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseDefinitionShallowReference>((ObjectBinder<ReleaseDefinitionShallowReference>) this.GetReleaseDefinitionShallowReferenceBinder());
        SqlColumnBinder folderPath = new SqlColumnBinder("Path");
        resultCollection.AddBinder<string>((ObjectBinder<string>) new SimpleObjectBinder<string>((System.Func<IDataReader, string>) (reader => PathHelper.DBPathToServerPath(folderPath.GetString(reader, false)))));
        List<ReleaseDefinitionShallowReference> items1 = resultCollection.GetCurrent<ReleaseDefinitionShallowReference>().Items;
        resultCollection.NextResult();
        List<string> items2 = resultCollection.GetCurrent<string>().Items;
        return new AllPipelinesViewData()
        {
          FolderPaths = (IList<string>) items2,
          ReleaseDefinitions = (IList<ReleaseDefinitionShallowReference>) items1
        };
      }
    }
  }
}
