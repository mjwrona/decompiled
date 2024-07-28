// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.Security.BackingStoreNamespaceBaseController
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.Security.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud.Security
{
  public abstract class BackingStoreNamespaceBaseController : SecurityBackingStoreController
  {
    private const string c_area = "BackingStoreNamespaceBase";
    private const string c_layer = "BackingStoreAccessControlEntriesController";
    private const string AclsWithSubjectInVsidFormatCanBeReturned = "VisualStudio.Services.AclsWithSubjectInVsidFormatCanBeReturned";

    protected SecurityNamespaceDataCollection QuerySecurityData(
      Guid securityNamespaceId,
      Guid aclStoreId = default (Guid),
      int oldSequenceId = -1,
      bool useVsidSubjects = false)
    {
      ArgumentUtility.CheckForEmptyGuid(securityNamespaceId, nameof (securityNamespaceId));
      ArgumentUtility.CheckForOutOfRange(oldSequenceId, nameof (oldSequenceId), -1);
      SecurityBackingStoreController.SbsSecurity.CheckPermission(this.TfsRequestContext, securityNamespaceId, false);
      LocalSecurityNamespace securityNamespace = this.TfsRequestContext.GetService<LocalSecurityService>().GetSecurityNamespace(this.TfsRequestContext, securityNamespaceId);
      if (securityNamespace == null || !securityNamespace.Description.IsRemotable)
        throw new InvalidSecurityNamespaceException(securityNamespaceId);
      if (!this.TfsRequestContext.To(TeamFoundationHostType.Deployment).GetService<SecuritySettingsService>().Settings.AllowDescriptorResponsesFromSecurityBackingStore)
      {
        useVsidSubjects = true;
        this.TfsRequestContext.Trace(50005, TraceLevel.Info, "BackingStoreNamespaceBase", "BackingStoreAccessControlEntriesController", "Allow descriptor responses from security backing store");
      }
      if (Guid.Empty == aclStoreId)
      {
        oldSequenceId = -1;
        List<Microsoft.VisualStudio.Services.Security.SecurityNamespaceData> source = new List<Microsoft.VisualStudio.Services.Security.SecurityNamespaceData>();
        foreach (ISecurityNamespaceBackingStore backingStore in securityNamespace.GetRemotableAclStores(this.TfsRequestContext).Values.Select<CachingAclStore, ISecurityNamespaceBackingStore>((Func<CachingAclStore, ISecurityNamespaceBackingStore>) (s => s.BackingStore)))
        {
          Microsoft.VisualStudio.Services.Security.SecurityNamespaceData securityNamespaceData = this.QuerySecurityDataForAclStore(backingStore, oldSequenceId, useVsidSubjects);
          source.Add(securityNamespaceData);
        }
        return new SecurityNamespaceDataCollection((IList<Microsoft.VisualStudio.Services.Security.SecurityNamespaceData>) source);
      }
      return new SecurityNamespaceDataCollection((IList<Microsoft.VisualStudio.Services.Security.SecurityNamespaceData>) new Microsoft.VisualStudio.Services.Security.SecurityNamespaceData[1]
      {
        this.QuerySecurityDataForAclStore(this.GetAclStore(securityNamespace, aclStoreId).BackingStore, oldSequenceId, useVsidSubjects)
      });
    }

    private Microsoft.VisualStudio.Services.Security.SecurityNamespaceData QuerySecurityDataForAclStore(
      ISecurityNamespaceBackingStore backingStore,
      int oldSequenceId,
      bool useVsidSubjects)
    {
      if (useVsidSubjects && backingStore is LocalSecurityNamespace.LocalUserBackingStore userBackingStore)
      {
        LocalSecurityNamespace.LocalUserBackingStore.RawQuerySecurityDataResult securityDataResult = userBackingStore.QuerySecurityDataRaw(this.TfsRequestContext, (long) oldSequenceId);
        return new Microsoft.VisualStudio.Services.Security.SecurityNamespaceData(securityDataResult.AclStoreId, securityDataResult.OldSequenceId, securityDataResult.NewSequenceId.ToArrayForRestReply(), this.TfsRequestContext.GetService<IdentityService>().IdentityServiceInternal().Domain.DomainId, ConvertRawAces((IEnumerable<DatabaseAccessControlEntry>) securityDataResult.AccessControlEntries), securityDataResult.NoInheritTokens);
      }
      IQuerySecurityDataResult securityDataResult1 = backingStore.QuerySecurityData(this.TfsRequestContext, (long) oldSequenceId);
      return new Microsoft.VisualStudio.Services.Security.SecurityNamespaceData(securityDataResult1.AclStoreId, securityDataResult1.OldSequenceId, securityDataResult1.NewSequenceId.ToArrayForRestReply(), this.TfsRequestContext.GetService<IdentityService>().IdentityServiceInternal().Domain.DomainId, BackingStoreNamespaceBaseController.ConvertAces(this.TfsRequestContext, securityDataResult1.AccessControlEntries, useVsidSubjects), securityDataResult1.NoInheritTokens);

      static IEnumerable<RemoteBackingStoreAccessControlEntry> ConvertRawAces(
        IEnumerable<DatabaseAccessControlEntry> componentAces)
      {
        foreach (DatabaseAccessControlEntry componentAce in componentAces)
          yield return new RemoteBackingStoreAccessControlEntry(componentAce.SubjectId.ToString("D"), componentAce.Token, componentAce.Allow, componentAce.Deny, componentAce.IsDeleted);
      }
    }

    private static IEnumerable<RemoteBackingStoreAccessControlEntry> ConvertAces(
      IVssRequestContext requestContext,
      IEnumerable<BackingStoreAccessControlEntry> aces,
      bool useVsidSubjects)
    {
      requestContext.TraceEnter(50001, "BackingStoreNamespaceBase", "BackingStoreAccessControlEntriesController", nameof (ConvertAces));
      List<RemoteBackingStoreAccessControlEntry> accessControlEntryList = new List<RemoteBackingStoreAccessControlEntry>();
      if (useVsidSubjects && requestContext.IsFeatureEnabled("VisualStudio.Services.AclsWithSubjectInVsidFormatCanBeReturned"))
      {
        requestContext.Trace(50002, TraceLevel.Info, "BackingStoreNamespaceBase", "BackingStoreAccessControlEntriesController", "Requesting RemoteBackingStoreAccessControlEntry objects with the Subject field in VSID(legacy) format");
        IDictionary<IdentityDescriptor, Guid> dictionary = BackingStoreAccessControlEntryHelpers.BuildReverseIdentityMap(requestContext, aces.Select<BackingStoreAccessControlEntry, IdentityDescriptor>((Func<BackingStoreAccessControlEntry, IdentityDescriptor>) (s => s.Subject)).Where<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (s => s != (IdentityDescriptor) null)));
        foreach (BackingStoreAccessControlEntry ace in aces)
        {
          Guid empty;
          if (ace.IsDeleted && (IdentityDescriptor) null == ace.Subject)
            empty = Guid.Empty;
          else if (!dictionary.TryGetValue(ace.Subject, out empty) || Guid.Empty == empty)
            continue;
          accessControlEntryList.Add(new RemoteBackingStoreAccessControlEntry(empty.ToString("D"), ace.Token, ace.Allow, ace.Deny, ace.IsDeleted));
        }
      }
      else
      {
        requestContext.Trace(50003, TraceLevel.Info, "BackingStoreNamespaceBase", "BackingStoreAccessControlEntriesController", "Requesting RemoteBackingStoreAccessControlEntry objects with the Subject field in IdentityDescriptor(modern) format");
        foreach (BackingStoreAccessControlEntry ace in aces)
        {
          string subject = !ace.IsDeleted ? ace.Subject.ToString() : (string) null;
          accessControlEntryList.Add(new RemoteBackingStoreAccessControlEntry(subject, ace.Token, ace.Allow, ace.Deny, ace.IsDeleted));
        }
      }
      requestContext.TraceLeave(50004, "BackingStoreNamespaceBase", "BackingStoreAccessControlEntriesController", nameof (ConvertAces));
      return (IEnumerable<RemoteBackingStoreAccessControlEntry>) accessControlEntryList;
    }
  }
}
