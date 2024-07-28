// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DataTierInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DebuggerDisplay("DataSource: {DataSource}, DatabaseCount: {DatabaseCount}, State: {State}")]
  public sealed class DataTierInfo
  {
    public DataTierInfo(
      ISqlConnectionInfo connectionInfo,
      DataTierState state,
      string tags,
      Guid signingKeyId)
    {
      ArgumentUtility.CheckForNull<ISqlConnectionInfo>(connectionInfo, nameof (connectionInfo));
      this.ConnectionInfo = connectionInfo;
      this.DataSource = new SqlConnectionStringBuilder(connectionInfo.ConnectionString).DataSource;
      this.State = state;
      this.DatabaseCount = -1;
      this.Tags = tags;
      this.SigningKeyId = signingKeyId;
    }

    public string GetDataSource(DataSourceOptions options) => TeamFoundationDataTierService.ManipulateDataSource(this.DataSource, options);

    public ISqlConnectionInfo ConnectionInfo { get; private set; }

    public string DataSource { get; private set; }

    public int DatabaseCount { get; internal set; }

    public DataTierState State { get; private set; }

    public string Tags { get; private set; }

    public Guid SigningKeyId { get; private set; }
  }
}
