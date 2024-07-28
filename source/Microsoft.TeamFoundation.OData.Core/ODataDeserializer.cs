// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataDeserializer
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData
{
  internal abstract class ODataDeserializer
  {
    protected IReaderValidator ReaderValidator;
    private readonly ODataInputContext inputContext;

    protected ODataDeserializer(ODataInputContext inputContext)
    {
      this.inputContext = inputContext;
      this.ReaderValidator = this.inputContext.MessageReaderSettings.Validator;
    }

    internal ODataMessageReaderSettings MessageReaderSettings => this.inputContext.MessageReaderSettings;

    internal bool ReadingResponse => this.inputContext.ReadingResponse;

    internal IEdmModel Model => this.inputContext.Model;

    internal PropertyAndAnnotationCollector CreatePropertyAndAnnotationCollector() => this.inputContext.CreatePropertyAndAnnotationCollector();
  }
}
