// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ContributedServiceContextData
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [GenerateAllConstants(null)]
  public static class ContributedServiceContextData
  {
    public const string ContributedServiceContextDataKey = "WebPlatform.ContributedServices";
    public const string ContributedServiceDataProviderType = "ServiceContext";

    [Obsolete("Use an IExtensionDataProvider contribution with a 'dataType' property set to 'ServiceContext'.", false)]
    public static void AddContributedServiceContext(
      Dictionary<string, object> data,
      ContributedServiceContext context)
    {
      IList<ContributedServiceContext> contributedServiceContextList = ContributedServiceContextData.GetDataProviderValue<IList<ContributedServiceContext>>(data, "WebPlatform.ContributedServices");
      if (contributedServiceContextList == null)
      {
        contributedServiceContextList = (IList<ContributedServiceContext>) new List<ContributedServiceContext>();
        data.Add("WebPlatform.ContributedServices", (object) contributedServiceContextList);
      }
      contributedServiceContextList.Add(context);
    }

    public static List<ContributedServiceContext> GetContributedServiceContexts(
      DataProviderResult result,
      IEnumerable<Contribution> contributions)
    {
      List<ContributedServiceContext> contexts = new List<ContributedServiceContext>();
      IEnumerable<object> dataProviderValue = ContributedServiceContextData.GetDataProviderValue<IEnumerable<object>>(result.Data, "WebPlatform.ContributedServices");
      if (dataProviderValue != null)
      {
        foreach (object rawValue in dataProviderValue)
          ContributedServiceContextData.AddServiceContextFromRawDataValue(rawValue, contexts);
      }
      foreach (Contribution contribution in contributions)
      {
        if (string.Equals(contribution.GetProperty<string>("dataType"), "ServiceContext", StringComparison.OrdinalIgnoreCase))
          ContributedServiceContextData.AddServiceContextFromRawDataValue(ContributedServiceContextData.GetDataProviderValue<object>(result.Data, contribution.Id), contexts);
      }
      return contexts;
    }

    private static T GetDataProviderValue<T>(Dictionary<string, object> data, string key)
    {
      object obj1;
      return data.TryGetValue(key, out obj1) && obj1 is T obj2 ? obj2 : default (T);
    }

    private static void AddServiceContextFromRawDataValue(
      object rawValue,
      List<ContributedServiceContext> contexts)
    {
      if (rawValue == null)
        return;
      if (!(rawValue is ContributedServiceContext contributedServiceContext))
        contributedServiceContext = JObject.FromObject(rawValue).ToObject<ContributedServiceContext>();
      if (contributedServiceContext == null)
        return;
      contexts.Add(contributedServiceContext);
    }
  }
}
