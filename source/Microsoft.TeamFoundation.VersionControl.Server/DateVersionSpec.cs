// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.DateVersionSpec
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class DateVersionSpec : VersionSpec
  {
    private DateTime m_date;
    private string m_originalText;
    private int m_changeset = VersionSpec.UnknownChangeset;

    [XmlAttribute("date")]
    public DateTime Date
    {
      get => this.m_date;
      set
      {
        this.m_date = value;
        this.m_date = this.m_date.ToUniversalTime();
        this.m_changeset = VersionSpec.UnknownChangeset;
      }
    }

    [XmlAttribute("otext")]
    public string OriginalText
    {
      get => string.IsNullOrEmpty(this.m_originalText) ? this.m_date.ToString() : this.m_originalText;
      set => this.m_originalText = value;
    }

    public override int CompareTo(VersionSpec other)
    {
      int num = base.CompareTo(other);
      if (num == 0)
        num = this.Date.CompareTo(((DateVersionSpec) other).Date);
      return num;
    }

    public override int GetHashCode() => this.Date.GetHashCode();

    public override string ToString() => this.m_date.ToString();

    public override string ToDBString(IVssRequestContext requestContext) => "D" + this.m_date.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", (IFormatProvider) DateTimeFormatInfo.InvariantInfo);

    public override int ToChangeset(IVssRequestContext requestContext)
    {
      VersionControlRequestContext controlRequestContext = requestContext.GetService<TeamFoundationVersionControlService>().GetVersionControlRequestContext(requestContext);
      if (this.m_changeset == VersionSpec.UnknownChangeset)
      {
        using (VersionedItemComponent versionedItemComponent = controlRequestContext.VersionControlService.GetVersionedItemComponent(controlRequestContext))
        {
          try
          {
            this.m_changeset = versionedItemComponent.FindChangesetByDate(this.m_date);
          }
          catch (DateVersionSpecBeforeBeginningOfRepositoryException ex)
          {
            requestContext.TraceException(700206, TraceLevel.Info, TraceArea.FindChangeset, TraceLayer.BusinessLogic, (Exception) ex);
            throw new DateVersionSpecBeforeBeginningOfRepositoryException(this.OriginalText, (Exception) ex);
          }
        }
      }
      return this.m_changeset;
    }

    internal void ValidateBeforeFirstChangeset(
      VersionControlRequestContext versionControlRequestContext)
    {
      if (this.m_date < versionControlRequestContext.VersionControlService.GetEarliestChangesetTime(versionControlRequestContext.RequestContext))
        throw new DateVersionSpecBeforeBeginningOfRepositoryException(this.OriginalText);
    }

    internal override void Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameter)
    {
      if (this.m_date < SqlDateTime.MinValue.Value)
        throw new InvalidSqlDateException(this.m_date);
      this.m_changeset = VersionSpec.UnknownChangeset;
    }
  }
}
