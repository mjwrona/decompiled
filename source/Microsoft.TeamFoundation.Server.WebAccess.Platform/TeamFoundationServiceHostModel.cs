// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TeamFoundationServiceHostModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class TeamFoundationServiceHostModel
  {
    public TeamFoundationServiceHostModel(IVssRequestContext requestContext)
    {
      IVssServiceHost serviceHost = requestContext.ServiceHost;
      this.InstanceId = serviceHost.InstanceId;
      this.Name = serviceHost.Name;
      this.HostType = serviceHost.HostType;
      this.VDir = requestContext.VirtualPath();
      this.RelVDir = requestContext.TrimmedVirtualDirectory();
    }

    public TeamFoundationServiceHostModel()
    {
    }

    [DataMember]
    public Guid InstanceId { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public TeamFoundationHostType HostType { get; set; }

    [DataMember]
    public string VDir { get; set; }

    [DataMember]
    public string RelVDir { get; set; }
  }
}
