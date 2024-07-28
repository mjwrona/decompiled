// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.SecureFileComponent
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class SecureFileComponent : TaskSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<SecureFileComponent>(1),
      (IComponentCreator) new ComponentCreator<SecureFileComponent2>(2),
      (IComponentCreator) new ComponentCreator<SecureFileComponent3>(3)
    }, "DistributedTaskSecureFile", "DistributedTask");

    public SecureFileComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual SecureFile AddSecureFile(Guid projectId, SecureFile secureFile)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AddSecureFile)))
      {
        this.PrepareStoredProcedure("Task.prc_AddSecureFile");
        this.BindDataspaceId(projectId);
        this.BindGuid("@secureFileId", secureFile.Id);
        this.BindString("@secureFileName", secureFile.Name, 1024, false, SqlDbType.NVarChar);
        string parameterValue = (string) null;
        if (secureFile.Properties != null)
        {
          if (secureFile.Properties.Any<KeyValuePair<string, string>>())
          {
            try
            {
              parameterValue = JsonUtility.ToString((object) secureFile.Properties);
            }
            catch (JsonSerializationException ex)
            {
              parameterValue = (string) null;
            }
          }
        }
        this.BindString("@secureFileProperties", parameterValue, int.MaxValue, true, SqlDbType.NVarChar);
        this.BindGuid("@createdBy", new Guid(secureFile.CreatedBy.Id));
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<SecureFile>((ObjectBinder<SecureFile>) new SecureFileBinder());
          return resultCollection.GetCurrent<SecureFile>().FirstOrDefault<SecureFile>();
        }
      }
    }

    public virtual SecureFile DeleteSecureFile(Guid projectId, Guid secureFileId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteSecureFile)))
      {
        this.PrepareStoredProcedure("Task.prc_DeleteSecureFile");
        this.BindDataspaceId(projectId);
        this.BindGuid("@secureFileId", secureFileId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<SecureFile>((ObjectBinder<SecureFile>) new SecureFileBinder());
          return resultCollection.GetCurrent<SecureFile>().FirstOrDefault<SecureFile>();
        }
      }
    }

    public virtual async Task<List<SecureFile>> GetSecureFilesAsync(
      Guid projectId,
      IEnumerable<Guid> secureFileIds)
    {
      SecureFileComponent component1 = this;
      List<SecureFile> items;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component1, nameof (GetSecureFilesAsync)))
      {
        component1.PrepareStoredProcedure("Task.prc_GetSecureFilesByIds");
        component1.BindDataspaceId(projectId);
        SecureFileComponent component2 = component1;
        IEnumerable<Guid> source = secureFileIds;
        IEnumerable<Guid> rows = source != null ? source.Distinct<Guid>() : (IEnumerable<Guid>) null;
        component2.BindGuidTable("@secureFileIds", rows);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component1.ExecuteReaderAsync(), component1.ProcedureName, component1.RequestContext))
        {
          resultCollection.AddBinder<SecureFile>((ObjectBinder<SecureFile>) new SecureFileBinder());
          items = resultCollection.GetCurrent<SecureFile>().Items;
        }
      }
      return items;
    }

    public virtual Task<List<SecureFile>> GetSecureFilesAsync(
      Guid projectId,
      IEnumerable<string> secureFileNames)
    {
      List<SecureFile> secureFiles = this.GetSecureFiles(projectId, (string) null);
      HashSet<string> matchSet = new HashSet<string>(secureFileNames, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      System.Func<SecureFile, bool> predicate = (System.Func<SecureFile, bool>) (x => matchSet.Contains(x.Name));
      return Task.FromResult<List<SecureFile>>(secureFiles.Where<SecureFile>(predicate).ToList<SecureFile>());
    }

    public virtual List<SecureFile> GetSecureFiles(Guid projectId, string secureFileName)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetSecureFiles)))
      {
        this.PrepareStoredProcedure("Task.prc_GetSecureFilesByName");
        this.BindDataspaceId(projectId);
        this.BindString("@secureFileNamePattern", secureFileName, 1024, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<SecureFile>((ObjectBinder<SecureFile>) new SecureFileBinder());
          return resultCollection.GetCurrent<SecureFile>().Items;
        }
      }
    }

    public virtual SecureFile UpdateSecureFile(
      Guid projectId,
      Guid secureFileId,
      SecureFile secureFile)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateSecureFile)))
      {
        this.PrepareStoredProcedure("Task.prc_UpdateSecureFile");
        this.BindDataspaceId(projectId);
        this.BindGuid("@secureFileId", secureFileId);
        this.BindString("@secureFileName", secureFile.Name, 1024, false, SqlDbType.NVarChar);
        string parameterValue = (string) null;
        if (secureFile.Properties != null)
        {
          if (secureFile.Properties.Any<KeyValuePair<string, string>>())
          {
            try
            {
              parameterValue = JsonUtility.ToString((object) secureFile.Properties);
            }
            catch (JsonSerializationException ex)
            {
              parameterValue = (string) null;
            }
          }
        }
        this.BindString("@secureFileProperties", parameterValue, int.MaxValue, true, SqlDbType.NVarChar);
        this.BindGuid("@modifiedBy", new Guid(secureFile.ModifiedBy.Id));
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<SecureFile>((ObjectBinder<SecureFile>) new SecureFileBinder());
          return resultCollection.GetCurrent<SecureFile>().FirstOrDefault<SecureFile>();
        }
      }
    }

    public virtual void DeleteTeamProject(Guid projectId)
    {
    }
  }
}
