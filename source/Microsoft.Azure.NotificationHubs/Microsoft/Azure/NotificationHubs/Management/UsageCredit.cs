// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Management.UsageCredit
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Microsoft.Azure.NotificationHubs.Management
{
  [DataContract(Name = "UsageCredit", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public class UsageCredit : IExtensibleDataObject, IEquatable<UsageCredit>
  {
    public static readonly DataContractSerializer Serializer = new DataContractSerializer(typeof (UsageCredit));
    private const char ConnectChar = '_';

    [DataMember(Name = "KeyName", IsRequired = false, Order = 1001, EmitDefaultValue = false)]
    public string KeyName
    {
      get => this.RequestorService + "_" + this.Identifier;
      set
      {
      }
    }

    [DataMember(Name = "RequestorService", IsRequired = true, Order = 1002, EmitDefaultValue = false)]
    public string RequestorService { get; set; }

    [DataMember(Name = "Identifier", IsRequired = true, Order = 1003, EmitDefaultValue = false)]
    public string Identifier { get; set; }

    [DataMember(Name = "NHBasicUnit", IsRequired = false, Order = 1004, EmitDefaultValue = false)]
    public int? NHBasicUnit { get; set; }

    [DataMember(Name = "NHStandardUnit", IsRequired = false, Order = 1005, EmitDefaultValue = false)]
    public int? NHStandardUnit { get; set; }

    [DataMember(Name = "Revision", IsRequired = false, Order = 1006, EmitDefaultValue = true)]
    public long Revision { get; set; }

    [DataMember(Name = "UpdatedAt", IsRequired = false, Order = 1007, EmitDefaultValue = false)]
    public DateTime UpdatedAt { get; set; }

    public ExtensionDataObject ExtensionData { get; set; }

    public string SubscriptionId { get; set; }

    public string NamespaceName { get; set; }

    internal long Id { get; set; }

    internal bool Hidden { get; set; }

    public override string ToString()
    {
      using (MemoryStream output = new MemoryStream())
      {
        using (XmlWriter.Create((Stream) output, new XmlWriterSettings()
        {
          Encoding = Encoding.UTF8
        }))
        {
          UsageCredit.Serializer.WriteObject((Stream) output, (object) this);
          output.Flush();
          return Encoding.UTF8.GetString(output.ToArray());
        }
      }
    }

    public bool Equals(UsageCredit other)
    {
      if (this.Id != 0L && other.Id != 0L)
        return this.Id == other.Id;
      return this.SubscriptionId.Equals(other.SubscriptionId, StringComparison.OrdinalIgnoreCase) && this.NamespaceName.Equals(other.NamespaceName, StringComparison.OrdinalIgnoreCase) && this.RequestorService.Equals(other.RequestorService, StringComparison.OrdinalIgnoreCase) && this.Identifier.Equals(other.Identifier, StringComparison.OrdinalIgnoreCase);
    }

    internal static bool TryParseKey(
      string key,
      out string identifier,
      out string requestorService)
    {
      string[] strArray = key.Split('_');
      if (strArray.Length == 2)
      {
        requestorService = strArray[0];
        identifier = strArray[1];
        return true;
      }
      identifier = (string) null;
      requestorService = (string) null;
      return false;
    }
  }
}
