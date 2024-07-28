// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FavoriteViewScopeInformation
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class FavoriteViewScopeInformation
  {
    public bool IsPersonal { get; set; }

    public string FeatureScope { get; set; }

    public Guid ProjectGuid { get; set; }
  }
}
