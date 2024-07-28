// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTrackingFaultCodes
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public sealed class WorkItemTrackingFaultCodes
  {
    private WorkItemTrackingFaultCodes()
    {
    }

    public static SoapFaultSubCode Unknown => new SoapFaultSubCode(new XmlQualifiedName(nameof (Unknown), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode Security => new SoapFaultSubCode(new XmlQualifiedName(nameof (Security), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode Service => new SoapFaultSubCode(new XmlQualifiedName(nameof (Service), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode Application => new SoapFaultSubCode(new XmlQualifiedName(nameof (Application), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode ArgumentException => new SoapFaultSubCode(new XmlQualifiedName(nameof (ArgumentException), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode ArgumentNullException => new SoapFaultSubCode(new XmlQualifiedName(nameof (ArgumentNullException), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode BadServerConfig => new SoapFaultSubCode(new XmlQualifiedName(nameof (BadServerConfig), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode MissingBackendServerName => new SoapFaultSubCode(new XmlQualifiedName(nameof (MissingBackendServerName), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode MissingBackendDBName => new SoapFaultSubCode(new XmlQualifiedName(nameof (MissingBackendDBName), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode MissingAttachmentsServerName => new SoapFaultSubCode(new XmlQualifiedName(nameof (MissingAttachmentsServerName), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode MissingAttachmentsDatabaseName => new SoapFaultSubCode(new XmlQualifiedName(nameof (MissingAttachmentsDatabaseName), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode MissingMaxAttachmentsSize => new SoapFaultSubCode(new XmlQualifiedName(nameof (MissingMaxAttachmentsSize), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode MaxAttachmentsSizeExceeded => new SoapFaultSubCode(new XmlQualifiedName(nameof (MaxAttachmentsSizeExceeded), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode Unexpected => new SoapFaultSubCode(new XmlQualifiedName(nameof (Unexpected), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode UnknownMetadataTable => new SoapFaultSubCode(new XmlQualifiedName(nameof (UnknownMetadataTable), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode InvalidRowVersion => new SoapFaultSubCode(new XmlQualifiedName(nameof (InvalidRowVersion), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode NoMetadataTablesSpecified => new SoapFaultSubCode(new XmlQualifiedName(nameof (NoMetadataTablesSpecified), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode NoWorkItemsRequested => new SoapFaultSubCode(new XmlQualifiedName(nameof (NoWorkItemsRequested), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode NoColumnsRequested => new SoapFaultSubCode(new XmlQualifiedName(nameof (NoColumnsRequested), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode NoQueryXml => new SoapFaultSubCode(new XmlQualifiedName(nameof (NoQueryXml), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode InvalidRequestId => new SoapFaultSubCode(new XmlQualifiedName(nameof (InvalidRequestId), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode SortColumnNameMissing => new SoapFaultSubCode(new XmlQualifiedName(nameof (SortColumnNameMissing), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode BisSyncGroupsAndUsersInProgress => new SoapFaultSubCode(new XmlQualifiedName(nameof (BisSyncGroupsAndUsersInProgress), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode BisSyncGroupsAndUsersFailed => new SoapFaultSubCode(new XmlQualifiedName(nameof (BisSyncGroupsAndUsersFailed), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode CallerNotServiceAccount => new SoapFaultSubCode(new XmlQualifiedName(nameof (CallerNotServiceAccount), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode CallerNotHavePermissions => new SoapFaultSubCode(new XmlQualifiedName(nameof (CallerNotHavePermissions), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode MissingAclSyncFrequency => new SoapFaultSubCode(new XmlQualifiedName(nameof (MissingAclSyncFrequency), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode MissingUserGroupSyncFrequency => new SoapFaultSubCode(new XmlQualifiedName(nameof (MissingUserGroupSyncFrequency), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode MissingCssSyncFrequency => new SoapFaultSubCode(new XmlQualifiedName(nameof (MissingCssSyncFrequency), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode InvalidAclSyncFrequency => new SoapFaultSubCode(new XmlQualifiedName(nameof (InvalidAclSyncFrequency), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode InvalidUsersGroupsSyncFrequency => new SoapFaultSubCode(new XmlQualifiedName(nameof (InvalidUsersGroupsSyncFrequency), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode InvalidCssSyncFrequency => new SoapFaultSubCode(new XmlQualifiedName(nameof (InvalidCssSyncFrequency), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode ServiceAccountNotFound => new SoapFaultSubCode(new XmlQualifiedName(nameof (ServiceAccountNotFound), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));

    public static SoapFaultSubCode UserNotFound => new SoapFaultSubCode(new XmlQualifiedName(nameof (UserNotFound), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03"));
  }
}
