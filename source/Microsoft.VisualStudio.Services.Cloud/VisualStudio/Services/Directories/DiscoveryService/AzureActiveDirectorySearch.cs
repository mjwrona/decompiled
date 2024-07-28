// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.AzureActiveDirectorySearch
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal class AzureActiveDirectorySearch : IAzureActiveDirectorySearch
  {
    private readonly IAadServicePrincipalConfigurationHelper aadServicePrincipalConfigurationHelper;

    internal AzureActiveDirectorySearch()
      : this(AadServicePrincipalConfigurationHelper.Instance)
    {
    }

    internal AzureActiveDirectorySearch(
      IAadServicePrincipalConfigurationHelper aadServicePrincipalConfigurationHelper)
    {
      this.aadServicePrincipalConfigurationHelper = aadServicePrincipalConfigurationHelper;
    }

    DirectoryInternalSearchResponse IAzureActiveDirectorySearch.Search(
      IVssRequestContext context,
      DirectoryInternalSearchRequest request)
    {
      DirectoryInternalSearchResponse internalSearchResponse = new DirectoryInternalSearchResponse()
      {
        Results = (IList<IDirectoryEntity>) Array.Empty<IDirectoryEntity>()
      };
      if (AzureActiveDirectoryRequestFilter.AllowRequest(context, (DirectoryInternalRequest) request))
      {
        SortedSet<IDirectoryEntity> sortedSet = new SortedSet<IDirectoryEntity>(DirectoryEntityComparer.DefaultComparer);
        if (request.TypesToSearch.Contains<string>("User"))
          this.AddUsers(context, request, sortedSet);
        if (request.TypesToSearch.Contains<string>("Group"))
          this.AddGroups(context, request, sortedSet);
        if (request.TypesToSearch.Contains<string>("ServicePrincipal") && this.aadServicePrincipalConfigurationHelper.IsSearchingForServicePrincipalsEnabled(context))
          this.AddServicePrincipals(context, request, sortedSet);
        internalSearchResponse.Results = (IList<IDirectoryEntity>) sortedSet.ToList<IDirectoryEntity>();
        internalSearchResponse.PagingToken = (string) null;
      }
      return internalSearchResponse;
    }

    private void AddUsers(
      IVssRequestContext context,
      DirectoryInternalSearchRequest request,
      SortedSet<IDirectoryEntity> results)
    {
      AadService service = context.GetService<AadService>();
      GetUsersRequest request1 = new GetUsersRequest();
      string[] strArray = new string[1]{ request.Query };
      bool flag = false;
      foreach (string str in request.PropertiesToSearch)
      {
        switch (str)
        {
          case "DisplayName":
            request1.DisplayNamePrefixes = (IEnumerable<string>) strArray;
            flag = true;
            continue;
          case "Surname":
            request1.SurnamePrefixes = (IEnumerable<string>) strArray;
            flag = true;
            continue;
          case "Mail":
            request1.MailPrefixes = (IEnumerable<string>) strArray;
            flag = true;
            continue;
          case "MailNickname":
            request1.MailNicknamePrefixes = (IEnumerable<string>) strArray;
            flag = true;
            continue;
          case "SignInAddress":
            request1.UserPrincipalNamePrefixes = (IEnumerable<string>) strArray;
            flag = true;
            continue;
          default:
            continue;
        }
      }
      if (!flag)
        return;
      request1.MaxResults = new int?(request.MaxResults);
      request1.PagingToken = (string) null;
      foreach (AadUser user in service.GetUsers(context, request1).Users)
        results.Add((IDirectoryEntity) AzureActiveDirectoryEntityConverter.ConvertUser(context, user, request.PropertiesToReturn));
    }

    private void AddGroups(
      IVssRequestContext context,
      DirectoryInternalSearchRequest request,
      SortedSet<IDirectoryEntity> results)
    {
      AadService service = context.GetService<AadService>();
      GetGroupsRequest request1 = new GetGroupsRequest();
      string[] strArray = new string[1]{ request.Query };
      bool flag = false;
      foreach (string str in request.PropertiesToSearch)
      {
        switch (str)
        {
          case "DisplayName":
            request1.DisplayNamePrefixes = (IEnumerable<string>) strArray;
            flag = true;
            continue;
          case "MailNickname":
            request1.MailNicknamePrefixes = (IEnumerable<string>) strArray;
            flag = true;
            continue;
          case "Mail":
            request1.MailPrefixes = (IEnumerable<string>) strArray;
            flag = true;
            continue;
          default:
            continue;
        }
      }
      if (!flag)
        return;
      request1.MaxResults = new int?(request.MaxResults);
      request1.PagingToken = (string) null;
      foreach (AadGroup group in service.GetGroups(context, request1).Groups)
        results.Add((IDirectoryEntity) AzureActiveDirectoryEntityConverter.ConvertGroup(context, group, request.PropertiesToReturn));
    }

    private void AddServicePrincipals(
      IVssRequestContext context,
      DirectoryInternalSearchRequest request,
      SortedSet<IDirectoryEntity> results)
    {
      AadService service = context.GetService<AadService>();
      GetServicePrincipalsRequest request1 = new GetServicePrincipalsRequest();
      string[] strArray = new string[1]{ request.Query };
      bool flag = false;
      foreach (string str in request.PropertiesToSearch)
      {
        switch (str)
        {
          case "DisplayName":
            request1.DisplayNamePrefixes = (IEnumerable<string>) strArray;
            flag = true;
            continue;
          case "AppId":
            request1.AppIds = (IEnumerable<string>) strArray;
            flag = true;
            continue;
          default:
            continue;
        }
      }
      if (!flag)
        return;
      request1.MaxResults = new int?(request.MaxResults);
      request1.PagingToken = (string) null;
      foreach (AadServicePrincipal servicePrincipal in service.GetServicePrincipals(context, request1).ServicePrincipals)
        results.Add((IDirectoryEntity) AzureActiveDirectoryEntityConverter.ConvertServicePrincipal(context, servicePrincipal, request.PropertiesToReturn));
    }
  }
}
