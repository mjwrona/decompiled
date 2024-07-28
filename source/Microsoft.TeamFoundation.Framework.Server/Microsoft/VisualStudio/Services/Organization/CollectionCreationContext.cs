// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.CollectionCreationContext
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Organization
{
  [DataContract]
  public class CollectionCreationContext
  {
    public CollectionCreationContext(string name, IDictionary<string, object> data)
    {
      ArgumentUtility.CheckForNull<string>(name, nameof (name));
      this.Name = name;
      this.Data = data ?? (IDictionary<string, object>) new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
    }

    [DataMember(IsRequired = true)]
    public string Name { get; }

    [DataMember(IsRequired = true)]
    public Guid OwnerId { get; set; }

    [DataMember(IsRequired = false)]
    public string PreferredRegion { get; set; }

    [DataMember(IsRequired = false)]
    public string PreferredGeography { get; set; }

    [DataMember(IsRequired = false)]
    public IDictionary<string, object> Data { get; }
  }
}
