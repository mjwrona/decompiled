// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.CustomerSupportRequest
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  [DataContract]
  public class CustomerSupportRequest
  {
    [DataMember]
    public string SourceLink { get; set; }

    [DataMember]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string DisplayName { get; set; }

    [DataMember]
    public string EmailId { get; set; }

    [DataMember]
    public string Reason { get; set; }

    [DataMember]
    public string Message { get; set; }

    [DataMember]
    public string ReporterVSID { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    [DataMember]
    public string ExtensionName { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    [DataMember]
    public string PublisherName { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    [DataMember]
    public Uri ExtensionURL { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    [DataMember]
    public Review Review { get; set; }

    [DefaultValue(null)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    [DataMember(Name = "reCaptchaToken")]
    public string ReCaptchaToken { get; set; }

    public void ValidateMandatoryRequestFields()
    {
      ArgumentUtility.CheckForNull<CustomerSupportRequest>(this, nameof (CustomerSupportRequest));
      ArgumentUtility.CheckEmailAddress(this.EmailId, "EmailId");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(this.Reason, "Reason");
      ArgumentUtility.CheckStringLength(this.Message, "Message", 5000, 50);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(this.SourceLink, "SourceLink");
    }

    public Stream ToJsonStream()
    {
      string str = JsonConvert.SerializeObject((object) this);
      MemoryStream jsonStream = new MemoryStream();
      StreamWriter streamWriter = new StreamWriter((Stream) jsonStream);
      streamWriter.Write(str);
      streamWriter.Flush();
      jsonStream.Position = 0L;
      return (Stream) jsonStream;
    }
  }
}
