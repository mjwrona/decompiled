// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CommandGetArtifactPropertyValue
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class CommandGetArtifactPropertyValue : Command
  {
    private ArtifactKind m_artifactKind;
    private IEnumerable<string> m_propertyNameFilters;
    private ObjectBinder<DbArtifactPropertyValue> m_binder;
    private StreamingCollection<ArtifactPropertyValue> m_result;
    private DbArtifactPropertyValue m_currentDbArtifact;
    private ArtifactPropertyValue m_currentArtifact;
    private PropertyComponent m_db;
    private ResultCollection m_rc;
    private HashSet<string> m_existingProperties = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private bool m_artifactQueued;
    private static readonly string s_area = "TeamFoundationPropertyService";
    private static readonly string s_layer = nameof (CommandGetArtifactPropertyValue);
    private int m_ctr;

    internal CommandGetArtifactPropertyValue(IVssRequestContext requestContext)
      : base(requestContext)
    {
    }

    internal void Execute(
      IEnumerable<ArtifactSpec> artifactSpecs,
      IEnumerable<string> propertyNameFilters,
      PropertiesOptions options)
    {
      this.RequestContext.TraceEnter(10005000, CommandGetArtifactPropertyValue.s_area, CommandGetArtifactPropertyValue.s_layer, nameof (Execute));
      try
      {
        ArtifactSpec var = artifactSpecs.FirstOrDefault<ArtifactSpec>();
        ArgumentUtility.CheckForNull<ArtifactSpec>(var, "spec");
        ArtifactKind artifactKind = this.RequestContext.GetService<TeamFoundationPropertyService>().GetArtifactKind(this.RequestContext, var.Kind);
        this.m_propertyNameFilters = propertyNameFilters;
        this.m_artifactKind = artifactKind;
        this.m_db = this.RequestContext.CreateComponent<PropertyComponent>(this.m_artifactKind.DataspaceCategory);
        this.m_result = new StreamingCollection<ArtifactPropertyValue>((Command) this);
        if (artifactSpecs != null && this.RequestContext.IsTracing(10005012, TraceLevel.Info, CommandGetArtifactPropertyValue.s_area, CommandGetArtifactPropertyValue.s_layer))
          this.RequestContext.Trace(10005012, TraceLevel.Info, CommandGetArtifactPropertyValue.s_area, CommandGetArtifactPropertyValue.s_layer, "Sending {0} specs to the database for kind {1}.", (object) artifactSpecs.Count<ArtifactSpec>(), (object) artifactKind.Kind);
        if (propertyNameFilters != null && this.RequestContext.IsTracing(10005014, TraceLevel.Info, CommandGetArtifactPropertyValue.s_area, CommandGetArtifactPropertyValue.s_layer))
          this.RequestContext.Trace(10005014, TraceLevel.Info, CommandGetArtifactPropertyValue.s_area, CommandGetArtifactPropertyValue.s_layer, "Sending {0} filters to the database for kind {1}.", (object) propertyNameFilters.Count<string>(), (object) artifactKind.Kind);
        if (this.RequestContext.IsTracing(10005015, TraceLevel.Verbose, CommandGetArtifactPropertyValue.s_area, CommandGetArtifactPropertyValue.s_layer))
          this.RequestContext.Trace(10005015, TraceLevel.Verbose, CommandGetArtifactPropertyValue.s_area, CommandGetArtifactPropertyValue.s_layer, "Getting Properties - Ids: {0}, PropertyNames: {1}", artifactSpecs == null ? (object) "null" : (object) artifactSpecs.Select<ArtifactSpec, Guid>((Func<ArtifactSpec, Guid>) (x => x.Id != null && x.Id.Length == 16 ? new Guid(x.Id) : Guid.Empty)).Serialize<IEnumerable<Guid>>(), propertyNameFilters == null ? (object) "null" : (object) propertyNameFilters.Serialize<IEnumerable<string>>());
        this.m_rc = this.m_db.GetPropertyValue(artifactSpecs, this.m_propertyNameFilters, this.m_artifactKind, options);
        this.m_binder = this.m_rc.GetCurrent<DbArtifactPropertyValue>();
      }
      finally
      {
        this.RequestContext.TraceLeave(10005005, CommandGetArtifactPropertyValue.s_area, CommandGetArtifactPropertyValue.s_layer, nameof (Execute));
      }
    }

    internal void Execute(
      ArtifactKind kind,
      IEnumerable<string> propertyNameFilters,
      Guid? dataspaceIdentifier)
    {
      this.RequestContext.TraceEnter(19824719, CommandGetArtifactPropertyValue.s_area, CommandGetArtifactPropertyValue.s_layer, nameof (Execute));
      try
      {
        this.m_propertyNameFilters = propertyNameFilters;
        this.m_artifactKind = kind;
        this.m_db = this.RequestContext.CreateComponent<PropertyComponent>(this.m_artifactKind.DataspaceCategory);
        this.m_result = new StreamingCollection<ArtifactPropertyValue>((Command) this);
        this.m_rc = this.m_db.GetPropertyValue(kind, propertyNameFilters, dataspaceIdentifier);
        this.m_binder = this.m_rc.GetCurrent<DbArtifactPropertyValue>();
      }
      finally
      {
        this.RequestContext.TraceLeave(12985343, CommandGetArtifactPropertyValue.s_area, CommandGetArtifactPropertyValue.s_layer, nameof (Execute));
      }
    }

    public override void ContinueExecution()
    {
      this.RequestContext.TraceEnter(10005030, CommandGetArtifactPropertyValue.s_area, CommandGetArtifactPropertyValue.s_layer, nameof (ContinueExecution));
      try
      {
        while (!this.IsCacheFull && this.m_binder.MoveNext())
        {
          ++this.m_ctr;
          DbArtifactPropertyValue current = this.m_binder.Current;
          int num = this.m_currentDbArtifact == null || this.m_currentDbArtifact.SequenceId != current.SequenceId || !ArrayUtil.Equals(this.m_currentDbArtifact.ArtifactId, current.ArtifactId) ? 1 : (this.m_currentDbArtifact.DataspaceIdentifier != current.DataspaceIdentifier ? 1 : 0);
          if (num != 0)
            this.m_existingProperties.Clear();
          if (num != 0 || this.m_currentDbArtifact.Version != current.Version)
          {
            if (this.m_currentArtifact != null)
              this.m_currentArtifact.PropertyValues.IsComplete = true;
            this.m_currentDbArtifact = current;
            ArtifactSpec artifactSpec = !string.IsNullOrEmpty(current.Moniker) ? new ArtifactSpec(this.m_artifactKind.Kind, current.Moniker, current.Version, current.DataspaceIdentifier) : new ArtifactSpec(this.m_artifactKind.Kind, current.ArtifactId, current.Version, current.DataspaceIdentifier);
            this.m_currentArtifact = new ArtifactPropertyValue();
            this.m_currentArtifact.Spec = artifactSpec;
            this.m_currentArtifact.PropertyValues = new StreamingCollection<PropertyValue>((Command) this);
            this.m_currentArtifact.SequenceId = current.SequenceId;
            this.m_artifactQueued = false;
          }
          if (current.RequestedVersion == 0 || current.RequestedVersion == current.Version)
          {
            if (!this.m_artifactQueued)
            {
              this.m_result.Enqueue(this.m_currentArtifact);
              this.m_artifactQueued = true;
            }
            this.m_currentArtifact.PropertyValues.Enqueue(new PropertyValue(current.PropertyName, current.Value, current.TypeMatch, current.ChangedDate, current.ChangedBy, false));
            if (current.RequestedVersion != 0)
              this.m_existingProperties.Add(current.PropertyName);
          }
          else if (current.Version == 0 && !this.m_existingProperties.Contains(current.PropertyName))
          {
            if (!this.m_artifactQueued)
            {
              this.m_result.Enqueue(this.m_currentArtifact);
              this.m_artifactQueued = true;
            }
            this.m_currentArtifact.PropertyValues.Enqueue(new PropertyValue(current.PropertyName, current.Value, current.TypeMatch, current.ChangedDate, current.ChangedBy, false));
          }
        }
        if (this.IsCacheFull)
          return;
        if (this.m_currentArtifact != null)
          this.m_currentArtifact.PropertyValues.IsComplete = true;
        this.m_result.IsComplete = true;
        this.RequestContext.Trace(10005040, TraceLevel.Info, CommandGetArtifactPropertyValue.s_area, CommandGetArtifactPropertyValue.s_layer, "Get properties returned {0} results.", (object) this.m_ctr);
      }
      finally
      {
        this.RequestContext.TraceLeave(10005050, CommandGetArtifactPropertyValue.s_area, CommandGetArtifactPropertyValue.s_layer, string.Format("ContinueExecution [{0}]", (object) this.m_ctr));
      }
    }

    internal ObjectBinder<DbArtifactPropertyValue> Binder => this.m_binder;

    public StreamingCollection<ArtifactPropertyValue> Result => this.m_result;

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.m_db != null)
      {
        this.m_db.Dispose();
        this.m_db = (PropertyComponent) null;
      }
      if (this.m_rc != null)
      {
        this.m_rc.Dispose();
        this.m_rc = (ResultCollection) null;
      }
      this.m_binder = (ObjectBinder<DbArtifactPropertyValue>) null;
    }
  }
}
