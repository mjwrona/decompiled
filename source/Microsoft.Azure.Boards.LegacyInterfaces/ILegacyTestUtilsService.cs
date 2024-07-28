// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.LegacyInterfaces.ILegacyTestUtilsService
// Assembly: Microsoft.Azure.Boards.LegacyInterfaces, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C0E0C41-D39C-453E-A6CF-32A7C57153EE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.LegacyInterfaces.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.WorkItemTracking.LegacyInterfaces
{
  [DefaultServiceImplementation("Microsoft.TeamFoundation.WorkItemTracking.LegacyTfsImplementations.LegacyTfsTestUtilsService, Microsoft.Tfs.WorkItemTracking.LegacyTfsImplementations")]
  public interface ILegacyTestUtilsService : IVssFrameworkService
  {
    string GenerateTestStepsHtml(string tcmStepsValue);
  }
}
