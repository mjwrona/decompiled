// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Proxy.CommonStructureService3
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Server;
using System.Web.Services.Protocols;

namespace Microsoft.TeamFoundation.Proxy
{
  internal class CommonStructureService3 : 
    CommonStructureService,
    ICommonStructureService3,
    ICommonStructureService
  {
    private Classification3 _proxy3;

    internal CommonStructureService3(TfsTeamProjectCollection tfsObject, string url)
      : base(tfsObject, url)
    {
      this._proxy3 = new Classification3(tfsObject, url);
    }

    public string GetChangedNodesAndProjects(int firstSequenceId)
    {
      try
      {
        return this._proxy3.GetChangedNodesAndProjects(firstSequenceId);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }
  }
}
