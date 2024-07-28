// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.EmailNotification.EmailInteraction
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Newtonsoft.Json;
using System;
using System.Text;
using System.Web;
using System.Web.Http.ModelBinding;

namespace Microsoft.VisualStudio.Services.EmailNotification
{
  [ModelBinder(typeof (EmailInteractionBinder))]
  public class EmailInteraction
  {
    public Guid EmailId { get; set; }

    public string EmailType { get; set; }

    public string EmailVariation { get; set; }

    public string InteractionId { get; set; }

    public InteractionType InteractionType { get; set; }

    public string MetaData { get; set; }

    public EmailInteraction() => this.MetaData = string.Empty;

    public override bool Equals(object obj) => obj != null && !(this.GetType() != obj.GetType()) && obj is EmailInteraction emailInteraction && this.EmailId == emailInteraction.EmailId && this.EmailType == emailInteraction.EmailType && this.EmailVariation == emailInteraction.EmailVariation && this.InteractionId == emailInteraction.InteractionId && this.InteractionType == emailInteraction.InteractionType && this.MetaData == emailInteraction.MetaData;

    public override int GetHashCode() => this.EmailId.GetHashCode() ^ this.EmailType.GetHashCode() ^ this.EmailVariation.GetHashCode() ^ this.InteractionId.GetHashCode() ^ this.InteractionType.GetHashCode() ^ this.MetaData.GetHashCode();

    public static string ToBase64Context(EmailInteraction interaction) => HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object) interaction)));

    public static EmailInteraction FromBase64Context(string context) => JsonConvert.DeserializeObject<EmailInteraction>(Encoding.UTF8.GetString(HttpServerUtility.UrlTokenDecode(context)));
  }
}
