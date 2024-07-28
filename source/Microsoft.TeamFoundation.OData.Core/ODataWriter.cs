// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataWriter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  public abstract class ODataWriter
  {
    public abstract void WriteStart(ODataResourceSet resourceSet);

    public ODataWriter Write(ODataResourceSet resourceSet)
    {
      this.WriteStart(resourceSet);
      this.WriteEnd();
      return this;
    }

    public ODataWriter Write(ODataResourceSet resourceSet, Action nestedAction)
    {
      this.WriteStart(resourceSet);
      nestedAction();
      this.WriteEnd();
      return this;
    }

    public virtual Task WriteStartAsync(ODataResourceSet resourceSet) => TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.WriteStart(resourceSet)));

    public virtual void WriteStart(ODataDeltaResourceSet deltaResourceSet) => throw new NotImplementedException();

    public ODataWriter Write(ODataDeltaResourceSet deltaResourceSet)
    {
      this.WriteStart(deltaResourceSet);
      this.WriteEnd();
      return this;
    }

    public ODataWriter Write(ODataDeltaResourceSet deltaResourceSet, Action nestedAction)
    {
      this.WriteStart(deltaResourceSet);
      nestedAction();
      this.WriteEnd();
      return this;
    }

    public virtual Task WriteStartAsync(ODataDeltaResourceSet deltaResourceSet) => TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.WriteStart(deltaResourceSet)));

    public abstract void WriteStart(ODataResource resource);

    public ODataWriter Write(ODataResource resource)
    {
      this.WriteStart(resource);
      this.WriteEnd();
      return this;
    }

    public ODataWriter Write(ODataResource resource, Action nestedAction)
    {
      this.WriteStart(resource);
      nestedAction();
      this.WriteEnd();
      return this;
    }

    public ODataWriter Write(ODataDeletedResource deletedResource)
    {
      this.WriteStart(deletedResource);
      this.WriteEnd();
      return this;
    }

    public ODataWriter Write(ODataDeletedResource deletedResource, Action nestedAction)
    {
      this.WriteStart(deletedResource);
      nestedAction();
      this.WriteEnd();
      return this;
    }

    public ODataWriter Write(ODataDeltaLink deltaLink)
    {
      this.WriteDeltaLink(deltaLink);
      return this;
    }

    public ODataWriter Write(ODataDeltaDeletedLink deltaDeletedLink)
    {
      this.WriteDeltaDeletedLink(deltaDeletedLink);
      return this;
    }

    public virtual Task WriteStartAsync(ODataResource resource) => TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.WriteStart(resource)));

    public virtual void WriteStart(ODataDeletedResource deletedResource) => throw new NotImplementedException();

    public virtual Task WriteStartAsync(ODataDeletedResource deletedResource) => TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.WriteStart(deletedResource)));

    public virtual void WriteDeltaLink(ODataDeltaLink deltaLink) => throw new NotImplementedException();

    public virtual Task WriteDeltaLinkAsync(ODataDeltaLink deltaLink) => TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.WriteDeltaLink(deltaLink)));

    public virtual void WriteDeltaDeletedLink(ODataDeltaDeletedLink deltaDeletedLink) => throw new NotImplementedException();

    public virtual Task WriteDeltaDeletedLinkAsync(ODataDeltaDeletedLink deltaDeletedLink) => TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.WriteDeltaDeletedLink(deltaDeletedLink)));

    public abstract void WriteStart(ODataNestedResourceInfo nestedResourceInfo);

    public ODataWriter Write(ODataNestedResourceInfo nestedResourceInfo)
    {
      this.WriteStart(nestedResourceInfo);
      this.WriteEnd();
      return this;
    }

    public ODataWriter Write(ODataNestedResourceInfo nestedResourceInfo, Action nestedAction)
    {
      this.WriteStart(nestedResourceInfo);
      nestedAction();
      this.WriteEnd();
      return this;
    }

    public virtual Task WriteStartAsync(ODataNestedResourceInfo nestedResourceInfo) => TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.WriteStart(nestedResourceInfo)));

    public virtual void WritePrimitive(ODataPrimitiveValue primitiveValue) => throw new NotImplementedException();

    public ODataWriter Write(ODataPrimitiveValue primitiveValue)
    {
      this.WritePrimitive(primitiveValue);
      return this;
    }

    public virtual Task WritePrimitiveAsync(ODataPrimitiveValue primitiveValue) => TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.WritePrimitive(primitiveValue)));

    public virtual void WriteStart(ODataPropertyInfo primitiveProperty) => throw new NotImplementedException();

    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    public ODataWriter Write(ODataProperty primitiveProperty)
    {
      this.WriteStart((ODataPropertyInfo) primitiveProperty);
      this.WriteEnd();
      return this;
    }

    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    public ODataWriter Write(ODataProperty primitiveProperty, Action nestedAction)
    {
      this.WriteStart((ODataPropertyInfo) primitiveProperty);
      nestedAction();
      this.WriteEnd();
      return this;
    }

    public virtual Task WriteStartAsync(ODataProperty primitiveProperty) => TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.WriteStart((ODataPropertyInfo) primitiveProperty)));

    public virtual Stream CreateBinaryWriteStream() => throw new NotImplementedException();

    public ODataWriter WriteStream(ODataBinaryStreamValue stream)
    {
      Stream binaryWriteStream = this.CreateBinaryWriteStream();
      stream.Stream.CopyTo(binaryWriteStream);
      binaryWriteStream.Flush();
      binaryWriteStream.Dispose();
      return this;
    }

    public virtual Task<Stream> CreateBinaryWriteStreamAsync() => TaskUtils.GetTaskForSynchronousOperation<Stream>((Func<Stream>) (() => this.CreateBinaryWriteStream()));

    public virtual TextWriter CreateTextWriter() => throw new NotImplementedException();

    public virtual Task<TextWriter> CreateTextWriterAsync() => TaskUtils.GetTaskForSynchronousOperation<TextWriter>((Func<TextWriter>) (() => this.CreateTextWriter()));

    public abstract void WriteEnd();

    public virtual Task WriteEndAsync() => TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.WriteEnd()));

    public abstract void WriteEntityReferenceLink(ODataEntityReferenceLink entityReferenceLink);

    public virtual Task WriteEntityReferenceLinkAsync(ODataEntityReferenceLink entityReferenceLink) => TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.WriteEntityReferenceLink(entityReferenceLink)));

    public abstract void Flush();

    public virtual Task FlushAsync() => TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.Flush()));
  }
}
