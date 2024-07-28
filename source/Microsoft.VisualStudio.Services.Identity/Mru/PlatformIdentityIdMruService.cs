// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Mru.PlatformIdentityIdMruService
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity.Mru
{
  internal class PlatformIdentityIdMruService : IIdentityIdMruService, IVssFrameworkService
  {
    public const string RemoveInvalidIdentitiesFeatureFlag = "VisualStudio.Services.MRU.RemoveInvalidIdentities";
    private Guid m_serviceHostId;
    private const string c_area = "Microsoft.VisualStudio.Services.Identity.Mru";
    private const string c_layer = "IdentityIdMruService";

    public void ServiceStart(IVssRequestContext context) => this.m_serviceHostId = context.ServiceHost.InstanceId;

    public void ServiceEnd(IVssRequestContext context)
    {
    }

    public void SetMruIdentities(
      IVssRequestContext context,
      Guid identityId,
      Guid containerId,
      IList<Guid> identityIds)
    {
      this.ValidateRequestContext(context);
      IdentityMruValidator.ValidateMruContainerId(containerId);
      IdentityMruValidator.ValidateMruValue(identityIds);
      PlatformIdentityIdMruService.CheckPermission(context, identityId, 2);
      IdentityMruValidator.ValidateIdentityExists(context, identityId);
      this.SetItems<Guid>(context, identityId, containerId, identityIds);
    }

    public IList<Guid> GetMruIdentities(
      IVssRequestContext context,
      Guid identityId,
      Guid containerId)
    {
      this.ValidateRequestContext(context);
      IdentityMruValidator.ValidateMruContainerId(containerId);
      PlatformIdentityIdMruService.CheckPermission(context, identityId, 1);
      IdentityMruValidator.ValidateIdentityExists(context, identityId);
      return this.GetItems<Guid>(context, identityId, containerId);
    }

    public void AddMruIdentities(
      IVssRequestContext context,
      Guid identityId,
      Guid containerId,
      IList<Guid> identityIds)
    {
      this.ValidateRequestContext(context);
      IdentityMruValidator.ValidateMruContainerId(containerId);
      IdentityMruValidator.ValidateMruValue(identityIds);
      PlatformIdentityIdMruService.CheckPermission(context, identityId, 2);
      IdentityMruValidator.ValidateIdentityExists(context, identityId);
      this.AddItems<Guid>(context, identityId, containerId, identityIds);
    }

    public IList<Guid> AddMruIdentitiesAndRemoveInactive(
      IVssRequestContext context,
      Guid identityId,
      Guid containerId,
      IList<Guid> identityIds)
    {
      this.ValidateRequestContext(context);
      IdentityMruValidator.ValidateMruContainerId(containerId);
      IdentityMruValidator.ValidateMruValue(identityIds);
      context.Trace(451208, TraceLevel.Info, "Microsoft.VisualStudio.Services.Identity.Mru", "IdentityIdMruService", "Updating MRU for {0} identity with following ids {1}", (object) identityId, (object) string.Join<Guid>(", ", (IEnumerable<Guid>) identityIds));
      PlatformIdentityIdMruService.CheckPermission(context, identityId, 2);
      IdentityMruValidator.ValidateIdentityExists(context, identityId);
      IList<Guid> inactiveInMru = (IList<Guid>) new List<Guid>();
      if (context.IsFeatureEnabled("VisualStudio.Services.MRU.RemoveInvalidIdentities"))
      {
        IList<Guid> mruIdentities = this.GetMruIdentities(context, identityId, containerId);
        List<Guid> guidList = (mruIdentities != null ? mruIdentities.Select<Guid, Guid>((Func<Guid, Guid>) (id => id)).ToList<Guid>() : (List<Guid>) null) ?? new List<Guid>();
        context.Trace(451208, TraceLevel.Info, "Microsoft.VisualStudio.Services.Identity.Mru", "IdentityIdMruService", "{0} identity has following MRU values {1}", (object) identityId, (object) string.Join<Guid>(", ", (IEnumerable<Guid>) guidList));
        IdentityService service = context.GetService<IdentityService>();
        IEnumerable<Guid> source = identityIds.Intersect<Guid>((IEnumerable<Guid>) guidList);
        inactiveInMru = (IList<Guid>) service.ReadIdentities(context, (IList<Guid>) source.ToArray<Guid>(), QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity != null && !identity.IsActive)).Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (identity => identity.Id)).ToList<Guid>();
        context.Trace(451208, TraceLevel.Info, "Microsoft.VisualStudio.Services.Identity.Mru", "IdentityIdMruService", "{0} identity has following inactive MRU values {1}", (object) identityId, (object) string.Join<Guid>(", ", (IEnumerable<Guid>) inactiveInMru));
      }
      if (inactiveInMru.Any<Guid>())
      {
        this.RemoveItems<Guid>(context, identityId, containerId, inactiveInMru);
        context.Trace(451208, TraceLevel.Info, "Microsoft.VisualStudio.Services.Identity.Mru", "IdentityIdMruService", "{0} identity has following MRU values removed {1}", (object) identityId, (object) string.Join<Guid>(", ", (IEnumerable<Guid>) inactiveInMru));
        identityIds = (IList<Guid>) identityIds.Where<Guid>((Func<Guid, bool>) (id => !inactiveInMru.Contains(id))).ToList<Guid>();
      }
      this.AddItems<Guid>(context, identityId, containerId, identityIds);
      context.Trace(451208, TraceLevel.Info, "Microsoft.VisualStudio.Services.Identity.Mru", "IdentityIdMruService", "{0} identity has following MRU values added {1}", (object) identityId, (object) string.Join<Guid>(", ", (IEnumerable<Guid>) identityIds));
      return identityIds;
    }

    public void RemoveMruIdentities(
      IVssRequestContext context,
      Guid identityId,
      Guid containerId,
      IList<Guid> identityIds)
    {
      this.ValidateRequestContext(context);
      IdentityMruValidator.ValidateMruContainerId(containerId);
      IdentityMruValidator.ValidateMruValue(identityIds);
      PlatformIdentityIdMruService.CheckPermission(context, identityId, 2);
      IdentityMruValidator.ValidateIdentityExists(context, identityId);
      this.RemoveItems<Guid>(context, identityId, containerId, identityIds);
    }

    private void SetItems<T>(
      IVssRequestContext context,
      Guid identityId,
      Guid containerId,
      IList<T> items,
      IEqualityComparer<T> itemEqualityComparer = null)
    {
      try
      {
        items = (IList<T>) PlatformIdentityIdMruService.GetDedupedItems<T>(items, itemEqualityComparer);
        ArtifactSpec artifactSpec = PlatformIdentityIdMruService.CreateArtifactSpec(identityId);
        PropertyValue propertyValue = new PropertyValue(PlatformIdentityIdMruService.CreatePropertyName(containerId), (object) items.Serialize<IList<T>>());
        PlatformIdentityIdMruService.PropertyServiceHelper.SetProperties(context, artifactSpec, (IEnumerable<PropertyValue>) new PropertyValue[1]
        {
          propertyValue
        });
      }
      catch (Exception ex)
      {
        if (!(ex is IdentityMruResourceExistsException))
          PlatformIdentityIdMruService.TraceException(context, ex, 451108);
        throw;
      }
    }

    private IList<T> GetItems<T>(IVssRequestContext context, Guid identityId, Guid containerId)
    {
      try
      {
        ArtifactSpec artifactSpec = PlatformIdentityIdMruService.CreateArtifactSpec(identityId);
        string propertyName = PlatformIdentityIdMruService.CreatePropertyName(containerId);
        IList<T> objList;
        return PlatformIdentityIdMruService.TryParse<T>(PlatformIdentityIdMruService.PropertyServiceHelper.GetProperty(context, artifactSpec, propertyName), out objList) ? objList : (IList<T>) null;
      }
      catch (Exception ex)
      {
        PlatformIdentityIdMruService.TraceException(context, ex, 451118);
        throw;
      }
    }

    private void AddItems<T>(
      IVssRequestContext context,
      Guid identityId,
      Guid containerId,
      IList<T> items,
      IEqualityComparer<T> itemEqualityComparer = null)
    {
      if (!items.Any<T>())
        return;
      try
      {
        IdentityMruSettings.IdentityMruServiceSettings settings = PlatformIdentityIdMruService.GetSettings(context);
        items = (IList<T>) PlatformIdentityIdMruService.GetDedupedItems<T>(items, itemEqualityComparer);
        List<T> list = items.Take<T>(settings.MaxMruSize).ToList<T>();
        IList<T> mergedItems = (IList<T>) list;
        int remainingSpots = settings.MaxMruSize - list.Count;
        if (remainingSpots > 0)
        {
          IList<T> items1 = this.GetItems<T>(context, identityId, containerId);
          PlatformIdentityIdMruService.ComputeMergedMru<T>((IList<T>) list, items1, remainingSpots, itemEqualityComparer, out mergedItems);
        }
        ArtifactSpec artifactSpec = PlatformIdentityIdMruService.CreateArtifactSpec(identityId);
        PropertyValue propertyValue = new PropertyValue(PlatformIdentityIdMruService.CreatePropertyName(containerId), (object) mergedItems.Serialize<IList<T>>());
        PlatformIdentityIdMruService.PropertyServiceHelper.SetProperties(context, artifactSpec, (IEnumerable<PropertyValue>) new PropertyValue[1]
        {
          propertyValue
        });
      }
      catch (Exception ex)
      {
        PlatformIdentityIdMruService.TraceException(context, ex, 451128);
        throw;
      }
    }

    private void RemoveItems<T>(
      IVssRequestContext context,
      Guid identityId,
      Guid containerId,
      IList<T> items,
      IEqualityComparer<T> itemEqualityComparer = null)
    {
      if (!items.Any<T>())
        return;
      try
      {
        IList<T> items1 = this.GetItems<T>(context, identityId, containerId);
        items = (IList<T>) PlatformIdentityIdMruService.GetDedupedItems<T>(items, itemEqualityComparer);
        IList<T> remainingItems;
        if (!PlatformIdentityIdMruService.ComputeRemainingItems<T>(items, items1, itemEqualityComparer, out remainingItems))
          return;
        ArtifactSpec artifactSpec = PlatformIdentityIdMruService.CreateArtifactSpec(identityId);
        PropertyValue propertyValue = new PropertyValue(PlatformIdentityIdMruService.CreatePropertyName(containerId), (object) remainingItems.Serialize<IList<T>>());
        PlatformIdentityIdMruService.PropertyServiceHelper.SetProperties(context, artifactSpec, (IEnumerable<PropertyValue>) new PropertyValue[1]
        {
          propertyValue
        });
      }
      catch (Exception ex)
      {
        PlatformIdentityIdMruService.TraceException(context, ex, 451138);
        throw;
      }
    }

    private void ValidateRequestContext(IVssRequestContext requestContext) => requestContext.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);

    private static void CheckPermission(
      IVssRequestContext context,
      Guid identityId,
      int permission)
    {
      if (permission == 1 && IdentityMruValidator.IsRequestForSelf(context, identityId))
        return;
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
      bool flag;
      if (vssRequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        flag = vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4);
      }
      else
      {
        IVssSecurityNamespace securityNamespace = vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.IdentitiesNamespaceId);
        string str = vssRequestContext.ServiceHost.InstanceId.ToString();
        IVssRequestContext requestContext = vssRequestContext;
        string token = str;
        int requestedPermissions = permission;
        flag = securityNamespace.HasPermission(requestContext, token, requestedPermissions);
      }
      if (!flag)
        throw new IdentityMruUnauthorizedException(FrameworkResources.UnauthorizedToAccessResource());
    }

    private static ArtifactSpec CreateArtifactSpec(Guid identityId) => new ArtifactSpec(IdentityMruConstants.PropertyArtifactKind, identityId.ToByteArray(), 0);

    private static string CreatePropertyName(Guid containerId) => containerId.ToString();

    private static IdentityMruSettings.IdentityMruServiceSettings GetSettings(
      IVssRequestContext context)
    {
      return context.To(TeamFoundationHostType.Deployment).GetService<IdentityMruSettings>().Settings;
    }

    private static bool TryParse<T>(PropertyValue propertyValue, out IList<T> value)
    {
      value = (IList<T>) null;
      if (propertyValue == null || propertyValue.Value == null)
        return false;
      value = JsonUtilities.Deserialize<IList<T>>(propertyValue.Value as string);
      return true;
    }

    private static void ComputeMergedMru<T>(
      IList<T> newItems,
      IList<T> existingItems,
      int remainingSpots,
      IEqualityComparer<T> itemEqualityComparer,
      out IList<T> mergedItems)
    {
      mergedItems = (IList<T>) new List<T>((IEnumerable<T>) newItems);
      if (existingItems.IsNullOrEmpty<T>())
        return;
      HashSet<T> dedupedNewItems = new HashSet<T>((IEnumerable<T>) newItems, itemEqualityComparer);
      foreach (T obj in existingItems.Where<T>((Func<T, bool>) (x => !dedupedNewItems.Contains(x))))
      {
        mergedItems.Add(obj);
        --remainingSpots;
        if (remainingSpots <= 0)
          break;
      }
    }

    private static bool ComputeRemainingItems<T>(
      IList<T> itemsToDelete,
      IList<T> existingItems,
      IEqualityComparer<T> itemEqualityComparer,
      out IList<T> remainingItems)
    {
      remainingItems = existingItems ?? (IList<T>) new List<T>();
      if (existingItems.IsNullOrEmpty<T>())
        return true;
      HashSet<T> itemsToDeleteSet = new HashSet<T>((IEnumerable<T>) itemsToDelete, itemEqualityComparer);
      remainingItems = (IList<T>) existingItems.Where<T>((Func<T, bool>) (x => !itemsToDeleteSet.Contains(x))).ToList<T>();
      return true;
    }

    private static List<T> GetDedupedItems<T>(
      IList<T> items,
      IEqualityComparer<T> itemEqualityComparer)
    {
      return items.Distinct<T>(itemEqualityComparer).ToList<T>();
    }

    private static void TraceException(IVssRequestContext context, Exception ex, int tracePoint) => context.TraceException(tracePoint, "Microsoft.VisualStudio.Services.Identity.Mru", "IdentityIdMruService", ex);

    private static class PropertyServiceHelper
    {
      private const string c_area = "Microsoft.VisualStudio.Services.Identity.Mru";
      private const string c_layer = "IdentityMruPropertyHelper";

      private static IEnumerable<PropertyValue> GetProperties(
        IVssRequestContext context,
        ArtifactSpec spec,
        IEnumerable<string> propertyNames)
      {
        using (TeamFoundationDataReader propertyReader = context.GetService<ITeamFoundationPropertyService>().GetProperties(context, spec, propertyNames))
        {
          foreach (ArtifactPropertyValue artifactPropertyValue in propertyReader)
          {
            foreach (PropertyValue propertyValue in artifactPropertyValue.PropertyValues)
              yield return propertyValue;
          }
        }
      }

      internal static PropertyValue GetProperty(
        IVssRequestContext context,
        ArtifactSpec spec,
        string propertyName)
      {
        List<PropertyValue> list = PlatformIdentityIdMruService.PropertyServiceHelper.GetProperties(context, spec, (IEnumerable<string>) new string[1]
        {
          propertyName
        }).ToList<PropertyValue>();
        if (list.Count <= 1)
          return list.FirstOrDefault<PropertyValue>();
        context.Trace(451208, TraceLevel.Error, "Microsoft.VisualStudio.Services.Identity.Mru", "IdentityMruPropertyHelper", "Was expecting one property but found {0} matches for artifact spec {1}", (object) list.Count, (object) spec);
        return (PropertyValue) null;
      }

      internal static void SetProperties(
        IVssRequestContext context,
        ArtifactSpec spec,
        IEnumerable<PropertyValue> propertyValues)
      {
        context.GetService<ITeamFoundationPropertyService>().SetProperties(context, spec, propertyValues);
      }
    }
  }
}
