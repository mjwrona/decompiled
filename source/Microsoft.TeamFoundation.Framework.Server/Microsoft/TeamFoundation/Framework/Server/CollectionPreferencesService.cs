// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CollectionPreferencesService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Organization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class CollectionPreferencesService : ICollectionPreferencesService, IVssFrameworkService
  {
    private const string c_preferencesRegistryPath = "/Configuration/CollectionPreferences";
    private static readonly RegistryQuery s_preferencesRegistryQuery = new RegistryQuery("/Configuration/CollectionPreferences", false);
    private const string c_area = "CollectionPreferencesService";
    private const string c_layer = "BusinessLogic";

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public CollectionPreferences GetCollectionPreferences(IVssRequestContext requestContext)
    {
      requestContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection);
      return !requestContext.ExecutionEnvironment.IsHostedDeployment ? (CollectionPreferences) null : this.LoadCollectionPreferences(requestContext);
    }

    public void SetCollectionPreferences(
      IVssRequestContext requestContext,
      CollectionPreferences collectionPreferences)
    {
      requestContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection);
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidOperationException("This service only works in hosted environment.");
      this.Validate(requestContext, collectionPreferences);
      this.SaveCollectionPreferences(requestContext, collectionPreferences);
    }

    internal virtual CollectionPreferences LoadCollectionPreferences(
      IVssRequestContext requestContext)
    {
      string str = requestContext.GetService<ICollectionService>().GetCollection(requestContext.Elevate(), (IEnumerable<string>) new string[1]
      {
        "SystemProperty.TimeZone"
      })?.Properties.GetValue<string>("SystemProperty.TimeZone", string.Empty);
      CollectionPreferences collectionPreferences = (CollectionPreferences) null;
      if (!string.IsNullOrEmpty(str))
      {
        try
        {
          TimeZoneInfo timeZoneInfo = JsonConvert.DeserializeObject<TimeZoneInfo>(str, (JsonConverter) new TimeZoneInfoConverter());
          collectionPreferences = new CollectionPreferences()
          {
            TimeZone = timeZoneInfo
          };
        }
        catch (Exception ex)
        {
          requestContext.TraceException(54445, nameof (CollectionPreferencesService), "BusinessLogic", ex);
        }
      }
      else
        collectionPreferences = new CollectionPreferences()
        {
          TimeZone = TimeZoneInfo.Utc
        };
      return collectionPreferences;
    }

    internal virtual void SaveCollectionPreferences(
      IVssRequestContext requestContext,
      CollectionPreferences collectionPreferences)
    {
      bool flag1 = !collectionPreferences.TimeZone.Equals(TimeZoneInfo.Utc);
      if (!flag1)
      {
        CollectionPreferences collectionPreferences1 = this.GetCollectionPreferences(requestContext);
        int num;
        if (collectionPreferences1 == null)
        {
          num = 0;
        }
        else
        {
          bool? nullable = collectionPreferences1.TimeZone?.Equals(TimeZoneInfo.Utc);
          bool flag2 = false;
          num = nullable.GetValueOrDefault() == flag2 & nullable.HasValue ? 1 : 0;
        }
        flag1 = num != 0;
      }
      if (!flag1)
        return;
      string str = JsonConvert.SerializeObject((object) collectionPreferences.TimeZone, (JsonConverter) new TimeZoneInfoConverter());
      requestContext.GetService<ICollectionService>().UpdateProperties(requestContext, new PropertyBag()
      {
        ["SystemProperty.TimeZone"] = (object) str
      });
    }

    internal void Validate(
      IVssRequestContext requestContext,
      CollectionPreferences collectionPreferences)
    {
      string str = nameof (collectionPreferences);
      ArgumentUtility.CheckForNull<CollectionPreferences>(collectionPreferences, str);
      CollectionPreferencesService.CheckSystemTimeZone(collectionPreferences.TimeZone, str, "TimeZone");
    }

    private static void CheckSystemTimeZone(
      TimeZoneInfo timeZone,
      string argumentName,
      string propertyName)
    {
      ArgumentUtility.CheckForNull<TimeZoneInfo>(timeZone, propertyName);
      string id = timeZone.Id;
      try
      {
        TimeZoneInfo.FindSystemTimeZoneById(id);
      }
      catch
      {
        throw new ArgumentException(FrameworkResources.InvalidSystemTimeZoneValue((object) propertyName), argumentName);
      }
    }
  }
}
