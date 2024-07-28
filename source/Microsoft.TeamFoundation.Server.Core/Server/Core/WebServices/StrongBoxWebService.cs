// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.WebServices.StrongBoxWebService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Server.Core.WebServices
{
  [WebService(Namespace = "http://microsoft.com/webservices/")]
  [ClientService(ComponentName = "Framework", RegistrationName = "Framework", ServerConfiguration = ServerConfiguration.TfsTeamProjectCollection, ServiceName = "StrongBoxService", CollectionServiceIdentifier = "2503DE38-D600-4CCC-87CF-DF7EE6D00396")]
  public sealed class StrongBoxWebService : TeamFoundationWebService
  {
    private TeamFoundationStrongBoxService m_strongBoxService;

    public StrongBoxWebService() => this.RequestContext.CheckProjectCollectionRequestContext();

    public TeamFoundationStrongBoxService StrongBoxService
    {
      get
      {
        if (this.m_strongBoxService == null)
          this.m_strongBoxService = this.RequestContext.GetService<TeamFoundationStrongBoxService>();
        return this.m_strongBoxService;
      }
    }

    [WebMethod]
    public Guid CreateDrawer(string name)
    {
      MethodInformation methodInformation = new MethodInformation(nameof (CreateDrawer), MethodType.LightWeight, EstimatedMethodCost.VeryLow, false);
      methodInformation.AddParameter(nameof (name), (object) name);
      try
      {
        this.RequestContext.EnterMethod(methodInformation);
        return this.StrongBoxService.CreateDrawer(this.RequestContext, name);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.RequestContext.LeaveMethod();
      }
    }

    [WebMethod]
    public Guid UnlockDrawer(string name)
    {
      MethodInformation methodInformation = new MethodInformation(nameof (UnlockDrawer), MethodType.LightWeight, EstimatedMethodCost.VeryLow, false);
      methodInformation.AddParameter(nameof (name), (object) name);
      try
      {
        this.RequestContext.EnterMethod(methodInformation);
        return this.StrongBoxService.UnlockDrawer(this.RequestContext, name, true);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.RequestContext.LeaveMethod();
      }
    }

    [WebMethod]
    public void DeleteDrawer(Guid drawerId)
    {
      MethodInformation methodInformation = new MethodInformation(nameof (DeleteDrawer), MethodType.LightWeight, EstimatedMethodCost.VeryLow, false);
      methodInformation.AddParameter(nameof (drawerId), (object) drawerId);
      try
      {
        this.RequestContext.EnterMethod(methodInformation);
        this.StrongBoxService.DeleteDrawer(this.RequestContext, drawerId);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.RequestContext.LeaveMethod();
      }
    }

    [WebMethod]
    public List<StrongBoxItemInfo> GetDrawerContents(Guid drawerId)
    {
      MethodInformation methodInformation = new MethodInformation(nameof (GetDrawerContents), MethodType.LightWeight, EstimatedMethodCost.Moderate, false);
      methodInformation.AddParameter(nameof (drawerId), (object) drawerId);
      try
      {
        this.RequestContext.EnterMethod(methodInformation);
        return this.StrongBoxService.GetDrawerContents(this.RequestContext, drawerId);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.RequestContext.LeaveMethod();
      }
    }

    [WebMethod]
    public StrongBoxItemInfo GetItemInfo(Guid drawerId, string lookupKey)
    {
      MethodInformation methodInformation = new MethodInformation(nameof (GetItemInfo), MethodType.LightWeight, EstimatedMethodCost.Moderate, false);
      methodInformation.AddParameter(nameof (drawerId), (object) drawerId);
      methodInformation.AddParameter(nameof (lookupKey), (object) lookupKey);
      try
      {
        this.RequestContext.EnterMethod(methodInformation);
        return this.StrongBoxService.GetItemInfo(this.RequestContext, drawerId, lookupKey);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.RequestContext.LeaveMethod();
      }
    }

    [WebMethod]
    public string GetString(Guid drawerId, string lookupKey)
    {
      MethodInformation methodInformation = new MethodInformation(nameof (GetString), MethodType.LightWeight, EstimatedMethodCost.Moderate, false);
      methodInformation.AddParameter(nameof (drawerId), (object) drawerId);
      methodInformation.AddParameter(nameof (lookupKey), (object) lookupKey);
      try
      {
        this.RequestContext.EnterMethod(methodInformation);
        return this.StrongBoxService.GetString(this.RequestContext, drawerId, lookupKey);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.RequestContext.LeaveMethod();
      }
    }

    [WebMethod]
    public void AddString(Guid drawerId, string lookupKey, string value)
    {
      MethodInformation methodInformation = new MethodInformation(nameof (AddString), MethodType.LightWeight, EstimatedMethodCost.Moderate, false);
      methodInformation.AddParameter(nameof (drawerId), (object) drawerId);
      methodInformation.AddParameter(nameof (lookupKey), (object) lookupKey);
      methodInformation.AddParameter(nameof (value), (object) value);
      try
      {
        this.RequestContext.EnterMethod(methodInformation);
        this.StrongBoxService.AddString(this.RequestContext, drawerId, lookupKey, value);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.RequestContext.LeaveMethod();
      }
    }

    [WebMethod]
    public void DeleteItem(Guid drawerId, string lookupKey)
    {
      MethodInformation methodInformation = new MethodInformation(nameof (DeleteItem), MethodType.LightWeight, EstimatedMethodCost.Moderate, false);
      methodInformation.AddParameter(nameof (drawerId), (object) drawerId);
      methodInformation.AddParameter(nameof (lookupKey), (object) lookupKey);
      try
      {
        this.RequestContext.EnterMethod(methodInformation);
        this.StrongBoxService.DeleteItem(this.RequestContext, drawerId, lookupKey);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.RequestContext.LeaveMethod();
      }
    }
  }
}
