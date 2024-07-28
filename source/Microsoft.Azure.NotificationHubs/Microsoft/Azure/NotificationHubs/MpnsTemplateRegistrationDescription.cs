// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.MpnsTemplateRegistrationDescription
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Azure.NotificationHubs
{
  [AmqpContract(Code = 8)]
  [DataContract(Name = "MpnsTemplateRegistrationDescription", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public class MpnsTemplateRegistrationDescription : MpnsRegistrationDescription
  {
    internal override string AppPlatForm => "windowsphone";

    internal override string RegistrationType => "template";

    internal override string PlatformType => "windowsphonetemplate";

    public MpnsTemplateRegistrationDescription(
      MpnsTemplateRegistrationDescription sourceRegistration)
      : base((MpnsRegistrationDescription) sourceRegistration)
    {
      this.MpnsHeaders = new MpnsHeaderCollection();
      if (sourceRegistration.MpnsHeaders != null)
      {
        foreach (KeyValuePair<string, string> mpnsHeader in (SortedDictionary<string, string>) sourceRegistration.MpnsHeaders)
          this.MpnsHeaders.Add(mpnsHeader.Key, mpnsHeader.Value);
      }
      this.BodyTemplate = sourceRegistration.BodyTemplate;
      this.TemplateName = sourceRegistration.TemplateName;
    }

    public MpnsTemplateRegistrationDescription(Uri channelUri)
      : base(channelUri)
    {
    }

    public MpnsTemplateRegistrationDescription(Uri channelUri, string templatePayload)
      : this(string.Empty, channelUri, templatePayload, (IDictionary<string, string>) null, (IEnumerable<string>) null)
    {
    }

    public MpnsTemplateRegistrationDescription(Uri channelUri, XmlDocument xmlTemplate)
      : this(string.Empty, channelUri, xmlTemplate.InnerXml, (IDictionary<string, string>) null, (IEnumerable<string>) null)
    {
    }

    public MpnsTemplateRegistrationDescription(
      Uri channelUri,
      string templatePayload,
      IDictionary<string, string> mpnsHeaders)
      : this(string.Empty, channelUri, templatePayload, mpnsHeaders, (IEnumerable<string>) null)
    {
    }

    public MpnsTemplateRegistrationDescription(
      Uri channelUri,
      string templatePayload,
      IEnumerable<string> tags)
      : this(string.Empty, channelUri, templatePayload, (IDictionary<string, string>) null, tags)
    {
    }

    public MpnsTemplateRegistrationDescription(
      Uri channelUri,
      string templatePayload,
      IDictionary<string, string> mpnsHeaders,
      IEnumerable<string> tags)
      : this(string.Empty, channelUri, templatePayload, mpnsHeaders, tags)
    {
    }

    public MpnsTemplateRegistrationDescription(string channelUri)
      : base(new Uri(channelUri))
    {
    }

    public MpnsTemplateRegistrationDescription(string channelUri, string templatePayload)
      : this(string.Empty, new Uri(channelUri), templatePayload, (IDictionary<string, string>) null, (IEnumerable<string>) null)
    {
    }

    public MpnsTemplateRegistrationDescription(string channelUri, XmlDocument xmlTemplate)
      : this(string.Empty, new Uri(channelUri), xmlTemplate.InnerXml, (IDictionary<string, string>) null, (IEnumerable<string>) null)
    {
    }

    public MpnsTemplateRegistrationDescription(
      string channelUri,
      string templatePayload,
      IDictionary<string, string> mpnsHeaders)
      : this(string.Empty, new Uri(channelUri), templatePayload, mpnsHeaders, (IEnumerable<string>) null)
    {
    }

    public MpnsTemplateRegistrationDescription(
      string channelUri,
      string templatePayload,
      IEnumerable<string> tags)
      : this(string.Empty, new Uri(channelUri), templatePayload, (IDictionary<string, string>) null, tags)
    {
    }

    public MpnsTemplateRegistrationDescription(
      string channelUri,
      string templatePayload,
      IDictionary<string, string> mpnsHeaders,
      IEnumerable<string> tags)
      : this(string.Empty, new Uri(channelUri), templatePayload, mpnsHeaders, tags)
    {
    }

    internal MpnsTemplateRegistrationDescription(
      string notificationHubPath,
      string channelUri,
      string templatePayload,
      IDictionary<string, string> mpnsHeaders,
      IEnumerable<string> tags)
      : this(notificationHubPath, new Uri(channelUri), templatePayload, mpnsHeaders, tags)
    {
    }

    internal MpnsTemplateRegistrationDescription(
      string notificationHubPath,
      Uri channelUri,
      string templatePayload,
      IDictionary<string, string> mpnsHeaders,
      IEnumerable<string> tags)
      : base(notificationHubPath, channelUri, tags)
    {
      this.BodyTemplate = !string.IsNullOrWhiteSpace(templatePayload) ? new CDataMember(templatePayload) : throw new ArgumentNullException(nameof (templatePayload));
      this.MpnsHeaders = new MpnsHeaderCollection();
      if (mpnsHeaders == null)
        return;
      foreach (KeyValuePair<string, string> mpnsHeader in (IEnumerable<KeyValuePair<string, string>>) mpnsHeaders)
        this.MpnsHeaders.Add(mpnsHeader.Key, mpnsHeader.Value);
    }

    [AmqpMember(Mandatory = true, Order = 5)]
    [DataMember(Name = "BodyTemplate", IsRequired = true, Order = 3001)]
    public CDataMember BodyTemplate { get; set; }

    [AmqpMember(Mandatory = true, Order = 4)]
    [DataMember(Name = "MpnsHeaders", IsRequired = true, Order = 3002)]
    public MpnsHeaderCollection MpnsHeaders { get; set; }

    [AmqpMember(Mandatory = false, Order = 6)]
    internal List<int> ExpressionStartIndices { get; set; }

    [AmqpMember(Mandatory = false, Order = 7)]
    internal List<int> ExpressionLengths { get; set; }

    [AmqpMember(Mandatory = false, Order = 8)]
    internal List<string> Expressions { get; set; }

    [AmqpMember(Mandatory = false, Order = 9)]
    [DataMember(Name = "TemplateName", IsRequired = false, Order = 3003)]
    public string TemplateName { get; set; }

    internal override void OnValidate(bool allowLocalMockPns, ApiVersion version)
    {
      base.OnValidate(allowLocalMockPns, version);
      this.ValidateMpnsHeaders(version);
      if (this.IsXmlPayLoad())
      {
        this.ValidateXmlPayLoad(version);
      }
      else
      {
        if (!this.IsJsonObjectPayLoad())
          throw new InvalidDataContractException(SRClient.InvalidPayLoadFormat);
        this.ValidateJsonPayLoad(version);
      }
      this.ValidateTemplateName();
    }

    internal override RegistrationDescription Clone() => (RegistrationDescription) new MpnsTemplateRegistrationDescription(this);

    internal bool IsXmlPayLoad() => this.BodyTemplate.Value.Trim().StartsWith("<", StringComparison.OrdinalIgnoreCase);

    internal bool IsJsonObjectPayLoad()
    {
      string str = this.BodyTemplate.Value.Trim();
      return str.StartsWith("{", StringComparison.OrdinalIgnoreCase) && str.EndsWith("}", StringComparison.OrdinalIgnoreCase);
    }

    private void ValidateTemplateName()
    {
      if (this.TemplateName != null && this.TemplateName.Length > 200)
        throw new InvalidDataContractException(SRClient.TemplateNameLengthExceedsLimit((object) 200));
    }

    private void ValidateMpnsHeaders(ApiVersion version)
    {
      if (this.MpnsHeaders == null || !this.MpnsHeaders.ContainsKey("X-NotificationClass") || string.IsNullOrWhiteSpace(this.MpnsHeaders["X-NotificationClass"]))
        throw new InvalidDataContractException(SRClient.MissingMpnsHeader((object) "X-NotificationClass"));
      foreach (string key in this.MpnsHeaders.Keys)
      {
        if (string.IsNullOrWhiteSpace(this.MpnsHeaders[key]))
          throw new InvalidDataContractException(SRClient.MpnsHeaderIsNullOrEmpty((object) key));
        int num = (int) ExpressionEvaluator.Validate(this.MpnsHeaders[key], version);
      }
    }

    private void ValidateXmlPayLoad(ApiVersion version)
    {
      XDocument xdocument = XDocument.Parse((string) this.BodyTemplate);
      this.ExpressionStartIndices = new List<int>();
      this.ExpressionLengths = new List<int>();
      this.Expressions = new List<string>();
      IDictionary<string, int> expressionToIndexMap = (IDictionary<string, int>) new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (XElement xelement in xdocument.Root.DescendantsAndSelf())
      {
        foreach (XAttribute attribute in xelement.Attributes())
        {
          if (ExpressionEvaluator.Validate(attribute.Value, version) != ExpressionEvaluator.ExpressionType.Literal)
          {
            string str1 = attribute.ToString();
            string str2 = str1.Substring(str1.IndexOf('=') + 1);
            string escapedExpression = str2.Substring(1, str2.Length - 2);
            this.AddExpression(attribute.Value, escapedExpression, expressionToIndexMap);
          }
        }
        if (!xelement.HasElements && !string.IsNullOrEmpty(xelement.Value) && ExpressionEvaluator.Validate(xelement.Value, version) != ExpressionEvaluator.ExpressionType.Literal)
        {
          using (XmlReader reader = xelement.CreateReader())
          {
            int content = (int) reader.MoveToContent();
            string escapedExpression = reader.ReadInnerXml();
            this.AddExpression(xelement.Value, escapedExpression, expressionToIndexMap);
          }
        }
      }
    }

    private void ValidateJsonPayLoad(ApiVersion version)
    {
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
    }

    private void AddExpression(
      string expression,
      string escapedExpression,
      IDictionary<string, int> expressionToIndexMap)
    {
      int num1;
      if (!expressionToIndexMap.TryGetValue(expression, out num1))
        num1 = -1;
      int num2 = this.BodyTemplate.Value.IndexOf(escapedExpression, num1 + 1, StringComparison.OrdinalIgnoreCase);
      if (num2 == -1)
        throw new InvalidDataContractException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unsupported expression: {0}", new object[1]
        {
          (object) expression
        }));
      this.ExpressionStartIndices.Add(num2);
      this.ExpressionLengths.Add(escapedExpression.Length);
      this.Expressions.Add(expression);
      expressionToIndexMap[expression] = num2;
    }

    private enum MpnsTemplateRegistrationType
    {
      Toast,
      Tile,
    }
  }
}
