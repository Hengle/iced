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

using System;
using System.IO;
using Generator.Enums;
using Generator.IO;

namespace Generator.Tables.Rust {
	sealed class RustD3nowCodeValuesTableGenerator : ID3nowCodeValuesTableGenerator {
		readonly IdentifierConverter idConverter;
		readonly GeneratorOptions generatorOptions;

		public RustD3nowCodeValuesTableGenerator(GeneratorOptions generatorOptions) {
			idConverter = RustIdentifierConverter.Create();
			this.generatorOptions = generatorOptions;
		}

		public void Generate((int index, EnumValue enumValue)[] infos) {
			var filename = Path.Combine(generatorOptions.RustDir, "decoder", "handlers_3dnow.rs");
			var updater = new FileUpdater(TargetLanguage.Rust, "D3nowCodeValues", filename);
			updater.Generate(writer => WriteTable(writer, infos));
		}

		void WriteTable(FileWriter writer, (int index, EnumValue enumValue)[] infos) {
			var values = new EnumValue?[0x100];
			foreach (var info in infos) {
				if (!(values[info.index] is null))
					throw new InvalidOperationException();
				values[info.index] = info.enumValue;
			}
			var codeName = CodeEnum.Instance.Name(idConverter);
			var invalid = CodeEnum.Instance["INVALID"];
			foreach (var value in values) {
				var enumValue = value ?? invalid;
				writer.WriteLine($"{codeName}::{enumValue.Name(idConverter)} as u16,");
			}
		}
	}
}
