// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProxyConstants
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ProxyConstants
  {
    public const string TimestampQueryString = "ts";
    public const string RequestedFileIdQueryString = "fid";
    public const string SignatureFileIdsQueryString = "sfid";
    public const string SignatureQueryString = "s";
    public const string RepositoryIdQueryString = "rid";
    public const string DownloadTicketType = "type";
    public const string RSATicketType = "rsa";
    public const string MD5TicketType = "md5";
    public const string InstanceIdQueryString = "iid";
    public const string CollectionPathQueryString = "cp";
    public const string ExceptionHeader = "X-VersionControl-Exception";
    public const string ProxyDownloadUrlSuffix = "/V1.0/item.asmx";
    public const string DownloadUrlSuffix = "/V4.0/item.asmx";
    public const string AppDir = "/V4.0";
    public const string ProxyAppDir = "/V1.0";
    public const string GenericDownloadService = "FileDownloadService";
    public const string NormalizedSignatureQueryString = "{0}={1}&{2}={3}";
    public static readonly Guid GenericDownloadServiceIdentifier = new Guid("17253FDD-B035-4AFF-9D4A-DEEC68CF7E59");
    public static readonly Guid ProxySigningKey = new Guid("03B91A76-9007-46B8-BD51-584E49326494");
  }
}
