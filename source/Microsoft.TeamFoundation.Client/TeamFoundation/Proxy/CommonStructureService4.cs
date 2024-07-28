// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Proxy.CommonStructureService4
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Server;
using System;
using System.Web.Services.Protocols;

namespace Microsoft.TeamFoundation.Proxy
{
  internal class CommonStructureService4 : 
    CommonStructureService3,
    ICommonStructureService4,
    ICommonStructureService3,
    ICommonStructureService
  {
    private Classification4 _proxy4;

    internal CommonStructureService4(TfsTeamProjectCollection tfsObject, string url)
      : base(tfsObject, url)
    {
      this._proxy4 = new Classification4(tfsObject, url);
    }

    public string CreateNode(
      string nodeName,
      string parentNodeUri,
      DateTime? startDate,
      DateTime? finishDate)
    {
      try
      {
        return this._proxy4.CreateNodeWithDates(nodeName, parentNodeUri, startDate, finishDate);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public void SetIterationDates(string nodeUri, DateTime? startDate, DateTime? finishDate)
    {
      try
      {
        this._proxy4.SetIterationDates(nodeUri, startDate, finishDate);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public ProjectProperty GetProjectProperty(string projectUri, string name)
    {
      try
      {
        return this._proxy4.GetProjectProperty(projectUri, name);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public void SetProjectProperty(string projectUri, string name, string value)
    {
      try
      {
        this._proxy4.SetProjectProperty(projectUri, name, value);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }
  }
}
