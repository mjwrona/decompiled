// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.Helper
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal static class Helper
  {
    private static AccessControlEntryDetails[] m_zeroLengthArrayOfAccessControlEntryDetails;
    private static AccessControlListDetails[] m_zeroLengthArrayOfAccessControlListDetails;
    private static AccessMapping[] m_zeroLengthArrayOfAccessMapping;
    private static ActionDefinition[] m_zeroLengthArrayOfActionDefinition;
    private static TeamFoundationIdentity[][] m_zeroLengthArrayOfArrayOfTeamFoundationIdentity;
    private static ArtifactPropertyValue[] m_zeroLengthArrayOfArtifactPropertyValue;
    private static ArtifactSpec[] m_zeroLengthArrayOfArtifactSpec;
    private static bool[] m_zeroLengthArrayOfBoolean;
    private static CatalogNode[] m_zeroLengthArrayOfCatalogNode;
    private static CatalogNodeDependency[] m_zeroLengthArrayOfCatalogNodeDependency;
    private static CatalogResource[] m_zeroLengthArrayOfCatalogResource;
    private static CatalogResourceType[] m_zeroLengthArrayOfCatalogResourceType;
    private static CatalogServiceReference[] m_zeroLengthArrayOfCatalogServiceReference;
    private static ConnectedServiceMetadata[] m_zeroLengthArrayOfConnectedServiceMetadata;
    private static FrameworkRegistrationEntry[] m_zeroLengthArrayOfFrameworkRegistrationEntry;
    private static FrameworkTemplateHeader[] m_zeroLengthArrayOfFrameworkTemplateHeader;
    private static Guid[] m_zeroLengthArrayOfGuid;
    private static IdentityDescriptor[] m_zeroLengthArrayOfIdentityDescriptor;
    private static KeyValueOfStringString[] m_zeroLengthArrayOfKeyValueOfStringString;
    private static LocationMapping[] m_zeroLengthArrayOfLocationMapping;
    private static OutboundLinkType[] m_zeroLengthArrayOfOutboundLinkType;
    private static PropertyValue[] m_zeroLengthArrayOfPropertyValue;
    private static RegistrationArtifactType[] m_zeroLengthArrayOfRegistrationArtifactType;
    private static RegistrationDatabase[] m_zeroLengthArrayOfRegistrationDatabase;
    private static RegistrationExtendedAttribute2[] m_zeroLengthArrayOfRegistrationExtendedAttribute2;
    private static RegistrationServiceInterface[] m_zeroLengthArrayOfRegistrationServiceInterface;
    private static RegistryAuditEntry[] m_zeroLengthArrayOfRegistryAuditEntry;
    private static RegistryEntry[] m_zeroLengthArrayOfRegistryEntry;
    private static SecurityNamespaceDescription[] m_zeroLengthArrayOfSecurityNamespaceDescription;
    private static ServiceDefinition[] m_zeroLengthArrayOfServiceDefinition;
    private static ServiceIdentity[] m_zeroLengthArrayOfServiceIdentity;
    private static ServiceTypeFilter[] m_zeroLengthArrayOfServiceTypeFilter;
    private static ServicingExecutionHandlerData[] m_zeroLengthArrayOfServicingExecutionHandlerData;
    private static ServicingJobDetail[] m_zeroLengthArrayOfServicingJobDetail;
    private static ServicingStep[] m_zeroLengthArrayOfServicingStep;
    private static ServicingStepDetail[] m_zeroLengthArrayOfServicingStepDetail;
    private static ServicingStepGroup[] m_zeroLengthArrayOfServicingStepGroup;
    private static string[] m_zeroLengthArrayOfString;
    private static StrongBoxItemInfo[] m_zeroLengthArrayOfStrongBoxItemInfo;
    private static Subscription[] m_zeroLengthArrayOfSubscription;
    private static TeamFoundationIdentity[] m_zeroLengthArrayOfTeamFoundationIdentity;
    private static TeamFoundationJobDefinition[] m_zeroLengthArrayOfTeamFoundationJobDefinition;
    private static TeamFoundationJobHistoryEntry[] m_zeroLengthArrayOfTeamFoundationJobHistoryEntry;
    private static TeamFoundationJobSchedule[] m_zeroLengthArrayOfTeamFoundationJobSchedule;
    private static TeamFoundationRequestInformation[] m_zeroLengthArrayOfTeamFoundationRequestInformation;
    private static TeamFoundationServiceHostActivity[] m_zeroLengthArrayOfTeamFoundationServiceHostActivity;
    private static TeamProjectCollectionProperties[] m_zeroLengthArrayOfTeamProjectCollectionProperties;
    private static AccessControlEntryDetails[] m_zeroLengthEnumerableOfAccessControlEntryDetails;
    private static AccessControlListDetails[] m_zeroLengthEnumerableOfAccessControlListDetails;
    private static Guid[] m_zeroLengthEnumerableOfGuid;
    private static IdentityDescriptor[] m_zeroLengthEnumerableOfIdentityDescriptor;
    private static int[] m_zeroLengthEnumerableOfInt32;
    private static RegistryEntry[] m_zeroLengthEnumerableOfRegistryEntry;
    private static ServicingStepGroup[] m_zeroLengthEnumerableOfServicingStepGroup;
    private static string[] m_zeroLengthEnumerableOfString;
    private static TeamFoundationJobDefinition[] m_zeroLengthEnumerableOfTeamFoundationJobDefinition;

    internal static AccessControlEntryDetails[] ZeroLengthArrayOfAccessControlEntryDetails
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfAccessControlEntryDetails == null)
          Helper.m_zeroLengthArrayOfAccessControlEntryDetails = new AccessControlEntryDetails[0];
        return Helper.m_zeroLengthArrayOfAccessControlEntryDetails;
      }
    }

    internal static AccessControlListDetails[] ZeroLengthArrayOfAccessControlListDetails
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfAccessControlListDetails == null)
          Helper.m_zeroLengthArrayOfAccessControlListDetails = new AccessControlListDetails[0];
        return Helper.m_zeroLengthArrayOfAccessControlListDetails;
      }
    }

    internal static AccessMapping[] ZeroLengthArrayOfAccessMapping
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfAccessMapping == null)
          Helper.m_zeroLengthArrayOfAccessMapping = new AccessMapping[0];
        return Helper.m_zeroLengthArrayOfAccessMapping;
      }
    }

    internal static ActionDefinition[] ZeroLengthArrayOfActionDefinition
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfActionDefinition == null)
          Helper.m_zeroLengthArrayOfActionDefinition = new ActionDefinition[0];
        return Helper.m_zeroLengthArrayOfActionDefinition;
      }
    }

    internal static TeamFoundationIdentity[][] ZeroLengthArrayOfArrayOfTeamFoundationIdentity
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfArrayOfTeamFoundationIdentity == null)
          Helper.m_zeroLengthArrayOfArrayOfTeamFoundationIdentity = new TeamFoundationIdentity[0][];
        return Helper.m_zeroLengthArrayOfArrayOfTeamFoundationIdentity;
      }
    }

    internal static ArtifactPropertyValue[] ZeroLengthArrayOfArtifactPropertyValue
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfArtifactPropertyValue == null)
          Helper.m_zeroLengthArrayOfArtifactPropertyValue = new ArtifactPropertyValue[0];
        return Helper.m_zeroLengthArrayOfArtifactPropertyValue;
      }
    }

    internal static ArtifactSpec[] ZeroLengthArrayOfArtifactSpec
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfArtifactSpec == null)
          Helper.m_zeroLengthArrayOfArtifactSpec = new ArtifactSpec[0];
        return Helper.m_zeroLengthArrayOfArtifactSpec;
      }
    }

    internal static bool[] ZeroLengthArrayOfBoolean
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfBoolean == null)
          Helper.m_zeroLengthArrayOfBoolean = new bool[0];
        return Helper.m_zeroLengthArrayOfBoolean;
      }
    }

    internal static CatalogNode[] ZeroLengthArrayOfCatalogNode
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfCatalogNode == null)
          Helper.m_zeroLengthArrayOfCatalogNode = new CatalogNode[0];
        return Helper.m_zeroLengthArrayOfCatalogNode;
      }
    }

    internal static CatalogNodeDependency[] ZeroLengthArrayOfCatalogNodeDependency
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfCatalogNodeDependency == null)
          Helper.m_zeroLengthArrayOfCatalogNodeDependency = new CatalogNodeDependency[0];
        return Helper.m_zeroLengthArrayOfCatalogNodeDependency;
      }
    }

    internal static CatalogResource[] ZeroLengthArrayOfCatalogResource
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfCatalogResource == null)
          Helper.m_zeroLengthArrayOfCatalogResource = new CatalogResource[0];
        return Helper.m_zeroLengthArrayOfCatalogResource;
      }
    }

    internal static CatalogResourceType[] ZeroLengthArrayOfCatalogResourceType
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfCatalogResourceType == null)
          Helper.m_zeroLengthArrayOfCatalogResourceType = new CatalogResourceType[0];
        return Helper.m_zeroLengthArrayOfCatalogResourceType;
      }
    }

    internal static CatalogServiceReference[] ZeroLengthArrayOfCatalogServiceReference
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfCatalogServiceReference == null)
          Helper.m_zeroLengthArrayOfCatalogServiceReference = new CatalogServiceReference[0];
        return Helper.m_zeroLengthArrayOfCatalogServiceReference;
      }
    }

    internal static ConnectedServiceMetadata[] ZeroLengthArrayOfConnectedServiceMetadata
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfConnectedServiceMetadata == null)
          Helper.m_zeroLengthArrayOfConnectedServiceMetadata = new ConnectedServiceMetadata[0];
        return Helper.m_zeroLengthArrayOfConnectedServiceMetadata;
      }
    }

    internal static FrameworkRegistrationEntry[] ZeroLengthArrayOfFrameworkRegistrationEntry
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfFrameworkRegistrationEntry == null)
          Helper.m_zeroLengthArrayOfFrameworkRegistrationEntry = new FrameworkRegistrationEntry[0];
        return Helper.m_zeroLengthArrayOfFrameworkRegistrationEntry;
      }
    }

    internal static FrameworkTemplateHeader[] ZeroLengthArrayOfFrameworkTemplateHeader
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfFrameworkTemplateHeader == null)
          Helper.m_zeroLengthArrayOfFrameworkTemplateHeader = new FrameworkTemplateHeader[0];
        return Helper.m_zeroLengthArrayOfFrameworkTemplateHeader;
      }
    }

    internal static Guid[] ZeroLengthArrayOfGuid
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfGuid == null)
          Helper.m_zeroLengthArrayOfGuid = new Guid[0];
        return Helper.m_zeroLengthArrayOfGuid;
      }
    }

    internal static IdentityDescriptor[] ZeroLengthArrayOfIdentityDescriptor
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfIdentityDescriptor == null)
          Helper.m_zeroLengthArrayOfIdentityDescriptor = new IdentityDescriptor[0];
        return Helper.m_zeroLengthArrayOfIdentityDescriptor;
      }
    }

    internal static KeyValueOfStringString[] ZeroLengthArrayOfKeyValueOfStringString
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfKeyValueOfStringString == null)
          Helper.m_zeroLengthArrayOfKeyValueOfStringString = new KeyValueOfStringString[0];
        return Helper.m_zeroLengthArrayOfKeyValueOfStringString;
      }
    }

    internal static LocationMapping[] ZeroLengthArrayOfLocationMapping
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfLocationMapping == null)
          Helper.m_zeroLengthArrayOfLocationMapping = new LocationMapping[0];
        return Helper.m_zeroLengthArrayOfLocationMapping;
      }
    }

    internal static OutboundLinkType[] ZeroLengthArrayOfOutboundLinkType
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfOutboundLinkType == null)
          Helper.m_zeroLengthArrayOfOutboundLinkType = new OutboundLinkType[0];
        return Helper.m_zeroLengthArrayOfOutboundLinkType;
      }
    }

    internal static PropertyValue[] ZeroLengthArrayOfPropertyValue
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfPropertyValue == null)
          Helper.m_zeroLengthArrayOfPropertyValue = new PropertyValue[0];
        return Helper.m_zeroLengthArrayOfPropertyValue;
      }
    }

    internal static RegistrationArtifactType[] ZeroLengthArrayOfRegistrationArtifactType
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfRegistrationArtifactType == null)
          Helper.m_zeroLengthArrayOfRegistrationArtifactType = new RegistrationArtifactType[0];
        return Helper.m_zeroLengthArrayOfRegistrationArtifactType;
      }
    }

    internal static RegistrationDatabase[] ZeroLengthArrayOfRegistrationDatabase
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfRegistrationDatabase == null)
          Helper.m_zeroLengthArrayOfRegistrationDatabase = new RegistrationDatabase[0];
        return Helper.m_zeroLengthArrayOfRegistrationDatabase;
      }
    }

    internal static RegistrationExtendedAttribute2[] ZeroLengthArrayOfRegistrationExtendedAttribute2
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfRegistrationExtendedAttribute2 == null)
          Helper.m_zeroLengthArrayOfRegistrationExtendedAttribute2 = new RegistrationExtendedAttribute2[0];
        return Helper.m_zeroLengthArrayOfRegistrationExtendedAttribute2;
      }
    }

    internal static RegistrationServiceInterface[] ZeroLengthArrayOfRegistrationServiceInterface
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfRegistrationServiceInterface == null)
          Helper.m_zeroLengthArrayOfRegistrationServiceInterface = new RegistrationServiceInterface[0];
        return Helper.m_zeroLengthArrayOfRegistrationServiceInterface;
      }
    }

    internal static RegistryAuditEntry[] ZeroLengthArrayOfRegistryAuditEntry
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfRegistryAuditEntry == null)
          Helper.m_zeroLengthArrayOfRegistryAuditEntry = new RegistryAuditEntry[0];
        return Helper.m_zeroLengthArrayOfRegistryAuditEntry;
      }
    }

    internal static RegistryEntry[] ZeroLengthArrayOfRegistryEntry
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfRegistryEntry == null)
          Helper.m_zeroLengthArrayOfRegistryEntry = new RegistryEntry[0];
        return Helper.m_zeroLengthArrayOfRegistryEntry;
      }
    }

    internal static SecurityNamespaceDescription[] ZeroLengthArrayOfSecurityNamespaceDescription
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfSecurityNamespaceDescription == null)
          Helper.m_zeroLengthArrayOfSecurityNamespaceDescription = new SecurityNamespaceDescription[0];
        return Helper.m_zeroLengthArrayOfSecurityNamespaceDescription;
      }
    }

    internal static ServiceDefinition[] ZeroLengthArrayOfServiceDefinition
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfServiceDefinition == null)
          Helper.m_zeroLengthArrayOfServiceDefinition = new ServiceDefinition[0];
        return Helper.m_zeroLengthArrayOfServiceDefinition;
      }
    }

    internal static ServiceIdentity[] ZeroLengthArrayOfServiceIdentity
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfServiceIdentity == null)
          Helper.m_zeroLengthArrayOfServiceIdentity = new ServiceIdentity[0];
        return Helper.m_zeroLengthArrayOfServiceIdentity;
      }
    }

    internal static ServiceTypeFilter[] ZeroLengthArrayOfServiceTypeFilter
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfServiceTypeFilter == null)
          Helper.m_zeroLengthArrayOfServiceTypeFilter = new ServiceTypeFilter[0];
        return Helper.m_zeroLengthArrayOfServiceTypeFilter;
      }
    }

    internal static ServicingExecutionHandlerData[] ZeroLengthArrayOfServicingExecutionHandlerData
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfServicingExecutionHandlerData == null)
          Helper.m_zeroLengthArrayOfServicingExecutionHandlerData = new ServicingExecutionHandlerData[0];
        return Helper.m_zeroLengthArrayOfServicingExecutionHandlerData;
      }
    }

    internal static ServicingJobDetail[] ZeroLengthArrayOfServicingJobDetail
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfServicingJobDetail == null)
          Helper.m_zeroLengthArrayOfServicingJobDetail = new ServicingJobDetail[0];
        return Helper.m_zeroLengthArrayOfServicingJobDetail;
      }
    }

    internal static ServicingStep[] ZeroLengthArrayOfServicingStep
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfServicingStep == null)
          Helper.m_zeroLengthArrayOfServicingStep = new ServicingStep[0];
        return Helper.m_zeroLengthArrayOfServicingStep;
      }
    }

    internal static ServicingStepDetail[] ZeroLengthArrayOfServicingStepDetail
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfServicingStepDetail == null)
          Helper.m_zeroLengthArrayOfServicingStepDetail = new ServicingStepDetail[0];
        return Helper.m_zeroLengthArrayOfServicingStepDetail;
      }
    }

    internal static ServicingStepGroup[] ZeroLengthArrayOfServicingStepGroup
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfServicingStepGroup == null)
          Helper.m_zeroLengthArrayOfServicingStepGroup = new ServicingStepGroup[0];
        return Helper.m_zeroLengthArrayOfServicingStepGroup;
      }
    }

    internal static string[] ZeroLengthArrayOfString
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfString == null)
          Helper.m_zeroLengthArrayOfString = new string[0];
        return Helper.m_zeroLengthArrayOfString;
      }
    }

    internal static StrongBoxItemInfo[] ZeroLengthArrayOfStrongBoxItemInfo
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfStrongBoxItemInfo == null)
          Helper.m_zeroLengthArrayOfStrongBoxItemInfo = new StrongBoxItemInfo[0];
        return Helper.m_zeroLengthArrayOfStrongBoxItemInfo;
      }
    }

    internal static Subscription[] ZeroLengthArrayOfSubscription
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfSubscription == null)
          Helper.m_zeroLengthArrayOfSubscription = new Subscription[0];
        return Helper.m_zeroLengthArrayOfSubscription;
      }
    }

    internal static TeamFoundationIdentity[] ZeroLengthArrayOfTeamFoundationIdentity
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfTeamFoundationIdentity == null)
          Helper.m_zeroLengthArrayOfTeamFoundationIdentity = new TeamFoundationIdentity[0];
        return Helper.m_zeroLengthArrayOfTeamFoundationIdentity;
      }
    }

    internal static TeamFoundationJobDefinition[] ZeroLengthArrayOfTeamFoundationJobDefinition
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfTeamFoundationJobDefinition == null)
          Helper.m_zeroLengthArrayOfTeamFoundationJobDefinition = new TeamFoundationJobDefinition[0];
        return Helper.m_zeroLengthArrayOfTeamFoundationJobDefinition;
      }
    }

    internal static TeamFoundationJobHistoryEntry[] ZeroLengthArrayOfTeamFoundationJobHistoryEntry
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfTeamFoundationJobHistoryEntry == null)
          Helper.m_zeroLengthArrayOfTeamFoundationJobHistoryEntry = new TeamFoundationJobHistoryEntry[0];
        return Helper.m_zeroLengthArrayOfTeamFoundationJobHistoryEntry;
      }
    }

    internal static TeamFoundationJobSchedule[] ZeroLengthArrayOfTeamFoundationJobSchedule
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfTeamFoundationJobSchedule == null)
          Helper.m_zeroLengthArrayOfTeamFoundationJobSchedule = new TeamFoundationJobSchedule[0];
        return Helper.m_zeroLengthArrayOfTeamFoundationJobSchedule;
      }
    }

    internal static TeamFoundationRequestInformation[] ZeroLengthArrayOfTeamFoundationRequestInformation
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfTeamFoundationRequestInformation == null)
          Helper.m_zeroLengthArrayOfTeamFoundationRequestInformation = new TeamFoundationRequestInformation[0];
        return Helper.m_zeroLengthArrayOfTeamFoundationRequestInformation;
      }
    }

    internal static TeamFoundationServiceHostActivity[] ZeroLengthArrayOfTeamFoundationServiceHostActivity
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfTeamFoundationServiceHostActivity == null)
          Helper.m_zeroLengthArrayOfTeamFoundationServiceHostActivity = new TeamFoundationServiceHostActivity[0];
        return Helper.m_zeroLengthArrayOfTeamFoundationServiceHostActivity;
      }
    }

    internal static TeamProjectCollectionProperties[] ZeroLengthArrayOfTeamProjectCollectionProperties
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfTeamProjectCollectionProperties == null)
          Helper.m_zeroLengthArrayOfTeamProjectCollectionProperties = new TeamProjectCollectionProperties[0];
        return Helper.m_zeroLengthArrayOfTeamProjectCollectionProperties;
      }
    }

    internal static IEnumerable<AccessControlEntryDetails> ZeroLengthEnumerableOfAccessControlEntryDetails
    {
      get
      {
        if (Helper.m_zeroLengthEnumerableOfAccessControlEntryDetails == null)
          Helper.m_zeroLengthEnumerableOfAccessControlEntryDetails = new AccessControlEntryDetails[0];
        return (IEnumerable<AccessControlEntryDetails>) Helper.m_zeroLengthEnumerableOfAccessControlEntryDetails;
      }
    }

    internal static IEnumerable<AccessControlListDetails> ZeroLengthEnumerableOfAccessControlListDetails
    {
      get
      {
        if (Helper.m_zeroLengthEnumerableOfAccessControlListDetails == null)
          Helper.m_zeroLengthEnumerableOfAccessControlListDetails = new AccessControlListDetails[0];
        return (IEnumerable<AccessControlListDetails>) Helper.m_zeroLengthEnumerableOfAccessControlListDetails;
      }
    }

    internal static IEnumerable<Guid> ZeroLengthEnumerableOfGuid
    {
      get
      {
        if (Helper.m_zeroLengthEnumerableOfGuid == null)
          Helper.m_zeroLengthEnumerableOfGuid = new Guid[0];
        return (IEnumerable<Guid>) Helper.m_zeroLengthEnumerableOfGuid;
      }
    }

    internal static IEnumerable<IdentityDescriptor> ZeroLengthEnumerableOfIdentityDescriptor
    {
      get
      {
        if (Helper.m_zeroLengthEnumerableOfIdentityDescriptor == null)
          Helper.m_zeroLengthEnumerableOfIdentityDescriptor = new IdentityDescriptor[0];
        return (IEnumerable<IdentityDescriptor>) Helper.m_zeroLengthEnumerableOfIdentityDescriptor;
      }
    }

    internal static IEnumerable<int> ZeroLengthEnumerableOfInt32
    {
      get
      {
        if (Helper.m_zeroLengthEnumerableOfInt32 == null)
          Helper.m_zeroLengthEnumerableOfInt32 = new int[0];
        return (IEnumerable<int>) Helper.m_zeroLengthEnumerableOfInt32;
      }
    }

    internal static IEnumerable<RegistryEntry> ZeroLengthEnumerableOfRegistryEntry
    {
      get
      {
        if (Helper.m_zeroLengthEnumerableOfRegistryEntry == null)
          Helper.m_zeroLengthEnumerableOfRegistryEntry = new RegistryEntry[0];
        return (IEnumerable<RegistryEntry>) Helper.m_zeroLengthEnumerableOfRegistryEntry;
      }
    }

    internal static IEnumerable<ServicingStepGroup> ZeroLengthEnumerableOfServicingStepGroup
    {
      get
      {
        if (Helper.m_zeroLengthEnumerableOfServicingStepGroup == null)
          Helper.m_zeroLengthEnumerableOfServicingStepGroup = new ServicingStepGroup[0];
        return (IEnumerable<ServicingStepGroup>) Helper.m_zeroLengthEnumerableOfServicingStepGroup;
      }
    }

    internal static IEnumerable<string> ZeroLengthEnumerableOfString
    {
      get
      {
        if (Helper.m_zeroLengthEnumerableOfString == null)
          Helper.m_zeroLengthEnumerableOfString = new string[0];
        return (IEnumerable<string>) Helper.m_zeroLengthEnumerableOfString;
      }
    }

    internal static IEnumerable<TeamFoundationJobDefinition> ZeroLengthEnumerableOfTeamFoundationJobDefinition
    {
      get
      {
        if (Helper.m_zeroLengthEnumerableOfTeamFoundationJobDefinition == null)
          Helper.m_zeroLengthEnumerableOfTeamFoundationJobDefinition = new TeamFoundationJobDefinition[0];
        return (IEnumerable<TeamFoundationJobDefinition>) Helper.m_zeroLengthEnumerableOfTeamFoundationJobDefinition;
      }
    }

    internal static AccessControlEntryDetails[] ArrayOfAccessControlEntryDetailsFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<AccessControlEntryDetails>(serviceProvider, reader, "AccessControlEntryDetails", inline, Helper.\u003C\u003EO.\u003C0\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C0\u003E__FromXml = new Func<IServiceProvider, XmlReader, AccessControlEntryDetails>(AccessControlEntryDetails.FromXml)));
    }

    internal static AccessControlListDetails[] ArrayOfAccessControlListDetailsFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<AccessControlListDetails>(serviceProvider, reader, "AccessControlListDetails", inline, Helper.\u003C\u003EO.\u003C1\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C1\u003E__FromXml = new Func<IServiceProvider, XmlReader, AccessControlListDetails>(AccessControlListDetails.FromXml)));
    }

    internal static AccessMapping[] ArrayOfAccessMappingFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<AccessMapping>(serviceProvider, reader, "AccessMapping", inline, Helper.\u003C\u003EO.\u003C2\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C2\u003E__FromXml = new Func<IServiceProvider, XmlReader, AccessMapping>(AccessMapping.FromXml)));
    }

    internal static ActionDefinition[] ArrayOfActionDefinitionFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<ActionDefinition>(serviceProvider, reader, "ActionDefinition", inline, Helper.\u003C\u003EO.\u003C3\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C3\u003E__FromXml = new Func<IServiceProvider, XmlReader, ActionDefinition>(ActionDefinition.FromXml)));
    }

    internal static TeamFoundationIdentity[][] ArrayOfArrayOfTeamFoundationIdentityFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<TeamFoundationIdentity[]>(serviceProvider, reader, "ArrayOfTeamFoundationIdentity", inline, Helper.\u003C\u003EO.\u003C4\u003E__ArrayOfTeamFoundationIdentityFromXml ?? (Helper.\u003C\u003EO.\u003C4\u003E__ArrayOfTeamFoundationIdentityFromXml = new Func<IServiceProvider, XmlReader, TeamFoundationIdentity[]>(Helper.ArrayOfTeamFoundationIdentityFromXml)));
    }

    internal static ArtifactPropertyValue[] ArrayOfArtifactPropertyValueFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<ArtifactPropertyValue>(serviceProvider, reader, "ArtifactPropertyValue", inline, Helper.\u003C\u003EO.\u003C5\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C5\u003E__FromXml = new Func<IServiceProvider, XmlReader, ArtifactPropertyValue>(ArtifactPropertyValue.FromXml)));
    }

    internal static ArtifactSpec[] ArrayOfArtifactSpecFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<ArtifactSpec>(serviceProvider, reader, "ArtifactSpec", inline, Helper.\u003C\u003EO.\u003C6\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C6\u003E__FromXml = new Func<IServiceProvider, XmlReader, ArtifactSpec>(ArtifactSpec.FromXml)));
    }

    internal static bool[] ArrayOfBooleanFromXml(XmlReader reader, bool inline) => XmlUtility.ArrayOfObjectFromXml<bool>(reader, "bool", inline, Helper.\u003C\u003EO.\u003C7\u003E__BooleanFromXmlElement ?? (Helper.\u003C\u003EO.\u003C7\u003E__BooleanFromXmlElement = new Func<XmlReader, bool>(XmlUtility.BooleanFromXmlElement)));

    internal static CatalogNodeDependency[] ArrayOfCatalogNodeDependencyFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<CatalogNodeDependency>(serviceProvider, reader, "CatalogNodeDependency", inline, Helper.\u003C\u003EO.\u003C8\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C8\u003E__FromXml = new Func<IServiceProvider, XmlReader, CatalogNodeDependency>(CatalogNodeDependency.FromXml)));
    }

    internal static CatalogNode[] ArrayOfCatalogNodeFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<CatalogNode>(serviceProvider, reader, "CatalogNode", inline, Helper.\u003C\u003EO.\u003C9\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C9\u003E__FromXml = new Func<IServiceProvider, XmlReader, CatalogNode>(CatalogNode.FromXml)));
    }

    internal static CatalogResource[] ArrayOfCatalogResourceFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<CatalogResource>(serviceProvider, reader, "CatalogResource", inline, Helper.\u003C\u003EO.\u003C10\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C10\u003E__FromXml = new Func<IServiceProvider, XmlReader, CatalogResource>(CatalogResource.FromXml)));
    }

    internal static CatalogResourceType[] ArrayOfCatalogResourceTypeFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<CatalogResourceType>(serviceProvider, reader, "CatalogResourceType", inline, Helper.\u003C\u003EO.\u003C11\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C11\u003E__FromXml = new Func<IServiceProvider, XmlReader, CatalogResourceType>(CatalogResourceType.FromXml)));
    }

    internal static CatalogServiceReference[] ArrayOfCatalogServiceReferenceFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<CatalogServiceReference>(serviceProvider, reader, "CatalogServiceReference", inline, Helper.\u003C\u003EO.\u003C12\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C12\u003E__FromXml = new Func<IServiceProvider, XmlReader, CatalogServiceReference>(CatalogServiceReference.FromXml)));
    }

    internal static ConnectedServiceMetadata[] ArrayOfConnectedServiceMetadataFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<ConnectedServiceMetadata>(serviceProvider, reader, "ConnectedServiceMetadata", inline, Helper.\u003C\u003EO.\u003C13\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C13\u003E__FromXml = new Func<IServiceProvider, XmlReader, ConnectedServiceMetadata>(ConnectedServiceMetadata.FromXml)));
    }

    internal static FrameworkRegistrationEntry[] ArrayOfFrameworkRegistrationEntryFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<FrameworkRegistrationEntry>(serviceProvider, reader, "RegistrationEntry", inline, Helper.\u003C\u003EO.\u003C14\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C14\u003E__FromXml = new Func<IServiceProvider, XmlReader, FrameworkRegistrationEntry>(FrameworkRegistrationEntry.FromXml)));
    }

    internal static FrameworkTemplateHeader[] ArrayOfFrameworkTemplateHeaderFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<FrameworkTemplateHeader>(serviceProvider, reader, "FrameworkTemplateHeader", inline, Helper.\u003C\u003EO.\u003C15\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C15\u003E__FromXml = new Func<IServiceProvider, XmlReader, FrameworkTemplateHeader>(FrameworkTemplateHeader.FromXml)));
    }

    internal static Guid[] ArrayOfGuidFromXml(XmlReader reader, bool inline) => XmlUtility.ArrayOfObjectFromXml<Guid>(reader, "guid", inline, Helper.\u003C\u003EO.\u003C16\u003E__GuidFromXmlElement ?? (Helper.\u003C\u003EO.\u003C16\u003E__GuidFromXmlElement = new Func<XmlReader, Guid>(XmlUtility.GuidFromXmlElement)));

    internal static IdentityDescriptor[] ArrayOfIdentityDescriptorFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<IdentityDescriptor>(serviceProvider, reader, "IdentityDescriptor", inline, Helper.\u003C\u003EO.\u003C17\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C17\u003E__FromXml = new Func<IServiceProvider, XmlReader, IdentityDescriptor>(IdentityDescriptor.FromXml)));
    }

    internal static KeyValueOfStringString[] ArrayOfKeyValueOfStringStringFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<KeyValueOfStringString>(serviceProvider, reader, "KeyValueOfStringString", inline, Helper.\u003C\u003EO.\u003C18\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C18\u003E__FromXml = new Func<IServiceProvider, XmlReader, KeyValueOfStringString>(KeyValueOfStringString.FromXml)));
    }

    internal static LocationMapping[] ArrayOfLocationMappingFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<LocationMapping>(serviceProvider, reader, "LocationMapping", inline, Helper.\u003C\u003EO.\u003C19\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C19\u003E__FromXml = new Func<IServiceProvider, XmlReader, LocationMapping>(LocationMapping.FromXml)));
    }

    internal static OutboundLinkType[] ArrayOfOutboundLinkTypeFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<OutboundLinkType>(serviceProvider, reader, "OutboundLinkType", inline, Helper.\u003C\u003EO.\u003C20\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C20\u003E__FromXml = new Func<IServiceProvider, XmlReader, OutboundLinkType>(OutboundLinkType.FromXml)));
    }

    internal static PropertyValue[] ArrayOfPropertyValueFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<PropertyValue>(serviceProvider, reader, "PropertyValue", inline, Helper.\u003C\u003EO.\u003C21\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C21\u003E__FromXml = new Func<IServiceProvider, XmlReader, PropertyValue>(PropertyValue.FromXml)));
    }

    internal static RegistrationArtifactType[] ArrayOfRegistrationArtifactTypeFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<RegistrationArtifactType>(serviceProvider, reader, "RegistrationArtifactType", inline, Helper.\u003C\u003EO.\u003C22\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C22\u003E__FromXml = new Func<IServiceProvider, XmlReader, RegistrationArtifactType>(RegistrationArtifactType.FromXml)));
    }

    internal static RegistrationDatabase[] ArrayOfRegistrationDatabaseFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<RegistrationDatabase>(serviceProvider, reader, "RegistrationDatabase", inline, Helper.\u003C\u003EO.\u003C23\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C23\u003E__FromXml = new Func<IServiceProvider, XmlReader, RegistrationDatabase>(RegistrationDatabase.FromXml)));
    }

    internal static RegistrationExtendedAttribute2[] ArrayOfRegistrationExtendedAttribute2FromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<RegistrationExtendedAttribute2>(serviceProvider, reader, "RegistrationExtendedAttribute2", inline, Helper.\u003C\u003EO.\u003C24\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C24\u003E__FromXml = new Func<IServiceProvider, XmlReader, RegistrationExtendedAttribute2>(RegistrationExtendedAttribute2.FromXml)));
    }

    internal static RegistrationServiceInterface[] ArrayOfRegistrationServiceInterfaceFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<RegistrationServiceInterface>(serviceProvider, reader, "RegistrationServiceInterface", inline, Helper.\u003C\u003EO.\u003C25\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C25\u003E__FromXml = new Func<IServiceProvider, XmlReader, RegistrationServiceInterface>(RegistrationServiceInterface.FromXml)));
    }

    internal static RegistryAuditEntry[] ArrayOfRegistryAuditEntryFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<RegistryAuditEntry>(serviceProvider, reader, "RegistryAuditEntry", inline, Helper.\u003C\u003EO.\u003C26\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C26\u003E__FromXml = new Func<IServiceProvider, XmlReader, RegistryAuditEntry>(RegistryAuditEntry.FromXml)));
    }

    internal static RegistryEntry[] ArrayOfRegistryEntryFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<RegistryEntry>(serviceProvider, reader, "RegistryEntry", inline, Helper.\u003C\u003EO.\u003C27\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C27\u003E__FromXml = new Func<IServiceProvider, XmlReader, RegistryEntry>(RegistryEntry.FromXml)));
    }

    internal static SecurityNamespaceDescription[] ArrayOfSecurityNamespaceDescriptionFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<SecurityNamespaceDescription>(serviceProvider, reader, "SecurityNamespaceDescription", inline, Helper.\u003C\u003EO.\u003C28\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C28\u003E__FromXml = new Func<IServiceProvider, XmlReader, SecurityNamespaceDescription>(SecurityNamespaceDescription.FromXml)));
    }

    internal static ServiceDefinition[] ArrayOfServiceDefinitionFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<ServiceDefinition>(serviceProvider, reader, "ServiceDefinition", inline, Helper.\u003C\u003EO.\u003C29\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C29\u003E__FromXml = new Func<IServiceProvider, XmlReader, ServiceDefinition>(ServiceDefinition.FromXml)));
    }

    internal static ServiceIdentity[] ArrayOfServiceIdentityFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<ServiceIdentity>(serviceProvider, reader, "ServiceIdentity", inline, Helper.\u003C\u003EO.\u003C30\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C30\u003E__FromXml = new Func<IServiceProvider, XmlReader, ServiceIdentity>(ServiceIdentity.FromXml)));
    }

    internal static ServiceTypeFilter[] ArrayOfServiceTypeFilterFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<ServiceTypeFilter>(serviceProvider, reader, "ServiceTypeFilter", inline, Helper.\u003C\u003EO.\u003C31\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C31\u003E__FromXml = new Func<IServiceProvider, XmlReader, ServiceTypeFilter>(ServiceTypeFilter.FromXml)));
    }

    internal static ServicingExecutionHandlerData[] ArrayOfServicingExecutionHandlerDataFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<ServicingExecutionHandlerData>(serviceProvider, reader, "ServicingExecutionHandlerData", inline, Helper.\u003C\u003EO.\u003C32\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C32\u003E__FromXml = new Func<IServiceProvider, XmlReader, ServicingExecutionHandlerData>(ServicingExecutionHandlerData.FromXml)));
    }

    internal static ServicingJobDetail[] ArrayOfServicingJobDetailFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<ServicingJobDetail>(serviceProvider, reader, "ServicingJobDetail", inline, Helper.\u003C\u003EO.\u003C33\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C33\u003E__FromXml = new Func<IServiceProvider, XmlReader, ServicingJobDetail>(ServicingJobDetail.FromXml)));
    }

    internal static ServicingStepDetail[] ArrayOfServicingStepDetailFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<ServicingStepDetail>(serviceProvider, reader, "ServicingStepDetail", inline, Helper.\u003C\u003EO.\u003C34\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C34\u003E__FromXml = new Func<IServiceProvider, XmlReader, ServicingStepDetail>(ServicingStepDetail.FromXml)));
    }

    internal static ServicingStep[] ArrayOfServicingStepFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<ServicingStep>(serviceProvider, reader, "ServicingStep", inline, Helper.\u003C\u003EO.\u003C35\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C35\u003E__FromXml = new Func<IServiceProvider, XmlReader, ServicingStep>(ServicingStep.FromXml)));
    }

    internal static ServicingStepGroup[] ArrayOfServicingStepGroupFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<ServicingStepGroup>(serviceProvider, reader, "ServicingStepGroup", inline, Helper.\u003C\u003EO.\u003C36\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C36\u003E__FromXml = new Func<IServiceProvider, XmlReader, ServicingStepGroup>(ServicingStepGroup.FromXml)));
    }

    internal static string[] ArrayOfStringFromXml(XmlReader reader, bool inline) => XmlUtility.ArrayOfObjectFromXml<string>(reader, "string", inline, Helper.\u003C\u003EO.\u003C37\u003E__StringFromXmlElement ?? (Helper.\u003C\u003EO.\u003C37\u003E__StringFromXmlElement = new Func<XmlReader, string>(XmlUtility.StringFromXmlElement)));

    internal static StrongBoxItemInfo[] ArrayOfStrongBoxItemInfoFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<StrongBoxItemInfo>(serviceProvider, reader, "StrongBoxItemInfo", inline, Helper.\u003C\u003EO.\u003C38\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C38\u003E__FromXml = new Func<IServiceProvider, XmlReader, StrongBoxItemInfo>(StrongBoxItemInfo.FromXml)));
    }

    internal static Subscription[] ArrayOfSubscriptionFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<Subscription>(serviceProvider, reader, "Subscription", inline, Helper.\u003C\u003EO.\u003C39\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C39\u003E__FromXml = new Func<IServiceProvider, XmlReader, Subscription>(Subscription.FromXml)));
    }

    internal static TeamFoundationIdentity[] ArrayOfTeamFoundationIdentityFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<TeamFoundationIdentity>(serviceProvider, reader, "TeamFoundationIdentity", inline, Helper.\u003C\u003EO.\u003C40\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C40\u003E__FromXml = new Func<IServiceProvider, XmlReader, TeamFoundationIdentity>(TeamFoundationIdentity.FromXml)));
    }

    internal static TeamFoundationIdentity[] ArrayOfTeamFoundationIdentityFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader)
    {
      return Helper.ArrayOfTeamFoundationIdentityFromXml(serviceProvider, reader, false);
    }

    internal static TeamFoundationJobDefinition[] ArrayOfTeamFoundationJobDefinitionFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<TeamFoundationJobDefinition>(serviceProvider, reader, "TeamFoundationJobDefinition", inline, Helper.\u003C\u003EO.\u003C41\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C41\u003E__FromXml = new Func<IServiceProvider, XmlReader, TeamFoundationJobDefinition>(TeamFoundationJobDefinition.FromXml)));
    }

    internal static TeamFoundationJobHistoryEntry[] ArrayOfTeamFoundationJobHistoryEntryFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<TeamFoundationJobHistoryEntry>(serviceProvider, reader, "TeamFoundationJobHistoryEntry", inline, Helper.\u003C\u003EO.\u003C42\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C42\u003E__FromXml = new Func<IServiceProvider, XmlReader, TeamFoundationJobHistoryEntry>(TeamFoundationJobHistoryEntry.FromXml)));
    }

    internal static TeamFoundationJobSchedule[] ArrayOfTeamFoundationJobScheduleFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<TeamFoundationJobSchedule>(serviceProvider, reader, "TeamFoundationJobSchedule", inline, Helper.\u003C\u003EO.\u003C43\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C43\u003E__FromXml = new Func<IServiceProvider, XmlReader, TeamFoundationJobSchedule>(TeamFoundationJobSchedule.FromXml)));
    }

    internal static TeamFoundationRequestInformation[] ArrayOfTeamFoundationRequestInformationFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<TeamFoundationRequestInformation>(serviceProvider, reader, "TeamFoundationRequestInformation", inline, Helper.\u003C\u003EO.\u003C44\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C44\u003E__FromXml = new Func<IServiceProvider, XmlReader, TeamFoundationRequestInformation>(TeamFoundationRequestInformation.FromXml)));
    }

    internal static TeamFoundationServiceHostActivity[] ArrayOfTeamFoundationServiceHostActivityFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<TeamFoundationServiceHostActivity>(serviceProvider, reader, "TeamFoundationServiceHostActivity", inline, Helper.\u003C\u003EO.\u003C45\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C45\u003E__FromXml = new Func<IServiceProvider, XmlReader, TeamFoundationServiceHostActivity>(TeamFoundationServiceHostActivity.FromXml)));
    }

    internal static TeamProjectCollectionProperties[] ArrayOfTeamProjectCollectionPropertiesFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<TeamProjectCollectionProperties>(serviceProvider, reader, "TeamProjectCollectionProperties", inline, Helper.\u003C\u003EO.\u003C46\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C46\u003E__FromXml = new Func<IServiceProvider, XmlReader, TeamProjectCollectionProperties>(TeamProjectCollectionProperties.FromXml)));
    }

    internal static string ArrayToString<T>(T[] array)
    {
      if (array == null)
        return "<null>";
      int num = Math.Min(array.Length, 100);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("[");
      stringBuilder.Append(array.Length);
      stringBuilder.Append("]");
      for (int index = 0; index < num; ++index)
      {
        stringBuilder.Append((object) array[index]);
        if (index != num - 1)
          stringBuilder.Append(", ");
      }
      if (num < array.Length)
        stringBuilder.Append(", ...");
      return stringBuilder.ToString();
    }

    internal static IEnumerable<AccessControlEntryDetails> EnumerableOfAccessControlEntryDetailsFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return (IEnumerable<AccessControlEntryDetails>) XmlUtility.ArrayOfObjectFromXml<AccessControlEntryDetails>(serviceProvider, reader, "AccessControlEntryDetails", inline, Helper.\u003C\u003EO.\u003C0\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C0\u003E__FromXml = new Func<IServiceProvider, XmlReader, AccessControlEntryDetails>(AccessControlEntryDetails.FromXml)));
    }

    internal static IEnumerable<AccessControlListDetails> EnumerableOfAccessControlListDetailsFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return (IEnumerable<AccessControlListDetails>) XmlUtility.ArrayOfObjectFromXml<AccessControlListDetails>(serviceProvider, reader, "AccessControlListDetails", inline, Helper.\u003C\u003EO.\u003C1\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C1\u003E__FromXml = new Func<IServiceProvider, XmlReader, AccessControlListDetails>(AccessControlListDetails.FromXml)));
    }

    internal static IEnumerable<Guid> EnumerableOfGuidFromXml(XmlReader reader, bool inline) => (IEnumerable<Guid>) XmlUtility.ArrayOfObjectFromXml<Guid>(reader, "guid", inline, Helper.\u003C\u003EO.\u003C16\u003E__GuidFromXmlElement ?? (Helper.\u003C\u003EO.\u003C16\u003E__GuidFromXmlElement = new Func<XmlReader, Guid>(XmlUtility.GuidFromXmlElement)));

    internal static IEnumerable<IdentityDescriptor> EnumerableOfIdentityDescriptorFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return (IEnumerable<IdentityDescriptor>) XmlUtility.ArrayOfObjectFromXml<IdentityDescriptor>(serviceProvider, reader, "IdentityDescriptor", inline, Helper.\u003C\u003EO.\u003C17\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C17\u003E__FromXml = new Func<IServiceProvider, XmlReader, IdentityDescriptor>(IdentityDescriptor.FromXml)));
    }

    internal static IEnumerable<int> EnumerableOfInt32FromXml(XmlReader reader, bool inline) => (IEnumerable<int>) XmlUtility.ArrayOfObjectFromXml<int>(reader, "int", inline, Helper.\u003C\u003EO.\u003C47\u003E__Int32FromXmlElement ?? (Helper.\u003C\u003EO.\u003C47\u003E__Int32FromXmlElement = new Func<XmlReader, int>(XmlUtility.Int32FromXmlElement)));

    internal static IEnumerable<RegistryEntry> EnumerableOfRegistryEntryFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return (IEnumerable<RegistryEntry>) XmlUtility.ArrayOfObjectFromXml<RegistryEntry>(serviceProvider, reader, "RegistryEntry", inline, Helper.\u003C\u003EO.\u003C27\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C27\u003E__FromXml = new Func<IServiceProvider, XmlReader, RegistryEntry>(RegistryEntry.FromXml)));
    }

    internal static IEnumerable<ServicingStepGroup> EnumerableOfServicingStepGroupFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return (IEnumerable<ServicingStepGroup>) XmlUtility.ArrayOfObjectFromXml<ServicingStepGroup>(serviceProvider, reader, "ServicingStepGroup", inline, Helper.\u003C\u003EO.\u003C36\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C36\u003E__FromXml = new Func<IServiceProvider, XmlReader, ServicingStepGroup>(ServicingStepGroup.FromXml)));
    }

    internal static IEnumerable<string> EnumerableOfStringFromXml(XmlReader reader, bool inline) => (IEnumerable<string>) XmlUtility.ArrayOfObjectFromXml<string>(reader, "string", inline, Helper.\u003C\u003EO.\u003C37\u003E__StringFromXmlElement ?? (Helper.\u003C\u003EO.\u003C37\u003E__StringFromXmlElement = new Func<XmlReader, string>(XmlUtility.StringFromXmlElement)));

    internal static IEnumerable<TeamFoundationJobDefinition> EnumerableOfTeamFoundationJobDefinitionFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return (IEnumerable<TeamFoundationJobDefinition>) XmlUtility.ArrayOfObjectFromXml<TeamFoundationJobDefinition>(serviceProvider, reader, "TeamFoundationJobDefinition", inline, Helper.\u003C\u003EO.\u003C41\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C41\u003E__FromXml = new Func<IServiceProvider, XmlReader, TeamFoundationJobDefinition>(TeamFoundationJobDefinition.FromXml)));
    }

    internal static void StringToXmlElement(XmlWriter writer, string element, string str)
    {
      if (str == null)
        return;
      try
      {
        writer.WriteElementString(element, str);
      }
      catch (ArgumentException ex)
      {
        throw new TeamFoundationServiceException(CommonResources.StringContainsIllegalChars());
      }
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      AccessControlEntryDetails[] array)
    {
      Helper.ToXml(writer, arrayName, array, false, false);
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      AccessControlEntryDetails[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<AccessControlEntryDetails>(writer, array, arrayName, "AccessControlEntryDetails", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C48\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C48\u003E__ToXml = new Action<XmlWriter, string, AccessControlEntryDetails>(AccessControlEntryDetails.ToXml)));
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      AccessControlListDetails[] array)
    {
      Helper.ToXml(writer, arrayName, array, false, false);
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      AccessControlListDetails[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<AccessControlListDetails>(writer, array, arrayName, "AccessControlListDetails", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C49\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C49\u003E__ToXml = new Action<XmlWriter, string, AccessControlListDetails>(AccessControlListDetails.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, AccessMapping[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      AccessMapping[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<AccessMapping>(writer, array, arrayName, "AccessMapping", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C50\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C50\u003E__ToXml = new Action<XmlWriter, string, AccessMapping>(AccessMapping.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, ActionDefinition[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      ActionDefinition[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<ActionDefinition>(writer, array, arrayName, "ActionDefinition", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C51\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C51\u003E__ToXml = new Action<XmlWriter, string, ActionDefinition>(ActionDefinition.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, ArtifactPropertyValue[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      ArtifactPropertyValue[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<ArtifactPropertyValue>(writer, array, arrayName, "ArtifactPropertyValue", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C52\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C52\u003E__ToXml = new Action<XmlWriter, string, ArtifactPropertyValue>(ArtifactPropertyValue.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, ArtifactSpec[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      ArtifactSpec[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<ArtifactSpec>(writer, array, arrayName, "ArtifactSpec", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C53\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C53\u003E__ToXml = new Action<XmlWriter, string, ArtifactSpec>(ArtifactSpec.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, bool[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      bool[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<bool>(writer, array, arrayName, "bool", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C54\u003E__ToXmlElement ?? (Helper.\u003C\u003EO.\u003C54\u003E__ToXmlElement = new Action<XmlWriter, string, bool>(XmlUtility.ToXmlElement)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, CatalogNodeDependency[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      CatalogNodeDependency[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<CatalogNodeDependency>(writer, array, arrayName, "CatalogNodeDependency", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C55\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C55\u003E__ToXml = new Action<XmlWriter, string, CatalogNodeDependency>(CatalogNodeDependency.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, CatalogNode[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      CatalogNode[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<CatalogNode>(writer, array, arrayName, "CatalogNode", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C56\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C56\u003E__ToXml = new Action<XmlWriter, string, CatalogNode>(CatalogNode.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, CatalogResourceType[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      CatalogResourceType[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<CatalogResourceType>(writer, array, arrayName, "CatalogResourceType", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C57\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C57\u003E__ToXml = new Action<XmlWriter, string, CatalogResourceType>(CatalogResourceType.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, CatalogResource[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      CatalogResource[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<CatalogResource>(writer, array, arrayName, "CatalogResource", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C58\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C58\u003E__ToXml = new Action<XmlWriter, string, CatalogResource>(CatalogResource.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, CatalogServiceReference[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      CatalogServiceReference[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<CatalogServiceReference>(writer, array, arrayName, "CatalogServiceReference", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C59\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C59\u003E__ToXml = new Action<XmlWriter, string, CatalogServiceReference>(CatalogServiceReference.ToXml)));
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      ConnectedServiceMetadata[] array)
    {
      Helper.ToXml(writer, arrayName, array, false, false);
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      ConnectedServiceMetadata[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<ConnectedServiceMetadata>(writer, array, arrayName, "ConnectedServiceMetadata", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C60\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C60\u003E__ToXml = new Action<XmlWriter, string, ConnectedServiceMetadata>(ConnectedServiceMetadata.ToXml)));
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      FrameworkRegistrationEntry[] array)
    {
      Helper.ToXml(writer, arrayName, array, false, false);
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      FrameworkRegistrationEntry[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<FrameworkRegistrationEntry>(writer, array, arrayName, "RegistrationEntry", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C61\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C61\u003E__ToXml = new Action<XmlWriter, string, FrameworkRegistrationEntry>(FrameworkRegistrationEntry.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, FrameworkTemplateHeader[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      FrameworkTemplateHeader[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<FrameworkTemplateHeader>(writer, array, arrayName, "FrameworkTemplateHeader", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C62\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C62\u003E__ToXml = new Action<XmlWriter, string, FrameworkTemplateHeader>(FrameworkTemplateHeader.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, Guid[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      Guid[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<Guid>(writer, array, arrayName, "guid", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C63\u003E__ToXmlElement ?? (Helper.\u003C\u003EO.\u003C63\u003E__ToXmlElement = new Action<XmlWriter, string, Guid>(XmlUtility.ToXmlElement)));
    }

    internal static void ToXml(
      XmlWriter writer,
      string enumerableName,
      IEnumerable<IdentityDescriptor> enumerable,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.EnumerableOfObjectToXml<IdentityDescriptor>(writer, enumerable, enumerableName, "IdentityDescriptor", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C64\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C64\u003E__ToXml = new Action<XmlWriter, string, IdentityDescriptor>(IdentityDescriptor.ToXml)));
    }

    internal static void ToXml(
      XmlWriter writer,
      string enumerableName,
      IEnumerable<RegistryEntry> enumerable,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.EnumerableOfObjectToXml<RegistryEntry>(writer, enumerable, enumerableName, "RegistryEntry", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C65\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C65\u003E__ToXml = new Action<XmlWriter, string, RegistryEntry>(RegistryEntry.ToXml)));
    }

    internal static void ToXml(
      XmlWriter writer,
      string enumerableName,
      IEnumerable<int> enumerable,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.EnumerableOfObjectToXml<int>(writer, enumerable, enumerableName, "int", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C66\u003E__ToXmlElement ?? (Helper.\u003C\u003EO.\u003C66\u003E__ToXmlElement = new Action<XmlWriter, string, int>(XmlUtility.ToXmlElement)));
    }

    internal static void ToXml(
      XmlWriter writer,
      string enumerableName,
      IEnumerable<AccessControlListDetails> enumerable,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.EnumerableOfObjectToXml<AccessControlListDetails>(writer, enumerable, enumerableName, "AccessControlListDetails", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C49\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C49\u003E__ToXml = new Action<XmlWriter, string, AccessControlListDetails>(AccessControlListDetails.ToXml)));
    }

    internal static void ToXml(
      XmlWriter writer,
      string enumerableName,
      IEnumerable<string> enumerable,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.EnumerableOfObjectToXml<string>(writer, enumerable, enumerableName, "string", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C67\u003E__ToXmlElement ?? (Helper.\u003C\u003EO.\u003C67\u003E__ToXmlElement = new Action<XmlWriter, string, string>(XmlUtility.ToXmlElement)));
    }

    internal static void ToXml(
      XmlWriter writer,
      string enumerableName,
      IEnumerable<Guid> enumerable,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.EnumerableOfObjectToXml<Guid>(writer, enumerable, enumerableName, "guid", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C63\u003E__ToXmlElement ?? (Helper.\u003C\u003EO.\u003C63\u003E__ToXmlElement = new Action<XmlWriter, string, Guid>(XmlUtility.ToXmlElement)));
    }

    internal static void ToXml(
      XmlWriter writer,
      string enumerableName,
      IEnumerable<AccessControlEntryDetails> enumerable,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.EnumerableOfObjectToXml<AccessControlEntryDetails>(writer, enumerable, enumerableName, "AccessControlEntryDetails", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C48\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C48\u003E__ToXml = new Action<XmlWriter, string, AccessControlEntryDetails>(AccessControlEntryDetails.ToXml)));
    }

    internal static void ToXml(
      XmlWriter writer,
      string enumerableName,
      IEnumerable<ServicingStepGroup> enumerable,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.EnumerableOfObjectToXml<ServicingStepGroup>(writer, enumerable, enumerableName, "ServicingStepGroup", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C68\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C68\u003E__ToXml = new Action<XmlWriter, string, ServicingStepGroup>(ServicingStepGroup.ToXml)));
    }

    internal static void ToXml(
      XmlWriter writer,
      string enumerableName,
      IEnumerable<TeamFoundationJobDefinition> enumerable,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.EnumerableOfObjectToXml<TeamFoundationJobDefinition>(writer, enumerable, enumerableName, "TeamFoundationJobDefinition", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C69\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C69\u003E__ToXml = new Action<XmlWriter, string, TeamFoundationJobDefinition>(TeamFoundationJobDefinition.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, IdentityDescriptor[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      IdentityDescriptor[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<IdentityDescriptor>(writer, array, arrayName, "IdentityDescriptor", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C64\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C64\u003E__ToXml = new Action<XmlWriter, string, IdentityDescriptor>(IdentityDescriptor.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, KeyValueOfStringString[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      KeyValueOfStringString[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<KeyValueOfStringString>(writer, array, arrayName, "KeyValueOfStringString", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C70\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C70\u003E__ToXml = new Action<XmlWriter, string, KeyValueOfStringString>(KeyValueOfStringString.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, LocationMapping[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      LocationMapping[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<LocationMapping>(writer, array, arrayName, "LocationMapping", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C71\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C71\u003E__ToXml = new Action<XmlWriter, string, LocationMapping>(LocationMapping.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, OutboundLinkType[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      OutboundLinkType[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<OutboundLinkType>(writer, array, arrayName, "OutboundLinkType", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C72\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C72\u003E__ToXml = new Action<XmlWriter, string, OutboundLinkType>(OutboundLinkType.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, PropertyValue[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      PropertyValue[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<PropertyValue>(writer, array, arrayName, "PropertyValue", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C73\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C73\u003E__ToXml = new Action<XmlWriter, string, PropertyValue>(PropertyValue.ToXml)));
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      RegistrationArtifactType[] array)
    {
      Helper.ToXml(writer, arrayName, array, false, false);
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      RegistrationArtifactType[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<RegistrationArtifactType>(writer, array, arrayName, "RegistrationArtifactType", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C74\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C74\u003E__ToXml = new Action<XmlWriter, string, RegistrationArtifactType>(RegistrationArtifactType.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, RegistrationDatabase[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      RegistrationDatabase[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<RegistrationDatabase>(writer, array, arrayName, "RegistrationDatabase", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C75\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C75\u003E__ToXml = new Action<XmlWriter, string, RegistrationDatabase>(RegistrationDatabase.ToXml)));
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      RegistrationExtendedAttribute2[] array)
    {
      Helper.ToXml(writer, arrayName, array, false, false);
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      RegistrationExtendedAttribute2[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<RegistrationExtendedAttribute2>(writer, array, arrayName, "RegistrationExtendedAttribute2", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C76\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C76\u003E__ToXml = new Action<XmlWriter, string, RegistrationExtendedAttribute2>(RegistrationExtendedAttribute2.ToXml)));
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      RegistrationServiceInterface[] array)
    {
      Helper.ToXml(writer, arrayName, array, false, false);
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      RegistrationServiceInterface[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<RegistrationServiceInterface>(writer, array, arrayName, "RegistrationServiceInterface", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C77\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C77\u003E__ToXml = new Action<XmlWriter, string, RegistrationServiceInterface>(RegistrationServiceInterface.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, RegistryAuditEntry[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      RegistryAuditEntry[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<RegistryAuditEntry>(writer, array, arrayName, "RegistryAuditEntry", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C78\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C78\u003E__ToXml = new Action<XmlWriter, string, RegistryAuditEntry>(RegistryAuditEntry.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, RegistryEntry[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      RegistryEntry[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<RegistryEntry>(writer, array, arrayName, "RegistryEntry", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C65\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C65\u003E__ToXml = new Action<XmlWriter, string, RegistryEntry>(RegistryEntry.ToXml)));
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      SecurityNamespaceDescription[] array)
    {
      Helper.ToXml(writer, arrayName, array, false, false);
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      SecurityNamespaceDescription[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<SecurityNamespaceDescription>(writer, array, arrayName, "SecurityNamespaceDescription", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C79\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C79\u003E__ToXml = new Action<XmlWriter, string, SecurityNamespaceDescription>(SecurityNamespaceDescription.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, ServiceDefinition[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      ServiceDefinition[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<ServiceDefinition>(writer, array, arrayName, "ServiceDefinition", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C80\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C80\u003E__ToXml = new Action<XmlWriter, string, ServiceDefinition>(ServiceDefinition.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, ServiceIdentity[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      ServiceIdentity[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<ServiceIdentity>(writer, array, arrayName, "ServiceIdentity", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C81\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C81\u003E__ToXml = new Action<XmlWriter, string, ServiceIdentity>(ServiceIdentity.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, ServiceTypeFilter[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      ServiceTypeFilter[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<ServiceTypeFilter>(writer, array, arrayName, "ServiceTypeFilter", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C82\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C82\u003E__ToXml = new Action<XmlWriter, string, ServiceTypeFilter>(ServiceTypeFilter.ToXml)));
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      ServicingExecutionHandlerData[] array)
    {
      Helper.ToXml(writer, arrayName, array, false, false);
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      ServicingExecutionHandlerData[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<ServicingExecutionHandlerData>(writer, array, arrayName, "ServicingExecutionHandlerData", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C83\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C83\u003E__ToXml = new Action<XmlWriter, string, ServicingExecutionHandlerData>(ServicingExecutionHandlerData.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, ServicingJobDetail[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      ServicingJobDetail[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<ServicingJobDetail>(writer, array, arrayName, "ServicingJobDetail", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C84\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C84\u003E__ToXml = new Action<XmlWriter, string, ServicingJobDetail>(ServicingJobDetail.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, ServicingStepDetail[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      ServicingStepDetail[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<ServicingStepDetail>(writer, array, arrayName, "ServicingStepDetail", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C85\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C85\u003E__ToXml = new Action<XmlWriter, string, ServicingStepDetail>(ServicingStepDetail.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, ServicingStepGroup[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      ServicingStepGroup[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<ServicingStepGroup>(writer, array, arrayName, "ServicingStepGroup", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C68\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C68\u003E__ToXml = new Action<XmlWriter, string, ServicingStepGroup>(ServicingStepGroup.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, ServicingStep[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      ServicingStep[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<ServicingStep>(writer, array, arrayName, "ServicingStep", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C86\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C86\u003E__ToXml = new Action<XmlWriter, string, ServicingStep>(ServicingStep.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, string[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      string[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<string>(writer, array, arrayName, "string", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C67\u003E__ToXmlElement ?? (Helper.\u003C\u003EO.\u003C67\u003E__ToXmlElement = new Action<XmlWriter, string, string>(XmlUtility.ToXmlElement)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, StrongBoxItemInfo[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      StrongBoxItemInfo[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<StrongBoxItemInfo>(writer, array, arrayName, "StrongBoxItemInfo", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C87\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C87\u003E__ToXml = new Action<XmlWriter, string, StrongBoxItemInfo>(StrongBoxItemInfo.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, Subscription[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      Subscription[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<Subscription>(writer, array, arrayName, "Subscription", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C88\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C88\u003E__ToXml = new Action<XmlWriter, string, Subscription>(Subscription.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, TeamFoundationIdentity[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      TeamFoundationIdentity[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<TeamFoundationIdentity>(writer, array, arrayName, "TeamFoundationIdentity", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C89\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C89\u003E__ToXml = new Action<XmlWriter, string, TeamFoundationIdentity>(TeamFoundationIdentity.ToXml)));
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      TeamFoundationJobDefinition[] array)
    {
      Helper.ToXml(writer, arrayName, array, false, false);
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      TeamFoundationJobDefinition[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<TeamFoundationJobDefinition>(writer, array, arrayName, "TeamFoundationJobDefinition", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C69\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C69\u003E__ToXml = new Action<XmlWriter, string, TeamFoundationJobDefinition>(TeamFoundationJobDefinition.ToXml)));
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      TeamFoundationJobHistoryEntry[] array)
    {
      Helper.ToXml(writer, arrayName, array, false, false);
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      TeamFoundationJobHistoryEntry[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<TeamFoundationJobHistoryEntry>(writer, array, arrayName, "TeamFoundationJobHistoryEntry", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C90\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C90\u003E__ToXml = new Action<XmlWriter, string, TeamFoundationJobHistoryEntry>(TeamFoundationJobHistoryEntry.ToXml)));
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      TeamFoundationJobSchedule[] array)
    {
      Helper.ToXml(writer, arrayName, array, false, false);
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      TeamFoundationJobSchedule[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<TeamFoundationJobSchedule>(writer, array, arrayName, "TeamFoundationJobSchedule", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C91\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C91\u003E__ToXml = new Action<XmlWriter, string, TeamFoundationJobSchedule>(TeamFoundationJobSchedule.ToXml)));
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      TeamFoundationRequestInformation[] array)
    {
      Helper.ToXml(writer, arrayName, array, false, false);
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      TeamFoundationRequestInformation[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<TeamFoundationRequestInformation>(writer, array, arrayName, "TeamFoundationRequestInformation", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C92\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C92\u003E__ToXml = new Action<XmlWriter, string, TeamFoundationRequestInformation>(TeamFoundationRequestInformation.ToXml)));
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      TeamFoundationServiceHostActivity[] array)
    {
      Helper.ToXml(writer, arrayName, array, false, false);
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      TeamFoundationServiceHostActivity[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<TeamFoundationServiceHostActivity>(writer, array, arrayName, "TeamFoundationServiceHostActivity", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C93\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C93\u003E__ToXml = new Action<XmlWriter, string, TeamFoundationServiceHostActivity>(TeamFoundationServiceHostActivity.ToXml)));
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      TeamProjectCollectionProperties[] array)
    {
      Helper.ToXml(writer, arrayName, array, false, false);
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      TeamProjectCollectionProperties[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<TeamProjectCollectionProperties>(writer, array, arrayName, "TeamProjectCollectionProperties", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C94\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C94\u003E__ToXml = new Action<XmlWriter, string, TeamProjectCollectionProperties>(TeamProjectCollectionProperties.ToXml)));
    }
  }
}
