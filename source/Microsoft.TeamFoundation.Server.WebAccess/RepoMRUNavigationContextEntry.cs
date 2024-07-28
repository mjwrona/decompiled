// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.RepoMRUNavigationContextEntry
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [TypeConverter(typeof (RepoMRUNavigationContextEntryConverter))]
  [DataContract]
  public class RepoMRUNavigationContextEntry
  {
    public RepoMRUNavigationContextEntry()
    {
    }

    public RepoMRUNavigationContextEntry(TfsWebContext tfsWebContext)
    {
      this.ServiceHost = new TfsServiceHostDescriptor(tfsWebContext.TfsRequestContext);
      this.LastAccessedByUser = new DateTime?(DateTime.UtcNow);
      if ((tfsWebContext.NavigationContext.Levels & NavigationContextLevels.Project) != NavigationContextLevels.Project)
        return;
      this.Project = tfsWebContext.Project.Name;
      string[] source = tfsWebContext.Url.RequestContext.HttpContext.Request.Path.Split('/');
      bool flag1 = StringComparer.OrdinalIgnoreCase.Equals(source[source.Length - 2], "_git");
      bool flag2 = StringComparer.OrdinalIgnoreCase.Equals(((IEnumerable<string>) source).Last<string>(), "_versioncontrol");
      this.Repo = flag1 ? ((IEnumerable<string>) source).Last<string>() : (flag2 ? tfsWebContext.Project.Name : (string) null);
      this.IsGit = flag1;
    }

    [DataMember(Name = "serviceHost", EmitDefaultValue = false)]
    public TfsServiceHostDescriptor ServiceHost { get; set; }

    [DataMember(Name = "project", EmitDefaultValue = false)]
    public string Project { get; set; }

    [DataMember(Name = "repo", EmitDefaultValue = false)]
    public string Repo { get; set; }

    [DataMember(Name = "isGit", EmitDefaultValue = false)]
    public bool IsGit { get; set; }

    [DataMember(Name = "lastAccessedByUser", EmitDefaultValue = false)]
    public DateTime? LastAccessedByUser { get; set; }

    internal void FixServiceHost(TfsWebContext tfsWebContext)
    {
      if (this.ServiceHost == null || this.ServiceHost.HostType != TeamFoundationHostType.ProjectCollection)
        return;
      if (this.ServiceHost.Id == tfsWebContext.TfsRequestContext.ServiceHost.InstanceId)
      {
        this.ServiceHost = new TfsServiceHostDescriptor(tfsWebContext.TfsRequestContext);
      }
      else
      {
        IVssRequestContext vssRequestContext = tfsWebContext.TfsRequestContext.To(TeamFoundationHostType.Deployment);
        HostProperties hostProperties = vssRequestContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostPropertiesCached(vssRequestContext, this.ServiceHost.Id);
        if (hostProperties == null)
          return;
        this.ServiceHost = new TfsServiceHostDescriptor(hostProperties, hostProperties.VirtualPath(vssRequestContext));
      }
    }

    public bool Equals(NavigationContext navContext) => navContext != null && !(this.ServiceHost.Id != navContext.InstanceId) && StringComparer.OrdinalIgnoreCase.Equals(this.Project, navContext.Project);

    public bool Equals(RepoMRUNavigationContextEntry entry) => entry != null && !(this.ServiceHost.Id != entry.ServiceHost.Id) && StringComparer.OrdinalIgnoreCase.Equals(this.Project, entry.Project) && StringComparer.OrdinalIgnoreCase.Equals(this.Repo, entry.Repo);

    public override bool Equals(object obj)
    {
      switch (obj)
      {
        case RepoMRUNavigationContextEntry entry:
          return this.Equals(entry);
        case NavigationContext navContext:
          return this.Equals(navContext);
        default:
          return base.Equals(obj);
      }
    }

    public override int GetHashCode()
    {
      int hashA = this.ServiceHost.GetHashCode();
      if (this.Repo != null)
        hashA = CommonUtility.CombineHashCodes(hashA, this.Repo.ToUpperInvariant().GetHashCode());
      if (this.Project != null)
        hashA = CommonUtility.CombineHashCodes(hashA, this.Project.ToUpperInvariant().GetHashCode());
      return hashA;
    }
  }
}
