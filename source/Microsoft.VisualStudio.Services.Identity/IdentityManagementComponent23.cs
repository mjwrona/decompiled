// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityManagementComponent23
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityManagementComponent23 : IdentityManagementComponent22
  {
    public override IList<Guid> GetIdentityIdsByTypeIdAndPartialSid(
      byte typeId,
      string partialSid,
      bool treatSidAsPrefix = true)
    {
      this.TraceEnter(47011190, nameof (GetIdentityIdsByTypeIdAndPartialSid));
      List<Guid> typeIdAndPartialSid = new List<Guid>();
      try
      {
        if (string.IsNullOrWhiteSpace(partialSid))
          throw new ArgumentNullException("Partial SID cannot be null");
        this.PrepareStoredProcedure("prc_GetIdentityIdsByTypeIdAndPartialSid");
        this.BindByte("@typeId", typeId);
        this.BindString("@partialSidPattern", treatSidAsPrefix ? partialSid + (object) '%' : partialSid, 256, false, SqlDbType.VarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new IdentityManagementComponent23.IdentityIdColumn());
          foreach (Guid guid in resultCollection.GetCurrent<Guid>())
          {
            if (guid != Guid.Empty)
              typeIdAndPartialSid.Add(guid);
          }
        }
      }
      finally
      {
        this.TraceLeave(47011199, nameof (GetIdentityIdsByTypeIdAndPartialSid));
      }
      return (IList<Guid>) typeIdAndPartialSid;
    }

    public override IdentityDescriptor GetIdentityDescriptorById(Guid id)
    {
      this.TraceEnter(47011193, nameof (GetIdentityDescriptorById));
      try
      {
        this.PrepareStoredProcedure("prc_GetIdentityDescriptorById");
        this.BindGuid("@id", id);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<IdentityDescriptor>((ObjectBinder<IdentityDescriptor>) new IdentityManagementComponent23.IdentityDescriptorColumns());
          return resultCollection.GetCurrent<IdentityDescriptor>().Items.SingleOrDefault<IdentityDescriptor>();
        }
      }
      finally
      {
        this.TraceLeave(47011194, nameof (GetIdentityDescriptorById));
      }
    }

    protected class IdentityIdColumn : ObjectBinder<Guid>
    {
      private SqlColumnBinder idColumn = new SqlColumnBinder("Id");

      protected override Guid Bind() => this.Bind(this.Reader);

      internal virtual Guid Bind(SqlDataReader reader) => this.idColumn.GetGuid((IDataReader) reader, true);
    }

    protected class IdentityDescriptorColumns : ObjectBinder<IdentityDescriptor>
    {
      private SqlColumnBinder Sid = new SqlColumnBinder(nameof (Sid));
      private SqlColumnBinder TypeId = new SqlColumnBinder(nameof (TypeId));

      protected override IdentityDescriptor Bind()
      {
        string identifier = this.Sid.GetString((IDataReader) this.Reader, false);
        byte typeId = this.TypeId.GetByte((IDataReader) this.Reader);
        return new IdentityDescriptor(IdentityTypeMapper.Instance.GetTypeNameFromId(typeId), identifier);
      }
    }
  }
}
