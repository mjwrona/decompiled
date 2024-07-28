// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.WebServices.TeamProjectCollectionWebService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Server.Core.WebServices
{
  [WebService(Namespace = "http://microsoft.com/webservices/")]
  [ClientService(ComponentName = "Framework", ServerConfiguration = ServerConfiguration.TfsConfigurationServer, ServiceName = "TeamProjectCollectionService", ConfigurationServiceIdentifier = "F358DF00-1881-4DB1-BB56-73ED3300CF38")]
  public class TeamProjectCollectionWebService : FrameworkWebService
  {
    [WebMethod]
    public ServicingJobDetail QueueCreateCollection(
      TeamProjectCollectionProperties collectionProperties,
      string dataTierConnectionString)
    {
      try
      {
        ArgumentUtility.CheckForNull<TeamProjectCollectionProperties>(collectionProperties, nameof (collectionProperties));
        MethodInformation methodInformation = new MethodInformation(nameof (QueueCreateCollection), MethodType.Admin, EstimatedMethodCost.High);
        methodInformation.AddParameter("Name", (object) collectionProperties.Name);
        methodInformation.AddParameter("Description", (object) collectionProperties.Description);
        methodInformation.AddParameter("IsDefault", (object) collectionProperties.IsDefault);
        methodInformation.AddParameter("VirtualDirectory", (object) collectionProperties.VirtualDirectory);
        methodInformation.AddParameter("State", (object) collectionProperties.StateValue);
        methodInformation.AddParameter(nameof (dataTierConnectionString), (object) dataTierConnectionString);
        methodInformation.AddParameter("DefaultConnectionString", (object) collectionProperties.DefaultConnectionString);
        methodInformation.AddArrayParameter<KeyValue<string, string>>("DatabaseCategoryConnectionStrings", (IList<KeyValue<string, string>>) collectionProperties.DatabaseCategoryConnectionStringsValue);
        methodInformation.AddArrayParameter<KeyValue<string, string>>("ServicingTokens", (IList<KeyValue<string, string>>) collectionProperties.ServicingTokensValue);
        this.EnterMethod(methodInformation);
        this.RequestContext.CheckOnPremisesDeployment(true);
        IDictionary<string, string> servicingTokens = collectionProperties.GetServicingTokens();
        IVssRequestContext vssRequestContext = this.RequestContext.To(TeamFoundationHostType.Application);
        return vssRequestContext.GetService<TeamProjectCollectionService>().QueueCreateCollection(vssRequestContext, collectionProperties, servicingTokens, SqlConnectionInfoFactory.Create(dataTierConnectionString));
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
    public ServicingJobDetail QueueDetachCollection(
      TeamProjectCollectionProperties collectionProperties,
      string collectionStoppedMessage,
      out string detachedConnectionString)
    {
      try
      {
        ArgumentUtility.CheckForNull<TeamProjectCollectionProperties>(collectionProperties, nameof (collectionProperties));
        MethodInformation methodInformation = new MethodInformation(nameof (QueueDetachCollection), MethodType.Admin, EstimatedMethodCost.High);
        methodInformation.AddParameter("hostId", (object) collectionProperties.Id);
        methodInformation.AddArrayParameter<KeyValue<string, string>>("ServicingTokens", (IList<KeyValue<string, string>>) collectionProperties.ServicingTokensValue);
        methodInformation.AddParameter(nameof (collectionStoppedMessage), (object) collectionStoppedMessage);
        this.EnterMethod(methodInformation);
        this.RequestContext.CheckOnPremisesDeployment(true);
        Guid id = collectionProperties.Id;
        IDictionary<string, string> servicingTokens = collectionProperties.GetServicingTokens();
        IVssRequestContext vssRequestContext = this.RequestContext.To(TeamFoundationHostType.Application);
        return vssRequestContext.GetService<TeamProjectCollectionService>().QueueDetachCollection(vssRequestContext, id, servicingTokens, collectionStoppedMessage, out detachedConnectionString);
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
    public ServicingJobDetail QueueAttachCollection(
      TeamProjectCollectionProperties collectionProperties,
      bool cloneCollection)
    {
      try
      {
        ArgumentUtility.CheckForNull<TeamProjectCollectionProperties>(collectionProperties, nameof (collectionProperties));
        MethodInformation methodInformation = new MethodInformation(nameof (QueueAttachCollection), MethodType.Admin, EstimatedMethodCost.High);
        methodInformation.AddParameter("Name", (object) collectionProperties.Name);
        methodInformation.AddParameter("Description", (object) collectionProperties.Description);
        methodInformation.AddParameter("IsDefault", (object) collectionProperties.IsDefault);
        methodInformation.AddParameter("VirtualDirectory", (object) collectionProperties.VirtualDirectory);
        methodInformation.AddParameter("State", (object) collectionProperties.StateValue);
        methodInformation.AddParameter("DefaultConnectionString", (object) collectionProperties.DefaultConnectionString);
        methodInformation.AddArrayParameter<KeyValue<string, string>>("DatabaseCategoryConnectionStrings", (IList<KeyValue<string, string>>) collectionProperties.DatabaseCategoryConnectionStringsValue);
        methodInformation.AddArrayParameter<KeyValue<string, string>>("ServicingTokens", (IList<KeyValue<string, string>>) collectionProperties.ServicingTokensValue);
        methodInformation.AddParameter(nameof (cloneCollection), (object) cloneCollection);
        this.EnterMethod(methodInformation);
        this.RequestContext.CheckOnPremisesDeployment(true);
        IDictionary<string, string> servicingTokens = collectionProperties.GetServicingTokens();
        IVssRequestContext vssRequestContext = this.RequestContext.To(TeamFoundationHostType.Application);
        return vssRequestContext.GetService<TeamProjectCollectionService>().QueueAttachCollection(vssRequestContext, collectionProperties, servicingTokens, cloneCollection);
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
    public ServicingJobDetail QueueDeleteProject(
      TeamProjectCollectionProperties collectionProperties,
      string projectUri)
    {
      try
      {
        ArgumentUtility.CheckForNull<TeamProjectCollectionProperties>(collectionProperties, nameof (collectionProperties));
        MethodInformation methodInformation = new MethodInformation(nameof (QueueDeleteProject), MethodType.Admin, EstimatedMethodCost.High);
        methodInformation.AddParameter("hostId", (object) collectionProperties.Id);
        methodInformation.AddArrayParameter<KeyValue<string, string>>("ServicingTokens", (IList<KeyValue<string, string>>) collectionProperties.ServicingTokensValue);
        methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
        this.EnterMethod(methodInformation);
        this.RequestContext.CheckOnPremisesDeployment(true);
        Guid id = collectionProperties.Id;
        IDictionary<string, string> servicingTokens = collectionProperties.GetServicingTokens();
        using (IVssRequestContext vssRequestContext = this.RequestContext.GetService<TeamFoundationHostManagementService>().BeginRequest(this.RequestContext, id, RequestContextType.UserContext, true, true))
          return vssRequestContext.GetService<ProjectWorkflowService>().QueueHardDeleteProject(vssRequestContext, projectUri, servicingTokens);
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
    public ServicingJobDetail QueueDeleteCollection(Guid collectionId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation("DeleteCollection", MethodType.Admin, EstimatedMethodCost.High);
        methodInformation.AddParameter("hostId", (object) collectionId);
        this.EnterMethod(methodInformation);
        this.RequestContext.CheckOnPremisesDeployment(true);
        IVssRequestContext vssRequestContext = this.RequestContext.To(TeamFoundationHostType.Application);
        return vssRequestContext.GetService<TeamProjectCollectionService>().QueueDeleteCollection(vssRequestContext, collectionId);
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
    public ServicingJobDetail QueueUpdateCollection(
      TeamProjectCollectionProperties collectionProperties)
    {
      try
      {
        ArgumentUtility.CheckForNull<TeamProjectCollectionProperties>(collectionProperties, nameof (collectionProperties));
        MethodInformation methodInformation = new MethodInformation(nameof (QueueUpdateCollection), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddParameter("Id", (object) collectionProperties.Id);
        methodInformation.AddParameter("Name", (object) collectionProperties.Name);
        methodInformation.AddParameter("Description", (object) collectionProperties.Description);
        methodInformation.AddParameter("VirtualDirectory", (object) collectionProperties.VirtualDirectory);
        this.EnterMethod(methodInformation);
        this.RequestContext.CheckOnPremisesDeployment(true);
        IDictionary<string, string> servicingTokens = collectionProperties.GetServicingTokens();
        IVssRequestContext vssRequestContext = this.RequestContext.To(TeamFoundationHostType.Application);
        return vssRequestContext.GetService<TeamProjectCollectionService>().QueueUpdateCollection(vssRequestContext, collectionProperties, servicingTokens);
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
    public Guid GetDefaultCollectionId()
    {
      try
      {
        this.EnterMethod(new MethodInformation("GetDefaultCollectionProperties", MethodType.LightWeight, EstimatedMethodCost.Low));
        return Guid.Empty;
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
    public TeamProjectCollectionProperties[] GetCollectionProperties([ClientType(typeof (IEnumerable<Guid>))] Guid[] ids, int filterFlags)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetCollectionProperties), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<Guid>("Ids", (IList<Guid>) ids);
        methodInformation.AddParameter(nameof (filterFlags), (object) filterFlags);
        this.EnterMethod(methodInformation);
        IVssRequestContext vssRequestContext = this.RequestContext.To(TeamFoundationHostType.Application);
        TeamProjectCollectionProperties[] array = vssRequestContext.GetService<ITeamProjectCollectionPropertiesService>().GetCollectionProperties(vssRequestContext, (IList<Guid>) ids, (ServiceHostFilterFlags) filterFlags).ToArray();
        if (array != null && this.RequestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          foreach (TeamProjectCollectionProperties collectionProperties in array)
          {
            if (collectionProperties != null)
            {
              collectionProperties.DatabaseId = -1;
              collectionProperties.DatabaseCategoryConnectionStringsValue = (KeyValue<string, string>[]) null;
              collectionProperties.DefaultConnectionString = (string) null;
              collectionProperties.FrameworkConnectionInfo = (ISqlConnectionInfo) null;
            }
          }
        }
        return array;
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
