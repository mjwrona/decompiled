// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.IProjectAlertsLauncher
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.Client
{
  [Guid("7C33D957-342E-444C-B0BD-3BDE908EAA67")]
  [ComVisible(true)]
  [InterfaceType(ComInterfaceType.InterfaceIsDual)]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IProjectAlertsLauncher
  {
    void Show(TfsTeamProjectCollection tpc, string projectName);
  }
}
