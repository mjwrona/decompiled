// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.PolicyInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Organization
{
  public class PolicyInfo
  {
    public PolicyInfo(string name, string description, Uri moreInfoLink)
    {
      ArgumentUtility.CheckForNull<string>(name, nameof (name));
      this.Name = name;
      this.Description = description;
      this.MoreInfoLink = moreInfoLink;
    }

    public string Name { get; }

    public string Description { get; }

    public Uri MoreInfoLink { get; }
  }
}
