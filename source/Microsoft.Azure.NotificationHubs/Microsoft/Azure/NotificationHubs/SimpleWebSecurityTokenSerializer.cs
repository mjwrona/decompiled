// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.SimpleWebSecurityTokenSerializer
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ServiceModel.Security;
using System.Text;
using System.Xml;

namespace Microsoft.Azure.NotificationHubs
{
  public class SimpleWebSecurityTokenSerializer : SecurityTokenSerializer
  {
    private const string LocalName = "BinarySecurityToken";
    private const string NamespaceUri = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";
    private const string UtilityNamespaceUri = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd";
    private const string ValueTypeUri = "http://schemas.xmlsoap.org/ws/2009/11/swt-token-profile-1.0";
    private const string EncodingTypeBase64 = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary";
    public static readonly SimpleWebSecurityTokenSerializer DefaultInstance = new SimpleWebSecurityTokenSerializer();
    private SecurityTokenSerializer innerSerializer;

    public SimpleWebSecurityTokenSerializer()
      : this((SecurityTokenSerializer) WSSecurityTokenSerializer.DefaultInstance)
    {
    }

    public SimpleWebSecurityTokenSerializer(SecurityTokenSerializer innerSerializer) => this.innerSerializer = innerSerializer != null ? innerSerializer : throw new ArgumentNullException(nameof (innerSerializer));

    private static bool IsSimpleWebSecurityToken(XmlReader reader) => reader.IsStartElement("BinarySecurityToken", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd") && reader.GetAttribute("ValueType", (string) null) == "http://schemas.xmlsoap.org/ws/2009/11/swt-token-profile-1.0";

    protected override bool CanReadKeyIdentifierClauseCore(XmlReader reader) => this.innerSerializer.CanReadKeyIdentifierClause(reader);

    protected override bool CanReadKeyIdentifierCore(XmlReader reader) => this.innerSerializer.CanReadKeyIdentifier(reader);

    protected override bool CanReadTokenCore(XmlReader reader) => SimpleWebSecurityTokenSerializer.IsSimpleWebSecurityToken(reader) || this.innerSerializer.CanReadToken(reader);

    protected override bool CanWriteKeyIdentifierClauseCore(
      SecurityKeyIdentifierClause keyIdentifierClause)
    {
      return this.innerSerializer.CanWriteKeyIdentifierClause(keyIdentifierClause);
    }

    protected override bool CanWriteKeyIdentifierCore(SecurityKeyIdentifier keyIdentifier) => this.innerSerializer.CanWriteKeyIdentifier(keyIdentifier);

    protected override bool CanWriteTokenCore(SecurityToken token) => token is SimpleWebSecurityToken || this.innerSerializer.CanWriteToken(token);

    protected override SecurityKeyIdentifierClause ReadKeyIdentifierClauseCore(XmlReader reader) => this.innerSerializer.ReadKeyIdentifierClause(reader);

    protected override SecurityKeyIdentifier ReadKeyIdentifierCore(XmlReader reader) => this.innerSerializer.ReadKeyIdentifier(reader);

    protected override SecurityToken ReadTokenCore(
      XmlReader reader,
      SecurityTokenResolver tokenResolver)
    {
      if (!SimpleWebSecurityTokenSerializer.IsSimpleWebSecurityToken(reader))
        return this.innerSerializer.ReadToken(reader, tokenResolver);
      XmlDictionaryReader dictionaryReader = XmlDictionaryReader.CreateDictionaryReader(reader);
      string attribute1 = dictionaryReader.GetAttribute("Id", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
      string attribute2 = dictionaryReader.GetAttribute("EncodingType", (string) null);
      if (attribute2 != null && !(attribute2 == "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary"))
        throw new NotSupportedException(SRClient.UnsupportedEncodingType);
      byte[] rawData = dictionaryReader.ReadElementContentAsBase64();
      return SimpleWebSecurityTokenSerializer.ReadBinaryCore(attribute1, rawData);
    }

    protected override void WriteKeyIdentifierClauseCore(
      XmlWriter writer,
      SecurityKeyIdentifierClause keyIdentifierClause)
    {
      this.innerSerializer.WriteKeyIdentifierClause(writer, keyIdentifierClause);
    }

    protected override void WriteKeyIdentifierCore(
      XmlWriter writer,
      SecurityKeyIdentifier keyIdentifier)
    {
      this.innerSerializer.WriteKeyIdentifier(writer, keyIdentifier);
    }

    protected override void WriteTokenCore(XmlWriter writer, SecurityToken token)
    {
      if (token is SimpleWebSecurityToken)
      {
        string id;
        byte[] rawData;
        SimpleWebSecurityTokenSerializer.WriteBinaryCore(token, out id, out rawData);
        if (rawData == null)
          throw new ArgumentNullException(SRClient.NullRawDataInToken);
        writer.WriteStartElement("wsse", "BinarySecurityToken", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
        if (id != null)
          writer.WriteAttributeString("wsu", "Id", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd", id);
        writer.WriteAttributeString("ValueType", (string) null, "http://schemas.xmlsoap.org/ws/2009/11/swt-token-profile-1.0");
        writer.WriteAttributeString("EncodingType", (string) null, "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary");
        writer.WriteBase64(rawData, 0, rawData.Length);
        writer.WriteEndElement();
      }
      else
        this.innerSerializer.WriteToken(writer, token);
    }

    private static SecurityToken ReadBinaryCore(string id, byte[] rawData)
    {
      string tokenString = Encoding.UTF8.GetString(rawData);
      return tokenString.StartsWith("SharedAccessSignature", StringComparison.Ordinal) ? (SecurityToken) new SharedAccessSignatureToken(id, tokenString) : (SecurityToken) new SimpleWebSecurityToken(id, tokenString);
    }

    private static void WriteBinaryCore(SecurityToken token, out string id, out byte[] rawData)
    {
      id = token is SimpleWebSecurityToken webSecurityToken ? webSecurityToken.Id : throw new ArgumentNullException(nameof (token));
      rawData = Encoding.UTF8.GetBytes(webSecurityToken.Token);
    }
  }
}
