// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.FirstPartyPublisherAccessService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.Types.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class FirstPartyPublisherAccessService : 
    IFirstPartyPublisherAccessService,
    IVssFrameworkService
  {
    private ConcurrentDictionary<string, IList<Guid>> m_accessCache;
    private static readonly string s_publisherAccessAllowedlistRegistryRoot = "/Configuration/Service/Gallery/InternalPublisherAccessAllowedlist/";
    private static readonly string s_allowedList = "InternalPublisherAccessAllowedlist/";
    private static readonly string s_area = "Gallery";
    private static readonly string s_layer = "MSIdentityAccessService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged), FirstPartyPublisherAccessService.s_publisherAccessAllowedlistRegistryRoot + "...");
      this.BuildAccessCache(systemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged));

    public bool CheckAtMicrosftDotComAccessIfRequired(
      IVssRequestContext requestContext,
      Publisher publisher)
    {
      using (requestContext.TraceBlock(12062080, 12062080, FirstPartyPublisherAccessService.s_area, FirstPartyPublisherAccessService.s_layer, nameof (CheckAtMicrosftDotComAccessIfRequired)))
      {
        if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.MsIdentityCheck"))
          return true;
        IList<Guid> guidList;
        if (this.m_accessCache.TryGetValue(publisher.PublisherName, out guidList))
        {
          requestContext.Trace(12062080, TraceLevel.Info, FirstPartyPublisherAccessService.s_area, FirstPartyPublisherAccessService.s_layer, "Publisher name " + publisher.PublisherName + " found in access cache");
          Guid userVsid = GalleryServerUtil.GetUserVsid(requestContext);
          if (guidList.Contains(userVsid))
          {
            requestContext.Trace(12062080, TraceLevel.Info, FirstPartyPublisherAccessService.s_area, FirstPartyPublisherAccessService.s_layer, string.Format("User VSID {0} found in allowed VSIDs", (object) userVsid));
            return true;
          }
          requestContext.Trace(12062080, TraceLevel.Info, FirstPartyPublisherAccessService.s_area, FirstPartyPublisherAccessService.s_layer, string.Format("User VSID {0} not found in allowed VSIDs", (object) userVsid));
        }
        return !this.IsMicrosoftPublisher(requestContext, publisher) || this.IsMicrosoftEmployee(requestContext, publisher, (PublishedExtension) null);
      }
    }

    public bool IsMicrosoftEmployee(
      IVssRequestContext requestContext,
      Publisher publisher,
      PublishedExtension existingExtension)
    {
      using (requestContext.TraceBlock(12062080, 12062080, FirstPartyPublisherAccessService.s_area, FirstPartyPublisherAccessService.s_layer, nameof (IsMicrosoftEmployee)))
      {
        if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.MsIdentityCheck"))
        {
          ExtensionMetadata extensionMetadata = existingExtension?.Metadata.Find((Predicate<ExtensionMetadata>) (x => x.Key == "IsPublishedByMsEmployeeOnVsGallery"));
          if (extensionMetadata != null && bool.Parse(extensionMetadata.Value))
          {
            requestContext.Trace(12062080, TraceLevel.Info, FirstPartyPublisherAccessService.s_area, FirstPartyPublisherAccessService.s_layer, "Extension metadata contains published by MS employee");
            return true;
          }
          bool flag1 = false;
          bool flag2 = this.IsInternalEmployee(requestContext);
          requestContext.Trace(12062080, TraceLevel.Info, FirstPartyPublisherAccessService.s_area, FirstPartyPublisherAccessService.s_layer, string.Format("isInternalUser is {0}", (object) flag2));
          IGalleryAdminAuthorizer extension = requestContext.GetExtension<IGalleryAdminAuthorizer>();
          if (extension != null)
          {
            try
            {
              flag1 = extension.IsDomainMSTenant(requestContext);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(12061121, FirstPartyPublisherAccessService.s_area, FirstPartyPublisherAccessService.s_layer, ex);
            }
          }
          requestContext.Trace(12062080, TraceLevel.Info, FirstPartyPublisherAccessService.s_area, FirstPartyPublisherAccessService.s_layer, string.Format("isDomainMSTenant is {0}", (object) flag1));
          return flag2 & flag1;
        }
        ExtensionMetadata extensionMetadata1 = existingExtension?.Metadata.Find((Predicate<ExtensionMetadata>) (x => x.Key == "IsPublishedByMsEmployeeOnVsGallery"));
        if (extensionMetadata1 == null || !bool.Parse(extensionMetadata1.Value))
          return this.IsMicrosoftPublisher(requestContext, publisher);
        requestContext.Trace(12062080, TraceLevel.Info, FirstPartyPublisherAccessService.s_area, FirstPartyPublisherAccessService.s_layer, "Extension metadata contains published by MS employee");
        return true;
      }
    }

    public bool IsMicrosoftPublisher(IVssRequestContext requestContext, Publisher publisher)
    {
      using (requestContext.TraceBlock(12062080, 12062080, FirstPartyPublisherAccessService.s_area, FirstPartyPublisherAccessService.s_layer, nameof (IsMicrosoftPublisher)))
      {
        bool flag = publisher.DisplayName.ToUpperInvariant().Contains("Microsoft".Trim().ToUpperInvariant());
        requestContext.Trace(12062080, TraceLevel.Info, FirstPartyPublisherAccessService.s_area, FirstPartyPublisherAccessService.s_layer, string.Format("isMicrosoftPublisher is {0}", (object) flag));
        return flag;
      }
    }

    public bool IsInternalEmployee(IVssRequestContext requestContext)
    {
      using (requestContext.TraceBlock(12062080, 12062080, FirstPartyPublisherAccessService.s_area, FirstPartyPublisherAccessService.s_layer, nameof (IsInternalEmployee)))
      {
        bool flag = false;
        Microsoft.VisualStudio.Services.Identity.Identity authenticatedIdentity = requestContext.GetAuthenticatedIdentity();
        if (authenticatedIdentity != null)
          flag = authenticatedIdentity.GetProperty<string>("Mail", string.Empty).ToUpperInvariant().EndsWith("MICROSOFT.COM");
        requestContext.Trace(12062080, TraceLevel.Info, FirstPartyPublisherAccessService.s_area, FirstPartyPublisherAccessService.s_layer, string.Format("isInternalUser is {0}", (object) flag));
        return flag;
      }
    }

    public bool IsInternalPartner(IVssRequestContext requestContext)
    {
      using (requestContext.TraceBlock(12062080, 12062080, FirstPartyPublisherAccessService.s_area, FirstPartyPublisherAccessService.s_layer, nameof (IsInternalPartner)))
      {
        bool flag = false;
        Microsoft.VisualStudio.Services.Identity.Identity authenticatedIdentity = requestContext.GetAuthenticatedIdentity();
        if (authenticatedIdentity != null)
          flag = authenticatedIdentity.GetProperty<string>("Mail", string.Empty).ToUpperInvariant().EndsWith("ONMICROSOFT.COM");
        requestContext.Trace(12062080, TraceLevel.Info, FirstPartyPublisherAccessService.s_area, FirstPartyPublisherAccessService.s_layer, string.Format("isInternalPartner is {0}", (object) flag));
        return flag;
      }
    }

    private void OnRegistrySettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.BuildAccessCache(requestContext);
    }

    private void BuildAccessCache(IVssRequestContext requestContext)
    {
      using (requestContext.TraceBlock(12062080, 12062080, FirstPartyPublisherAccessService.s_area, FirstPartyPublisherAccessService.s_layer, "IsMicrosoftPublisher"))
      {
        this.m_accessCache = new ConcurrentDictionary<string, IList<Guid>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        RegistryEntryCollection collection1 = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) (FirstPartyPublisherAccessService.s_publisherAccessAllowedlistRegistryRoot + "**"));
        requestContext.Trace(12062080, TraceLevel.Info, FirstPartyPublisherAccessService.s_area, FirstPartyPublisherAccessService.s_layer, string.Format("entryCollection has {0} entries", (object) collection1.Count));
        if (collection1.Count <= 0)
          return;
        collection1.ForEach<RegistryEntry>((Action<RegistryEntry>) (entry =>
        {
          string str = entry.Path.TrimEnd('/');
          string publisherName = str.Substring(str.IndexOf(FirstPartyPublisherAccessService.s_allowedList) + FirstPartyPublisherAccessService.s_allowedList.Length);
          IList<Guid> vsidList = this.m_accessCache.GetOrAdd(publisherName, (IList<Guid>) new List<Guid>());
          string[] collection2 = entry.Value.Split(';');
          requestContext.Trace(12062080, TraceLevel.Info, FirstPartyPublisherAccessService.s_area, FirstPartyPublisherAccessService.s_layer, string.Format("{0} entries present in registry for {1}", (object) collection2.Length, (object) publisherName));
          if (collection2.Length == 0)
            return;
          ((IEnumerable<string>) collection2).ForEach<string>((Action<string>) (vsid =>
          {
            Guid result;
            if (Guid.TryParse(vsid, out result))
            {
              requestContext.Trace(12062080, TraceLevel.Info, FirstPartyPublisherAccessService.s_area, FirstPartyPublisherAccessService.s_layer, "Added VSID " + vsid + " to cache for " + publisherName);
              vsidList.Add(result);
            }
            else
              requestContext.TraceAlways(12062080, TraceLevel.Error, FirstPartyPublisherAccessService.s_area, FirstPartyPublisherAccessService.s_layer, "Failed to parse {0} into GUID", (object) vsid);
          }));
        }));
      }
    }
  }
}
