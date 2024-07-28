// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.RegistrationDescription
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Microsoft.Azure.NotificationHubs
{
  [AmqpContract(Code = 0)]
  [DataContract(Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  [KnownType(typeof (GcmRegistrationDescription))]
  [KnownType(typeof (AppleRegistrationDescription))]
  [KnownType(typeof (AppleTemplateRegistrationDescription))]
  [KnownType(typeof (EmailRegistrationDescription))]
  [KnownType(typeof (WindowsRegistrationDescription))]
  [KnownType(typeof (WindowsTemplateRegistrationDescription))]
  [KnownType(typeof (MpnsRegistrationDescription))]
  [KnownType(typeof (MpnsTemplateRegistrationDescription))]
  [KnownType(typeof (GcmTemplateRegistrationDescription))]
  [KnownType(typeof (AdmRegistrationDescription))]
  [KnownType(typeof (AdmTemplateRegistrationDescription))]
  [KnownType(typeof (NokiaXRegistrationDescription))]
  [KnownType(typeof (NokiaXTemplateRegistrationDescription))]
  [KnownType(typeof (BaiduRegistrationDescription))]
  [KnownType(typeof (BaiduTemplateRegistrationDescription))]
  public abstract class RegistrationDescription : EntityDescription, IResourceDescription
  {
    internal const string TemplateRegistrationType = "template";
    internal static Regex SingleTagRegex = new Regex("^((\\$InstallationId:\\{[\\w-_@#.:=]+\\})|([\\w-_@#.:]+))$", RegexOptions.IgnoreCase);
    internal static Regex TagRegex = new Regex("^(((\\$InstallationId:\\{[\\w-_@#.:=]+\\})+?(,[\\w-_@#.:]+)*)|(([\\w-_@#.:]+)(,[\\w-_@#.:]+)*((,\\$InstallationId:\\{[\\w-_@#.:=]+\\})?(,[\\w-_@#.:]+)*)))$", RegexOptions.IgnoreCase);
    internal static string[] RegistrationRange = new string[32]
    {
      "_",
      "1",
      "2",
      "3",
      "4",
      "5",
      "6",
      "7",
      "9",
      "A",
      "B",
      "C",
      "D",
      "E",
      "F",
      "G",
      "H",
      "J",
      "K",
      "L",
      "M",
      "N",
      "P",
      "Q",
      "R",
      "S",
      "T",
      "U",
      "V",
      "W",
      "Y",
      "ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ"
    };
    private string channelHash;

    public RegistrationDescription(RegistrationDescription registration)
    {
      this.NotificationHubPath = registration.NotificationHubPath;
      this.RegistrationId = registration.RegistrationId;
      this.Tags = registration.Tags;
      this.ETag = registration.ETag;
      this.PropertyBagString = registration.PropertyBagString;
    }

    internal RegistrationDescription(string notificationHubPath) => this.NotificationHubPath = notificationHubPath;

    internal RegistrationDescription(string notificationHubPath, string registrationId)
    {
      this.NotificationHubPath = notificationHubPath;
      this.RegistrationId = registrationId;
    }

    internal abstract string AppPlatForm { get; }

    internal abstract string RegistrationType { get; }

    internal abstract string PlatformType { get; }

    [DataMember(Name = "ETag", IsRequired = false, Order = 1001, EmitDefaultValue = false)]
    public string ETag { get; internal set; }

    [AmqpMember(Order = 2, Mandatory = false)]
    [DataMember(Name = "ExpirationTime", IsRequired = false, Order = 1002, EmitDefaultValue = false)]
    public DateTime? ExpirationTime { get; internal set; }

    [AmqpMember(Order = 0, Mandatory = false)]
    [DataMember(Name = "RegistrationId", Order = 1003, IsRequired = false)]
    public string RegistrationId { get; set; }

    [AmqpMember(Name = "TagsString", Order = 1, Mandatory = false)]
    internal string TagsStringLightweight
    {
      get => this.GetTagsString();
      set => this.SetTagsString(value, false);
    }

    [DataMember(Name = "Tags", IsRequired = false, Order = 1004, EmitDefaultValue = false)]
    internal string TagsString
    {
      get => this.GetTagsString();
      set => this.SetTagsString(value, true);
    }

    [DataMember(Name = "PushVariables", IsRequired = false, Order = 1005, EmitDefaultValue = false)]
    internal string PropertyBagString
    {
      get => this.PushVariables != null && this.PushVariables.Count > 0 ? JsonConvert.SerializeObject((object) this.PushVariables) : (string) null;
      set
      {
        if (string.IsNullOrEmpty(value))
          return;
        this.PushVariables = (IDictionary<string, string>) JsonConvert.DeserializeObject<Dictionary<string, string>>(value);
      }
    }

    public ISet<string> Tags { get; set; }

    public IDictionary<string, string> PushVariables { get; set; }

    internal string FormattedETag => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "W/\"{0}\"", new object[1]
    {
      (object) this.ETag
    });

    internal int DbVersion { get; set; }

    internal string NotificationHubPath { get; set; }

    internal string NotificationHubRuntimeUrl { get; set; }

    internal long NotificationHubId { get; set; }

    internal long DatabaseId { get; set; }

    internal string Namespace { get; set; }

    internal string InstallationVersion { get; set; }

    internal bool ChannelExpired { get; set; }

    string IResourceDescription.CollectionName => "Registrations";

    internal string GetChannelHash()
    {
      if (string.IsNullOrEmpty(this.channelHash))
        this.channelHash = RegistrationDescription.ComputeChannelHash(this.GetPnsHandle());
      return this.channelHash;
    }

    internal abstract string GetPnsHandle();

    internal abstract void SetPnsHandle(string pnsHandle);

    internal abstract RegistrationDescription Clone();

    public static bool ValidateTags(string tags) => RegistrationDescription.TagRegex.IsMatch(tags);

    public static int TagCount(string tags) => tags.Split(',').Length;

    public string Serialize()
    {
      this.Validate(true, ApiVersion.Four, false);
      return new MessagingDescriptionSerializer<RegistrationDescription>().Serialize(this);
    }

    public static RegistrationDescription Deserialize(string descriptionString)
    {
      using (XmlReader reader = XmlReader.Create((TextReader) new StringReader(descriptionString)))
        return (RegistrationDescription) new DataContractSerializer(typeof (RegistrationDescription)).ReadObject(reader);
    }

    internal void Validate(bool allowLocalMockPns, ApiVersion version, bool checkExpirationTime = true)
    {
      if (checkExpirationTime && this.ExpirationTime.HasValue)
        throw new InvalidDataContractException(SRClient.CannotSpecifyExpirationTime);
      this.OnValidate(allowLocalMockPns, version);
    }

    internal static string ComputeChannelHash(string pnsHandle) => RegistrationDescription.GenerateUrlSafeBase64(new SHA1CryptoServiceProvider().ComputeHash(new ASCIIEncoding().GetBytes(pnsHandle)));

    protected static string GenerateUrlSafeBase64(byte[] hash) => Convert.ToBase64String(hash).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    internal virtual void OnValidate(bool allowLocalMockPns, ApiVersion version)
    {
    }

    internal bool InvalidTags { get; private set; }

    private string GetTagsString()
    {
      if (this.Tags == null || this.Tags.Count <= 0)
        return (string) null;
      StringBuilder stringBuilder = new StringBuilder();
      string[] array = this.Tags.ToArray<string>();
      for (int index = 0; index < array.Length; ++index)
      {
        stringBuilder.Append(array[index]);
        if (index < array.Length - 1)
          stringBuilder.Append(",");
      }
      return stringBuilder.ToString();
    }

    private void SetTagsString(string tagsString, bool validate)
    {
      HashSet<string> stringSet1;
      if (!string.IsNullOrEmpty(tagsString))
        stringSet1 = new HashSet<string>((IEnumerable<string>) tagsString.Split(','), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      else
        stringSet1 = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      HashSet<string> stringSet2 = stringSet1;
      this.InvalidTags = false;
      if (validate)
      {
        this.InvalidTags = !string.IsNullOrEmpty(tagsString) && !RegistrationDescription.ValidateTags(tagsString);
        if (!this.InvalidTags)
        {
          foreach (string str in stringSet2)
          {
            if (str.Length > Microsoft.Azure.NotificationHubs.Messaging.Constants.MaximumTagSize)
            {
              this.InvalidTags = true;
              break;
            }
          }
        }
      }
      if (this.InvalidTags)
        this.Tags = (ISet<string>) new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      else
        this.Tags = (ISet<string>) stringSet2;
    }
  }
}
