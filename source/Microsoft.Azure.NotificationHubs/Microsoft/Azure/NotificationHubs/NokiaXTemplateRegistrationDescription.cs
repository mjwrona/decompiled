// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.NokiaXTemplateRegistrationDescription
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
  [AmqpContract(Code = 14)]
  [DataContract(Name = "NokiaXTemplateRegistrationDescription", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  internal class NokiaXTemplateRegistrationDescription : NokiaXRegistrationDescription
  {
    public NokiaXTemplateRegistrationDescription(
      NokiaXTemplateRegistrationDescription sourceRegistration)
      : base((NokiaXRegistrationDescription) sourceRegistration)
    {
      this.BodyTemplate = sourceRegistration.BodyTemplate;
      this.TemplateName = sourceRegistration.TemplateName;
    }

    public NokiaXTemplateRegistrationDescription(string nokiaXRegistrationId)
      : base(nokiaXRegistrationId)
    {
    }

    public NokiaXTemplateRegistrationDescription(string nokiaXRegistrationId, string jsonPayload)
      : this(string.Empty, nokiaXRegistrationId, jsonPayload, (IEnumerable<string>) null)
    {
    }

    public NokiaXTemplateRegistrationDescription(
      string nokiaXRegistrationId,
      string jsonPayload,
      IEnumerable<string> tags)
      : this(string.Empty, nokiaXRegistrationId, jsonPayload, tags)
    {
    }

    internal NokiaXTemplateRegistrationDescription(
      string notificationHubPath,
      string nokiaXRegistrationId,
      string jsonPayload,
      IEnumerable<string> tags)
      : base(notificationHubPath, nokiaXRegistrationId, tags)
    {
      this.BodyTemplate = !string.IsNullOrWhiteSpace(jsonPayload) ? new CDataMember(jsonPayload) : throw new ArgumentNullException(nameof (jsonPayload));
    }

    [AmqpMember(Mandatory = true, Order = 4)]
    [DataMember(Name = "BodyTemplate", IsRequired = true, Order = 3001)]
    public CDataMember BodyTemplate { get; set; }

    [AmqpMember(Mandatory = false, Order = 5)]
    [DataMember(Name = "TemplateName", IsRequired = false, Order = 3002)]
    public string TemplateName { get; set; }

    internal override string AppPlatForm => "nokiax";

    internal override string RegistrationType => "template";

    internal override string PlatformType => "nokiaxtemplate";

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

    internal override RegistrationDescription Clone() => (RegistrationDescription) new NokiaXTemplateRegistrationDescription(this);

    private void ValidateTemplateName()
    {
      if (this.TemplateName != null && this.TemplateName.Length > 200)
        throw new InvalidDataContractException(SRClient.TemplateNameLengthExceedsLimit((object) 200));
    }
  }
}
