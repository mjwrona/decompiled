// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.SmtpCredential
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
  [DataContract(Name = "SmtpCredential", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  internal class SmtpCredential : PnsCredential
  {
    internal const string AppPlatformName = "smtp";

    public SmtpCredential(
      string smtpHost,
      ushort port,
      string userName,
      string password,
      bool enableSsl,
      string fromAddress)
    {
      this.SmtpHost = smtpHost;
      this.Port = port;
      this.UserName = userName;
      this.Password = password;
      this.EnableSsl = enableSsl;
      this.FromAddress = fromAddress;
    }

    public string Password
    {
      get => this[nameof (Password)];
      set => this[nameof (Password)] = value;
    }

    public ushort Port
    {
      get
      {
        string s = this[nameof (Port)];
        ushort result;
        if (string.IsNullOrEmpty(s) || !ushort.TryParse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
          throw new InvalidDataContractException("Illegal Port value");
        return result;
      }
      set => this[nameof (Port)] = value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }

    public string SmtpHost
    {
      get => this[nameof (SmtpHost)];
      set => this[nameof (SmtpHost)] = value;
    }

    public string UserName
    {
      get => this[nameof (UserName)];
      set => this[nameof (UserName)] = value;
    }

    public string FromAddress
    {
      get => this[nameof (FromAddress)];
      set => this[nameof (FromAddress)] = value;
    }

    public bool EnableSsl
    {
      get
      {
        string str = this[nameof (EnableSsl)];
        bool result;
        if (string.IsNullOrEmpty(str) || !bool.TryParse(str, out result))
          throw new InvalidDataContractException("Illegal EnableSsl value");
        return result;
      }
      set => this[nameof (EnableSsl)] = value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }

    internal override string AppPlatform => "smtp";

    protected override void OnValidate(bool allowLocalMockPns)
    {
      if (this.Properties == null || this.Properties.Count > 6)
        throw new InvalidDataContractException("Only the SmtpHost, Port, EnableSsl, Username, Password and FromAddress should be specified");
      if (this.Properties.Count < 6 || string.IsNullOrWhiteSpace(this.SmtpHost) || this.Port == (ushort) 0 || string.IsNullOrEmpty(this.UserName) || string.IsNullOrEmpty(this.Password) || this.EnableSsl != this.EnableSsl || string.IsNullOrEmpty(this.FromAddress) || !EmailRegistrationDescription.EmailAddressRegex.IsMatch(this.FromAddress))
        throw new InvalidDataContractException("One or more properties is missing or invalid");
    }
  }
}
