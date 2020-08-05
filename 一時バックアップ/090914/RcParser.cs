using System;
using System.Text;
using System.IO;
using System.Collections;

using RigidChips;
using System.Collections.Generic;

namespace RigidChips {

	///<summery>
	///モデルデータを解析し、それを指定したコアに読み込むクラス。
	///</summery>
	class RcModelParser {
		public static RcChipCore Parse(string modelData, RcChipCore coreChip, RcData gen) {
			char openBrace = char.Parse("{");
			char closeBrace = char.Parse("}");

			modelData = modelData.Replace(" ","").Replace("\t","").Replace("\n","").Replace("\r","");
			string[] blocks = modelData.Split(openBrace,closeBrace);
			Stack<RcChipBase> chipstack = new Stack<RcChipBase>();
			RcChipBase buff;

			for(int i = 0; i < blocks.Length;i++){
				if (blocks[i].ToLower().StartsWith("core")) {
					coreChip.Read(blocks[i]);
					chipstack.Push(coreChip);
				}
				else if (blocks[i] == "") {
					if (chipstack.Count == 1) break;
					buff = (RcChipBase)chipstack.Pop();
					((RcChipBase)chipstack.Peek()).Add(buff.JointPosition, buff);
				}
				else
					chipstack.Push(RcChipBase.Parse(gen, blocks[i]));
				
			}

			return (RcChipCore)chipstack.Pop();


		}

	}
}