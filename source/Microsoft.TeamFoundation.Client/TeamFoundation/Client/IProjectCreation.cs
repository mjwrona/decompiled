// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.IProjectCreation
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.Client
{
  [Guid("D2E07D5A-2DA2-4118-9C6E-31E8EFEDB9BC")]
  [ComVisible(true)]
  [InterfaceType(ComInterfaceType.InterfaceIsDual)]
  public interface IProjectCreation
  {
    bool BatchCreateTeamProject(string teamProjectCreationSettingFile, out string logFileFullPath);
  }
}
