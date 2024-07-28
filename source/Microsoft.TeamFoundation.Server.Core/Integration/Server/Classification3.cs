// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.Classification3
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Integration.Server
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03", Description = "DevOps Classification web service V3.0")]
  [ClientService(ServiceName = "CommonStructure3", CollectionServiceIdentifier = "02ea5fcc-1e40-4d94-a8e5-ed62c15cb676")]
  [ProxyParentClass("Classification", IgnoreInheritedMethods = true)]
  public class Classification3 : Classification
  {
    public Classification3()
      : base(3)
    {
    }

    public Classification3(int serviceVersion)
      : base(serviceVersion)
    {
    }

    [WebMethod]
    public string GetChangedNodesAndProjects(int firstSequenceId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetChangedNodesAndProjects), MethodType.Normal, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (firstSequenceId), (object) firstSequenceId);
        this.EnterMethod(methodInformation);
        return this.RequestContext.GetService<CommonStructureService>().GetChangedNodesAndProjects(this.RequestContext, firstSequenceId);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }
  }
}
