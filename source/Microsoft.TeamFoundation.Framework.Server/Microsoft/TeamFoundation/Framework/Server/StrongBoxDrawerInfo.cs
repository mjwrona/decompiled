// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StrongBoxDrawerInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class StrongBoxDrawerInfo
  {
    internal StrongBoxDrawerInfo(
      string name,
      Guid drawerId,
      Guid signingKeyId,
      DateTime lastRotateDate)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      ArgumentUtility.CheckForEmptyGuid(drawerId, nameof (drawerId));
      this.Name = name;
      this.DrawerId = drawerId;
      this.SigningKeyId = signingKeyId;
      this.LastRotateDate = lastRotateDate;
    }

    internal string Name { get; private set; }

    internal Guid DrawerId { get; private set; }

    internal Guid SigningKeyId { get; set; }

    internal DateTime LastRotateDate { get; private set; }
  }
}
