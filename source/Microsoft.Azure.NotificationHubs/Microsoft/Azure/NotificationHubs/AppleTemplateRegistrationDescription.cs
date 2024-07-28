// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.AppleTemplateRegistrationDescription
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
  [AmqpContract(Code = 3)]
  [DataContract(Name = "AppleTemplateRegistrationDescription", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public class AppleTemplateRegistrationDescription : AppleRegistrationDescription
  {
    internal override string AppPlatForm => "apple";

    internal override string RegistrationType => "template";

    internal override string PlatformType => "appletemplate";

    public AppleTemplateRegistrationDescription(
      AppleTemplateRegistrationDescription sourceRegistration)
      : base((AppleRegistrationDescription) sourceRegistration)
    {
      this.BodyTemplate = sourceRegistration.BodyTemplate;
      this.Expiry = sourceRegistration.Expiry;
      this.TemplateName = sourceRegistration.TemplateName;
      this.Priority = sourceRegistration.Priority;
    }

    public AppleTemplateRegistrationDescription(string deviceToken)
      : base(deviceToken)
    {
    }

    public AppleTemplateRegistrationDescription(string deviceToken, string jsonPayload)
      : this(string.Empty, deviceToken, jsonPayload, (IEnumerable<string>) null)
    {
    }

    public AppleTemplateRegistrationDescription(
      string deviceToken,
      string jsonPayload,
      IEnumerable<string> tags)
      : this(string.Empty, deviceToken, jsonPayload, tags)
    {
    }

    internal AppleTemplateRegistrationDescription(
      string notificationHubPath,
      string deviceToken,
      string jsonPayload,
      IEnumerable<string> tags)
      : base(notificationHubPath, deviceToken, tags)
    {
      this.BodyTemplate = !string.IsNullOrWhiteSpace(jsonPayload) ? new CDataMember(jsonPayload) : throw new ArgumentNullException(nameof (jsonPayload));
    }

    [AmqpMember(Mandatory = true, Order = 4)]
    [DataMember(Name = "BodyTemplate", IsRequired = true, Order = 3001)]
    public CDataMember BodyTemplate { get; set; }

    [AmqpMember(Mandatory = false, Order = 5)]
    [DataMember(Name = "Expiry", IsRequired = false, Order = 3002)]
    public string Expiry { get; set; }

    [AmqpMember(Mandatory = false, Order = 6)]
    [DataMember(Name = "TemplateName", IsRequired = false, Order = 3003)]
    public string TemplateName { get; set; }

    [AmqpMember(Mandatory = false, Order = 7)]
    [DataMember(Name = "Priority", IsRequired = false, Order = 3004, EmitDefaultValue = false)]
    public string Priority { get; set; }

    internal override void OnValidate(bool allowLocalMockPns, ApiVersion version)
    {
      base.OnValidate(allowLocalMockPns, version);
      if (this.Expiry != null)
      {
        if (this.Expiry == string.Empty)
          throw new InvalidDataContractException(SRClient.EmptyExpiryValue);
        if (ExpressionEvaluator.Validate(this.Expiry, version) == ExpressionEvaluator.ExpressionType.Literal && !DateTime.TryParse(this.Expiry, out DateTime _) && !string.Equals("0", this.Expiry, StringComparison.OrdinalIgnoreCase))
          throw new InvalidDataContractException(SRClient.ExpiryDeserializationError);
      }
      if (this.Priority != null)
      {
        if (this.Priority == string.Empty)
          throw new InvalidDataContractException(SRClient.EmptyPriorityValue);
        if (ExpressionEvaluator.Validate(this.Priority, version) == ExpressionEvaluator.ExpressionType.Literal && !byte.TryParse(this.Priority, out byte _))
          throw new InvalidDataContractException(SRClient.PriorityDeserializationError);
      }
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

    private void ValidateTemplateName()
    {
      if (this.TemplateName != null && this.TemplateName.Length > 200)
        throw new InvalidDataContractException(SRClient.TemplateNameLengthExceedsLimit((object) 200));
    }

    internal override RegistrationDescription Clone() => (RegistrationDescription) new AppleTemplateRegistrationDescription(this);
  }
}
