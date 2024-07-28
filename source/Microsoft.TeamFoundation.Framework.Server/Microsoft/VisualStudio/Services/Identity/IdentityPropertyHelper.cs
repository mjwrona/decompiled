// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityPropertyHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class IdentityPropertyHelper : IdentityPropertyHelper<Microsoft.VisualStudio.Services.Identity.Identity>
  {
    protected override Guid GetArtifactId(IdentityPropertyScope propertyScope, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (propertyScope != IdentityPropertyScope.Global)
        return identity.Id;
      return !(identity.MasterId != Guid.Empty) || !(identity.MasterId != IdentityConstants.LinkedId) ? identity.Id : identity.MasterId;
    }

    protected override void SetProperty(
      IdentityPropertyScope propertyScope,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string propertyName,
      object propertyValue)
    {
      identity.Properties[propertyName] = propertyValue;
    }

    protected override object GetProperty(
      IdentityPropertyScope propertyScope,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string propertyName)
    {
      return identity.GetProperty<object>(propertyName, (object) null);
    }

    protected override HashSet<string> GetModifiedProperties(
      IdentityPropertyScope propertyScope,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      return identity.GetModifiedProperties();
    }

    protected override void ResetModifiedProperties(
      IdentityPropertyScope propertyScope,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      identity.ResetModifiedProperties();
    }
  }
}
