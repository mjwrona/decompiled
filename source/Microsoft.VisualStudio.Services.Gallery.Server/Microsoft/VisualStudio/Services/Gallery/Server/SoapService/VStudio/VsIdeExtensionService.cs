// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SoapService.VStudio.VsIdeExtensionService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Galleries.Domain.Model;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Galleries;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using VsGallery.WebServices;

namespace Microsoft.VisualStudio.Services.Gallery.Server.SoapService.VStudio
{
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
  public class VsIdeExtensionService : VisualStudioWcfServiceBase, IVsIdeService
  {
    protected VisualStudioIdeVersion DefaultVsIdeVersion;

    public VsIdeExtensionService() => this.DefaultVsIdeVersion = VisualStudioIdeVersion.Dev12;

    public VsIdeExtensionService(VisualStudioIdeVersion defaultVsIdeVersion) => this.DefaultVsIdeVersion = defaultVsIdeVersion;

    public ICollection<IdeCategory> GetRootCategories(IDictionary<string, string> requestContext)
    {
      try
      {
        VisualStudioIdeVersion clientVsIdeVersion = this.GetClientVsIdeVersion(requestContext, (string) null, this.DefaultVsIdeVersion);
        MethodInformation methodInformation = new MethodInformation("VisualStudio.GetRootCategories2." + clientVsIdeVersion.ToString(), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (requestContext), (object) requestContext);
        this.EnterMethod(methodInformation);
        return this.RequestContext.GetService<IVisualStudioApiService>().GetRootCategories(this.RequestContext, clientVsIdeVersion, requestContext);
      }
      catch (ArgumentException ex)
      {
        WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
        return (ICollection<IdeCategory>) null;
      }
      catch (MethodNotAvailableException ex)
      {
        WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.ServiceUnavailable;
        return (ICollection<IdeCategory>) null;
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

    public ICollection<IdeCategory> GetRootCategories(string cultureName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation("VisualStudio.GetRootCategories." + this.DefaultVsIdeVersion.ToString(), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (cultureName), (object) cultureName);
        this.EnterMethod(methodInformation);
        return this.RequestContext.GetService<IVisualStudioApiService>().GetRootCategories(this.RequestContext, this.DefaultVsIdeVersion, cultureName);
      }
      catch (MethodNotAvailableException ex)
      {
        WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.ServiceUnavailable;
        return (ICollection<IdeCategory>) null;
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

    public IdeCategory GetCategoryTree(
      Guid categoryId,
      int level,
      string projectType,
      string templateType,
      string[] skus,
      string[] subSkus,
      int[] templateGroupIds,
      int[] vsVersions,
      string cultureName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation("VisualStudio.GetCategoryTree" + this.DefaultVsIdeVersion.ToString(), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (categoryId), (object) categoryId);
        methodInformation.AddParameter(nameof (level), (object) level);
        methodInformation.AddParameter(nameof (projectType), (object) projectType);
        methodInformation.AddParameter(nameof (templateType), (object) templateType);
        methodInformation.AddParameter(nameof (skus), (object) skus);
        methodInformation.AddParameter(nameof (subSkus), (object) cultureName);
        methodInformation.AddParameter(nameof (templateGroupIds), (object) templateGroupIds);
        methodInformation.AddParameter(nameof (vsVersions), (object) vsVersions);
        methodInformation.AddParameter(nameof (cultureName), (object) cultureName);
        this.EnterMethod(methodInformation);
        return this.RequestContext.GetService<IVisualStudioApiService>().GetCategoryTree(this.RequestContext, this.DefaultVsIdeVersion, categoryId, level, projectType, templateType, skus, subSkus, templateGroupIds, vsVersions, cultureName);
      }
      catch (MethodNotAvailableException ex)
      {
        WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.ServiceUnavailable;
        return (IdeCategory) null;
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

    public IdeCategory GetCategoryTree(
      Guid categoryId,
      int level,
      IDictionary<string, string> requestContext)
    {
      try
      {
        VisualStudioIdeVersion clientVsIdeVersion = this.GetClientVsIdeVersion(requestContext, (string) null, this.DefaultVsIdeVersion);
        MethodInformation methodInformation = new MethodInformation("VisualStudio.GetCategoryTree2." + clientVsIdeVersion.ToString(), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (categoryId), (object) categoryId);
        methodInformation.AddParameter(nameof (level), (object) level);
        methodInformation.AddParameter(nameof (requestContext), (object) requestContext);
        this.EnterMethod(methodInformation);
        return this.RequestContext.GetService<IVisualStudioApiService>().GetCategoryTree(this.RequestContext, clientVsIdeVersion, categoryId, level, requestContext);
      }
      catch (ArgumentException ex)
      {
        WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
        return (IdeCategory) null;
      }
      catch (MethodNotAvailableException ex)
      {
        WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.ServiceUnavailable;
        return (IdeCategory) null;
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

    public ReleaseQueryResult SearchReleases(
      string searchText,
      string whereClause,
      string orderByClause,
      int? locale,
      int? skip,
      int? take)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation("VisualStudio." + this.GetSearchPurpose(searchText, whereClause) + "." + this.DefaultVsIdeVersion.ToString(), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (searchText), (object) searchText);
        methodInformation.AddParameter(nameof (whereClause), (object) whereClause);
        methodInformation.AddParameter(nameof (orderByClause), (object) orderByClause);
        methodInformation.AddParameter(nameof (locale), (object) locale);
        methodInformation.AddParameter(nameof (skip), (object) skip);
        methodInformation.AddParameter(nameof (take), (object) take);
        this.EnterMethod(methodInformation);
        return this.RequestContext.GetService<IVisualStudioApiService>().SearchReleases(this.RequestContext, this.DefaultVsIdeVersion, searchText, whereClause, orderByClause, locale, skip, take);
      }
      catch (MethodNotAvailableException ex)
      {
        WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.ServiceUnavailable;
        return (ReleaseQueryResult) null;
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

    public ReleaseQueryResult SearchReleases(
      string searchText,
      string whereClause,
      string orderByClause,
      int? skip,
      int? take,
      IDictionary<string, string> requestContext)
    {
      try
      {
        VisualStudioIdeVersion clientVsIdeVersion = this.GetClientVsIdeVersion(requestContext, whereClause, this.DefaultVsIdeVersion);
        MethodInformation methodInformation = new MethodInformation("VisualStudio." + this.GetSearchPurpose(searchText, whereClause) + "." + clientVsIdeVersion.ToString(), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (searchText), (object) searchText);
        methodInformation.AddParameter(nameof (whereClause), (object) whereClause);
        methodInformation.AddParameter(nameof (orderByClause), (object) orderByClause);
        methodInformation.AddParameter(nameof (skip), (object) skip);
        methodInformation.AddParameter(nameof (take), (object) take);
        methodInformation.AddParameter(nameof (requestContext), (object) requestContext);
        this.EnterMethod(methodInformation);
        return this.RequestContext.GetService<IVisualStudioApiService>().SearchReleases(this.RequestContext, clientVsIdeVersion, searchText, whereClause, orderByClause, skip, take, requestContext);
      }
      catch (ArgumentException ex)
      {
        WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
        return (ReleaseQueryResult) null;
      }
      catch (MethodNotAvailableException ex)
      {
        WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.ServiceUnavailable;
        return (ReleaseQueryResult) null;
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

    public string[] GetCurrentVersionsForVsixList(
      string[] vsixIds,
      IDictionary<string, string> requestContext)
    {
      try
      {
        VisualStudioIdeVersion clientVsIdeVersion = this.GetClientVsIdeVersion(requestContext, (string) null, this.DefaultVsIdeVersion);
        MethodInformation methodInformation = new MethodInformation("VisualStudio.GetCurrentVersionsForVsixList." + clientVsIdeVersion.ToString(), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (vsixIds), (object) vsixIds);
        methodInformation.AddParameter(nameof (requestContext), (object) requestContext);
        this.EnterMethod(methodInformation);
        return this.RequestContext.GetService<IVisualStudioApiService>().GetCurrentVersionsForVsixList(this.RequestContext, clientVsIdeVersion, vsixIds, requestContext);
      }
      catch (ArgumentException ex)
      {
        WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
        return (string[]) null;
      }
      catch (MethodNotAvailableException ex)
      {
        WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.ServiceUnavailable;
        return (string[]) null;
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
