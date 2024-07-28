// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ContentValidation.TokenizeableContentItem
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Ops.Cvs.Client.DataContracts;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Cloud.ContentValidation
{
  [DataContract]
  internal class TokenizeableContentItem
  {
    [JsonConstructor]
    public TokenizeableContentItem()
    {
    }

    public TokenizeableContentItem(ContentItem ci)
      : this()
    {
      this.ExternalId = JsonConvert.DeserializeObject<ContentValidationExternalId>(ci.ExternalId);
      this.ContentType = ci.ContentType.ToString();
      this.ReporteeName = ci.ReporteeName;
      this.ReporteeAddress = ci.ReporteeAddress;
    }

    [DataMember(Order = 1)]
    public ContentValidationExternalId ExternalId { get; set; }

    [DataMember(Order = 2)]
    public string ContentType { get; set; }

    [DataMember(Order = 3)]
    public string ReporteeName { get; set; }

    [DataMember(Order = 4)]
    public string ReporteeAddress { get; set; }

    public byte[] AsEncodedMessage()
    {
      if (this.ExternalId.Token != null)
        throw new InvalidOperationException("ExternalId.Token needs to be removed before the message can be encoded or validated");
      return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object) this));
    }
  }
}
