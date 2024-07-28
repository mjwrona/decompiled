// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ThrottlingNotificationEmailHistoryComponent
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class ThrottlingNotificationEmailHistoryComponent : TeamFoundationSqlResourceComponent
  {
    internal static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<ThrottlingNotificationEmailHistoryComponent>(1)
    }, "ThrottlingNotification");

    public ThrottlingNotificationEmailHistoryComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    internal List<UserEmailStatus> CheckAndUpdateRecentEmailHistory(
      List<UserIdAndNamespacePair> userIdAndNamespacePairs,
      TimeSpan defaultCooldown)
    {
      this.PrepareStoredProcedure("prc_CheckAndUpdateThrottlingNotificationEmailHistory");
      this.BindGuidTinyIntTable("@userIdsAndNamespaces", userIdAndNamespacePairs.Select<UserIdAndNamespacePair, Tuple<Guid, byte>>((System.Func<UserIdAndNamespacePair, Tuple<Guid, byte>>) (x => Tuple.Create<Guid, byte>(x.UserId, (byte) x.ThrottleType))));
      this.BindInt("@defaultCooldown", (int) defaultCooldown.TotalSeconds);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<UserEmailStatus>((ObjectBinder<UserEmailStatus>) new ThrottlingNotificationEmailHistoryComponent.UserEmailStatusBinder());
      return resultCollection.GetCurrent<UserEmailStatus>().Items;
    }

    internal int CleanOldEmailHistory(TimeSpan purgeThreshold)
    {
      this.PrepareStoredProcedure("prc_CleanThrottlingNotificationEmailHistory");
      this.BindInt("@secondsToKeep", (int) purgeThreshold.TotalSeconds);
      return (int) this.ExecuteScalar();
    }

    private class UserEmailStatusBinder : ObjectBinder<UserEmailStatus>
    {
      private SqlColumnBinder UserIdColumn = new SqlColumnBinder("UserId");
      private SqlColumnBinder NamespaceColumn = new SqlColumnBinder("Namespace");
      private SqlColumnBinder LastEmailTimeColumn = new SqlColumnBinder("TimeOfLastSend");
      private SqlColumnBinder ShouldEmailColumn = new SqlColumnBinder("ShouldEmail");

      protected override UserEmailStatus Bind() => new UserEmailStatus()
      {
        UserAndNamespace = new UserIdAndNamespacePair(this.UserIdColumn.GetGuid((IDataReader) this.Reader), (ResourceState) this.NamespaceColumn.GetByte((IDataReader) this.Reader, (byte) 0)),
        LastEmailTime = this.LastEmailTimeColumn.GetDateTime((IDataReader) this.Reader),
        ShouldEmail = this.ShouldEmailColumn.GetBoolean((IDataReader) this.Reader)
      };
    }
  }
}
