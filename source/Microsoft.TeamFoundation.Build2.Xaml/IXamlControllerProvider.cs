// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Xaml.IXamlControllerProvider
// Assembly: Microsoft.TeamFoundation.Build2.Xaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 48A241AC-D20F-49E0-A581-C219E1ED7760
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Xaml.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Xaml
{
  [DefaultServiceImplementation(typeof (XamlControllerProvider))]
  public interface IXamlControllerProvider : IVssFrameworkService
  {
    BuildController GetController(IVssRequestContext requestContext, int controllerId);

    IEnumerable<BuildController> GetControllers(IVssRequestContext requestContext, string name);
  }
}
