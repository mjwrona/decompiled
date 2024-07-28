// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataMessageExtensions
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData
{
  public static class ODataMessageExtensions
  {
    public static ODataVersion GetODataVersion(
      this IODataResponseMessage message,
      ODataVersion defaultVersion)
    {
      return ODataUtilsInternal.GetODataVersion((ODataMessage) new ODataResponseMessage(message, false, false, long.MaxValue), defaultVersion);
    }

    public static ODataVersion GetODataVersion(
      this IODataRequestMessage message,
      ODataVersion defaultVersion)
    {
      return ODataUtilsInternal.GetODataVersion((ODataMessage) new ODataRequestMessage(message, false, false, long.MaxValue), defaultVersion);
    }

    public static ODataPreferenceHeader PreferHeader(this IODataRequestMessage requestMessage)
    {
      ExceptionUtils.CheckArgumentNotNull<IODataRequestMessage>(requestMessage, nameof (requestMessage));
      return new ODataPreferenceHeader(requestMessage);
    }

    public static ODataPreferenceHeader PreferenceAppliedHeader(
      this IODataResponseMessage responseMessage)
    {
      ExceptionUtils.CheckArgumentNotNull<IODataResponseMessage>(responseMessage, nameof (responseMessage));
      return new ODataPreferenceHeader(responseMessage);
    }
  }
}
