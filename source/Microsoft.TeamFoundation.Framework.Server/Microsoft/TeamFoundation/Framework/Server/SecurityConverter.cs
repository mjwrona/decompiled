// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecurityConverter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class SecurityConverter
  {
    public static AccessControlEntry Convert(Microsoft.VisualStudio.Services.Security.AccessControlEntry vssAce)
    {
      AccessControlEntry accessControlEntry = (AccessControlEntry) null;
      if (vssAce != null)
        accessControlEntry = vssAce.ExtendedInfo == null ? new AccessControlEntry(vssAce.Descriptor, vssAce.Allow, vssAce.Deny) : new AccessControlEntry(vssAce.Descriptor, vssAce.Allow, vssAce.Deny, vssAce.ExtendedInfo.InheritedAllow, vssAce.ExtendedInfo.InheritedDeny, vssAce.ExtendedInfo.EffectiveAllow, vssAce.ExtendedInfo.EffectiveDeny);
      return accessControlEntry;
    }

    public static IEnumerable<AccessControlEntry> Convert(IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry> vssAces)
    {
      IEnumerable<AccessControlEntry> accessControlEntries = (IEnumerable<AccessControlEntry>) null;
      if (vssAces != null)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        accessControlEntries = vssAces.Select<Microsoft.VisualStudio.Services.Security.AccessControlEntry, AccessControlEntry>(SecurityConverter.\u003C\u003EO.\u003C0\u003E__Convert ?? (SecurityConverter.\u003C\u003EO.\u003C0\u003E__Convert = new Func<Microsoft.VisualStudio.Services.Security.AccessControlEntry, AccessControlEntry>(SecurityConverter.Convert)));
      }
      return accessControlEntries;
    }

    public static AccessControlList Convert(Microsoft.VisualStudio.Services.Security.AccessControlList vssAcl)
    {
      AccessControlList accessControlList = (AccessControlList) null;
      if (vssAcl != null)
      {
        IEnumerable<AccessControlEntry> aces = (IEnumerable<AccessControlEntry>) null;
        if (vssAcl.AcesDictionary != null)
          aces = SecurityConverter.Convert((IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry>) vssAcl.AcesDictionary.Values);
        accessControlList = new AccessControlList(vssAcl.Token, vssAcl.InheritPermissions, aces);
      }
      return accessControlList;
    }

    public static IEnumerable<AccessControlList> Convert(IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlList> vssAcls)
    {
      IEnumerable<AccessControlList> accessControlLists = (IEnumerable<AccessControlList>) null;
      if (vssAcls != null)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        accessControlLists = vssAcls.Select<Microsoft.VisualStudio.Services.Security.AccessControlList, AccessControlList>(SecurityConverter.\u003C\u003EO.\u003C1\u003E__Convert ?? (SecurityConverter.\u003C\u003EO.\u003C1\u003E__Convert = new Func<Microsoft.VisualStudio.Services.Security.AccessControlList, AccessControlList>(SecurityConverter.Convert)));
      }
      return accessControlLists;
    }

    public static SecurityNamespaceDescription Convert(Microsoft.VisualStudio.Services.Security.SecurityNamespaceDescription vssDescription)
    {
      SecurityNamespaceDescription namespaceDescription = (SecurityNamespaceDescription) null;
      if (vssDescription != null)
      {
        List<ActionDefinition> actions = (List<ActionDefinition>) null;
        Func<Microsoft.VisualStudio.Services.Security.ActionDefinition, ActionDefinition> selector = (Func<Microsoft.VisualStudio.Services.Security.ActionDefinition, ActionDefinition>) (vssAction =>
        {
          ActionDefinition actionDefinition = (ActionDefinition) null;
          if (vssAction != null)
            actionDefinition = new ActionDefinition(vssAction.NamespaceId, vssAction.Bit, vssAction.Name, vssAction.DisplayName);
          return actionDefinition;
        });
        if (vssDescription.Actions != null)
          actions = vssDescription.Actions.Select<Microsoft.VisualStudio.Services.Security.ActionDefinition, ActionDefinition>(selector).ToList<ActionDefinition>();
        namespaceDescription = new SecurityNamespaceDescription(vssDescription.NamespaceId, vssDescription.Name, vssDescription.DisplayName, vssDescription.DataspaceCategory ?? "Default", vssDescription.SeparatorValue, vssDescription.ElementLength, (SecurityNamespaceStructure) vssDescription.StructureValue, vssDescription.WritePermission, vssDescription.ReadPermission, actions, vssDescription.ExtensionType, vssDescription.IsRemotable, vssDescription.UseTokenTranslator, vssDescription.SystemBitMask);
      }
      return namespaceDescription;
    }

    public static IEnumerable<SecurityNamespaceDescription> Convert(
      IEnumerable<Microsoft.VisualStudio.Services.Security.SecurityNamespaceDescription> vssDescriptions)
    {
      IEnumerable<SecurityNamespaceDescription> namespaceDescriptions = (IEnumerable<SecurityNamespaceDescription>) null;
      if (vssDescriptions != null)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        namespaceDescriptions = vssDescriptions.Select<Microsoft.VisualStudio.Services.Security.SecurityNamespaceDescription, SecurityNamespaceDescription>(SecurityConverter.\u003C\u003EO.\u003C2\u003E__Convert ?? (SecurityConverter.\u003C\u003EO.\u003C2\u003E__Convert = new Func<Microsoft.VisualStudio.Services.Security.SecurityNamespaceDescription, SecurityNamespaceDescription>(SecurityConverter.Convert)));
      }
      return namespaceDescriptions;
    }

    public static Microsoft.VisualStudio.Services.Security.AccessControlEntry Convert(
      IAccessControlEntry ace)
    {
      Microsoft.VisualStudio.Services.Security.AccessControlEntry accessControlEntry = (Microsoft.VisualStudio.Services.Security.AccessControlEntry) null;
      if (ace != null)
      {
        AceExtendedInformation extendedInfo = (AceExtendedInformation) null;
        if (ace.IncludesExtendedInfo)
          extendedInfo = new AceExtendedInformation(ace.InheritedAllow, ace.InheritedDeny, ace.EffectiveAllow, ace.EffectiveDeny);
        accessControlEntry = new Microsoft.VisualStudio.Services.Security.AccessControlEntry(ace.Descriptor, ace.Allow, ace.Deny, extendedInfo);
      }
      return accessControlEntry;
    }

    public static IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry> Convert(
      IEnumerable<IAccessControlEntry> aces)
    {
      IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry> accessControlEntries = (IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry>) null;
      if (aces != null)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        accessControlEntries = aces.Select<IAccessControlEntry, Microsoft.VisualStudio.Services.Security.AccessControlEntry>(SecurityConverter.\u003C\u003EO.\u003C3\u003E__Convert ?? (SecurityConverter.\u003C\u003EO.\u003C3\u003E__Convert = new Func<IAccessControlEntry, Microsoft.VisualStudio.Services.Security.AccessControlEntry>(SecurityConverter.Convert)));
      }
      return accessControlEntries;
    }

    public static Microsoft.VisualStudio.Services.Security.AccessControlList Convert(
      IAccessControlList acl)
    {
      Microsoft.VisualStudio.Services.Security.AccessControlList accessControlList = (Microsoft.VisualStudio.Services.Security.AccessControlList) null;
      if (acl != null)
      {
        Dictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Security.AccessControlEntry> dictionary = acl.AccessControlEntries.ToDictionary<IAccessControlEntry, IdentityDescriptor, Microsoft.VisualStudio.Services.Security.AccessControlEntry>((Func<IAccessControlEntry, IdentityDescriptor>) (x => x.Descriptor), (Func<IAccessControlEntry, Microsoft.VisualStudio.Services.Security.AccessControlEntry>) (x => SecurityConverter.Convert(x)));
        accessControlList = new Microsoft.VisualStudio.Services.Security.AccessControlList(acl.Token, acl.InheritPermissions, dictionary, acl.AccessControlEntries.Any<IAccessControlEntry>((Func<IAccessControlEntry, bool>) (s => s.IncludesExtendedInfo)));
      }
      return accessControlList;
    }

    public static IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlList> Convert(
      IEnumerable<IAccessControlList> acls)
    {
      IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlList> accessControlLists = (IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlList>) null;
      if (acls != null)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        accessControlLists = acls.Select<IAccessControlList, Microsoft.VisualStudio.Services.Security.AccessControlList>(SecurityConverter.\u003C\u003EO.\u003C4\u003E__Convert ?? (SecurityConverter.\u003C\u003EO.\u003C4\u003E__Convert = new Func<IAccessControlList, Microsoft.VisualStudio.Services.Security.AccessControlList>(SecurityConverter.Convert)));
      }
      return accessControlLists;
    }

    public static Microsoft.VisualStudio.Services.Security.SecurityNamespaceDescription Convert(
      SecurityNamespaceDescription description)
    {
      Microsoft.VisualStudio.Services.Security.SecurityNamespaceDescription namespaceDescription = (Microsoft.VisualStudio.Services.Security.SecurityNamespaceDescription) null;
      if (description != null)
      {
        List<Microsoft.VisualStudio.Services.Security.ActionDefinition> actions = (List<Microsoft.VisualStudio.Services.Security.ActionDefinition>) null;
        Func<ActionDefinition, Microsoft.VisualStudio.Services.Security.ActionDefinition> selector = (Func<ActionDefinition, Microsoft.VisualStudio.Services.Security.ActionDefinition>) (action =>
        {
          Microsoft.VisualStudio.Services.Security.ActionDefinition actionDefinition = (Microsoft.VisualStudio.Services.Security.ActionDefinition) null;
          if (action != null)
            actionDefinition = new Microsoft.VisualStudio.Services.Security.ActionDefinition(action.NamespaceId, action.Bit, action.Name, action.InternalDisplayName);
          return actionDefinition;
        });
        if (description.Actions != null)
          actions = description.Actions.Select<ActionDefinition, Microsoft.VisualStudio.Services.Security.ActionDefinition>(selector).ToList<Microsoft.VisualStudio.Services.Security.ActionDefinition>();
        namespaceDescription = new Microsoft.VisualStudio.Services.Security.SecurityNamespaceDescription(description.NamespaceId, description.Name, description.InternalDisplayName, description.DataspaceCategory, description.SeparatorValue, description.ElementLength, description.StructureValue, description.WritePermission, description.ReadPermission, actions, description.ExtensionType, description.IsRemotable, description.UseTokenTranslator, description.SystemBitMask);
      }
      return namespaceDescription;
    }

    public static IEnumerable<Microsoft.VisualStudio.Services.Security.SecurityNamespaceDescription> Convert(
      IEnumerable<SecurityNamespaceDescription> descriptions)
    {
      IEnumerable<Microsoft.VisualStudio.Services.Security.SecurityNamespaceDescription> namespaceDescriptions = (IEnumerable<Microsoft.VisualStudio.Services.Security.SecurityNamespaceDescription>) null;
      if (descriptions != null)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        namespaceDescriptions = descriptions.Select<SecurityNamespaceDescription, Microsoft.VisualStudio.Services.Security.SecurityNamespaceDescription>(SecurityConverter.\u003C\u003EO.\u003C5\u003E__Convert ?? (SecurityConverter.\u003C\u003EO.\u003C5\u003E__Convert = new Func<SecurityNamespaceDescription, Microsoft.VisualStudio.Services.Security.SecurityNamespaceDescription>(SecurityConverter.Convert)));
      }
      return namespaceDescriptions;
    }

    public static Microsoft.VisualStudio.Services.Security.TokenRename Convert(
      TokenRename tokenRename)
    {
      Microsoft.VisualStudio.Services.Security.TokenRename tokenRename1 = (Microsoft.VisualStudio.Services.Security.TokenRename) null;
      if (tokenRename != null)
        tokenRename1 = new Microsoft.VisualStudio.Services.Security.TokenRename(tokenRename.OldToken, tokenRename.NewToken, tokenRename.Copy, tokenRename.Recurse);
      return tokenRename1;
    }

    public static IEnumerable<Microsoft.VisualStudio.Services.Security.TokenRename> Convert(
      IEnumerable<TokenRename> tokenRenames)
    {
      IEnumerable<Microsoft.VisualStudio.Services.Security.TokenRename> tokenRenames1 = (IEnumerable<Microsoft.VisualStudio.Services.Security.TokenRename>) null;
      if (tokenRenames != null)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        tokenRenames1 = tokenRenames.Select<TokenRename, Microsoft.VisualStudio.Services.Security.TokenRename>(SecurityConverter.\u003C\u003EO.\u003C6\u003E__Convert ?? (SecurityConverter.\u003C\u003EO.\u003C6\u003E__Convert = new Func<TokenRename, Microsoft.VisualStudio.Services.Security.TokenRename>(SecurityConverter.Convert)));
      }
      return tokenRenames1;
    }

    public static TokenRename Convert(Microsoft.VisualStudio.Services.Security.TokenRename vssTokenRename)
    {
      TokenRename tokenRename = (TokenRename) null;
      if (vssTokenRename != null)
        tokenRename = new TokenRename(vssTokenRename.OldToken, vssTokenRename.NewToken, vssTokenRename.Copy, vssTokenRename.Recurse);
      return tokenRename;
    }

    public static IEnumerable<TokenRename> Convert(IEnumerable<Microsoft.VisualStudio.Services.Security.TokenRename> vssTokenRenames)
    {
      IEnumerable<TokenRename> tokenRenames = (IEnumerable<TokenRename>) null;
      if (vssTokenRenames != null)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        tokenRenames = vssTokenRenames.Select<Microsoft.VisualStudio.Services.Security.TokenRename, TokenRename>(SecurityConverter.\u003C\u003EO.\u003C7\u003E__Convert ?? (SecurityConverter.\u003C\u003EO.\u003C7\u003E__Convert = new Func<Microsoft.VisualStudio.Services.Security.TokenRename, TokenRename>(SecurityConverter.Convert)));
      }
      return tokenRenames;
    }
  }
}
