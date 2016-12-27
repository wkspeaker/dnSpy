﻿/*
    Copyright (C) 2014-2016 de4dot@gmail.com

    This file is part of dnSpy

    dnSpy is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    dnSpy is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with dnSpy.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.ComponentModel.Composition;
using dnSpy.Contracts.Hex;
using dnSpy.Contracts.Hex.Editor;
using dnSpy.Contracts.Hex.Files;
using dnSpy.Contracts.Hex.Files.DotNet;
using VSUTIL = Microsoft.VisualStudio.Utilities;

namespace dnSpy.Hex.Files.DotNet {
	[Export(typeof(HexFileStructureInfoProviderFactory))]
	[VSUTIL.Name(PredefinedHexFileStructureInfoProviderFactoryNames.DotNet)]
	[VSUTIL.Order(Before = PredefinedHexFileStructureInfoProviderFactoryNames.Default)]
	sealed class DotNetHexFileStructureInfoProviderFactory : HexFileStructureInfoProviderFactory {
		public override HexFileStructureInfoProvider Create(HexView hexView) =>
			new DotNetHexFileStructureInfoProvider();
	}

	sealed class DotNetHexFileStructureInfoProvider : HexFileStructureInfoProvider {
		public override HexIndexes[] GetSubStructureIndexes(HexBufferFile file, ComplexData structure, HexPosition position) {
			var body = structure as DotNetMethodBody;
			if (body != null) {
				if (body.Kind == DotNetMethodBodyKind.Tiny)
					return Array.Empty<HexIndexes>();
				var fatBody = body as FatMethodBody;
				if (fatBody != null) {
					if (fatBody.EHTable == null)
						return subStructFatWithoutEH;
					return subStructFatWithEH;
				}
			}

			return base.GetSubStructureIndexes(file, structure, position);
		}
		static readonly HexIndexes[] subStructFatWithEH = new HexIndexes[] {
			new HexIndexes(0, 4),
			new HexIndexes(4, 1),
			// Skip padding bytes @ 5
			new HexIndexes(6, 1),
		};
		static readonly HexIndexes[] subStructFatWithoutEH = new HexIndexes[] {
			new HexIndexes(0, 4),
			new HexIndexes(4, 1),
		};
	}
}
