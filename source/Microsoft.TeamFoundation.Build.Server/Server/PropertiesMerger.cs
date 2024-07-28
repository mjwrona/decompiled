// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.PropertiesMerger
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal sealed class PropertiesMerger : IDisposable
  {
    public PropertiesMerger(
      string artifactTypeName,
      BuildCommand command,
      IVssRequestContext requestContext,
      IList<string> propertyNames)
    {
      this.MasterCommand = command;
      this.PropertyNames = propertyNames;
      this.RequestContext = requestContext;
      this.ArtifactTypeName = artifactTypeName;
      this.PropertyService = this.RequestContext.GetService<TeamFoundationPropertyService>();
      this.BuildComponent = this.RequestContext.CreateComponent<BuildComponent>("Build");
      this.DataReader = (TeamFoundationDataReader) null;
      this.ArtifactSpecs = new List<ArtifactSpec>();
      this.PropertiesCollections = new List<StreamingCollection<PropertyValue>>();
      requestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "BuildInformationMerger constructed");
    }

    public void Dispose()
    {
      if (this.BuildComponent != null)
      {
        this.BuildComponent.Dispose();
        this.BuildComponent = (BuildComponent) null;
      }
      if (this.DataReader == null)
        return;
      this.DataReader.Dispose();
      this.DataReader = (TeamFoundationDataReader) null;
    }

    public void Enqueue(string buildUri, StreamingCollection<PropertyValue> propertyValues)
    {
      this.RequestContext.TraceEnter(0, "Build", "Command", nameof (Enqueue));
      propertyValues.IsComplete = false;
      this.ArtifactSpecs.Add(ArtifactHelper.CreateArtifactSpec(buildUri));
      this.PropertiesCollections.Add(propertyValues);
      this.RequestContext.TraceLeave(0, "Build", "Command", nameof (Enqueue));
    }

    public bool TryMergeNext()
    {
      this.RequestContext.TraceEnter(0, "Build", "Command", nameof (TryMergeNext));
      if (this.ArtifactSpecs.Count == 0)
      {
        this.RequestContext.Trace(0, TraceLevel.Warning, "Build", "Command", "Exiting due to no artifact specs");
        return false;
      }
      if (this.DataReader == null)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Querying artifacts");
        this.DataReader = this.PropertyService.GetProperties(this.RequestContext, (IEnumerable<ArtifactSpec>) this.ArtifactSpecs, (IEnumerable<string>) this.PropertyNames, PropertiesOptions.None);
        this.ArtifactEnumerator = this.DataReader.CurrentEnumerable<ArtifactPropertyValue>().GetEnumerator();
        if (this.ArtifactEnumerator.MoveNext())
        {
          this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Getting first artifact query result");
          this.CurrentArtifactValue = this.ArtifactEnumerator.Current;
          if (this.CurrentArtifactValue != null)
            this.PropertyEnumerator = this.CurrentArtifactValue.PropertyValues.GetEnumerator();
        }
        else
        {
          this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "No artifact query result");
          this.CurrentArtifactValue = (ArtifactPropertyValue) null;
          this.ArtifactEnumerator = (IEnumerator<ArtifactPropertyValue>) null;
          this.PropertyEnumerator = (IEnumerator<PropertyValue>) null;
        }
      }
      while (this.CurrentArtifactValue != null)
      {
        string y = ArtifactHelper.EncodeUri(this.ArtifactTypeName, this.CurrentArtifactValue.Spec);
        string x = ArtifactHelper.EncodeUri(this.ArtifactTypeName, this.ArtifactSpecs[0]);
        bool flag = VssStringComparer.Url.Equals(x, y);
        if (this.PropertyEnumerator != null & flag)
        {
          this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Enumerating properties of uri '{0}'", (object) x);
          while (!this.MasterCommand.IsCacheFull && this.PropertyEnumerator.MoveNext())
          {
            this.PropertiesCollections[0].Enqueue(this.PropertyEnumerator.Current);
            this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Read property '<{0},{1}>'", (object) this.PropertyEnumerator.Current.PropertyName, this.PropertyEnumerator.Current.Value);
          }
          if (this.MasterCommand.IsCacheFull)
          {
            this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "The cache is full, exiting but still having data to merge");
            return true;
          }
        }
        this.PropertiesCollections[0].IsComplete = true;
        this.PropertiesCollections.RemoveAt(0);
        this.ArtifactSpecs.RemoveAt(0);
        if (!flag)
          this.RequestContext.Trace(0, TraceLevel.Warning, "Build", "Command", "Skipped uri '{0}' because it is not matched querried uri '{1}'", (object) x, (object) y);
        else if (this.ArtifactEnumerator.MoveNext())
        {
          this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Getting next artifact query result");
          this.CurrentArtifactValue = this.ArtifactEnumerator.Current;
          if (this.CurrentArtifactValue != null)
            this.PropertyEnumerator = this.CurrentArtifactValue.PropertyValues.GetEnumerator();
        }
        else
        {
          this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "No artifact query result left");
          this.CurrentArtifactValue = (ArtifactPropertyValue) null;
          this.ArtifactEnumerator = (IEnumerator<ArtifactPropertyValue>) null;
          this.PropertyEnumerator = (IEnumerator<PropertyValue>) null;
        }
      }
      while (this.PropertiesCollections.Count > 0)
      {
        this.PropertiesCollections[0].IsComplete = true;
        this.PropertiesCollections.RemoveAt(0);
        this.ArtifactSpecs.RemoveAt(0);
      }
      if (this.DataReader != null)
      {
        this.DataReader.Dispose();
        this.DataReader = (TeamFoundationDataReader) null;
      }
      this.RequestContext.TraceLeave(0, "Build", "Command", nameof (TryMergeNext));
      return false;
    }

    private BuildCommand MasterCommand { get; set; }

    private TeamFoundationDataReader DataReader { get; set; }

    private ArtifactPropertyValue CurrentArtifactValue { get; set; }

    private IEnumerator<PropertyValue> PropertyEnumerator { get; set; }

    private IEnumerator<ArtifactPropertyValue> ArtifactEnumerator { get; set; }

    private IVssRequestContext RequestContext { get; set; }

    private TeamFoundationPropertyService PropertyService { get; set; }

    private BuildComponent BuildComponent { get; set; }

    private string ArtifactTypeName { get; set; }

    private IList<string> PropertyNames { get; set; }

    private List<ArtifactSpec> ArtifactSpecs { get; set; }

    private List<StreamingCollection<PropertyValue>> PropertiesCollections { get; set; }
  }
}
