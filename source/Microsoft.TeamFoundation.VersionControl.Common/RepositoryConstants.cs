// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.RepositoryConstants
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  public static class RepositoryConstants
  {
    public const string AuthenticatedUser = ".";
    public const int EncodingBinary = -1;
    public const int EncodingUnchanged = -2;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const int EncodingFolder = -3;
    public const int MaxChangesetCommentSize = 1073741823;
    public const int MaxCommentSize = 1073741823;
    public const int MaxLabelCommentSize = 2048;
    public const int MaxComputerNameSize = 31;
    public const int MaxWorkspaceNameSize = 64;
    public const int MaxShelvesetNameSize = 64;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const int MinFileNameReserveSize = 12;
    public const int MaxServerPathSize = 399;
    public const int MaxServerPathWithGuidSize = 434;
    public const int ComponentReserveSize = 3;
    public const int MaxLocalPathSize = 259;
    public const int MaxLocalPathComponentSize = 256;
    public const int MaxIdentityNameSize = 256;
    public const int MaxLabelNameSize = 64;
    public const int MaxPolicyNameSize = 256;
    public const int MaxPolicyOverrideCommentSize = 2048;
    public const int MaxPropertyNameSize = 128;
    public const int CheckinNoteNameSize = 64;
    public const string NoCICheckInComment = "***NO_CI***";
    public const string NoTriggerCheckInComment = "//***NO_CI***//";
    public const int IndeterminateChangeset = -1;
    private const string PublicDir = "/public";
    internal const string SchemaDir = "/public/schemas";
    private const string PublicAppDir = "/public/application";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string TransformDir = "/V1.0/Transforms";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string TransformLcidDir = "/V1.0/Transforms/{0}";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string AppDir = "/V1.0";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string UrlSuffix = "/V1.0/repository.asmx";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string AdminUrlSuffix = "/V1.0/Administration.asmx";
    internal const string UploadUrlSuffix = "/V1.0/upload.asmx";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string ProxyStatisticsUrlSuffix = "/V1.0/ProxyStatistics.asmx";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const int ProxyResponseTimeout = 60000;
    internal const string ServiceDefinitionPath = "~/public/application/ServiceDefinition.aspx";
    public const string IBISEnablement = "IBISEnablement";
    public const string LinkingProviderInterfaceName = "LinkingProviderService";
    public const string ISCCProvider = "ISCCProvider";
    public static readonly Guid ISCCProviderServiceIdentifier = new Guid("b2b178f5-bef9-460d-a5cf-35bcc0281cc4");
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string ISCCProviderServiceIdentifierString = "b2b178f5-bef9-460d-a5cf-35bcc0281cc4";
    public const string ISCCAdmin = "ISCCAdmin";
    public static readonly Guid ISCCAdminServiceIdentifier = new Guid("0ade2b5a-efa4-419e-bf11-24f7cfe7c1a2");
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string ISCCAdminServiceIdentifierString = "0ade2b5a-efa4-419e-bf11-24f7cfe7c1a2";
    public const string ISCCProvider3 = "ISCCProvider3";
    public static readonly Guid ISCCProvider3ServiceIdentifier = new Guid("ec9b0153-ee54-450e-b6e0-664ecb033c99");
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string ISCCProvider3ServiceIdentifierString = "ec9b0153-ee54-450e-b6e0-664ecb033c99";
    public const string ISCCProvider4 = "ISCCProvider4";
    public static readonly Guid ISCCProvider4ServiceIdentifier = new Guid("FA9FCC37-F9BD-496F-A1B8-CE351F6BFE8A");
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string ISCCProvider4ServiceIdentifierString = "FA9FCC37-F9BD-496F-A1B8-CE351F6BFE8A";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string ISCCProvider4Dot1 = "ISCCProvider4.1";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly Guid ISCCProvider4Dot1ServiceIdentifier = new Guid("C8926592-E3D0-4F4F-BBE5-9F52EDF5103E");
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string ISCCProvider4Dot2 = "ISCCProvider4.2";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly Guid ISCCProvider4Dot2ServiceIdentifier = new Guid("CEEE60A4-39FD-4013-8B33-5DC5BA4A6BD9");
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string ISCCProvider4Dot3 = "ISCCProvider4.3";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly Guid ISCCProvider4Dot3ServiceIdentifier = new Guid("71900729-7BE6-45CA-923D-3B00AA97DAE8");
    public const string ISCCProvider5 = "ISCCProvider5";
    public static readonly Guid ISCCProvider5ServiceIdentifier = new Guid("A25D0656-DA63-4F51-9DA9-800FFF229D1A");
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string ISCCProvider5ServiceIdentifierString = "A25D0656-DA63-4F51-9DA9-800FFF229D1A";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string ISCCProvider5Dot1 = "ISCCProvider5.1";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly Guid ISCCProvider5Dot1ServiceIdentifier = new Guid("54EB89EB-36D1-46AD-85C1-84EB5E8C7DE7");
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string ISCCProvider5Dot2 = "ISCCProvider5.2";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly Guid ISCCProvider5Dot2ServiceIdentifier = new Guid("96793C43-C34D-4AAD-8B2F-218EF3B46B8A");
    public const string IProjectMaintenance = "IProjectMaintenance";
    public const string Download = "Download";
    public static readonly Guid DownloadServiceIdentifier = new Guid("29b91065-1314-41d5-ab70-0bfa9896a51d");
    public const string Upload = "Upload";
    public static readonly Guid UploadServiceIdentifier = new Guid("1c04c122-7ad1-4f02-87ba-979b9d278bee");
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly Guid InitialPendingChangesSignature = new Guid("B2140B25-F70A-4B4D-BFB1-184703037010");
    internal static readonly Guid LocalVersionReplayRequiredSignature = new Guid("87DA1886-6E87-461F-A156-F8C745FFA2EA");
    public const string ExceptionHeader = "X-VersionControl-Exception";
    internal const string InstanceHeader = "X-VersionControl-Instance";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string RepositoryHashHeader = "X-TFS-RH";
    internal const string SHA1AlgorithmIdentifier = "SHA1";
    internal const string NormalizedSignatureQueryString = "{0}={1}&{2}={3}";
    public const string ServerItemField = "item";
    public const string WorkspaceNameField = "wsname";
    public const string WorkspaceOwnerField = "wsowner";
    public const string ModifiedDateField = "mod";
    public const string HashField = "hash";
    public const string ContentField = "content";
    public const string LengthField = "filelength";
    public const string RangeField = "range";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string CheckinPoliciesAnnotation = "CheckinPolicies";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string ExclusiveCheckoutAnnotation = "ExclusiveCheckout";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string GetLatestOnCheckoutAnnotation = "GetLatestOnCheckout";
    internal const int DestroyedFileId = 1023;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const int WorkItemLinkTypeMask = 1024;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const int WorkItemToResolveLinkType = 1025;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const int WorkItemToAssociateLinkType = 1026;
    public static readonly int SourceControlCapabilityFlag = 1;
  }
}
