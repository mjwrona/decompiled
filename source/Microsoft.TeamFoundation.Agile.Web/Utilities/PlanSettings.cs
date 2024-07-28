// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Utilities.PlanSettings
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Agile.Web.Utilities
{
  internal class PlanSettings
  {
    internal const int c_maxPlanCountPerProjectDefault = 1000;
    internal const string c_maxPlanCountPerProjectRegistryPath = "/Configuration/Plan/MaxPlanCountPerProject";

    public int MaxPlanCountPerProject { get; private set; }

    public static PlanSettings Create(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      return new PlanSettings()
      {
        MaxPlanCountPerProject = service.GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Plan/MaxPlanCountPerProject", 1000)
      };
    }

    internal PlanSettings()
    {
    }
  }
}
