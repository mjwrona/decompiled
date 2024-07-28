// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightDeltaReader
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Threading.Tasks;

namespace Microsoft.OData.JsonLight
{
  internal sealed class ODataJsonLightDeltaReader : ODataDeltaReader
  {
    private readonly ODataJsonLightReader underlyingReader;
    private int nestedLevel;

    public ODataJsonLightDeltaReader(
      ODataJsonLightInputContext jsonLightInputContext,
      IEdmNavigationSource navigationSource,
      IEdmEntityType expectedEntityType)
    {
      this.underlyingReader = new ODataJsonLightReader(jsonLightInputContext, navigationSource, (IEdmStructuredType) expectedEntityType, true, readingDelta: true);
    }

    public override ODataDeltaReaderState State
    {
      get
      {
        if (this.nestedLevel > 0 || this.underlyingReader.State == ODataReaderState.NestedResourceInfoEnd)
          return ODataDeltaReaderState.NestedResource;
        switch (this.underlyingReader.State)
        {
          case ODataReaderState.Start:
            return ODataDeltaReaderState.Start;
          case ODataReaderState.ResourceStart:
            return ODataDeltaReaderState.DeltaResourceStart;
          case ODataReaderState.ResourceEnd:
            return ODataDeltaReaderState.DeltaResourceEnd;
          case ODataReaderState.Completed:
            return ODataDeltaReaderState.Completed;
          case ODataReaderState.DeltaResourceSetStart:
            return ODataDeltaReaderState.DeltaResourceSetStart;
          case ODataReaderState.DeltaResourceSetEnd:
            return ODataDeltaReaderState.DeltaResourceSetEnd;
          case ODataReaderState.DeletedResourceEnd:
            return ODataDeltaReaderState.DeltaDeletedEntry;
          case ODataReaderState.DeltaLink:
            return ODataDeltaReaderState.DeltaLink;
          case ODataReaderState.DeltaDeletedLink:
            return ODataDeltaReaderState.DeltaDeletedLink;
          default:
            return ODataDeltaReaderState.NestedResource;
        }
      }
    }

    public override ODataReaderState SubState
    {
      get
      {
        if (this.nestedLevel == 1 && this.underlyingReader.State == ODataReaderState.NestedResourceInfoStart)
          return ODataReaderState.Start;
        if (this.nestedLevel == 0 && this.underlyingReader.State == ODataReaderState.NestedResourceInfoEnd)
          return ODataReaderState.Completed;
        return this.nestedLevel <= 0 ? ODataReaderState.Start : this.underlyingReader.State;
      }
    }

    public override ODataItem Item => this.underlyingReader.Item is ODataDeletedResource entry ? (ODataItem) ODataDeltaDeletedEntry.GetDeltaDeletedEntry(entry) : this.underlyingReader.Item;

    public override bool Read()
    {
      bool flag = this.underlyingReader.Read();
      if (this.underlyingReader.State == ODataReaderState.DeletedResourceStart)
      {
        while ((flag = this.underlyingReader.Read()) && this.underlyingReader.State != ODataReaderState.DeletedResourceEnd)
          this.SetNestedLevel();
      }
      this.SetNestedLevel();
      return flag;
    }

    public override Task<bool> ReadAsync() => this.underlyingReader.ReadAsync().FollowOnSuccessWith<bool, bool>((Func<Task<bool>, bool>) (t =>
    {
      if (this.underlyingReader.State == ODataReaderState.DeletedResourceStart)
        this.SkipToDeletedResourceEnd();
      this.SetNestedLevel();
      return t.Result;
    }));

    private async void SkipToDeletedResourceEnd()
    {
      ODataJsonLightDeltaReader lightDeltaReader = this;
      if (lightDeltaReader.underlyingReader.State == ODataReaderState.DeletedResourceEnd)
        return;
      // ISSUE: reference to a compiler-generated method
      await lightDeltaReader.underlyingReader.ReadAsync().FollowOnSuccessWith<bool>(new Action<Task<bool>>(lightDeltaReader.\u003CSkipToDeletedResourceEnd\u003Eb__11_0));
    }

    private void SetNestedLevel()
    {
      if (this.underlyingReader.State == ODataReaderState.NestedResourceInfoStart)
      {
        ++this.nestedLevel;
      }
      else
      {
        if (this.underlyingReader.State != ODataReaderState.NestedResourceInfoEnd)
          return;
        --this.nestedLevel;
      }
    }
  }
}
