// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Protocol.RefAdvertiser
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Git.Server.Protocol
{
  public class RefAdvertiser
  {
    private readonly IVssRequestContext m_requestContext;
    private readonly ITfsGitRepository m_repository;
    private readonly AnnotatedTagPeeler m_tagPeeler;
    private readonly Stream m_stream;
    private readonly bool m_isOptimized;
    private const string c_uploadPackHead = " HEAD";
    private const string c_uploadPackSupportedFeatures = "multi_ack thin-pack side-band side-band-64k no-progress multi_ack_detailed no-done shallow allow-tip-sha1-in-want";
    private const string c_uploadPackSupportedFeaturesWithFilter = "multi_ack thin-pack side-band side-band-64k no-progress multi_ack_detailed no-done shallow allow-tip-sha1-in-want filter";
    private const string c_uploadPackSymRef = " symref=HEAD:";
    private const string c_receivePackSupportedFeatures = "report-status side-band-64k quiet delete-refs";
    private const string c_packFeaturesInterlude = "\0 ";
    private const string c_capabilitiesMagic = " capabilities^{}";
    private const string c_layer = "RefAdvertiser";

    public RefAdvertiser(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      Stream stream,
      bool isOptimized)
    {
      this.m_requestContext = requestContext;
      this.m_repository = repository;
      this.m_tagPeeler = new AnnotatedTagPeeler(repository);
      this.m_stream = stream;
      this.m_isOptimized = isOptimized;
    }

    public void AdvertiseUploadPack()
    {
      List<TfsGitRef> refs = this.GetRefs();
      string str = !this.m_requestContext.IsFeatureEnabled("Git.EnablePartialClone") ? "multi_ack thin-pack side-band side-band-64k no-progress multi_ack_detailed no-done shallow allow-tip-sha1-in-want" : "multi_ack thin-pack side-band side-band-64k no-progress multi_ack_detailed no-done shallow allow-tip-sha1-in-want filter";
      if (refs.Count != 0)
      {
        TfsGitRef tfsGitRef = refs.FirstOrDefault<TfsGitRef>((Func<TfsGitRef, bool>) (s => s.IsDefaultBranch)) ?? refs.FirstOrDefault<TfsGitRef>((Func<TfsGitRef, bool>) (s => s.Name.Equals("refs/heads/master", StringComparison.Ordinal))) ?? refs.FirstOrDefault<TfsGitRef>((Func<TfsGitRef, bool>) (s => s.Name.StartsWith("refs/heads/", StringComparison.Ordinal)));
        bool flag = false;
        if (tfsGitRef != null)
        {
          StringBuilder stringBuilder = new StringBuilder(40 + " HEAD".Length + "\0 ".Length + str.Length + " symref=HEAD:".Length + tfsGitRef.Name.Length);
          stringBuilder.Append((object) tfsGitRef.ObjectId);
          stringBuilder.Append(" HEAD");
          stringBuilder.Append("\0 ");
          stringBuilder.Append(str);
          stringBuilder.Append(" symref=HEAD:");
          stringBuilder.Append(tfsGitRef.Name);
          flag = true;
          ProtocolHelper.WriteLine(this.m_stream, stringBuilder.ToString());
        }
        foreach (TfsGitRef tagRef in refs)
        {
          StringBuilder stringBuilder1 = new StringBuilder(41 + tagRef.Name.Length + (flag ? 0 : "\0 ".Length + str.Length));
          stringBuilder1.Append((object) tagRef.ObjectId);
          stringBuilder1.Append(" ");
          stringBuilder1.Append(tagRef.Name);
          if (!flag)
          {
            stringBuilder1.Append("\0 ");
            stringBuilder1.Append(str);
            flag = true;
          }
          ProtocolHelper.WriteLine(this.m_stream, stringBuilder1.ToString());
          TfsGitObject peeledObject;
          if (this.m_tagPeeler.TryPeelTagRef(tagRef, out peeledObject))
          {
            StringBuilder stringBuilder2 = new StringBuilder(41 + tagRef.Name.Length + "^{}".Length);
            stringBuilder2.Append((object) peeledObject.ObjectId);
            stringBuilder2.Append(" ");
            stringBuilder2.Append(tagRef.Name);
            stringBuilder2.Append("^{}");
            ProtocolHelper.WriteLine(this.m_stream, stringBuilder2.ToString());
          }
        }
      }
      ProtocolHelper.WriteLine(this.m_stream, (string) null);
    }

    public void AdvertiseReceivePack()
    {
      if (!this.m_repository.Permissions.CanCreateBranch(string.Empty))
        this.m_repository.Permissions.CheckWrite(true);
      bool flag = false;
      List<TfsGitRef> refs = this.GetRefs(true);
      if (refs.Count == 0)
      {
        this.OutputCapabilitiesForEmptyRepository();
      }
      else
      {
        foreach (TfsGitRef tfsGitRef in refs)
        {
          StringBuilder stringBuilder = new StringBuilder(41 + tfsGitRef.Name.Length);
          stringBuilder.Append((object) tfsGitRef.ObjectId);
          stringBuilder.Append(" ");
          stringBuilder.Append(tfsGitRef.Name);
          if (!flag)
          {
            flag = true;
            stringBuilder.Append("\0 ");
            stringBuilder.Append("report-status side-band-64k quiet delete-refs");
          }
          ProtocolHelper.WriteLine(this.m_stream, stringBuilder.ToString());
        }
      }
      ProtocolHelper.WriteLine(this.m_stream, (string) null);
    }

    public void AdvertiseHead()
    {
      using (StreamWriter streamWriter = new StreamWriter(this.m_stream, GitEncodingUtil.SafeUtf8NoBom))
      {
        foreach (TfsGitRef tfsGitRef in this.GetRefs())
          streamWriter.Write("{0}\t{1}\n", (object) tfsGitRef.ObjectId, (object) tfsGitRef.Name);
      }
    }

    private List<TfsGitRef> GetRefs(bool includeBreadcrumbs = false)
    {
      int breadcrumbDays = includeBreadcrumbs ? this.m_repository.Settings.BreadcrumbDays : 0;
      return !this.m_isOptimized ? this.m_repository.Refs.All() : this.m_repository.Refs.Limited(breadcrumbDays);
    }

    private void OutputCapabilitiesForEmptyRepository()
    {
      StringBuilder stringBuilder = new StringBuilder(41 + " capabilities^{}".Length + "\0 ".Length + "report-status side-band-64k quiet delete-refs".Length);
      stringBuilder.Append("0000000000000000000000000000000000000000");
      stringBuilder.Append(" capabilities^{}");
      stringBuilder.Append("\0 ");
      stringBuilder.Append("report-status side-band-64k quiet delete-refs");
      ProtocolHelper.WriteLine(this.m_stream, stringBuilder.ToString());
    }
  }
}
