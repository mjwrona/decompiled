// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphResourceIds
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Graph
{
  public class GraphResourceIds
  {
    public const string AreaName = "Graph";
    public const string AreaId = "BB1E7EC9-E901-4B68-999A-DE7012B920F8";
    public static readonly Guid AreaIdGuid = new Guid("BB1E7EC9-E901-4B68-999A-DE7012B920F8");

    public class Groups
    {
      public const string GroupsResourceName = "Groups";
      public static readonly Guid GroupsResourceLocationId = new Guid("EBBE6AF8-0B91-4C13-8CF1-777C14858188");
    }

    public class Descriptors
    {
      public static readonly Guid DescriptorsResourceLocationId = new Guid("048AEE0A-7072-4CDE-AB73-7AF77B1E0B4E");
      public const string DescriptorsResourceName = "Descriptors";
    }

    public class Memberships
    {
      public static readonly Guid MembershipsResourceLocationId = new Guid("3FD2E6CA-FB30-443A-B579-95B19ED0934C");
      public const string MembershipsResourceName = "Memberships";
      public static readonly Guid MembershipsBatchResourceLocationId = new Guid("E34B6394-6B30-4435-94A9-409A5EEF3E31");
      public const string MembershipsBatchResourceName = "MembershipsBatch";
      public static readonly Guid MembershipStatesResourceLocationId = new Guid("1FFE5C94-1144-4191-907B-D0211CAD36A8");
      public const string MembershipStatesResourceName = "MembershipStates";
    }

    public class Scopes
    {
      public const string ScopesResourceName = "Scopes";
      public static readonly Guid ScopesResourceLocationId = new Guid("21B5FEA7-2513-41D0-AF78-B8CDB0F328BB");
    }

    public class SubjectLookup
    {
      public const string SubjectLookupResourceName = "SubjectLookup";
      public static readonly Guid SubjectLookupResourceLocationId = new Guid("4DD4D168-11F2-48C4-83E8-756FA0DE027C");
    }

    public class SubjectQuery
    {
      public const string SubjectQueryResourceName = "SubjectQuery";
      public static readonly Guid SubjectQueryResourceLocationId = new Guid("05942C89-006A-48CE-BB79-BAEB8ABF99C6");
    }

    public class Users
    {
      public const string UsersResourceName = "Users";
      public static readonly Guid UsersResourceLocationId = new Guid("005E26EC-6B77-4E4F-A986-B3827BF241F5");

      public class ProviderInfo
      {
        public const string ProviderInfoResourceName = "ProviderInfo";
        public static readonly Guid ProviderInfoResourceLocationId = new Guid("1E377995-6FA2-4588-BD64-930186ABDCFA");
      }

      public class Mapping
      {
        public const string MappingResourceName = "ResolveDisconnectedUsers";
        public static readonly Guid MappingResourceLocationId = new Guid("2F0CAE3A-74A3-4C40-A13B-974889698E6B");
      }
    }

    public class ServicePrincipals
    {
      public const string ServicePrincipalsResourceName = "ServicePrincipals";
      public static readonly Guid ServicePrincipalsResourceLocationId = new Guid("E1DBB0AE-49CB-4532-95A1-86CD89CFCAB4");
    }

    public class Subjects
    {
      public const string SubjectsResourceName = "Subjects";
      public static readonly Guid SubjectsResourceLocationId = new Guid("1D44A2AC-4F8A-459E-83C2-1C92626FB9C6");

      public class Avatars
      {
        public const string AvatarsResourceName = "Avatars";
        public static readonly Guid AvatarsResourceLocationId = new Guid("801EAF9C-0585-4BE8-9CDB-B0EFA074DE91");
      }
    }

    public class Members
    {
      public const string MembersResourceName = "Members";
      public const string MembersByDescriptorResourceLocationIdString = "B9AF63A7-5DB6-4AF8-AAE7-387F775EA9C6";
      public static readonly Guid MembersByDescriptorResourceLocationId = new Guid("B9AF63A7-5DB6-4AF8-AAE7-387F775EA9C6");
      public const string MembersResourceLocationIdString = "8B9ECDB2-B752-485A-8418-CC15CF12EE07";
      public static readonly Guid MembersResourceLocationId = new Guid("8B9ECDB2-B752-485A-8418-CC15CF12EE07");
    }

    public class CachePolicies
    {
      public const string CachePoliciesResourceName = "CachePolicies";
      public static readonly Guid CachePoliciesResourceLocationId = new Guid("BEB83272-B415-48E8-AC1E-A9B805760739");
    }

    public class MemberLookup
    {
      public const string MemberLookupResourceName = "MemberLookup";
      public static readonly Guid MemberLookupResourceLocationId = new Guid("3D74D524-AE3D-4D24-A9A7-F8A5CF82347A");
    }

    public class StorageKeys
    {
      public const string StorageKeysResourceName = "StorageKeys";
      public static readonly Guid StorageKeysResourceLocationId = new Guid("EB85F8CC-F0F6-4264-A5B1-FFE2E4D4801F");
    }

    public class MembershipTraversals
    {
      public const string MembershipTraversalsResourceName = "MembershipTraversals";
      public static readonly Guid MembershipTraversalsLocationId = new Guid("5D59D874-746F-4F9B-9459-0E571F1DED8C");
    }

    public class FederatedProviderData
    {
      public static readonly Guid FederatedProviderDataResourceLocationId = new Guid("5DCD28D6-632D-477F-AC6B-398EA9FC2F71");
      public const string FederatedProviderDataResourceName = "FederatedProviderData";
    }

    public class RequestAccess
    {
      public const string RequestAccessResourceName = "RequestAccess";
      public static readonly Guid RequestAccessLocationId = new Guid("8D54BF92-8C99-47F2-9972-B21341F1722E");
    }

    public class IdentityTranslation
    {
      public const string IdentityTranslationResourceName = "IdentityTranslation";
      public static readonly Guid IdentityTranslationLocationId = new Guid("9CACC4DA-06E3-474A-A1FA-604DD34A2FA2");
    }
  }
}
