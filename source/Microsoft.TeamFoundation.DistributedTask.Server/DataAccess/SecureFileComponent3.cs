// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.SecureFileComponent3
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class SecureFileComponent3 : SecureFileComponent2
  {
    public override async Task<List<SecureFile>> GetSecureFilesAsync(
      Guid projectId,
      IEnumerable<string> secureFileNames)
    {
      SecureFileComponent3 component1 = this;
      List<SecureFile> items;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component1, nameof (GetSecureFilesAsync)))
      {
        component1.PrepareStoredProcedure("Task.prc_GetSecureFilesByNames");
        component1.BindDataspaceId(projectId);
        SecureFileComponent3 component2 = component1;
        IEnumerable<string> source = secureFileNames;
        IEnumerable<string> rows = source != null ? source.Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : (IEnumerable<string>) null;
        component2.BindStringTable("@secureFileNames", rows);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component1.ExecuteReaderAsync(), component1.ProcedureName, component1.RequestContext))
        {
          resultCollection.AddBinder<SecureFile>((ObjectBinder<SecureFile>) new SecureFileBinder());
          items = resultCollection.GetCurrent<SecureFile>().Items;
        }
      }
      return items;
    }
  }
}
