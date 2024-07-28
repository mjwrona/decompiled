// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.IcmFault
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [DataContract]
  public class IcmFault
  {
    public IcmFault()
    {
    }

    public IcmFault(string message, string code)
    {
      this.Message = message;
      this.Code = code;
    }

    public IcmFault(string message, string code, bool isClientError)
      : this(message, code)
    {
      this.IsClientError = isClientError;
    }

    [DataMember]
    public string Message { get; set; }

    [DataMember]
    public string Code { get; set; }

    [DataMember]
    public string ErrorDetail { get; set; }

    [IgnoreDataMember]
    public bool IsClientError { get; set; }
  }
}
