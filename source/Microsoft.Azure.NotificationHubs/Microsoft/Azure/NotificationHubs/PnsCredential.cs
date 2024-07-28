// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.PnsCredential
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
  [DataContract(Name = "PnsCredential", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  [KnownType(typeof (ApnsCredential))]
  [KnownType(typeof (GcmCredential))]
  [KnownType(typeof (SmtpCredential))]
  [KnownType(typeof (MpnsCredential))]
  [KnownType(typeof (WnsCredential))]
  [KnownType(typeof (AdmCredential))]
  [KnownType(typeof (NokiaXCredential))]
  public abstract class PnsCredential : EntityDescription
  {
    internal PnsCredential() => this.Properties = new PnsCredentialProperties();

    internal abstract string AppPlatform { get; }

    [DataMember(IsRequired = true)]
    public PnsCredentialProperties Properties { get; set; }

    [DataMember(Name = "BlockedOn", IsRequired = false, EmitDefaultValue = false)]
    public DateTime? BlockedOn { get; set; }

    protected string this[string name]
    {
      get => this.Properties.ContainsKey(name) ? this.Properties[name] : (string) null;
      set
      {
        if (this.Properties.ContainsKey(name))
          this.Properties[name] = value;
        else
          this.Properties.Add(name, value);
      }
    }

    internal void Validate(bool allowLocalMockPns) => this.OnValidate(allowLocalMockPns);

    protected virtual void OnValidate(bool allowLocalMockPns)
    {
    }

    public static bool IsEqual(PnsCredential cred1, PnsCredential cred2) => cred1 == null || cred2 == null ? cred1 == cred2 : cred1.Equals((object) cred2);
  }
}
