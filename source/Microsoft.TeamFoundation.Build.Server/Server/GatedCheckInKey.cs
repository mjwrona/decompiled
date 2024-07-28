// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.GatedCheckInKey
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal sealed class GatedCheckInKey
  {
    internal GatedCheckInKey(byte[] key, byte[] iv, int blockSize)
    {
      this.IV = iv;
      this.Key = key;
      this.BlockSize = blockSize;
    }

    private byte[] Encrypt(byte[] data)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (AesCryptoServiceProvider cryptoServiceProvider = new AesCryptoServiceProvider())
        {
          cryptoServiceProvider.BlockSize = this.BlockSize;
          cryptoServiceProvider.KeySize = 256;
          using (CryptoStream output = new CryptoStream((Stream) memoryStream, cryptoServiceProvider.CreateEncryptor(this.Key, this.IV), CryptoStreamMode.Write))
          {
            using (BinaryWriter binaryWriter = new BinaryWriter((Stream) output))
              binaryWriter.Write(data);
          }
          cryptoServiceProvider.Clear();
          return memoryStream.ToArray();
        }
      }
    }

    private byte[] Decrypt(byte[] data)
    {
      using (MemoryStream memoryStream = new MemoryStream(data))
      {
        using (AesCryptoServiceProvider cryptoServiceProvider = new AesCryptoServiceProvider())
        {
          cryptoServiceProvider.BlockSize = this.BlockSize;
          cryptoServiceProvider.KeySize = 256;
          using (CryptoStream input = new CryptoStream((Stream) memoryStream, cryptoServiceProvider.CreateDecryptor(this.Key, this.IV), CryptoStreamMode.Read))
          {
            using (BinaryReader binaryReader = new BinaryReader((Stream) input))
            {
              byte[] numArray = binaryReader.ReadBytes(data.Length);
              cryptoServiceProvider.Clear();
              return numArray;
            }
          }
        }
      }
    }

    internal string CreateCheckInTicket(
      IVssRequestContext requestContext,
      string shelvesetSpec,
      IEnumerable<string> affectedDefinitions)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (CreateCheckInTicket));
      TeamFoundationIdentity foundationIdentity = requestContext.GetService<TeamFoundationIdentityService>().ReadRequestIdentity(requestContext);
      string workspaceName;
      string workspaceOwner;
      WorkspaceSpec.Parse(shelvesetSpec, (string) null, out workspaceName, out workspaceOwner);
      Shelveset shelveset = requestContext.GetService<TeamFoundationVersionControlService>().QueryShelvesets(requestContext, workspaceName, workspaceOwner).FirstOrDefault<Shelveset>();
      if (shelveset != null)
      {
        GatedCheckInKey.Ticket ticket = new GatedCheckInKey.Ticket()
        {
          ExpirationDate = DateTime.UtcNow.AddHours(24.0),
          Owner = foundationIdentity,
          ShelvesetCreationDate = shelveset.CreationDate,
          ShelvesetName = shelveset.Name,
          ShelvesetOwner = shelveset.Owner
        };
        requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Created ticket '{0}'", (object) ticket);
        ticket.AffectedDefinitions.AddRange(affectedDefinitions);
        string serializedString = ticket.ToSerializedString(requestContext, this);
        requestContext.TraceLeave(0, "Build", "Service", nameof (CreateCheckInTicket));
        return serializedString;
      }
      requestContext.Trace(0, TraceLevel.Warning, "Build", "Service", "Exiting due to shelveset not found for '{0}'", (object) shelvesetSpec);
      return (string) null;
    }

    internal void ValidateCheckInTicket(
      IVssRequestContext requestContext,
      string checkInTicket,
      string definitionUri,
      string shelvesetName)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (ValidateCheckInTicket));
      GatedCheckInKey.Ticket ticket = GatedCheckInKey.Ticket.FromSerializedString(requestContext, this, checkInTicket);
      if (ticket == null)
      {
        requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Ticket cannot be created from '{0}'", (object) checkInTicket);
        throw new GatedCheckInTicketValidationException(ResourceStrings.GatedCheckInTicketValidationError());
      }
      if (!IdentityDescriptorComparer.Instance.Equals(ticket.Owner.Descriptor, requestContext.UserContext))
      {
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Ticket owner '{0}' is not the request user", (object) ticket.Owner.UniqueName);
        throw new GatedCheckInTicketValidationException(ResourceStrings.GatedCheckInTicketInvalidOwner((object) userIdentity.DisplayName));
      }
      if (ticket.ExpirationDate.ToUniversalTime() < DateTime.UtcNow)
      {
        requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Tiket is expired at {0}", (object) ticket.ExpirationDate.ToUniversalTime());
        throw new GatedCheckInTicketValidationException(ResourceStrings.GatedCheckInTicketExpired());
      }
      if (ticket.AffectedDefinitions.Find((Predicate<string>) (x => StringComparer.OrdinalIgnoreCase.Equals(x, definitionUri))) == null)
      {
        requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Ticket is not valid for definition '{0}'", (object) definitionUri);
        throw new GatedCheckInTicketValidationException(ResourceStrings.GatedCheckInTicketInvalidDefinition());
      }
      ArgumentValidation.CheckShelvesetName(nameof (shelvesetName), shelvesetName, false);
      string workspaceName;
      string workspaceOwner;
      WorkspaceSpec.Parse(shelvesetName, (string) null, out workspaceName, out workspaceOwner);
      if (!TFStringComparer.WorkspaceName.Equals(workspaceName, ticket.ShelvesetName) || !VssStringComparer.IdentityName.Equals(workspaceOwner, ticket.ShelvesetOwner))
      {
        requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Ticket shelveset does not match '{0}'", (object) shelvesetName);
        throw new GatedCheckInTicketValidationException(ResourceStrings.GatedCheckInTicketInvalidShelveset());
      }
      TeamFoundationVersionControlService service = requestContext.GetService<TeamFoundationVersionControlService>();
      Shelveset shelveset = service != null ? service.QueryShelvesets(requestContext, ticket.ShelvesetName, ticket.ShelvesetOwner).FirstOrDefault<Shelveset>() : (Shelveset) null;
      if (shelveset != null && shelveset.CreationDate.ToUniversalTime() != ticket.ShelvesetCreationDate.ToUniversalTime())
      {
        requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Ticket shelveset has been modified at {0}", (object) shelveset.CreationDate);
        throw new GatedCheckInTicketValidationException(ResourceStrings.GatedCheckInTicketShelvesetModified());
      }
      requestContext.TraceLeave(0, "Build", "Service", nameof (ValidateCheckInTicket));
    }

    internal static void GenerateNewKey(out byte[] key, out byte[] iv, out int blockSize)
    {
      using (AesCryptoServiceProvider cryptoServiceProvider = new AesCryptoServiceProvider())
      {
        cryptoServiceProvider.KeySize = 256;
        cryptoServiceProvider.GenerateIV();
        cryptoServiceProvider.GenerateKey();
        iv = cryptoServiceProvider.IV;
        key = cryptoServiceProvider.Key;
        blockSize = cryptoServiceProvider.BlockSize;
        cryptoServiceProvider.Clear();
      }
    }

    private byte[] IV { get; set; }

    private byte[] Key { get; set; }

    private int BlockSize { get; set; }

    private sealed class Ticket
    {
      private List<string> m_affectedDefinitions = new List<string>();

      public TeamFoundationIdentity Owner { get; set; }

      public string ShelvesetName { get; set; }

      public string ShelvesetOwner { get; set; }

      public DateTime ShelvesetCreationDate { get; set; }

      public DateTime ExpirationDate { get; set; }

      public List<string> AffectedDefinitions => this.m_affectedDefinitions;

      public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[Ticket Owner={0} ShelvesetName={1} ShelvesetOwner={2} ShelvesetCreationDate={3} ExpirationDate={4}]", (object) this.Owner.UniqueName, (object) this.ShelvesetName, (object) this.ShelvesetOwner, (object) this.ShelvesetCreationDate, (object) this.ExpirationDate);

      internal string ToSerializedString(IVssRequestContext requestContext, GatedCheckInKey key)
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendFormat("%owner=\"{0}\";%", (object) this.Owner.TeamFoundationId);
        stringBuilder.AppendFormat("%expirationDate=\"{0}\";%", (object) XmlUtility.ToString(this.ExpirationDate));
        stringBuilder.AppendFormat("%shelvesetName=\"{0}\";%", (object) this.ShelvesetName);
        stringBuilder.AppendFormat("%shelvesetOwner=\"{0}\";%", (object) this.ShelvesetOwner);
        stringBuilder.AppendFormat("%shelvesetCreationDate=\"{0}\";%", (object) XmlUtility.ToString(this.ShelvesetCreationDate));
        stringBuilder.AppendFormat("%definitions=\"{0}\";%", (object) string.Join("|", this.AffectedDefinitions.ToArray()));
        using (SHA256CryptoServiceProvider cryptoServiceProvider = new SHA256CryptoServiceProvider())
        {
          byte[] inArray = key.Encrypt(cryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(stringBuilder.ToString())));
          stringBuilder.AppendFormat("%hash=\"{0}\";%", (object) Convert.ToBase64String(inArray, 0, inArray.Length, Base64FormattingOptions.None));
          cryptoServiceProvider.Clear();
        }
        string serializedString = stringBuilder.ToString();
        requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Serialized ticket: '{0}'", (object) serializedString);
        return serializedString;
      }

      internal static GatedCheckInKey.Ticket FromSerializedString(
        IVssRequestContext requestContext,
        GatedCheckInKey key,
        string serializedTicket)
      {
        requestContext.TraceEnter(0, "Build", "Service", nameof (FromSerializedString));
        if (string.IsNullOrEmpty(serializedTicket))
        {
          requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Exiting due to null or empty serialized ticket");
          return (GatedCheckInKey.Ticket) null;
        }
        int startPos;
        string s = GatedCheckInKey.Ticket.ExtractValue(serializedTicket, "hash", out startPos);
        if (string.IsNullOrEmpty(s))
        {
          requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Exiting due to null or empty hash string");
          return (GatedCheckInKey.Ticket) null;
        }
        string str1 = serializedTicket.Remove(startPos);
        try
        {
          using (SHA256CryptoServiceProvider cryptoServiceProvider = new SHA256CryptoServiceProvider())
          {
            if (!ArrayUtil.Equals(key.Decrypt(Convert.FromBase64String(s)), cryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(str1))))
            {
              requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Exiting due to invalid hash");
              return (GatedCheckInKey.Ticket) null;
            }
          }
        }
        catch (CryptographicException ex)
        {
          requestContext.TraceException(0, "Build", "Service", (Exception) ex);
          return (GatedCheckInKey.Ticket) null;
        }
        GatedCheckInKey.Ticket ticket = new GatedCheckInKey.Ticket()
        {
          ExpirationDate = XmlUtility.ToDateTime(GatedCheckInKey.Ticket.ExtractValue(str1, "expirationDate")),
          ShelvesetCreationDate = XmlUtility.ToDateTime(GatedCheckInKey.Ticket.ExtractValue(str1, "shelvesetCreationDate")),
          ShelvesetName = GatedCheckInKey.Ticket.ExtractValue(str1, "shelvesetName"),
          ShelvesetOwner = GatedCheckInKey.Ticket.ExtractValue(str1, "shelvesetOwner")
        };
        TeamFoundationIdentityService service = requestContext.GetService<TeamFoundationIdentityService>();
        Guid tfid = new Guid(GatedCheckInKey.Ticket.ExtractValue(str1, "owner"));
        ticket.Owner = ((IEnumerable<TeamFoundationIdentity>) service.ReadIdentities(requestContext, new Guid[1]
        {
          tfid
        }, MembershipQuery.None, ReadIdentityOptions.None, (IEnumerable<string>) null)).FirstOrDefault<TeamFoundationIdentity>();
        if (ticket.Owner == null)
        {
          requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Identity not found for '{0}'", (object) tfid);
          throw new Microsoft.VisualStudio.Services.Identity.IdentityNotFoundException(tfid);
        }
        string str2 = GatedCheckInKey.Ticket.ExtractValue(str1, "definitions");
        ticket.AffectedDefinitions.AddRange((IEnumerable<string>) str2.Split(new string[1]
        {
          "|"
        }, StringSplitOptions.RemoveEmptyEntries));
        requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Created ticket '{0}' from serialized string", (object) ticket);
        requestContext.TraceLeave(0, "Build", "Service", nameof (FromSerializedString));
        return ticket;
      }

      private static string ExtractValue(string serializedTicket, string name) => GatedCheckInKey.Ticket.ExtractValue(serializedTicket, name, out int _);

      public static string ExtractValue(string serializedTicket, string name, out int startPos)
      {
        string str = "%" + name + "=\"";
        startPos = serializedTicket.IndexOf(str, StringComparison.Ordinal);
        if (startPos == -1)
          return string.Empty;
        int num = serializedTicket.IndexOf("\";%", startPos + 1, StringComparison.Ordinal);
        return num == -1 ? string.Empty : serializedTicket.Substring(startPos + str.Length, num - (startPos + str.Length));
      }
    }
  }
}
