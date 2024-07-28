// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Routing.TfsServiceHostDescriptor
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web;

namespace Microsoft.TeamFoundation.Server.WebAccess.Routing
{
  [DataContract]
  public class TfsServiceHostDescriptor : WebSdkMetadata
  {
    private string m_relativeVirtualDirectory;

    public TfsServiceHostDescriptor()
    {
    }

    public TfsServiceHostDescriptor(IVssRequestContext requestContext)
      : this(requestContext.ServiceHost, requestContext.VirtualPath())
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal TfsServiceHostDescriptor(IVssServiceHost serviceHost, string hostVirtualPath)
    {
      this.Id = serviceHost.InstanceId;
      this.Name = serviceHost.Name;
      this.HostType = serviceHost.HostType;
      this.VirtualDirectory = hostVirtualPath ?? UrlHostResolutionService.ApplicationVirtualPath;
      this.Status = serviceHost.Status;
      this.StatusReason = serviceHost.StatusReason;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public TfsServiceHostDescriptor(HostProperties serviceHostProperties, string hostVirtualPath)
    {
      this.Id = serviceHostProperties.Id;
      this.Name = serviceHostProperties.Name;
      this.HostType = serviceHostProperties.HostType;
      this.VirtualDirectory = hostVirtualPath ?? UrlHostResolutionService.ApplicationVirtualPath;
      this.Status = serviceHostProperties.Status;
      this.StatusReason = serviceHostProperties.StatusReason;
    }

    [DataMember(Name = "id", EmitDefaultValue = false)]
    public Guid Id { get; set; }

    [DataMember(Name = "name", EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(Name = "hostType", EmitDefaultValue = false)]
    public TeamFoundationHostType HostType { get; set; }

    [DataMember(Name = "vdir", EmitDefaultValue = false)]
    public string VirtualDirectory { get; set; }

    [DataMember(Name = "relVdir", EmitDefaultValue = false)]
    public string RelativeVirtualDirectory
    {
      get
      {
        if (this.m_relativeVirtualDirectory == null)
          this.SetRelativeVirtualDirectory();
        return this.m_relativeVirtualDirectory;
      }
      set => this.m_relativeVirtualDirectory = value;
    }

    private void SetRelativeVirtualDirectory()
    {
      if (!string.IsNullOrEmpty(this.VirtualDirectory))
        this.RelativeVirtualDirectory = PlatformHelpers.TrimVirtualPath(VirtualPathUtility.ToAppRelative(this.VirtualDirectory));
      else
        this.RelativeVirtualDirectory = string.Empty;
    }

    [IgnoreDataMember]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public TeamFoundationServiceHostStatus Status { get; set; }

    [IgnoreDataMember]
    internal string StatusReason { get; set; }

    public override string ToString() => this.RelativeVirtualDirectory;

    public override bool Equals(object obj) => obj is TfsServiceHostDescriptor serviceHostDescriptor && this.Id == serviceHostDescriptor.Id;

    public override int GetHashCode() => this.Id.GetHashCode();
  }
}
