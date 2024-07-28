// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsMetadataComponent6
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.OData;
using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class AnalyticsMetadataComponent6 : AnalyticsMetadataComponent5
  {
    public override ModelCreationProcessFields GetModelCreationProcessFields(
      Guid? processId,
      bool includeDeleted = false)
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_GetProcessFields");
      this.BindNullableGuid("@processId", processId);
      this.BindBoolean("@includeDeleted", includeDeleted);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ProcessFieldInfo>((ObjectBinder<ProcessFieldInfo>) new AnalyticsMetadataComponent.ProcessFieldInfoColumns());
        return new ModelCreationProcessFields(resultCollection.GetCurrent<ProcessFieldInfo>().Items);
      }
    }

    internal class FieldIsDeletedInModelRowBinder : ObjectBinder<bool>
    {
      private SqlColumnBinder m_isDeleted = new SqlColumnBinder("IsDeleted");

      protected override bool Bind() => this.m_isDeleted.GetBoolean((IDataReader) this.Reader);
    }
  }
}
