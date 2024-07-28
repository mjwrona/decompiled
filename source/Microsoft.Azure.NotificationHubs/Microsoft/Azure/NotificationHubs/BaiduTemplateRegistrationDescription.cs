// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.BaiduTemplateRegistrationDescription
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Azure.NotificationHubs
{
  [AmqpContract(Code = 16)]
  [DataContract(Name = "BaiduTemplateRegistrationDescription", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public class BaiduTemplateRegistrationDescription : BaiduRegistrationDescription
  {
    public BaiduTemplateRegistrationDescription(
      BaiduTemplateRegistrationDescription sourceRegistration)
      : base((BaiduRegistrationDescription) sourceRegistration)
    {
      this.BodyTemplate = sourceRegistration.BodyTemplate;
      this.TemplateName = sourceRegistration.TemplateName;
      this.MessageType = sourceRegistration.MessageType;
    }

    public BaiduTemplateRegistrationDescription(string baiduUserId, string baiduChannelId)
      : base(baiduUserId, baiduChannelId, (IEnumerable<string>) null)
    {
    }

    public BaiduTemplateRegistrationDescription(
      string baiduUserId,
      string baiduChannelId,
      string jsonPayload)
      : this(string.Empty, baiduUserId, baiduChannelId, jsonPayload, (IEnumerable<string>) null, new int?())
    {
    }

    public BaiduTemplateRegistrationDescription(
      string baiduUserId,
      string baiduChannelId,
      string jsonPayload,
      IEnumerable<string> tags)
      : this(string.Empty, baiduUserId, baiduChannelId, jsonPayload, tags, new int?())
    {
    }

    public BaiduTemplateRegistrationDescription(
      string baiduUserId,
      string baiduChannelId,
      string jsonPayload,
      int messageType)
      : this(string.Empty, baiduUserId, baiduChannelId, jsonPayload, (IEnumerable<string>) null, new int?(messageType))
    {
    }

    public BaiduTemplateRegistrationDescription(
      string baiduUserId,
      string baiduChannelId,
      string jsonPayload,
      IEnumerable<string> tags,
      int messageType)
      : this(string.Empty, baiduUserId, baiduChannelId, jsonPayload, tags, new int?(messageType))
    {
    }

    internal BaiduTemplateRegistrationDescription(
      string notificationHubPath,
      string baiduUserId,
      string baiduChannelId,
      string jsonPayload,
      IEnumerable<string> tags,
      int? messageType)
      : base(notificationHubPath, baiduUserId, baiduChannelId, tags)
    {
      if (string.IsNullOrWhiteSpace(jsonPayload))
        throw new ArgumentNullException(nameof (jsonPayload));
      this.MessageType = messageType;
      this.BodyTemplate = new CDataMember(jsonPayload);
    }

    [AmqpMember(Mandatory = true, Order = 5)]
    [DataMember(Name = "BodyTemplate", IsRequired = true, Order = 3001)]
    public CDataMember BodyTemplate { get; set; }

    [AmqpMember(Mandatory = false, Order = 6)]
    [DataMember(Name = "TemplateName", IsRequired = false, Order = 3002)]
    public string TemplateName { get; set; }

    [AmqpMember(Mandatory = false, Order = 7)]
    [DataMember(Name = "MessageType", IsRequired = false, Order = 3003, EmitDefaultValue = false)]
    public int? MessageType { get; set; }

    internal override string AppPlatForm => "baidu";

    internal override string RegistrationType => "template";

    internal override string PlatformType => "baidutemplate";

    internal override void OnValidate(bool allowLocalMockPns, ApiVersion version)
    {
      base.OnValidate(allowLocalMockPns, version);
      try
      {
        using (XmlReader jsonReader = (XmlReader) JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes((string) this.BodyTemplate), new XmlDictionaryReaderQuotas()))
        {
          foreach (XElement xelement in XDocument.Load(jsonReader).Root.DescendantsAndSelf())
          {
            foreach (XAttribute attribute in xelement.Attributes())
            {
              int num = (int) ExpressionEvaluator.Validate(attribute.Value, version);
            }
            if (!xelement.HasElements && !string.IsNullOrEmpty(xelement.Value))
            {
              int num1 = (int) ExpressionEvaluator.Validate(xelement.Value, version);
            }
          }
        }
      }
      catch (InvalidOperationException ex)
      {
        throw new XmlException(SRClient.FailedToDeserializeBodyTemplate);
      }
      this.ValidateTemplateName();
    }

    internal override RegistrationDescription Clone() => (RegistrationDescription) new BaiduTemplateRegistrationDescription(this);

    private void ValidateTemplateName()
    {
      if (this.TemplateName != null && this.TemplateName.Length > 200)
        throw new InvalidDataContractException(SRClient.TemplateNameLengthExceedsLimit((object) 200));
    }
  }
}
