// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.IProcessTemplateManagerLauncher
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.Client
{
  [Guid("e9e3586b-e007-4762-b598-b76b30fb6ada")]
  [ComVisible(true)]
  [InterfaceType(ComInterfaceType.InterfaceIsDual)]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IProcessTemplateManagerLauncher
  {
    void Show(TfsTeamProjectCollection tpc);
  }
}
