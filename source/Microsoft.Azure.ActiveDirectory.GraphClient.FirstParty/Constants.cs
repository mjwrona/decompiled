// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.Constants
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  public class Constants
  {
    public const string AadGraphEndpointFormat = "https://{0}/{1}";
    public const string QueryParameterNameApiVersion = "api-version";
    public const string DefaultGraphApiDomainName = "graph.windows.net";
    public const int MaxRetryAttempts = 10;
    public const string HeaderAadClientRequestId = "client-request-id";
    public const string PreferHeaderName = "Prefer";
    public const string PreferHeaderValue = "return-content";
    public const string HeaderAadRequestId = "request-id";
    public const string HeaderAadServerName = "ocp-aad-diagnostics-server-name";
    public const string UserAgentString = "Microsoft Azure Graph Client Library 1.0";
    public const string HeaderShortLivedToken = "ShortLivedToken";
    public const string ClaimNameTenantId = "tid";
    public const string OdataTypeKey = "odata.type";
    public const string ODataNextLink = "odata.nextLink";
    public const string ODataMetadataKey = "odata.metadata";
    public const string ODataValues = "value";
    public const string ODataErrorKey = "odata.error";
    public const string ODataCodeKey = "code";
    public const string ODataMessageKey = "message";
    public const string MinimalMetadataContentType = "application/json;odata=minimalmetadata";
    public const string FilterOperator = "$filter";
    public const string TopOperator = "$top";
    public const string ExpandOperator = "$expand";
    public const string OrderByOperator = "$orderby";
    public const string SelectOperator = "$select";
    public const string LinksFragment = "$links";
    public const string CommonTenantName = "myorganization";
    public const string ActionGetMemberGroups = "getMemberGroups";
    public const string ActionCheckMemberGroups = "checkMemberGroups";
    public const string ActionAssignLicense = "assignLicense";
    public const string ActionIsMemberOf = "isMemberOf";
    public const string ActionRestoreApplication = "restore";
    public static string DefaultApiVersion = "1.5";

    static Constants() => Constants.DefaultApiVersion = "1.5-internal";
  }
}
