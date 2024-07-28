// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility.WorkItemUpdateSerializer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility
{
  public sealed class WorkItemUpdateSerializer
  {
    public readonly int ClientVersion;
    private readonly bool isBulk;

    public WorkItemUpdateSerializer(int clientVersion, bool isBulk = false)
    {
      this.ClientVersion = clientVersion;
      this.isBulk = isBulk;
    }

    public WorkItemUpdateDeserializeResult Deserialize(
      IVssRequestContext requestContext,
      XElement packageElement)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<XElement>(packageElement, nameof (packageElement));
      return requestContext.TraceBlock<WorkItemUpdateDeserializeResult>(905061, 905064, "WebServices", "ClientService", "Compat.Deserialize", (Func<WorkItemUpdateDeserializeResult>) (() =>
      {
        try
        {
          requestContext.Trace(905063, TraceLevel.Info, "WebServices", "ClientService", "ClientVersion: {0}. Entering Deserialize method for NewAPI update path", (object) this.ClientVersion);
          return new WorkItemUpdateDeserializer(this.ClientVersion).Deserialize(requestContext, packageElement);
        }
        catch (Exception ex)
        {
          requestContext.Trace(905062, TraceLevel.Error, "WebServices", "ClientService", "ClientVersion: {0}. An error occurred during deserialization in compat layer: {1}", (object) this.ClientVersion, (object) ex);
          throw;
        }
      }));
    }

    public XElement Serialize(
      IVssRequestContext requestContext,
      WorkItemUpdateDeserializeResult deserializedPackage,
      IEnumerable<WorkItemUpdateResult> results)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WorkItemUpdateDeserializeResult>(deserializedPackage, nameof (deserializedPackage));
      ArgumentUtility.CheckForNull<IEnumerable<WorkItemUpdateResult>>(results, nameof (results));
      return requestContext.TraceBlock<XElement>(905065, 905068, "WebServices", "ClientService", "Compat.Serialize", (Func<XElement>) (() =>
      {
        try
        {
          requestContext.Trace(905067, TraceLevel.Info, "WebServices", "ClientService", "ClientVersion: {0}. Entering Serialize method for NewAPI update path", (object) this.ClientVersion);
          return new WorkItemUpdateResultSerializer(this.ClientVersion, this.isBulk).Serialize(requestContext, deserializedPackage, results);
        }
        catch (Exception ex)
        {
          requestContext.Trace(905066, TraceLevel.Error, "WebServices", "ClientService", "ClientVersion: {0}. An error occurred during serialization in compat layer: {1}", (object) this.ClientVersion, (object) ex);
          throw;
        }
      }));
    }
  }
}
