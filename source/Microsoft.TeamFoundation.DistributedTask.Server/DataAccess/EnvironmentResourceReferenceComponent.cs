// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.EnvironmentResourceReferenceComponent
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class EnvironmentResourceReferenceComponent : TaskSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<EnvironmentResourceReferenceComponent>(1)
    }, "DistributedTaskEnvironmentResourceReference", "DistributedTask");

    public EnvironmentResourceReferenceComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public EnvironmentResourceReference GetEnvironmentResourceReferenceByIdOrName(
      Guid projectId,
      int environmentId,
      int? resourceId = null,
      string resourceName = "")
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetEnvironmentResourceReferenceByIdOrName)))
      {
        this.PrepareStoredProcedure("Task.prc_GetEnvironmentResourceReferenceByIdOrName");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@environmentId", environmentId);
        this.BindNullableInt("@resourceId", resourceId);
        this.BindString("@resourceName", resourceName, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        EnvironmentResourceReference referenceByIdOrName;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<EnvironmentResourceReference>((ObjectBinder<EnvironmentResourceReference>) new EnvironmentResourceBinder());
          resultCollection.AddBinder<EnvironmentResourceTag>((ObjectBinder<EnvironmentResourceTag>) new EnvironmentResourceTagBinder());
          referenceByIdOrName = resultCollection.GetCurrent<EnvironmentResourceReference>().Items.FirstOrDefault<EnvironmentResourceReference>();
          if (referenceByIdOrName != null)
          {
            resultCollection.NextResult();
            List<string> list = resultCollection.GetCurrent<EnvironmentResourceTag>().Items.Select<EnvironmentResourceTag, string>((System.Func<EnvironmentResourceTag, string>) (x => x.Tag)).Distinct<string>().Where<string>((System.Func<string, bool>) (t => !string.IsNullOrWhiteSpace(t))).ToList<string>();
            referenceByIdOrName.Tags = (IList<string>) list;
          }
        }
        return referenceByIdOrName;
      }
    }

    public IList<EnvironmentResourceReference> GetEnvironmentResourceReferencesByTypeAndTags(
      Guid projectId,
      int environmentId,
      EnvironmentResourceType? resourceType,
      IList<string> tagFilters,
      int top = 50,
      string continuationToken = "")
    {
      top = top > 1000 ? 1000 : top;
      this.PrepareStoredProcedure("Task.prc_GetEnvironmentResourceReferencesByTypeAndTags");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
      this.BindInt("@environmentId", environmentId);
      this.BindNullableByte("@resourceType", new byte?((byte) resourceType.Value));
      this.BindStringTable("@tags", (IEnumerable<string>) tagFilters);
      this.BindInt("@top", top);
      this.BindString("@continuationToken", continuationToken, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<EnvironmentResourceReference>((ObjectBinder<EnvironmentResourceReference>) new EnvironmentResourceBinder());
        resultCollection.AddBinder<EnvironmentResourceTag>((ObjectBinder<EnvironmentResourceTag>) new EnvironmentResourceTagBinder());
        IList<EnvironmentResourceReference> items = (IList<EnvironmentResourceReference>) resultCollection.GetCurrent<EnvironmentResourceReference>().Items;
        if (items.Any<EnvironmentResourceReference>())
        {
          resultCollection.NextResult();
          EnvironmentResourceTag[] array = resultCollection.GetCurrent<EnvironmentResourceTag>().Items.ToArray();
          foreach (EnvironmentResourceReference resourceReference in (IEnumerable<EnvironmentResourceReference>) items)
          {
            EnvironmentResourceReference resource = resourceReference;
            List<string> stringList = new List<string>();
            stringList.AddRange(((IEnumerable<EnvironmentResourceTag>) array).Where<EnvironmentResourceTag>((System.Func<EnvironmentResourceTag, bool>) (x => x.ResourceId == resource.Id)).Select<EnvironmentResourceTag, string>((System.Func<EnvironmentResourceTag, string>) (x => x.Tag)));
            resource.Tags = (IList<string>) stringList;
          }
        }
        return items;
      }
    }
  }
}
