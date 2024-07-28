// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServiceBusPropertyHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal static class ServiceBusPropertyHelper
  {
    internal const string NamespaceName = "NamespaceName";
    internal const string SourceHostId = "SourceHostId";
    internal const string ParentSourceHostId = "ParentSourceHostId";
    internal const string SourceHostType = "SourceHostType";
    internal const string SourceServiceInstanceId = "SourceServiceInstanceId";
    internal const string SourceServiceInstanceType = "SourceServiceInstanceType";
    internal const string TopicName = "TopicName";

    internal static string GetServiceInstanceProperty(Guid serviceOwner) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "VSService_{0}", (object) serviceOwner.ToString("N"));

    internal static string GetServiceInstanceValue(Guid instanceId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "|{0}|", (object) instanceId.ToString("D"));

    internal static string GetServiceInstancesValue(List<Guid> instanceIds) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "|{0}|", (object) string.Join("|", instanceIds.Select<Guid, string>((Func<Guid, string>) (id => id.ToString("D")))));

    internal static string GetServiceInstanceProperty(IVssRequestContext requestContext) => ServiceBusPropertyHelper.GetServiceInstanceProperty(requestContext.ServiceInstanceType());

    internal static TeamFoundationHostType GetNormalizedHostType(TeamFoundationHostType hostType) => hostType.HasFlag((Enum) TeamFoundationHostType.Deployment) ? hostType & ~TeamFoundationHostType.Application : hostType;
  }
}
