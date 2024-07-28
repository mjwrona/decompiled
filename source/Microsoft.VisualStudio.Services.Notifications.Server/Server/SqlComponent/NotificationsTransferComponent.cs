// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.SqlComponent.NotificationsTransferComponent
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Notifications.Server.SqlComponent
{
  public class NotificationsTransferComponent : FilterTransferComponentBase
  {
    public const string EventTypesFilterName = "Notifications.EventTypes";
    public const string ArtifactFilterExpression = "Notifications.ArtifactFilterExpressions";
    private HashSet<string> m_eventTypes;
    private HashSet<string> m_artifactFilterExpressions;

    public override string TableName => "tbl_EventSubscription";

    public override void SetFilters(ICopyFilterProvider provider)
    {
      ArgumentUtility.CheckForNull<ICopyFilterProvider>(provider, nameof (provider));
      this.m_eventTypes = provider.GetFilter<string>("Notifications.EventTypes");
      this.m_artifactFilterExpressions = provider.GetFilter<string>("Notifications.ArtifactFilterExpressions");
      if (this.HasDynamicFilters())
        return;
      this.Logger.Info("Filter Tokens not found, no filtering of Notification EventTypes will take place.");
    }

    public override bool HasDynamicFilters() => this.m_eventTypes.Any<string>() || this.m_artifactFilterExpressions.Any<string>();

    public override string ReadStatementBase(PartitionTransferContext transferContext)
    {
      if (!this.HasDynamicFilters())
        return base.ReadStatementBase(transferContext);
      List<string> values = new List<string>();
      if (this.m_eventTypes.Any<string>())
        values.Add("( " + this.SchemaName + "." + this.TableName + ".EventTypeName IN ( " + this.m_eventTypes.ToQuotedStringList<string>() + " ) )");
      foreach (string filterExpression in this.m_artifactFilterExpressions)
        values.Add("( " + this.SchemaName + "." + this.TableName + ".IndexedExpression LIKE '%" + filterExpression + "%' )");
      return "\r\n            FROM    " + this.SchemaName + "." + this.TableName + "\r\n            WHERE   " + this.SchemaName + "." + this.TableName + ".PartitionId = @partitionId\r\n                    AND ( " + string.Join(" OR ", (IEnumerable<string>) values) + " )";
    }
  }
}
