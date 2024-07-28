// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.PageToken
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [DataContract]
  public class PageToken
  {
    public static readonly long NoMoreDataId = -1;

    public PageToken()
    {
    }

    public PageToken(long token, int? pageSize)
    {
      this.Token = PageToken.ConvertToToken(token);
      this.PageSize = pageSize;
    }

    [DataMember]
    public byte[] Token { get; set; }

    [DataMember]
    public int? PageSize { get; set; }

    public static byte[] ConvertToToken(long pageToken) => BitConverter.GetBytes(pageToken);

    public static long ConvertToLong(byte[] byteToken) => BitConverter.ToInt64(byteToken, 0);
  }
}
