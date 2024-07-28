// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.OrganizationCreationContext
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Organization
{
  public class OrganizationCreationContext
  {
    public OrganizationCreationContext(
      string name,
      bool autoGenerateName = false,
      IDictionary<string, object> data = null)
    {
      if (!autoGenerateName)
        ArgumentUtility.CheckForNull<string>(name, nameof (name));
      this.Name = name;
      this.AutoGenerateName = autoGenerateName;
      this.Data = data ?? (IDictionary<string, object>) new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
    }

    public string Name { get; }

    public bool AutoGenerateName { get; }

    public Guid CreatorId { get; set; }

    public string PreferredRegion { get; set; }

    public string PreferredGeography { get; set; }

    public CollectionCreationContext PrimaryCollection { get; set; }

    public IDictionary<string, object> Data { get; }
  }
}
