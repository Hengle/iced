/*
Copyright (C) 2018-2019 de4dot@gmail.com

Permission is hereby granted, free of charge, to any person obtaining
a copy of this software and associated documentation files (the
"Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System.Collections.Generic;
using System.IO;
using Generator.IO;

namespace Generator.Formatters.CSharp {
	sealed class CSharpFormatterTableGenerator : IFormatterTableGenerator {
		readonly GeneratorOptions generatorOptions;

		public CSharpFormatterTableGenerator(GeneratorOptions generatorOptions) => this.generatorOptions = generatorOptions;

		public void Generate() {
			var serializers = new List<CSharpFormatterTableSerializer>();
			if (generatorOptions.HasGasFormatter)
				serializers.Add(new GasCSharpFormatterTableSerializer());
			if (generatorOptions.HasIntelFormatter)
				serializers.Add(new IntelCSharpFormatterTableSerializer());
			if (generatorOptions.HasMasmFormatter)
				serializers.Add(new MasmCSharpFormatterTableSerializer());
			if (generatorOptions.HasNasmFormatter)
				serializers.Add(new NasmCSharpFormatterTableSerializer());
			if (serializers.Count == 0)
				return;

			const string FormatterStringsTableName = "FormatterStringsTable";
			var stringsTable = new StringsTableImpl(CSharpConstants.FormatterNamespace, FormatterStringsTableName, CSharpConstants.AnyFormatterDefine);

			foreach (var serializer in serializers)
				serializer.Initialize(stringsTable);

			stringsTable.Freeze();

			using (var writer = new FileWriter(TargetLanguage.CSharp, FileUtils.OpenWrite(Path.Combine(CSharpConstants.GetDirectory(generatorOptions, CSharpConstants.FormatterNamespace), FormatterStringsTableName + ".g.cs"))))
				stringsTable.Serialize(writer);

			foreach (var serializer in serializers) {
				using (var writer = new FileWriter(TargetLanguage.CSharp, FileUtils.OpenWrite(serializer.GetFilename(generatorOptions))))
					serializer.Serialize(writer, stringsTable);
			}
		}
	}
}
