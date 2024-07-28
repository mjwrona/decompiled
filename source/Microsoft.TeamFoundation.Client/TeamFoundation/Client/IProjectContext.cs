// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.IProjectContext
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;

namespace Microsoft.TeamFoundation.Client
{
  [Obsolete("Obsolete and will be removed in the subsequent release, use ITeamFoundationContext.")]
  public interface IProjectContext
  {
    string DomainName { get; }

    string DomainUri { get; }

    string ProjectName { get; }

    string ProjectUri { get; }
  }
}
