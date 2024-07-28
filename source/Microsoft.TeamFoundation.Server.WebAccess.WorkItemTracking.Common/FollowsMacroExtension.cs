// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FollowsMacroExtension
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Notifications.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class FollowsMacroExtension : QueryMacroExtension
  {
    private const string c_area = "FollowsMacroExtension";
    private const string c_layer = "WorkItemTracking.Common.Plugins";
    public const string FollowsMacro = "Follows";
    private const string EmptyList = "0";

    public override string Name => "Follows";

    public override object DefaultValue
    {
      get
      {
        NodeValueList defaultValue = new NodeValueList();
        defaultValue.Add((Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeNumber("0"));
        return (object) defaultValue;
      }
    }

    public override DataType DataType => DataType.Numeric;

    public override object GetValue(
      IVssRequestContext requestContext,
      string macro,
      ProjectInfo project,
      WebApiTeam team = null,
      NodeParameters parameters = null,
      TimeZone timeZone = null,
      CultureInfo cultureInfo = null)
    {
      List<Microsoft.VisualStudio.Services.Notifications.Server.Subscription> source;
      try
      {
        SubscriptionLookup typesForUserLookup = SubscriptionLookup.CreateFollowArtifactTypesForUserLookup("WorkItem", requestContext.GetUserId());
        source = requestContext.GetService<INotificationSubscriptionService>().QuerySubscriptions(requestContext, typesForUserLookup);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(516591, nameof (FollowsMacroExtension), "WorkItemTracking.Common.Plugins", ex);
        throw;
      }
      if (!source.Any<Microsoft.VisualStudio.Services.Notifications.Server.Subscription>())
        return this.DefaultValue;
      try
      {
        NodeValueList nodeValueList = new NodeValueList();
        foreach (string uri in source.Select<Microsoft.VisualStudio.Services.Notifications.Server.Subscription, string>((Func<Microsoft.VisualStudio.Services.Notifications.Server.Subscription, string>) (n => n.IndexedExpression)))
          nodeValueList.Add((Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeNumber(LinkingUtilities.DecodeUri(uri).ToolSpecificId));
        return (object) nodeValueList;
      }
      catch (ArgumentException ex)
      {
        requestContext.TraceException(516592, nameof (FollowsMacroExtension), "WorkItemTracking.Common.Plugins", (Exception) ex);
        throw;
      }
    }
  }
}
