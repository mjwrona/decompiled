// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ITeamFoundationContextManager
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.Client
{
  [CLSCompliant(false)]
  [Guid("C4EDF2C2-A987-4AFC-AC67-8E8ADE6D2E13")]
  public interface ITeamFoundationContextManager
  {
    ITeamFoundationContext CurrentContext { get; }

    void SetContext(TfsTeamProjectCollection teamProjectCollection, string projectUri);

    void SetContext(TfsTeamProjectCollection teamProjectCollection, string projectUri, Guid teamId);

    event EventHandler<ContextChangedEventArgs> ContextChanged;

    event EventHandler<ContextChangingEventArgs> ContextChanging;
  }
}
