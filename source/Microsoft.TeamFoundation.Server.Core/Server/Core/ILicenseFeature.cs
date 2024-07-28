// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ILicenseFeature
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System;

namespace Microsoft.TeamFoundation.Server.Core
{
  public interface ILicenseFeature
  {
    string Name { get; }

    string Description { get; }

    Guid Id { get; }

    bool InPreviewMode { get; }
  }
}
