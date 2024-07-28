// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.AdmTemplateRegistrationDescription
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
  [AmqpContract(Code = 12)]
  [DataContract(Name = "AdmTemplateRegistrationDescription", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public class AdmTemplateRegistrationDescription : AdmRegistrationDescription
  {
    public AdmTemplateRegistrationDescription(
      AdmTemplateRegistrationDescription sourceRegistration)
      : base((AdmRegistrationDescription) sourceRegistration)
    {
      this.BodyTemplate = sourceRegistration.BodyTemplate;
      this.TemplateName = sourceRegistration.TemplateName;
    }

    public AdmTemplateRegistrationDescription(string admRegistrationId)
      : base(admRegistrationId)
    {
    }

    public AdmTemplateRegistrationDescription(string admRegistrationId, string jsonPayload)
      : this(string.Empty, admRegistrationId, jsonPayload, (IEnumerable<string>) null)
    {
    }

    public AdmTemplateRegistrationDescription(
      string admRegistrationId,
      string jsonPayload,
      IEnumerable<string> tags)
      : this(string.Empty, admRegistrationId, jsonPayload, tags)
    {
    }

    internal AdmTemplateRegistrationDescription(
      string notificationHubPath,
      string admRegistrationId,
      string jsonPayload,
      IEnumerable<string> tags)
      : base(notificationHubPath, admRegistrationId, tags)
    {
      this.BodyTemplate = !string.IsNullOrWhiteSpace(jsonPayload) ? new CDataMember(jsonPayload) : throw new ArgumentNullException(nameof (jsonPayload));
    }

    [AmqpMember(Mandatory = true, Order = 4)]
    [DataMember(Name = "BodyTemplate", IsRequired = true, Order = 3001)]
    public CDataMember BodyTemplate { get; set; }

    [AmqpMember(Mandatory = false, Order = 5)]
    [DataMember(Name = "TemplateName", IsRequired = false, Order = 3002)]
    public string TemplateName { get; set; }

    internal override string AppPlatForm => "adm";

    internal override string RegistrationType => "template";

    internal override string PlatformType => "admtemplate";

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

    internal override RegistrationDescription Clone() => (RegistrationDescription) new AdmTemplateRegistrationDescription(this);

    private void ValidateTemplateName()
    {
      if (this.TemplateName != null && this.TemplateName.Length > 200)
        throw new InvalidDataContractException(SRClient.TemplateNameLengthExceedsLimit((object) 200));
    }
  }
}
