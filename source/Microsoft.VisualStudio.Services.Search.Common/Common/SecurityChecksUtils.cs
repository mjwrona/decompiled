// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.SecurityChecksUtils
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public static class SecurityChecksUtils
  {
    public static void LoadRemoteSecurityNamespace(
      IVssRequestContext requestContext,
      Guid securityNamespaceId)
    {
      if (requestContext == null || !requestContext.ExecutionEnvironment.IsDevFabricDeployment)
        return;
      LocalSecurityService localSecurityService = requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? requestContext.GetService<LocalSecurityService>() : throw new UnsupportedHostTypeException(requestContext.ServiceHost.HostType);
      try
      {
        RemoteSecurityNamespaceDescription description = new RemoteSecurityNamespaceDescription(securityNamespaceId, ServiceInstanceTypes.TFS);
        localSecurityService.CreateRemoteSecurityNamespace(requestContext, description);
      }
      catch (SecurityNamespaceAlreadyExistsException ex)
      {
      }
    }

    public static IVssSecurityNamespace GetSecurityNamespace(
      IVssRequestContext requestContext,
      Guid securityNamespaceId)
    {
      return requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, securityNamespaceId);
    }

    public static IVssSecurityNamespace GetProjectSecurityNamespace(
      IVssRequestContext requestContext)
    {
      return requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectNamespaceId);
    }

    public static IEnumerable<IAccessControlEntry> GetRepositoryAces(
      IVssRequestContext requestContext,
      Guid repositoryId,
      Guid projectId)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      List<IAccessControlEntry> repositoryAces = new List<IAccessControlEntry>();
      string securable = GitUtils.CalculateSecurable(projectId, repositoryId, (string) null);
      IVssSecurityNamespace securityNamespace = SecurityChecksUtils.GetSecurityNamespace(requestContext, GitConstants.GitSecurityNamespaceId);
      IQueryableAclStore queryableAclStore = securityNamespace.GetQueryableAclStore(requestContext, WellKnownAclStores.System);
      IEnumerable<IAccessControlList> accessControlLists1 = securityNamespace.QueryAccessControlLists(requestContext, securable, true, false);
      IEnumerable<QueriedAccessControlList> accessControlLists2 = queryableAclStore.QueryAccessControlLists(requestContext, securable, (IEnumerable<EvaluationPrincipal>) null, true, false);
      foreach (IAccessControlList accessControlList in accessControlLists1)
      {
        foreach (IAccessControlEntry accessControlEntry in accessControlList.AccessControlEntries)
          repositoryAces.Add(accessControlEntry);
      }
      foreach (QueriedAccessControlList accessControlList in accessControlLists2)
      {
        foreach (QueriedAccessControlEntry accessControlEntry in (IEnumerable<QueriedAccessControlEntry>) accessControlList.AccessControlEntries)
          repositoryAces.Add(SecurityChecksUtils.Convert(accessControlEntry));
      }
      return (IEnumerable<IAccessControlEntry>) repositoryAces;
    }

    private static IAccessControlEntry Convert(QueriedAccessControlEntry ace) => (IAccessControlEntry) new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(ace.IdentityDescriptor, ace.Allow, ace.Deny, ace.InheritedAllow, ace.InheritedDeny, ace.EffectiveAllow, ace.EffectiveDeny);

    public static IEnumerable<IAccessControlEntry> GetClassificationNodeAces(
      IVssRequestContext requestContext,
      string token)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      List<IAccessControlEntry> classificationNodeAces = new List<IAccessControlEntry>();
      foreach (IAccessControlList accessControlList in SecurityChecksUtils.GetSecurityNamespace(requestContext, AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid).QueryAccessControlLists(requestContext, token, true, false))
      {
        foreach (IAccessControlEntry accessControlEntry in accessControlList.AccessControlEntries)
          classificationNodeAces.Add(accessControlEntry);
      }
      return (IEnumerable<IAccessControlEntry>) classificationNodeAces;
    }

    public static IEnumerable<IAccessControlList> GetFeedOrViewAcls(
      IVssRequestContext requestContext,
      string token)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      return SecurityChecksUtils.GetSecurityNamespace(requestContext, CommonConstants.FeedSecurityNamespaceGuid).QueryAccessControlLists(requestContext, token, true, false);
    }

    public static IEnumerable<IAccessControlEntry> GetFeedOrViewAces(
      IVssRequestContext requestContext,
      string token)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      List<IAccessControlEntry> feedOrViewAces = new List<IAccessControlEntry>();
      foreach (IAccessControlList accessControlList in SecurityChecksUtils.GetSecurityNamespace(requestContext, CommonConstants.FeedSecurityNamespaceGuid).QueryAccessControlLists(requestContext, token, true, false))
      {
        foreach (IAccessControlEntry accessControlEntry in accessControlList.AccessControlEntries)
          feedOrViewAces.Add(accessControlEntry);
      }
      return (IEnumerable<IAccessControlEntry>) feedOrViewAces;
    }

    [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Calling Dispose multiple times for MemoryStream object does not break anything")]
    public static byte[] GetSecurityHashcode(
      IEnumerable<IAccessControlEntry> aces,
      int permissionToBeChecked)
    {
      if (aces == null || !aces.Any<IAccessControlEntry>())
        return (byte[]) null;
      List<IAccessControlEntry> list = aces.ToList<IAccessControlEntry>();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      list.Sort(SecurityChecksUtils.\u003C\u003EO.\u003C0\u003E__CompareAces ?? (SecurityChecksUtils.\u003C\u003EO.\u003C0\u003E__CompareAces = new Comparison<IAccessControlEntry>(SecurityChecksUtils.CompareAces)));
      using (MemoryStream output = new MemoryStream())
      {
        using (BinaryWriter binaryWriter = new BinaryWriter((Stream) output))
        {
          foreach (IAccessControlEntry accessControlEntry in list)
          {
            binaryWriter.Write(accessControlEntry.Descriptor.IdentityType);
            binaryWriter.Write(accessControlEntry.Descriptor.Identifier);
            binaryWriter.Write(accessControlEntry.EffectiveAllow & permissionToBeChecked);
            binaryWriter.Write(accessControlEntry.EffectiveDeny & permissionToBeChecked);
          }
        }
        using (SHA256 shA256 = (SHA256) new SHA256Managed())
          return shA256.ComputeHash(output.ToArray());
      }
    }

    private static int CompareAces(IAccessControlEntry leftAce, IAccessControlEntry rightAce)
    {
      if (leftAce == null && rightAce == null)
        return 0;
      if (leftAce == null && rightAce != null)
        return -1;
      if (leftAce != null && rightAce == null)
        return 1;
      int num1 = string.CompareOrdinal(leftAce.Descriptor.IdentityType, rightAce.Descriptor.IdentityType);
      if (num1 != 0)
        return num1;
      int num2 = string.CompareOrdinal(leftAce.Descriptor.Identifier, rightAce.Descriptor.Identifier);
      if (num2 != 0)
        return num2;
      int num3 = (leftAce.EffectiveAllow & 2) - (rightAce.EffectiveAllow & 2);
      if (num3 != 0)
        return num3;
      int num4 = (leftAce.EffectiveDeny & 2) - (rightAce.EffectiveDeny & 2);
      return num4 != 0 ? num4 : 0;
    }

    public static bool ShouldUpdateHash(byte[] storedSecHash, byte[] currentTfsSecHash) => storedSecHash == null || currentTfsSecHash == null ? storedSecHash != currentTfsSecHash : !((IEnumerable<byte>) storedSecHash).SequenceEqual<byte>((IEnumerable<byte>) currentTfsSecHash);

    public static List<ClassificationNode> UpdateClassificationNodesSecurityHashAndToken(
      IVssRequestContext requestContext,
      List<ClassificationNode> nodes,
      string projectToken)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (nodes == null)
        throw new ArgumentNullException(nameof (nodes));
      IEnumerable<IAccessControlList> collectionOrProject = SecurityChecksUtils.GetAllAclsInCollectionOrProject(requestContext, projectToken);
      Dictionary<Guid, string> identifierToTokenMap = SecurityChecksUtils.GetClassificationNodeIdentifierToTokenMap(collectionOrProject);
      foreach (ClassificationNode node1 in nodes)
      {
        ClassificationNode node = node1;
        if (node.NodeType == ClassificationNodeType.Area)
        {
          if (identifierToTokenMap.ContainsKey(node.Identifier))
          {
            node.Token = identifierToTokenMap[node.Identifier];
            IEnumerable<IAccessControlList> source = collectionOrProject.Where<IAccessControlList>((Func<IAccessControlList, bool>) (acl => acl.Token.Equals(node.Token)));
            if (source.FirstOrDefault<IAccessControlList>() != null)
            {
              List<IAccessControlEntry> aces = new List<IAccessControlEntry>();
              foreach (IAccessControlList accessControlList in source)
              {
                foreach (IAccessControlEntry accessControlEntry in accessControlList.AccessControlEntries)
                  aces.Add(accessControlEntry);
              }
              node.SecurityHashcode = SecurityChecksUtils.GetSecurityHashcode((IEnumerable<IAccessControlEntry>) aces, AuthorizationCssNodePermissions.WorkItemRead);
            }
          }
          else
          {
            foreach (IAccessControlList accessControlList in collectionOrProject)
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1081367, "Indexing Pipeline", "IndexingOperation", FormattableString.Invariant(FormattableStringFactory.Create("Entry in TokenIdentifierMap: Token: {0}, NodeIdentifier: {1}", (object) accessControlList.Token, (object) SecurityChecksUtils.GetIdentifierFromClassificationNodeToken(accessControlList.Token))));
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1080351, "Indexing Pipeline", "IndexingOperation", FormattableString.Invariant(FormattableStringFactory.Create("Error in fetching Acls for area node {0} with identifier {1} in project {2}", (object) node.Name, (object) node.Identifier, (object) (projectToken ?? string.Empty))));
            if (nodes.Count == 1)
              throw new InvalidSecurityTokenException(FormattableString.Invariant(FormattableStringFactory.Create("Error in fetching Acls for area node {0} with identifier {1}", (object) node.Name, (object) node.Identifier)));
          }
        }
      }
      return nodes;
    }

    public static List<PackageContainer> UpdateFeedSecurityHashAndToken(
      IVssRequestContext requestContext,
      List<PackageContainer> packageContainers)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (packageContainers == null)
        throw new ArgumentNullException(nameof (packageContainers));
      IEnumerable<IAccessControlList> feedOrViewAcls = SecurityChecksUtils.GetFeedOrViewAcls(requestContext, (string) null);
      foreach (PackageContainer packageContainer1 in packageContainers)
      {
        PackageContainer packageContainer = packageContainer1;
        IEnumerable<IAccessControlList> source = feedOrViewAcls.Where<IAccessControlList>((Func<IAccessControlList, bool>) (acl => acl.Token.Equals(packageContainer.Token)));
        if (source.FirstOrDefault<IAccessControlList>() != null)
        {
          List<IAccessControlEntry> aces = new List<IAccessControlEntry>();
          foreach (IAccessControlList accessControlList in source)
          {
            foreach (IAccessControlEntry accessControlEntry in accessControlList.AccessControlEntries)
              aces.Add(accessControlEntry);
          }
          packageContainer.SecurityHashCode = SecurityChecksUtils.GetSecurityHashcode((IEnumerable<IAccessControlEntry>) aces, 32);
        }
        else
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1080055, "Indexing Pipeline", "IndexingOperation", FormattableString.Invariant(FormattableStringFactory.Create("Error in fetching Acls for package container {0} with Id {1}", (object) packageContainer.Name, (object) packageContainer.ContainerId)));
          if (packageContainers.Count == 1)
            throw new InvalidSecurityTokenException(FormattableString.Invariant(FormattableStringFactory.Create("Error in fetching Acls for package container {0} with Id {1}", (object) packageContainer.Name, (object) packageContainer.ContainerId)));
        }
      }
      return packageContainers;
    }

    private static IEnumerable<IAccessControlList> GetAllAclsInCollectionOrProject(
      IVssRequestContext requestContext,
      string rootAreaNodetoken)
    {
      return SecurityChecksUtils.GetSecurityNamespace(requestContext, AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid).QueryAccessControlLists(requestContext, rootAreaNodetoken, true, true);
    }

    private static Dictionary<Guid, string> GetClassificationNodeIdentifierToTokenMap(
      IEnumerable<IAccessControlList> acls)
    {
      Dictionary<Guid, string> identifierToTokenMap = new Dictionary<Guid, string>();
      foreach (IAccessControlList acl in acls)
      {
        Guid classificationNodeToken = SecurityChecksUtils.GetIdentifierFromClassificationNodeToken(acl.Token);
        identifierToTokenMap[classificationNodeToken] = acl.Token;
      }
      return identifierToTokenMap;
    }

    private static Guid GetIdentifierFromClassificationNodeToken(string token)
    {
      MatchCollection matchCollection = Regex.Matches(token, "[0-9a-fA-F]{8}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{12}");
      if (matchCollection.Count > 0)
      {
        Guid result;
        if (Guid.TryParse(matchCollection[matchCollection.Count - 1].ToString(), out result))
          return result;
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080351, "Indexing Pipeline", "IndexingOperation", FormattableString.Invariant(FormattableStringFactory.Create("Error in parsing security token: {0}", (object) token)));
      }
      else if (!token.Equals("vstfs"))
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080351, "Indexing Pipeline", "IndexingOperation", FormattableString.Invariant(FormattableStringFactory.Create("Node identifier Guid missing from security token {0}", (object) token)));
      return Guid.Empty;
    }
  }
}
