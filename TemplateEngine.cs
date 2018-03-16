﻿using System.Collections.Generic;

using JetBrains.Annotations;

using SKBKontur.Catalogue.ExcelObjectPrinter.DocumentPrimitivesInterfaces;
using SKBKontur.Catalogue.ExcelObjectPrinter.ParseCollection;
using SKBKontur.Catalogue.ExcelObjectPrinter.RenderCollection;
using SKBKontur.Catalogue.ExcelObjectPrinter.RenderingTemplates;
using SKBKontur.Catalogue.ExcelObjectPrinter.TableBuilder;
using SKBKontur.Catalogue.ExcelObjectPrinter.TableParser;
using SKBKontur.Catalogue.Objects;

namespace SKBKontur.Catalogue.ExcelObjectPrinter
{
    public class TemplateEngine : ITemplateEngine
    {
        public TemplateEngine([NotNull] ITable templateTable)
        {
            this.templateTable = templateTable;
            templateCollection = new TemplateCollection(templateTable);
            rendererCollection = new RendererCollection(templateCollection);
            parserCollection = new ParserCollection();
        }

        public void Render<TModel>([NotNull] ITableBuilder tableBuilder, [NotNull] TModel model)
        {
            var renderingTemplate = templateCollection.GetTemplate(rootTemplateName)
                                    ?? throw new InvalidProgramStateException($"Template with name {rootTemplateName} not found in xlsx");
            tableBuilder.CopyFormControlsFrom(templateTable);
            tableBuilder.CopyDataValidationsFrom(templateTable);
            var render = rendererCollection.GetRenderer(model.GetType());
            render.Render(tableBuilder, model, renderingTemplate);
        }

        public (TModel model, Dictionary<string, string> mappingForErrors) Parse<TModel>([NotNull] ITableParser tableParser)
            where TModel : new()
        {
            var renderingTemplate = templateCollection.GetTemplate(rootTemplateName)
                                    ?? throw new InvalidProgramStateException($"Template with name {rootTemplateName} not found in xlsx");
            var parser = parserCollection.GetClassParser();
            var fieldsMappingForErrors = new Dictionary<string, string>();
            return (model : parser.Parse<TModel>(tableParser, renderingTemplate, (name, value) => fieldsMappingForErrors.Add(name, value)), mappingForErrors: fieldsMappingForErrors);
        }

        public void Dispose()
        {
            templateTable.Dispose();
        }

        private const string rootTemplateName = "RootTemplate";

        [NotNull]
        private readonly ITable templateTable;

        [NotNull]
        private readonly ITemplateCollection templateCollection;

        [NotNull]
        private readonly IRendererCollection rendererCollection;

        [NotNull]
        private readonly IParserCollection parserCollection;
    }
}