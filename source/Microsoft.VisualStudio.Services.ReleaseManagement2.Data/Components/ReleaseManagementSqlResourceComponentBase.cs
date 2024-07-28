// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseManagementSqlResourceComponentBase
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class ReleaseManagementSqlResourceComponentBase : TeamFoundationSqlResourceComponent
  {
    private readonly IDictionary<int, SqlExceptionFactory> sqlExceptionFactory = ReleaseManagementExceptionTranslator.GetTranslatedExceptions();

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => this.sqlExceptionFactory;

    public ReleaseManagementSqlResourceComponentBase() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    protected void PrepareStoredProcedure(string storedProcedure, Guid dataspaceIdentifier)
    {
      this.PrepareStoredProcedure(storedProcedure);
      this.BindInt("dataspaceId", this.GetOrCreateDataspaceId(dataspaceIdentifier));
    }

    protected override SqlParameter BindNullableDateTime(
      string parameterName,
      DateTime? parameterValue)
    {
      parameterValue = parameterValue.ToUtcDateTime();
      return base.BindNullableDateTime(parameterName, parameterValue);
    }

    protected override SqlParameter BindNullableDateTime2(
      string parameterName,
      DateTime? parameterValue)
    {
      parameterValue = parameterValue.ToUtcDateTime();
      return base.BindNullableDateTime2(parameterName, parameterValue);
    }

    protected override SqlParameter BindNullableDate(string parameterName, DateTime? parameterValue)
    {
      parameterValue = parameterValue.ToUtcDateTime();
      return base.BindNullableDate(parameterName, parameterValue);
    }

    protected override SqlParameter BindDateTime2(string parameterName, DateTime parameterValue)
    {
      parameterValue = parameterValue.ToUtcDateTime();
      return base.BindDateTime2(parameterName, parameterValue);
    }

    protected override SqlParameter BindDateTime(string parameterName, DateTime parameterValue)
    {
      parameterValue = parameterValue.ToUtcDateTime();
      return base.BindDateTime(parameterName, parameterValue);
    }

    protected override SqlParameter BindDate(string parameterName, DateTime parameterValue)
    {
      parameterValue = parameterValue.ToUtcDateTime();
      return base.BindDate(parameterName, parameterValue);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "dataspace", Justification = "dataspace is a valid term here.")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "dataspace", Justification = "dataspace is a valid term here.")]
    private int GetOrCreateDataspaceId(Guid dataspaceIdentifier) => this.GetDataspaceIdOfReleaseManagementDataspaceCategory(dataspaceIdentifier);

    private int GetDataspaceIdOfReleaseManagementDataspaceCategory(Guid dataspaceIdentifier)
    {
      try
      {
        return this.GetDataspaceId(dataspaceIdentifier);
      }
      catch (DataspaceNotFoundException ex)
      {
        this.TraceInfoMessage(1961035, "Cannot find dataspaceId for the project {0}, project level provisioning did not happen for this project. Returning from here. CallStack {1}", (object) dataspaceIdentifier, (object) new StackTrace());
        throw;
      }
    }

    private void TraceInfoMessage(int tracePoint, string format, params object[] args)
    {
      if (this.RequestContext == null)
        return;
      VssRequestContextExtensions.Trace(this.RequestContext, tracePoint, TraceLevel.Info, "ReleaseManagementService", "Service", format, args);
    }
  }
}
