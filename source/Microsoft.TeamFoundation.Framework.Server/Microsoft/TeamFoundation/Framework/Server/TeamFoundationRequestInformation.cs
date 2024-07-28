// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationRequestInformation
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [CallOnDeserialization("AfterDeserialize")]
  public class TeamFoundationRequestInformation
  {
    private List<KeyValue<string, string>> m_parameters = new List<KeyValue<string, string>>();

    public TeamFoundationRequestInformation()
    {
    }

    internal TeamFoundationRequestInformation(
      IVssRequestContext requestContext,
      IVssRequestContext activeRequest,
      bool includeDetails)
    {
      ITimeRequest requestTimer = activeRequest.RequestTimer;
      this.RequestId = activeRequest.ContextId;
      this.StartTime = requestTimer.StartTime;
      this.RemoteComputer = activeRequest.RemoteIPAddress();
      this.RemotePort = activeRequest.RemotePort();
      this.Queued = false;
      this.QueuedTime = 0L;
      this.UserAgent = activeRequest.UserAgent;
      this.DelayTime = (long) requestTimer.DelaySpan.TotalMilliseconds;
      this.ExecutionTime = (long) requestTimer.ExecutionSpan.TotalMilliseconds;
      this.UserDescriptor = activeRequest.UserContext;
      this.MethodName = string.IsNullOrEmpty(activeRequest.RawUrl()) ? FrameworkResources.UnknownRequestMethod() : activeRequest.RawUrl();
      try
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          activeRequest.UserContext
        }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (identity != null)
          this.UserName = identity.DisplayName;
        else if (activeRequest.UserContext != (IdentityDescriptor) null)
          this.UserName = activeRequest.UserContext.Identifier;
      }
      catch (Exception ex)
      {
      }
      if (activeRequest.Method == null)
        return;
      if (!string.IsNullOrEmpty(activeRequest.Method.Name))
        this.MethodName = activeRequest.Method.Name;
      if (!includeDetails)
        return;
      foreach (string allKey in activeRequest.Method.Parameters.AllKeys)
        this.m_parameters.Add(new KeyValue<string, string>()
        {
          Key = allKey,
          Value = activeRequest.Method.Parameters[allKey]
        });
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public long RequestId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public DateTime StartTime { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public long DelayTime { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public long ExecutionTime { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string UserName { get; set; }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public IdentityDescriptor UserDescriptor { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string MethodName { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string RemoteComputer { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string RemotePort { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string UserAgent { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public bool Queued { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public long QueuedTime { get; set; }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public List<KeyValue<string, string>> Parameters => this.m_parameters;
  }
}
