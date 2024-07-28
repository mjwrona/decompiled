// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DownloadContext
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DownloadContext
  {
    internal int FileId;
    private NameValueCollection RequestParameters;
    private RepositoryInformation m_repositoryInfo;
    private string m_ticketType;

    internal DownloadContext(
      IEnumerable<KeyValuePair<string, string>> parameters)
      : this(parameters.ToNameValueCollection())
    {
    }

    internal DownloadContext(NameValueCollection parameters)
    {
      this.RequestParameters = parameters;
      string parameter = parameters["fid"];
      if (string.IsNullOrEmpty(parameter))
        throw new DownloadTicketValidationException(FrameworkResources.RequestFileIdMissing());
      if (!int.TryParse(parameter, out this.FileId) || this.FileId <= 0)
        throw new DownloadTicketValidationException(FrameworkResources.RequestFileIdMalformed());
      this.m_ticketType = parameters["type"];
      if (string.IsNullOrEmpty(this.m_ticketType))
        this.m_ticketType = "rsa";
      this.RequireTicketType("rsa");
      this.m_repositoryInfo = new RepositoryInformation(parameters["rid"], parameters["iid"], parameters["cp"]);
    }

    internal string DownloadUrl => RequestSignatures.RegenerateUrl(this.RequestParameters);

    internal RepositoryInformation RepositoryInfo => this.m_repositoryInfo.RepositoryId != null ? this.m_repositoryInfo : throw new DownloadTicketValidationException(FrameworkResources.RequestServerIdMissing());

    internal void RequireTicketType(string ticketType)
    {
      if (!string.Equals(this.m_ticketType, ticketType, StringComparison.OrdinalIgnoreCase))
        throw new DownloadTicketValidationException(FrameworkResources.RequestTicketTypeNotSupported((object) ticketType));
    }

    internal void DemandValidSignature(ISigner signer, DateTime arrivalTime)
    {
      if (!this.TryValidateSignature(signer, arrivalTime))
        throw new DownloadTicketValidationException(FrameworkResources.RequestSignatureValidationFailed());
    }

    private bool TryValidateSignature(ISigner signer, DateTime arrivalTime)
    {
      if (!string.Equals(this.m_ticketType, "rsa", StringComparison.OrdinalIgnoreCase))
        return false;
      TicketValidator ticketValidator = new TicketValidator();
      if (!ticketValidator.IsUnexpired(this.RequestParameters, arrivalTime))
        throw new DownloadTicketValidationException(FrameworkResources.RequestExpired());
      return ticketValidator.IsValidRsaSignedTicket(signer, this.RequestParameters);
    }
  }
}
