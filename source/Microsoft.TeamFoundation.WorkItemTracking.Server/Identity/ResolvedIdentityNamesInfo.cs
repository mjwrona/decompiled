// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Identity.ResolvedIdentityNamesInfo
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Identity
{
  public class ResolvedIdentityNamesInfo
  {
    public IDictionary<string, ConstantsSearchRecord> NamesLookup { get; private set; }

    public IDictionary<string, ConstantsSearchRecord[]> AmbiguousNamesLookup { get; private set; }

    public HashSet<string> NotFoundNames { get; set; }

    public HashSet<WorkItemIdentity> NotFoundIdentities { get; set; }

    public Lazy<IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>> IdentityMap { get; internal set; }

    public IEnumerable<ConstantsSearchRecord> AllRecords { get; set; }

    public IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> ResolvedAadIdentities { get; private set; }

    public IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> AadIdentityLookup { get; private set; }

    public IDictionary<string, Guid> ResolvedNonLicensedIdentities { get; private set; }

    public IDictionary<string, string> AadNamesLookup { get; internal set; }

    public IDictionary<string, string> AadNotInScopeNames { get; internal set; }

    public ResolvedIdentityNamesInfo()
    {
      this.AadNamesLookup = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.AadIdentityLookup = (IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>) new Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.NamesLookup = (IDictionary<string, ConstantsSearchRecord>) new Dictionary<string, ConstantsSearchRecord>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.AmbiguousNamesLookup = (IDictionary<string, ConstantsSearchRecord[]>) new Dictionary<string, ConstantsSearchRecord[]>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.NotFoundNames = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.NotFoundIdentities = new HashSet<WorkItemIdentity>();
      this.AllRecords = Enumerable.Empty<ConstantsSearchRecord>();
      this.IdentityMap = new Lazy<IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>>((Func<IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>>) (() => (IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>) new Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)));
      this.ResolvedAadIdentities = (IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>) new Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>();
      this.AadNotInScopeNames = (IDictionary<string, string>) new Dictionary<string, string>();
      this.ResolvedNonLicensedIdentities = (IDictionary<string, Guid>) new Dictionary<string, Guid>();
    }
  }
}
