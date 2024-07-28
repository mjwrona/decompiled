// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.UserReportedConcernComponent
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class UserReportedConcernComponent : TeamFoundationSqlResourceComponent
  {
    private const string s_area = "UserReportedConcernComponent";
    [StaticSafe]
    private static readonly Dictionary<int, SqlExceptionFactory> SqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<UserReportedConcernComponent>(1)
    }, "UserReportedConcern");

    protected override string TraceArea => nameof (UserReportedConcernComponent);

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) UserReportedConcernComponent.SqlExceptionFactories;

    public virtual IEnumerable<UserReportedConcern> GetUserReportedConcernByUserId(Guid userId)
    {
      this.PrepareStoredProcedure("Gallery.prc_GetUserReportedConcernByUserId");
      this.BindGuid(nameof (userId), userId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_GetUserReportedConcernByUserId", this.RequestContext))
      {
        resultCollection.AddBinder<UserReportedConcern>((ObjectBinder<UserReportedConcern>) new UserReportedConcernBinder());
        return (IEnumerable<UserReportedConcern>) resultCollection.GetCurrent<UserReportedConcern>().Items;
      }
    }

    public virtual int AnonymizeUserReportedConcern(Guid userId)
    {
      this.PrepareStoredProcedure("Gallery.prc_AnonymizeUserReportedConcern");
      this.BindGuid(nameof (userId), userId);
      SqlParameter sqlParameter = this.BindInt("@rowCount", 0);
      sqlParameter.Direction = ParameterDirection.Output;
      this.ExecuteNonQuery();
      return (int) sqlParameter.Value;
    }
  }
}
