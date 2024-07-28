// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.WebServices.RegistryWebService
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
  [ClientService(ComponentName = "Framework", RegistrationName = "Framework", ServerConfiguration = ServerConfiguration.TfsConnection, ServiceName = "RegistryService", CollectionServiceIdentifier = "9ebadf24-f99a-42f5-a615-7598045b47dd", ConfigurationServiceIdentifier = "48DF7C8F-5554-4A80-80B8-7C205CE39B7A")]
  public class RegistryWebService : FrameworkWebService
  {
    private SecuredRegistryManager m_registryManager;

    public RegistryWebService() => this.m_registryManager = this.RequestContext.GetService<SecuredRegistryManager>();

    [WebMethod]
    public List<RegistryEntry> QueryRegistryEntries(string registryPathPattern, bool includeFolders)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryRegistryEntries), MethodType.LightWeight, EstimatedMethodCost.Free);
        methodInformation.AddParameter(nameof (registryPathPattern), (object) registryPathPattern);
        methodInformation.AddParameter(nameof (includeFolders), (object) includeFolders);
        this.EnterMethod(methodInformation);
        return this.RequestContext.ExecutionEnvironment.IsHostedDeployment ? new List<RegistryEntry>() : this.m_registryManager.QueryRegistryEntries(this.RequestContext, registryPathPattern, includeFolders);
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

    [WebMethod]
    public void UpdateRegistryEntries([ClientType(typeof (IEnumerable<RegistryEntry>))] RegistryEntry[] registryEntries)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateRegistryEntries), MethodType.ReadWrite, EstimatedMethodCost.Free);
        methodInformation.AddArrayParameter<RegistryEntry>(nameof (registryEntries), (IList<RegistryEntry>) registryEntries);
        this.EnterMethod(methodInformation);
        this.RequestContext.CheckOnPremisesDeployment(true);
        this.m_registryManager.UpdateRegistryEntries(this.RequestContext, registryEntries);
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

    [WebMethod]
    public int RemoveRegistryEntries(string[] registryPathPatterns)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (RemoveRegistryEntries), MethodType.ReadWrite, EstimatedMethodCost.Free);
        methodInformation.AddArrayParameter<string>(nameof (registryPathPatterns), (IList<string>) registryPathPatterns);
        this.EnterMethod(methodInformation);
        this.RequestContext.CheckOnPremisesDeployment(true);
        return this.m_registryManager.RemoveRegistryEntries(this.RequestContext, registryPathPatterns);
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

    [WebMethod]
    public List<RegistryAuditEntry> QueryAuditLog(int changeIndex, bool returnOlder)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryAuditLog), MethodType.LightWeight, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (changeIndex), (object) changeIndex);
        methodInformation.AddParameter(nameof (returnOlder), (object) returnOlder);
        this.EnterMethod(methodInformation);
        return this.RequestContext.ExecutionEnvironment.IsHostedDeployment ? new List<RegistryAuditEntry>() : this.m_registryManager.QueryAuditLog(this.RequestContext, changeIndex, returnOlder);
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

    [WebMethod]
    public List<RegistryEntry> QueryUserEntries(string registryPathPattern, bool includeFolders)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryUserEntries), MethodType.Normal, EstimatedMethodCost.VeryLow);
        methodInformation.AddParameter(nameof (registryPathPattern), (object) registryPathPattern);
        methodInformation.AddParameter(nameof (includeFolders), (object) includeFolders);
        this.EnterMethod(methodInformation);
        return this.m_registryManager.QueryUserEntries(this.RequestContext, registryPathPattern, includeFolders);
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

    [WebMethod]
    public void UpdateUserEntries([ClientType(typeof (IEnumerable<RegistryEntry>))] RegistryEntry[] registryEntries)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateUserEntries), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<RegistryEntry>(nameof (registryEntries), (IList<RegistryEntry>) registryEntries);
        this.EnterMethod(methodInformation);
        this.m_registryManager.UpdateUserEntries(this.RequestContext, registryEntries);
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

    [WebMethod]
    public int RemoveUserEntries(string[] registryPathPatterns)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (RemoveUserEntries), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (registryPathPatterns), (IList<string>) registryPathPatterns);
        this.EnterMethod(methodInformation);
        return this.m_registryManager.RemoveUserEntries(this.RequestContext, registryPathPatterns);
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
