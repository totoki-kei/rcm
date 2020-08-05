using System;
using System.Text;
using System.IO;
using System.Collections;

using RigidChips;

namespace RigidChips {
	class RcModelParser {
		public static RcChipCore Parse(string modelData, RcChipCore coreChip, RcData gen) {
			char openBrace = char.Parse("{");
			char closeBrace = char.Parse("}");

			int index;
			int nIndex;
			while ((index = modelData.IndexOf("//")) != -1) {
				nIndex = modelData.IndexOf("\n", index);
				if (nIndex == -1) {
					nIndex = modelData.Length;
				}
				modelData = modelData.Remove(index, (nIndex - index));
			}

			modelData = modelData.Replace(" ","").Replace("\t","").Replace("\n","").Replace("\r","");
			string[] blocks = modelData.Split(openBrace,closeBrace);
			Stack chipstack = new Stack();
			RcChipBase buff;

			for(int i = 0; i < blocks.Length;i++){
				if(blocks[i].ToLower() == "body")
					chipstack.Push(blocks[i]);
				else if(blocks[i].ToLower().StartsWith("core")){
					coreChip.Read(blocks[i]);
					chipstack.Push(coreChip);
				}
				else if(blocks[i] == ""){
					if(chipstack.Count == 1)break;
					buff = (RcChipBase)chipstack.Pop();
					((RcChipBase)chipstack.Peek()).Add(buff.JointPosition,buff);
				}
				else
					chipstack.Push(RcChipBase.Parse(gen,blocks[i]));
				
			}

			return (RcChipCore)chipstack.Pop();


		}
/*
		private static object parseInfoInfoBlock(string infoBlockStr, RcChipCore core, RcData gen, RcChipBase parent) {
			char openParenthesis = char.Parse("(");
			char colon = char.Parse(":");
			char comma = char.Parse(",");
			char equal = char.Parse("=");
			string infoBlockStrLower = infoBlockStr.Trim().ToLower();
			string typeBlockLower = null;
			string attrBlock = null;
			string[] tempCol = infoBlockStr.Split(openParenthesis);
			if (tempCol.Length == 2) {
				typeBlockLower = tempCol[0].Trim().ToLower();
				attrBlock = tempCol[1].Replace(")", "").Trim();

			}
			else if (tempCol.Length > 2) {
				throw new ApplicationException("パースエラーが発生しました。");
			}

			if (infoBlockStrLower == "val") {
				return infoBlock.Val;
			}
			else if (infoBlockStrLower == "key") {
				return infoBlock.Key;
			}
			else if (infoBlockStrLower == "body") {
				return infoBlock.Body;
			}
			else if (infoBlockStrLower == "script") {
				return infoBlock.Script;
			}
			else if (infoBlockStrLower == "lua") {
				return infoBlock.Lua;
			}
			else if (typeBlockLower != null) {
				RcChipBase newChip = null;
				if (typeBlockLower.IndexOf(":") != -1) {
					string posBlock = null;
					string chipTypeBlock = null;
					string[] tempCol2 = typeBlockLower.Split(colon);
					if (tempCol2.Length == 2) {
						posBlock = tempCol2[0].Trim();
						chipTypeBlock = tempCol2[1].Trim();


						if (chipTypeBlock == "chip") {
							newChip = new RcChipChip(gen, parent, RcJointPosition.NULL);
						}
						else if (chipTypeBlock == "rudder") {
							newChip = new RcChipRudder(gen, parent, RcJointPosition.NULL);
						}
						else if (chipTypeBlock == "trim") {
							newChip = new RcChipTrim(gen, parent, RcJointPosition.NULL);
						}
						else if (chipTypeBlock == "frame") {
							newChip = new RcChipFrame(gen, parent, RcJointPosition.NULL);
						}
						else if (chipTypeBlock == "rudderf") {
							newChip = new RcChipRudderF(gen, parent, RcJointPosition.NULL);
						}
						else if (chipTypeBlock == "trimf") {
							newChip = new RcChipTrimF(gen, parent, RcJointPosition.NULL);
						}
						else if (chipTypeBlock == "weight") {
							newChip = new RcChipWeight(gen, parent, RcJointPosition.NULL);
						}
						else if (chipTypeBlock == "cowl") {
							newChip = new RcChipCowl(gen, parent, RcJointPosition.NULL);
						}
						else if (chipTypeBlock == "jet") {
							newChip = new RcChipJet(gen, parent, RcJointPosition.NULL);
						}
						else if (chipTypeBlock == "wheel") {
							newChip = new RcChipWheel(gen, parent, RcJointPosition.NULL);
						}
						else if (chipTypeBlock == "rlw") {
							newChip = new RcChipRLW(gen, parent, RcJointPosition.NULL);
						}
						else if (chipTypeBlock == "arm") {
							newChip = new RcChipArm(gen, parent, RcJointPosition.NULL);
						}

						if (posBlock == "n") {
							newChip.JointPosition = RcJointPosition.North;
						}
						else if (posBlock == "s") {
							newChip.JointPosition = RcJointPosition.South;
						}
						else if (posBlock == "e") {
							newChip.JointPosition = RcJointPosition.East;
						}
						else if (posBlock == "w") {
							newChip.JointPosition = RcJointPosition.West;
						}
						else {
							throw new ApplicationException("パースエラーが発生しました。");
						}

						if (attrBlock != null) {
							string[] attrs = attrBlock.Split(comma);

							for (int i = 0; i < attrs.Length; i++) {
								string[] tempAttr = attrs[i].Split(equal);
								if (tempAttr.Length == 2) {
									tempAttr[0] = tempAttr[0].Trim();
									tempAttr[1] = tempAttr[1].Trim();
									try {
										//if(tempAttr[0] == ""){
										//    tempAttr[0] = "";
										//}
										tempAttr[0] = tempAttr[0].ToUpper().Substring(0, 1) + tempAttr[0].Substring(1, tempAttr[0].Length - 1);
										RigidChips.RcAttrValue atr = newChip[tempAttr[0]];
										atr.Const = float.Parse(tempAttr[1]);
										newChip[tempAttr[0]] = atr;
									}
									catch {
									}
								}
								else if(tempAttr.Length > 2){
									throw new ApplicationException("パースエラーが発生しました。");
								}
							}

						}

						return newChip;
					}
					else if (typeBlockLower == "core") {
						newChip = core;

						if (attrBlock != null) {
							string[] attrs = attrBlock.Split(comma);
							if (attrs.Length != 1) {
								for (int i = 0; i < attrs.Length; i++) {
									string[] tempAttr = attrs[i].Split(equal);
									if (tempAttr.Length == 2) {
										tempAttr[0] = tempAttr[0].Trim();
										tempAttr[1] = tempAttr[1].Trim();
										try {
											//if(tempAttr[0] == ""){
											//    tempAttr[0] = "";
											//}
											tempAttr[0] = tempAttr[0].ToUpper().Substring(0, 1) + tempAttr[0].Substring(1, tempAttr[0].Length - 1);
											RigidChips.RcAttrValue atr = newChip[tempAttr[0]];
											atr.Const = float.Parse(tempAttr[1]);
											newChip[tempAttr[0]] = atr;
										}
										catch {
										}
									}
									else if (tempAttr.Length > 2) {
										throw new ApplicationException("パースエラーが発生しました。");
									}
								}
							}
						}


						return newChip;
					}
					else if (tempCol2.Length > 2) {
						throw new ApplicationException("パースエラーが発生しました。");
					}

				}
			}

			return infoBlockStr.Trim();

		}
	*/
	}
}