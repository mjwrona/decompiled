// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.MRUNavigationContextEntry
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [TypeConverter(typeof (MRUNavigationContextEntryConverter))]
  [DataContract]
  public class MRUNavigationContextEntry
  {
    public MRUNavigationContextEntry()
    {
    }

    public MRUNavigationContextEntry(TfsWebContext tfsWebContext)
    {
      this.TopMostLevel = tfsWebContext.NavigationContext.TopMostLevel;
      this.ServiceHost = new TfsServiceHostDescriptor(tfsWebContext.TfsRequestContext);
      this.LastAccessedByUser = new DateTime?(DateTime.UtcNow);
      if ((tfsWebContext.NavigationContext.Levels & NavigationContextLevels.Project) != NavigationContextLevels.Project)
        return;
      this.Project = tfsWebContext.Project.Name;
      if ((tfsWebContext.NavigationContext.Levels & NavigationContextLevels.Team) != NavigationContextLevels.Team)
        return;
      this.Team = tfsWebContext.Team.Name;
    }

    [DataMember(Name = "topMostLevel", EmitDefaultValue = false)]
    public NavigationContextLevels TopMostLevel { get; set; }

    [DataMember(Name = "serviceHost", EmitDefaultValue = false)]
    public TfsServiceHostDescriptor ServiceHost { get; set; }

    [DataMember(Name = "project", EmitDefaultValue = false)]
    public string Project { get; set; }

    [DataMember(Name = "team", EmitDefaultValue = false)]
    public string Team { get; set; }

    [DataMember(Name = "lastAccessedByUser", EmitDefaultValue = false)]
    public DateTime? LastAccessedByUser { get; set; }

    internal void FixServiceHost(TfsWebContext tfsWebContext) => this.FixServiceHost(tfsWebContext.TfsRequestContext);

    internal void FixServiceHost(IVssRequestContext requestContext)
    {
      if (this.ServiceHost == null || this.ServiceHost.HostType != TeamFoundationHostType.ProjectCollection)
        return;
      if (this.ServiceHost.Id == requestContext.ServiceHost.InstanceId)
      {
        this.ServiceHost = new TfsServiceHostDescriptor(requestContext);
      }
      else
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        HostProperties hostProperties = vssRequestContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostPropertiesCached(vssRequestContext, this.ServiceHost.Id);
        if (hostProperties == null)
          return;
        this.ServiceHost = new TfsServiceHostDescriptor(hostProperties, hostProperties.VirtualPath(vssRequestContext));
      }
    }

    public bool Equals(NavigationContext navContext) => navContext != null && this.TopMostLevel == navContext.TopMostLevel && !(this.ServiceHost.Id != navContext.InstanceId) && StringComparer.OrdinalIgnoreCase.Equals(this.Project, navContext.Project) && StringComparer.OrdinalIgnoreCase.Equals(this.Team, navContext.Team);

    public bool Equals(MRUNavigationContextEntry entry) => entry != null && this.TopMostLevel == entry.TopMostLevel && !(this.ServiceHost.Id != entry.ServiceHost.Id) && StringComparer.OrdinalIgnoreCase.Equals(this.Project, entry.Project) && StringComparer.OrdinalIgnoreCase.Equals(this.Team, entry.Team);

    public override bool Equals(object obj)
    {
      switch (obj)
      {
        case MRUNavigationContextEntry entry:
          return this.Equals(entry);
        case NavigationContext navContext:
          return this.Equals(navContext);
        default:
          return base.Equals(obj);
      }
    }

    public override int GetHashCode()
    {
      int hashA = CommonUtility.CombineHashCodes(this.TopMostLevel.GetHashCode(), this.ServiceHost.GetHashCode());
      if (this.Project != null)
        hashA = CommonUtility.CombineHashCodes(hashA, this.Project.ToUpperInvariant().GetHashCode());
      if (this.Team != null)
        hashA = CommonUtility.CombineHashCodes(hashA, this.Team.ToUpperInvariant().GetHashCode());
      return hashA;
    }
  }
}
