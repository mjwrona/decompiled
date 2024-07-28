// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.UrlSigner
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class UrlSigner : IDisposable
  {
    private ISignable[] m_owners;
    private int[] m_indices;
    private int[] m_fileIds;
    private string[] m_urls;
    private TicketGenerator m_generator;
    private DateTime m_expirationDate;
    private int m_queueTail;
    private readonly int m_idsPerUrl;
    private IVssRequestContext m_requestContext;
    private static readonly int s_defaultFilesPerTicket = 21;

    public UrlSigner(IVssRequestContext requestContext)
      : this(requestContext, DateTime.MinValue)
    {
    }

    public UrlSigner(IVssRequestContext requestContext, DateTime expirationDate)
    {
      this.m_requestContext = requestContext;
      this.m_idsPerUrl = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.FilesPerTicket, true, UrlSigner.s_defaultFilesPerTicket);
      if (this.m_idsPerUrl < 0)
      {
        TeamFoundationTrace.Warning("FilesPerTicket registry value is out of range. Using default.");
        this.m_idsPerUrl = UrlSigner.s_defaultFilesPerTicket;
      }
      this.m_expirationDate = expirationDate;
    }

    public void Dispose()
    {
      this.FlushDeferredSignatures();
      GC.SuppressFinalize((object) this);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void SignObject(ISignable o)
    {
      if (this.m_urls == null)
      {
        this.m_fileIds = new int[this.m_idsPerUrl];
        this.m_indices = new int[this.m_idsPerUrl];
        this.m_owners = new ISignable[this.m_idsPerUrl];
        this.m_urls = new string[this.m_idsPerUrl];
      }
      for (int index = 0; index < o.GetDownloadUrlCount(); ++index)
      {
        int fileId = o.GetFileId(index);
        if (fileId != 0)
        {
          this.m_owners[this.m_queueTail] = o;
          this.m_indices[this.m_queueTail] = index;
          this.m_fileIds[this.m_queueTail] = fileId;
          ++this.m_queueTail;
          if (this.m_queueTail == this.m_idsPerUrl)
            this.FlushDeferredSignatures();
        }
      }
    }

    internal void SignObjects(ICollection objectsToSign)
    {
      foreach (ISignable o in (IEnumerable) objectsToSign)
      {
        if (o != null)
          this.SignObject(o);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void FlushDeferredSignatures()
    {
      if (this.m_queueTail <= 0)
        return;
      if (this.m_generator == null)
        this.m_generator = new TicketGenerator(this.m_expirationDate, this.m_requestContext.ServiceHost.OrganizationServiceHost.InstanceId.ToString(), this.m_requestContext.VirtualPath());
      this.m_generator.GenerateRsaSignedTickets(this.m_requestContext, this.m_fileIds, this.m_urls, 0, this.m_queueTail);
      for (int index = 0; index < this.m_queueTail; ++index)
        this.m_owners[index].SetDownloadUrl(this.m_indices[index], this.m_urls[index]);
      Array.Clear((Array) this.m_urls, 0, this.m_queueTail);
      this.m_queueTail = 0;
    }
  }
}
