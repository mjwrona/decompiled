// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.GatedCheckInTicketService
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal class GatedCheckInTicketService : IVssFrameworkService
  {
    private GatedCheckInKey m_key;

    public bool HasKey => this.m_key != null;

    public string CreateCheckInTicket(
      IVssRequestContext requestContext,
      string shelvesetSpec,
      IEnumerable<string> affectedDefinitions)
    {
      ArgumentUtility.CheckForNull<GatedCheckInKey>(this.m_key, "Key");
      return this.m_key.CreateCheckInTicket(requestContext, shelvesetSpec, affectedDefinitions);
    }

    public void ValidateCheckInTicket(
      IVssRequestContext requestContext,
      string checkInTicket,
      string definitionUri,
      string shelvesetSpec)
    {
      ArgumentUtility.CheckForNull<GatedCheckInKey>(this.m_key, "Key");
      this.m_key.ValidateCheckInTicket(requestContext, checkInTicket, definitionUri, shelvesetSpec);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      using (BuildComponent component = systemRequestContext.CreateComponent<BuildComponent>("Build"))
      {
        using (ResultCollection resultCollection = component.QueryBuildServerProperties())
          this.m_key = resultCollection.GetCurrent<GatedCheckInKey>().Items.FirstOrDefault<GatedCheckInKey>();
      }
      if (this.m_key != null)
        return;
      systemRequestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Generating new gated checkin key");
      byte[] key;
      byte[] iv;
      int blockSize;
      GatedCheckInKey.GenerateNewKey(out key, out iv, out blockSize);
      using (BuildComponent component = systemRequestContext.CreateComponent<BuildComponent>("Build"))
      {
        using (ResultCollection resultCollection = component.UpdateBuildServerProperties(key, iv, blockSize, false))
          this.m_key = resultCollection.GetCurrent<GatedCheckInKey>().Items.FirstOrDefault<GatedCheckInKey>();
      }
      if (this.m_key != null)
        return;
      string message = ResourceStrings.ApplicationStartKeyError((object) string.Join(", ", new string[2]
      {
        BuildTypeResource.QueueBuildsPermission(),
        BuildTypeResource.ViewBuildDefinitionPermission()
      }));
      systemRequestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Failed to update gated checkin key. Error: {0}", (object) message);
      TeamFoundationEventLog.Default.Log(message, TeamFoundationEventId.ApplicationInitialization, EventLogEntryType.Error);
    }
  }
}
