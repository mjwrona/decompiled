// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.MessagingDescriptionSerializer`1
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
  internal class MessagingDescriptionSerializer<TMessagingDescription>
  {
    public static MessagingDescriptionSerializer<TMessagingDescription> Serializer = new MessagingDescriptionSerializer<TMessagingDescription>();
    private const int MaxItemsInObjectGraph = 256;
    private const int MessagingDescriptionStringCommonLength = 512;
    private const int MessagingDescriptionStringMaxLength = 24576;
    private readonly DataContractSerializer serializer;
    private readonly Dictionary<string, DataContractSerializer> registrationSerializers;

    public MessagingDescriptionSerializer()
    {
      this.serializer = this.CreateSerializer<TMessagingDescription>();
      this.registrationSerializers = new Dictionary<string, DataContractSerializer>();
      this.registrationSerializers.Add(typeof (WindowsRegistrationDescription).Name, this.CreateSerializer<WindowsRegistrationDescription>());
      this.registrationSerializers.Add(typeof (WindowsTemplateRegistrationDescription).Name, this.CreateSerializer<WindowsTemplateRegistrationDescription>());
      this.registrationSerializers.Add(typeof (AppleRegistrationDescription).Name, this.CreateSerializer<AppleRegistrationDescription>());
      this.registrationSerializers.Add(typeof (AppleTemplateRegistrationDescription).Name, this.CreateSerializer<AppleTemplateRegistrationDescription>());
      this.registrationSerializers.Add(typeof (GcmRegistrationDescription).Name, this.CreateSerializer<GcmRegistrationDescription>());
      this.registrationSerializers.Add(typeof (GcmTemplateRegistrationDescription).Name, this.CreateSerializer<GcmTemplateRegistrationDescription>());
      this.registrationSerializers.Add(typeof (MpnsRegistrationDescription).Name, this.CreateSerializer<MpnsRegistrationDescription>());
      this.registrationSerializers.Add(typeof (MpnsTemplateRegistrationDescription).Name, this.CreateSerializer<MpnsTemplateRegistrationDescription>());
      this.registrationSerializers.Add(typeof (AdmRegistrationDescription).Name, this.CreateSerializer<AdmRegistrationDescription>());
      this.registrationSerializers.Add(typeof (AdmTemplateRegistrationDescription).Name, this.CreateSerializer<AdmTemplateRegistrationDescription>());
      this.registrationSerializers.Add(typeof (NokiaXRegistrationDescription).Name, this.CreateSerializer<NokiaXRegistrationDescription>());
      this.registrationSerializers.Add(typeof (NokiaXTemplateRegistrationDescription).Name, this.CreateSerializer<NokiaXTemplateRegistrationDescription>());
      this.registrationSerializers.Add(typeof (BaiduRegistrationDescription).Name, this.CreateSerializer<BaiduRegistrationDescription>());
      this.registrationSerializers.Add(typeof (BaiduTemplateRegistrationDescription).Name, this.CreateSerializer<BaiduTemplateRegistrationDescription>());
    }

    private DataContractSerializer CreateSerializer<T>() => new DataContractSerializer(typeof (T), (IEnumerable<Type>) null, 256, false, false, (IDataContractSurrogate) null);

    public TMessagingDescription DeserializeFromAtomFeed(Stream stream)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      XmlReaderSettings settings = new XmlReaderSettings()
      {
        ValidationType = ValidationType.None
      };
      if (typeof (TMessagingDescription) == typeof (RegistrationDescription))
      {
        MemoryStream memoryStream = new MemoryStream();
        stream.CopyTo((Stream) memoryStream);
        foreach (KeyValuePair<string, DataContractSerializer> registrationSerializer in this.registrationSerializers)
        {
          memoryStream.Seek(0L, SeekOrigin.Begin);
          using (XmlReader reader = XmlReader.Create((Stream) memoryStream, settings))
          {
            if (reader.ReadToDescendant(registrationSerializer.Key))
              return (TMessagingDescription) registrationSerializer.Value.ReadObject(reader);
          }
        }
        throw new SerializationException();
      }
      using (XmlReader reader = XmlReader.Create(stream, settings))
      {
        reader.ReadToDescendant(typeof (TMessagingDescription).Name);
        return (TMessagingDescription) this.serializer.ReadObject(reader);
      }
    }

    public TMessagingDescription Deserialize(string description)
    {
      if (description == null)
        throw new ArgumentNullException(nameof (description));
      XmlReaderSettings settings = new XmlReaderSettings()
      {
        ValidationType = ValidationType.None
      };
      if (typeof (TMessagingDescription) == typeof (RegistrationDescription))
      {
        foreach (KeyValuePair<string, DataContractSerializer> registrationSerializer in this.registrationSerializers)
        {
          using (XmlReader reader = XmlReader.Create((TextReader) new StringReader(description), settings))
          {
            if (reader.ReadToDescendant(registrationSerializer.Key))
              return (TMessagingDescription) registrationSerializer.Value.ReadObject(reader);
          }
        }
        throw new SerializationException();
      }
      using (XmlReader reader = XmlReader.Create((TextReader) new StringReader(description), settings))
        return (TMessagingDescription) this.serializer.ReadObject(reader);
    }

    public string Serialize(TMessagingDescription description)
    {
      StringBuilder output = new StringBuilder(512, 24576);
      XmlWriterSettings settings = new XmlWriterSettings()
      {
        ConformanceLevel = ConformanceLevel.Fragment,
        NamespaceHandling = NamespaceHandling.OmitDuplicates
      };
      using (XmlWriter writer = XmlWriter.Create(output, settings))
        this.serializer.WriteObject(writer, (object) description);
      return output.ToString();
    }
  }
}
