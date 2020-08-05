using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Specialized;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Runtime.InteropServices.ComTypes;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace RigidChips {

	///<summery>
	///ごく単純な、コールバック用デリゲート
	///</summery>
	public delegate void MessageCallback(object param);

	//----------------------------------------------------------------------------------//
	///<summery>
	///すべてのチップの基本クラス
	///このクラスが実装するメソッドの大半は、派生クラス(各チップ)によってオーバーライドされる。
	///</summery>
	public class RcChipBase{
		public RcData Generics;
		public string Name;
		protected RcAttrValue angle;
		protected RcAttrValue user1,user2;
		public RcAttrValue ChipColor;
		public RcChipBase Parent;
		public RcChipBase[] Child;

		public int RegistID = -1;

		protected RcJointPosition jointPosition;
		protected Matrix matTranslation;
		protected Matrix matRotation;
		protected Matrix matParent;
		protected long matVersion;

		public string Comment = "";

		/// <summary>
		/// チップの基本的な初期化を行う。
		/// </summary>
		/// <param name="gen">所属する RcData インスタンス</param>
		/// <param name="parent">接続されるチップ</param>
		public RcChipBase(RcData gen,RcChipBase parent,RcJointPosition pos){
			Child = new RcChipBase[RcData.ChildCapasity];	//	仕様が変わっていなければひとつのチップにつけられるチップの最大数は12
			Generics = gen;
			Attach(parent,pos);
			angle.Const = 0f;
			angle.Val = null;
			angle.isNegative = false;
			if(parent != null)
				this.ChipColor = parent.ChipColor;
			else
				this.ChipColor.SetValue(0xFFFFFF);
		}

		///<summery>
		///コンストラクタ(ヌル)
		///</summery>
		public RcChipBase(){}
		
		/// <summary>
		/// チップのワールド座標変換行列を、座標や親チップ、angleをふまえて更新する。
		/// </summary>
		public virtual void UpdateMatrix(){
			// このチップのangle値。
			float a = angle.Value;
			
			// 現在の変換行列の逆行列を取得する。
			Matrix invRotation = matRotation;
			invRotation.Invert();
			
			
			// angleを適用する。
			switch (this.AngleType) {
				case RcAngle.x:
					matTranslation = invRotation
						* Matrix.Translation(0f, 0f, 0.3f)
						* Matrix.RotationX((float)(a / 180f * Math.PI))
						* Matrix.Translation(0f, 0f, 0.3f)
						* matRotation;
					break;
				case RcAngle.y:
					matTranslation = invRotation
						* Matrix.Translation(0f, 0f, 0.3f)
						* Matrix.RotationY((float)(a / 180f * Math.PI))
						* Matrix.Translation(0f, 0f, 0.3f)
						* matRotation;
					break;
				case RcAngle.z:
					matTranslation = invRotation
						* Matrix.Translation(0f, 0f, 0.3f)
						* Matrix.RotationZ((float)(-a / 180f * Math.PI))
						* Matrix.Translation(0f, 0f, 0.3f)
						* matRotation;
					break;
			}

			// 親チップがWheel系だった場合、回転量を反映させる
			RcChipWheel wh = this.Parent as RcChipWheel;
			if (Generics.Preview && wh != null) {
				matTranslation *= Matrix.RotationY(wh.PreviewRotation);
			}

			// 親チップの変換行列を取得し、それを掛ける。
			if(Parent != null)	matTranslation *= Parent.Matrix;

			// 行列の生成タイムスタンプを更新。
			matVersion = System.DateTime.Now.Ticks;

			// 子チップが存在する場合、それらも更新。
			foreach(RcChipBase cb in Child)if(cb != null)
											   cb.UpdateMatrix();

		}

		/// <summary>
		/// 表示上の位置を表す行列
		/// </summary>
		public virtual Matrix Matrix{
			get{
				// 親の行列がより新しいものになっていたら更新。
				if(this.Parent != null && this.Parent.MatrixVersion > matVersion){
					this.UpdateMatrix();
				}

				return matTranslation;
			}
		}

		///<summery>
		///回転も含めた完全な行列
		///</summery>
		public virtual Matrix FullMatrix{
			get{
				return matRotation * Matrix;
			}
		}

		/// <summary>
		/// 行列のバージョン
		/// </summary>
		public virtual long MatrixVersion{
			get{
				return matVersion;
			}
		}

		/// <summary>
		/// 子チップを追加する。すでに最大数接続されているときには Exception が発生する。
		/// </summary>
		/// <param name="joint">どこに追加するかの RcJointPosition 定数</param>
		/// <param name="chip">追加するチップ</param>
		/// <param name="Registeration">ライブラリに登録するかどうか 省略時はTrue</param>
		public virtual void Add(RcJointPosition joint,RcChipBase chip, bool Registeration){
			for(int i = 0;i < RcData.ChildCapasity;i++){
				if(Child[i] == null){
					Child[i] = chip;
					Child[i].JointPosition = joint;
					Child[i].Parent = this;
					Child[i].UpdateMatrix();
					if(this.GetType() == chip.GetType() && Generics.EditOption.ConvertParentAttributes){
						string[] list = this.AttrNameList;
						foreach(string s in list){
							try{
								if(s == "Angle")continue;
								chip[s] = this[s];
							}
							catch{}
						}
					}
					if(Registeration)
						Generics.RegisterChipAll(chip);
					return;
				}
			}
			throw new Exception("これ以上子チップを格納できません。ひとつのチップに取り付けられるチップの数は12までです。");
		}

		/// <summary>
		/// 子チップを追加する。すでに最大数接続されているときには Exception が発生する。
		/// </summary>
		/// <param name="joint">どこに追加するかの RcJointPosition 定数</param>
		/// <param name="chip">追加するチップ</param>
		public void Add(RcJointPosition joint, RcChipBase chip)
		{
				Add(joint,chip,true);
		}
		

		/// <summary>
		/// すでについているチップを取り外す。存在しないチップを指定したときには Exception が発生する。
		/// </summary>
		/// <param name="chip">取り外したいチップ。</param>
		public virtual void Remove(RcChipBase chip){
			bool Removed = false;
			for(int i = 0; i < RcData.ChildCapasity;i++){
				if(Removed){
					Child[i-1] = Child[i];
				}
				else if(Child[i] == chip){
					chip.JointPosition = RcJointPosition.NULL;
					Generics.UnregisterChipAll(chip);
					Child[i] = null;
					Removed = true;
				}
			}
			if (Removed) {
				Child[RcData.ChildCapasity - 1] = null;
				return;
			}
			else {
				throw new Exception("指定されたチップは見つかりませんでした : " + chip.ToString());
			}
		}

		/// <summary>
		/// このチップを他のチップに接続する。
		/// </summary>
		/// <param name="to">接続されるチップ。</param>
		/// <param name="pos">接続位置。</param>
		public virtual void Attach(RcChipBase to,RcJointPosition pos){
			if(to == null)return;
				to.Add(pos,this);
		}

		/// <summary>
		/// 現在接続されているチップから取り外す。
		/// </summary>
		public virtual void Detach(){
				this.Parent.Remove(this);
		}

		/// <summary>
		/// チップを画面に描画する。
		/// </summary>
		public virtual void DrawChip(){
			MessageBox.Show("RcChipBase::DrawChip() Was Called.");
		}

		/// <summary>
		/// 使用可能な属性の名前の配列を得る。
		/// </summary>
		public virtual string[] AttrNameList{
			get{
				return null;
			}
		}

		/// <summary>
		/// 現在の全属性の値の配列を得る。インデックスはAttrNameListに対応。
		/// </summary>
		public virtual RcAttrValue[] AttrValList{
			get{
				return null;
			}
		}

		/// <summary>
		/// 属性の規定値を得る。インデックスはAttrNameListに対応。
		/// </summary>
		public virtual float[] AttrDefaultValueList{
			get{
				return null;
			}
		}

		/// <summary>
		/// このチップの種類を文字列で得る。
		/// </summary>
		public virtual string ChipType{
			get{
				return null;
			}
		}

		/// <summary>
		/// 指定した名前の属性の説明文を得る。
		/// </summary>
		/// <param name="AttrName">使用可能な属性名。</param>
		/// <returns>指定した属性の説明文。</returns>
		public virtual string AttrTip(string AttrName){
			if(AttrName == "Color")	return "チップ色"; else return null;
		}

		/// <summary>
		/// 指定した属性の値を設定・取得する。
		/// </summary>
		public virtual RcAttrValue this[string AttrName]{
			set{
				if(AttrName == "Color")
					ChipColor = value;
				else
					throw new Exception("指定された名前の属性は存在しません : " + AttrName);
			}

			get{
				if(AttrName == "Color")
					return ChipColor;

				else
					throw new Exception("指定された名前の属性は存在しません : " + AttrName);
			}
		}

		/// <summary>
		/// このチップの情報の文字列を生成する。
		/// </summary>
		/// <returns>このチップの文字列情報。</returns>
		public override string ToString(){
			return ToString(Generics.OutputOption);
		}
		/// <summary>
		/// このチップの情報の文字列を生成する。
		/// </summary>
		/// <returns>このチップの文字列情報。</returns>
		public virtual string ToString(RcOutputOptions outputOptions) {
			StringBuilder str = new StringBuilder();			
//			string str = "";
			string comma = outputOptions.CommaWithSpace ? ", " : ",";
			str.Append(this.ChipType);
			str.Append('(');

			string[] attrname = this.AttrNameList;
			float[] attrdef = this.AttrDefaultValueList;
			RcAttrValue[] attrval = this.AttrValList;
			if(this.Name != null && this.Name != ""){
				str.Append("Name=");
				str.Append(Name);
				str.Append(comma);
			}
			for(int i = 0;i < attrname.Length;i++){
				if(outputOptions.PrintAllAttributes || attrval[i].Val != null || attrval[i].Const != attrdef[i]){
					str.Append(attrname[i]);
					str.Append('=');
					str.Append(attrval[i].ToString());
					str.Append(comma);
				}
			}

			while(str[str.Length-1] == ' ' || str[str.Length-1] == ',')str.Length--;

			str.Append(')');


			return str.ToString();
		}

		/// <summary>
		/// このチップとその派生についての完全表現文字列を得る。
		/// </summary>
		/// <param name="tabs">インデントのタブ数</param>
		/// <returns>.rcd内に使用可能な Body ブロック用文字列</returns>
		public virtual string ToString(int tabs) {
			return ToString(tabs, Generics.OutputOption);
		}

		/// <summary>
		/// このチップとその派生についての完全表現文字列を得る。
		/// </summary>
		/// <param name="tabs">インデントのタブ数</param>
		/// <param name="outputOptions">使用する出力オプション</param>
		/// <returns>.rcd内に使用可能な Body ブロック用文字列</returns>
		public virtual string ToString(int tabs, RcOutputOptions outputOptions) {
//			string s = "";
			StringBuilder s = new StringBuilder();
			StringBuilder st = new StringBuilder();
			StringBuilder ss = new StringBuilder();
			if(outputOptions.IndentEnable){
				if(outputOptions.IndentBySpace)
					for(int i = 0;i < tabs * outputOptions.IndentNum ;  i++)st.Append(' ');
				else
					for(int i = 0;i < tabs; i++)st.Append('\t');
			}
			s.Append(st);
			//	接続方位
			switch(this.jointPosition){
				case RcJointPosition.North:
					s.Append("N:");
					break;
				case RcJointPosition.South:
					s.Append("S:");
					break;
				case RcJointPosition.East:
					s.Append("E:");
					break;
				case RcJointPosition.West:
					s.Append("W:");
					break;
			}


			s.Append(this.ToString(outputOptions));

			//	派生ブロック取得
			if(outputOptions.OpenBracketWithChipDefinition){

				if(!outputOptions.ReturnEndChipBracket){
					foreach(RcChipBase cb in this.Child)
						if(cb != null){
							ss.Append("\r\n");
							ss.Append(cb.ToString(tabs + 1, outputOptions));
						}
					if(ss.Length == 0){
						s.Append("{}");
						s.Append((this.Comment == null || this.Comment == "") ? "" : ("//" + this.Comment));
					}
					else{
						s.Append("{");
						s.Append((this.Comment == null || this.Comment == "") ? "" : ("//" + this.Comment));
						s.Append(ss.ToString());
						s.Append("\r\n");
						s.Append(st.ToString());
						s.Append("}");
					}
				}
				else{
					s.Append('{');
					s.Append((this.Comment == null || this.Comment == "") ? "" : ("//" + this.Comment));
					foreach(RcChipBase cb in this.Child)
						if(cb != null){
							s.Append("\r\n");
							s.Append(cb.ToString(tabs + 1, outputOptions));
						}
					s.Append("\r\n");
					s.Append(st.ToString());
					s.Append('}');
				}

			}
			else{
				s.Append(((this.Comment == null || this.Comment == "") ? "" : ("//" + this.Comment)) + "\r\n" + st.ToString() + "{");
				foreach(RcChipBase cb in this.Child)
					if(cb != null){
						s.Append("\r\n");
						s.Append(cb.ToString(tabs + 1, outputOptions));
					}
				if(!outputOptions.ReturnEndChipBracket && s[s.Length-1] == '{')
					s.Append('}');
				else{
					s.Append("\r\n");
					s.Append(st.ToString());
					s.Append('}');
				}

			}

			return s.ToString();
		}

		/// <summary>
		///	このチップ及び派生チップの総数。読み取り専用。
		/// </summary>
		public int ChipCount{
			get{
				int c = 1;
				foreach(RcChipBase cb in Child)if(cb != null){
					c += cb.ChipCount;	// 再帰
				}
				return c;
			}
		}

		/// <summary>
		/// マウスポインティングに対し、このオブジェクトが選択されたかどうかを得る。
		/// </summary>
		/// <param name="X">マウスカーソルのX座標</param>
		/// <param name="Y">マウスカーソルのY座標</param>
		/// <param name="ScrWidth">スクリーンの幅</param>
		/// <param name="ScrHeight">スクリーンの高さ</param>
		/// <returns>選択されたチップと距離の情報</returns>
		public virtual RcHitStatus IsHit(int X,int Y,int ScrWidth,int ScrHeight){
			RcHitStatus dist ,buff;
			dist.distance = float.MaxValue;
			dist.HitChip = null;
			foreach(RcChipBase c in Child){
				if(c != null){
					buff = c.IsHit(X,Y,ScrWidth,ScrHeight);
					if(dist.distance > buff.distance){
						dist = buff;
					}
				}
			}
			// 投影行列
			Matrix projMat = Generics.d3ddevice.Transform.Projection;
			// ビュー行列
			Matrix viewMat = Generics.d3ddevice.Transform.View;
			// ビューポート
			Viewport viewport = new Viewport();

			IntersectInformation sectinfo = new IntersectInformation();

			viewport.Width = ScrWidth; 
			viewport.Height = ScrHeight; 
			viewport.X = viewport.Y = 0;
			viewport.MaxZ = 1.0f; 
			viewport.MinZ = 0.0f;

			// クリックしたスクリーン座標からレイを計算し、対象メッシュとの交差をチェック 
			Vector3 vNear = Vector3.Unproject(new Vector3(X, Y, viewport.MinZ), 
				viewport, projMat, viewMat, this.Matrix /* ここがWorld行列 */);
			Vector3 vFar = Vector3.Unproject(new Vector3(X, Y, viewport.MaxZ), 
				viewport, projMat, viewMat, this.Matrix /* ここがWorld行列 */);
			Vector3 vDir = Vector3.Normalize(vFar - vNear);
			
			buff.distance = (Generics.imesh.Intersect(vNear, vDir, out sectinfo)) ? sectinfo.Dist : float.MaxValue; 
			if(dist.distance > buff.distance){
				dist.distance = buff.distance;
				dist.HitChip = this;
			}

			return dist;
		}

		/// <summary>
		/// 指定された相対インデックス値のチップを得る。
		/// </summary>
		/// <param name="idx">取得するチップのインデックス。この値は変更される。</param>
		/// <returns>要求されたインデックスのチップ</returns>
		public RcChipBase GetChildChip(ref int idx){
			RcChipBase buff;
			if(idx == 0)
				return this;
			else{
				idx--;
				foreach(RcChipBase cld in Child)if(cld != null){
					buff = cld.GetChildChip(ref idx);
					if(buff != null) return buff;
				}
			}
			return null;
		}

		/// <summary>
		/// このチップとその派生チップ全てを描画する。
		/// </summary>
		public void DrawChipAll(){
			DrawChip();
			foreach(RcChipBase cld in Child)if(cld != null)
				cld.DrawChipAll();
		}


		public virtual void ReserveDraw(ThreadStart fnc) {
			if (this.Parent != null)
				this.Parent.ReserveDraw(fnc);
			else
				fnc();
		}

		///<summery>
		///接続方向を得る。
		///</summery>
		public virtual RcJointPosition JointPosition{
			get{
				return jointPosition;
			}
			set{
				jointPosition = value;
				switch(jointPosition){
					case RcJointPosition.South:
						matRotation = Matrix.RotationY((float)Math.PI);
						break;
					case RcJointPosition.East:
						matRotation = Matrix.RotationY((float)(Math.PI * 0.5f));
						break;
					case RcJointPosition.West:
						matRotation = Matrix.RotationY((float)(Math.PI * 1.5f));
						break;
					case RcJointPosition.North:
					default:
						matRotation = Matrix.Identity;
						break;
				}
			}
		}

		/// <summary>
		/// このチップと同一の内容のRcChipBaseインスタンスを作成する。
		/// </summary>
		/// <param name="WithChild">派生チップ情報のクローンも作成するかどうかのフラグ</param>
		/// <returns>同一の内容を保持した新しいRcChipBaseへの参照</returns>
		public virtual RcChipBase Clone(bool WithChild,RcChipBase parent){
			return null;
		}

		public virtual void RotateLeft(){
			RcChipBase[] cld = new RcChipBase[RcData.ChildCapasity];
			RcJointPosition jp;
			this.Child.CopyTo(cld,0);

			foreach(RcChipBase cb in cld){
				if(cb == null)continue;
				jp = cb.JointPosition;
				cb.RotateLeft();
				cb.Detach();
				cb.Attach(this,(RcJointPosition)(((int)jp + 3) % 4));
			}
		}
		public virtual void RotateRight(){
			RcChipBase[] cld = new RcChipBase[RcData.ChildCapasity];
			RcJointPosition jp;
			this.Child.CopyTo(cld,0);

			foreach(RcChipBase cb in cld){
				if(cb == null)continue;
				jp = cb.JointPosition;
				cb.RotateRight();
				cb.Detach();
				cb.Attach(this,(RcJointPosition)(((int)jp + 1) % 4));
			}

		}

		public virtual void ReverseX(){
			RcChipBase[] cld = new RcChipBase[RcData.ChildCapasity];
			RcJointPosition jp;
			this.Child.CopyTo(cld,0);

			foreach(RcChipBase cb in cld){
				if(cb == null)continue;
				jp = cb.JointPosition;
				cb.ReverseX();
				if(((int)jp & 1) > 0){
					cb.Detach();
					cb.Attach(this,(RcJointPosition)(((int)jp + 2) % 4));
				}
			}
		}
		public virtual void ReverseY(){
			RcChipBase[] cld = new RcChipBase[RcData.ChildCapasity];
			RcJointPosition jp;
			this.Child.CopyTo(cld,0);

			foreach(RcChipBase cb in cld){
				if(cb == null)continue;
				jp = cb.JointPosition;
				cb.ReverseY();
			}
			
		}
		public virtual void ReverseZ(){
			RcChipBase[] cld = new RcChipBase[RcData.ChildCapasity];
			RcJointPosition jp;
			this.Child.CopyTo(cld,0);

			foreach(RcChipBase cb in cld){
				if(cb == null)continue;
				jp = cb.JointPosition;
				cb.ReverseZ();
				if(((int)jp % 2) == 0){
					cb.Detach();
					cb.Attach(this,(RcJointPosition)(((int)jp + 2) % 4));
				}
			}
		}

		///<summery>
		///文字列からチップを生成する。
		///</summery>
		public static RcChipBase Parse(RcData generics,string data){
			string[] param = data.Split(',',':','(',')','=');
			int index = 0;
			RcJointPosition jp = RcJointPosition.NULL;
			RcChipBase newchip;

			switch(param[index]){
				case "n":
				case "N":
					jp = RcJointPosition.North;
					index++;
					break;
				case "s":
				case "S":
					jp = RcJointPosition.South;
					index++;
					break;
				case "e":
				case "E":
					jp = RcJointPosition.East;
					index++;
					break;
				case "w":
				case "W":
					jp = RcJointPosition.West;
					index++;
					break;
			}

			switch(param[index].ToLower()){
				case "core":
					newchip = new RcChipCore(generics,null,RcJointPosition.NULL);
					index++;
					break;
				case "chip":
					newchip = new RcChipChip(generics,null,RcJointPosition.NULL);
					index++;
					break;
				case "frame":
					newchip = new RcChipFrame(generics,null,RcJointPosition.NULL);
					index++;
					break;
				case "rudder":
					newchip = new RcChipRudder(generics,null,RcJointPosition.NULL);
					index++;
					break;
				case "rudderf":
					newchip = new RcChipRudderF(generics,null,RcJointPosition.NULL);
					index++;
					break;
				case "trim":
					newchip = new RcChipTrim(generics,null,RcJointPosition.NULL);
					index++;
					break;
				case "trimf":
					newchip = new RcChipTrimF(generics,null,RcJointPosition.NULL);
					index++;
					break;
				case "wheel":
					newchip = new RcChipWheel(generics,null,RcJointPosition.NULL);
					index++;
					break;
				case "rlw":
					newchip = new RcChipRLW(generics,null,RcJointPosition.NULL);
					index++;
					break;
				case "jet":
					newchip = new RcChipJet(generics,null,RcJointPosition.NULL);
					index++;
					break;
				case "weight":
					newchip = new RcChipWeight(generics,null,RcJointPosition.NULL);
					index++;
					break;
				case "cowl":
					newchip = new RcChipCowl(generics,null,RcJointPosition.NULL);
					index++;
					break;
				case "arm":
					newchip = new RcChipArm(generics,null,RcJointPosition.NULL);
					index++;
					break;
				default:
					throw new Exception("不明なチップ種類が検出されました : " + param[index]);
			}
			newchip.JointPosition = jp;
			RcAttrValue attr = new RcAttrValue();
			try{
				for( ; param[index] != "" ; index += 2){
					param[index] = param[index].ToLower();
					if(param[index] == "name")
						newchip.Name = param[index+1];
					else if(param[index] == "color")
						newchip.ChipColor.SetValue(param[index+1],generics.vals);
					else{
						param[index] = char.ToUpper(param[index][0]).ToString() + param[index].Substring(1);
						attr.SetValue(param[index+1],generics.vals);
						newchip[param[index]] = attr;
					}

				}
			}
			catch(IndexOutOfRangeException){
				throw new Exception("不正な属性値指定が存在します。");
			}
			catch(Exception e){
				MessageBox.Show(e.Message);
			}

			return newchip;

		}

		///<summery>
		///チップの種類を得る。
		///</summery>
		public static RcChipType CheckType(RcChipBase chip){
			if(chip is RcChipCore){
				return RcChipType.Core;
			}
			else if(chip is RcChipChip){
				if(chip is RcChipFrame){
					return RcChipType.Frame;
				}
				else if(chip is RcChipRudder){
					if(chip is RcChipRudderF){
						return RcChipType.RudderF;
					}
					else{
						return RcChipType.Rudder;
					}
				}
				else if(chip is RcChipTrim){
					if(chip is RcChipTrimF){
						return RcChipType.TrimF;
					}
					else{
						return RcChipType.Trim;
					}
				}
				else if(chip is RcChipWeight){
					return RcChipType.Weight;
				}
				else{
					return RcChipType.Chip;
				}
			}
			else if(chip is RcChipWheel){
				if(chip is RcChipRLW){
					return RcChipType.RLW;
				}
				else{
					return RcChipType.Wheel;
				}
			}
			else if(chip is RcChipJet){
				return RcChipType.Jet;
			}
			else if(chip is RcChipArm){
				return RcChipType.Arm;
			}
			else if(chip is RcChipCowl){
				return RcChipType.Cowl;
			}
			else{
				return RcChipType.Unknown;
			}
		}

		///<summery>
		///このチップはどの方向へ折れ曲がるチップかを得る。
		///</summery>
		public virtual RcAngle AngleType{
			get{
				return RcAngle.x;
			}
		}

		///<summery>
		///このチップの重量。
		///</summery>
		public virtual float Weight{
			get{
				return 0f;
			}
		}

		///<summery>
		///重量による重み付きベクトルを得る。
		///</summery>
		public Vector3 WeightedVector{
			get{
				Vector3 v = new Vector3();
				v.TransformCoordinate(matTranslation);
				return Weight * v;
			}
		}

		///<summery>
		///このチップと同一の属性を持ち、種類が違う新しいチップを生成する。
		///</summery>
		public RcChipBase ChangeType(RcChipType type){
			RcChipBase buff;

			if(type != RcChipType.Cowl && this.Parent is RcChipCowl)
				throw new Exception("親チップがカウルなので、種類の変更が出来ません。");
			
			switch(type){
				case RcChipType.Arm:
					buff = new RcChipArm(Generics,null,RcJointPosition.NULL);
					break;
				case RcChipType.Chip:
					buff = new RcChipChip(Generics,null,RcJointPosition.NULL);
					break;
				case RcChipType.Frame:
					buff = new RcChipFrame(Generics,null,RcJointPosition.NULL);
					break;
				case RcChipType.Rudder:
					buff = new RcChipRudder(Generics,null,RcJointPosition.NULL);
					break;
				case RcChipType.RudderF:
					buff = new RcChipRudderF(Generics,null,RcJointPosition.NULL);
					break;
				case RcChipType.Trim:
					buff = new RcChipTrim(Generics,null,RcJointPosition.NULL);
					break;
				case RcChipType.TrimF:
					buff = new RcChipTrimF(Generics,null,RcJointPosition.NULL);
					break;
				case RcChipType.Wheel:
					buff = new RcChipWheel(Generics,null,RcJointPosition.NULL);
					break;
				case RcChipType.RLW:
					buff = new RcChipRLW(Generics,null,RcJointPosition.NULL);
					break;
				case RcChipType.Weight:
					buff = new RcChipWeight(Generics,null,RcJointPosition.NULL);
					break;
				case RcChipType.Cowl:
					buff = new RcChipCowl(Generics,null,RcJointPosition.NULL);
					break;
				case RcChipType.Jet:
					buff = new RcChipJet(Generics,null,RcJointPosition.NULL);
					break;
				default:
					return this;
			}
			RcJointPosition jp;
			RcChipBase parent = this.Parent;

			string[] attrlist = this.AttrNameList;
			foreach(string attr in attrlist){
				try{
					buff[attr] = this[attr];
				}
				catch{}
			}

			if (type == RcChipType.Cowl) {
				switch (RcChipBase.CheckType(this)) {
					case RcChipType.Jet:
					case RcChipType.RLW:
					case RcChipType.Wheel:
						buff["Option"] = new RcAttrValue(2);
						break;
					case RcChipType.Frame:
					case RcChipType.RudderF:
					case RcChipType.TrimF:
						buff["Option"] = new RcAttrValue(1);
						break;
					default:
						buff["Option"] = new RcAttrValue(0);
						break;
				}

			}

			RcChipBase c;
			do{
				c = this.Child[0];
				if(c == null)break;
				jp = c.JointPosition;
				c.Detach();
				buff.Add(jp,c,false);
			}while(true);

			jp = this.JointPosition;
			this.Detach();
			buff.Attach(parent,jp);

			return buff;
		}

		public virtual float Fuel{
			get { return 1000000; }
		}


	}

	
	///<summery>
	///モデリングに関して総合的なデータを持つクラス。基本的に単一インスタンス。
	///</summery>
	public class RcData{
		public string ResourcePath = "";
		public const int MaxChipCount = 512;
		public const int ChipMeshCount = 64;
		public const int KeyCount = 17;
		public const int ChildCapasity = 12;

		public Device d3ddevice;
		public RcModel model;
		public RcValList vals;
		public RcKeyList keys;
		public string headercomment;
		public string script;
		public bool luascript;

		public Mesh imesh;
		public int chipCount;

		RcXFile[] meshes;
		public RcChipCursor Cursor;

		RcChipBase[] ChipLib;

		Vector3 weightCenter;

		bool RegisteringAll = false;

		RcDrawOptions drawOption;
		RcOutputOptions outputOption;
		RcEditOptions editOption;

		bool multiSelect = false;
		int selectedCount = 0;
	//	System.Collections.ArrayList selectedChipArray = new System.Collections.ArrayList();
		List<RcChipBase> selectedChipArray = new List<RcChipBase>();

		RcXFile roll;

		/// <summary>
		/// RcData インスタンスを初期化する。
		/// </summary>
		/// <param name="D3DDevice">表示などに用いるMicrosoft.DirectX.Direct3D.Device インスタンス。</param>
		public RcData(Device D3DDevice,RcDrawOptions draw,RcOutputOptions output,RcEditOptions edit,string pathResources){

			//	各インスタンスの初期化
			d3ddevice = D3DDevice;
			drawOption = draw;
			outputOption = output;
			editOption = edit;
			ResourcePath = pathResources;

			vals = new RcValList();

			keys = new RcKeyList(KeyCount);

			meshes = new RcXFile[ChipMeshCount];

			model = new RcModel(this);

			headercomment = script = "";

			ChipLib = new RcChipBase[MaxChipCount];
			
			chipCount = 0;

			Cursor = new RcChipCursor(this,model.root,RcJointPosition.NULL);
			new RcChipGuide(this,Cursor,RcJointPosition.North);
			new RcChipGuide(this,Cursor,RcJointPosition.South);
			new RcChipGuide(this,Cursor,RcJointPosition.East);
			new RcChipGuide(this,Cursor,RcJointPosition.West);
			RegisterChip(model.root);

			roll = new RcXFile();
			roll.Load(d3ddevice,Application.StartupPath + "\\Resources\\roll.x");

			ExtendedMaterial[] matbuff;
			imesh = Mesh.FromFile(ResourcePath + @"\Chip.x",MeshFlags.SystemMemory,d3ddevice,out matbuff);
		}

		///<summery>
		///回転方向表示用ガイドモデル
		///</summery>
		public RcXFile RollMesh{
			get{ return roll; }
		}

		///<summery>
		///描画オプション
		///</summery>
		public RcDrawOptions DrawOption{
			get{
				return drawOption;
			}
		}

		///<summery>
		///ファイル出力オプション
		///</summery>
		public RcOutputOptions OutputOption{
			get{
				return outputOption;
			}
		}

		///<summery>
		///編集オプション
		///</summery>
		public RcEditOptions EditOption{
			get{
				return editOption;
			}
		}

		///<summery>
		///選択されているチップの数。値の代入は、現在0(選択解除)しかできない。
		///</summery>
		public int SelectedChipCount{
			get{
				return (multiSelect ? selectedCount : 0);
			}
			set{
				if(value != 0)
					throw new Exception("SelectedChipCount プロパティに設定できる値は 0 のみです。");
				selectedChipArray.Clear();
			}
		}
		
		///<summery>
		///選択されているチップのリスト。
		///</summery>
		public RcChipBase[] SelectedChipList{
			get{
				if(multiSelect)
				return selectedChipArray.ToArray();
				else return null;
			}
		}
		
		/// <summary>
		/// チップ表示用RcXFileクラスへの参照を得る。
		/// </summary>
		/// <param name="FileName">.x ファイルのパス</param>
		/// <returns>ロードしたRcXFileへの参照</returns>
		public RcXFile GetMesh(string FileName){
			string tmp = Directory.GetCurrentDirectory();
			Directory.SetCurrentDirectory(ResourcePath);

			for(int i = 0;i < ChipMeshCount;i++){
				if(meshes[i] == null){
					meshes[i] = new RcXFile();
					meshes[i].FileName = FileName;
					meshes[i].Load(this.d3ddevice);
					Directory.SetCurrentDirectory(tmp);
					if(meshes[i].mesh == null)
						meshes[i] = null;

					return meshes[i];
				}else if(meshes[i].FileName == FileName){
					// すでに同一ファイル名のメッシュが読み込まれていたら、それを返す。
					Directory.SetCurrentDirectory(tmp);
					return meshes[i];
				}
			}
			Directory.SetCurrentDirectory(tmp);

			
			return null;
		}

		/// <summary>
		/// チップ表示用RcXFileクラスへの参照を得る。
		/// </summary>
		/// <param name="FileName">.x ファイルのパス</param>
		/// <param name="FullPath">FileNameがフルパスであることを表すフラグ</param>
		/// <returns>ロードしたRcXFileへの参照</returns>
		public RcXFile GetMesh(string FileName,bool FullPath){
			if(FullPath){
				string tmp = Directory.GetCurrentDirectory();
				Directory.SetCurrentDirectory(Path.GetDirectoryName(FileName));
				for(int i = 0;i < ChipMeshCount;i++){
					if(meshes[i] == null){
						meshes[i] = new RcXFile();
						meshes[i].FileName = FileName;
						meshes[i].Load(this.d3ddevice);
						Directory.SetCurrentDirectory(tmp);
						if(meshes[i].mesh == null)
							meshes[i] = null;

						return meshes[i];
					}else if(meshes[i].FileName == FileName){
						Directory.SetCurrentDirectory(tmp);
						return meshes[i];
					}
				}

			
				return null;
			}
			else return GetMesh(FileName);
		}

		///<summery>
		///GetMeshによって読み込んだすべてのXファイルデータを解放する。
		///</summery>
		public void DisposeMeshes(){
			for(int i = 0;i < ChipMeshCount;i++){
				if(meshes[i] != null && meshes[i].FileName != ""){
					if(meshes[i].texture != null){
						meshes[i].texture.Dispose();
						meshes[i].texture = null;
					}
					if(meshes[i].mesh != null){
						meshes[i].mesh.Dispose();
						meshes[i].mesh = null;
					}
				}
			}

			imesh.Dispose();
		}

		///<summery>
		///全メッシュのリロード
		///</summery>
		public void ReloadMeshes(){
			for(int i = 0;i < ChipMeshCount;i++){
				if(meshes[i] != null && meshes[i].FileName != ""){
					meshes[i].Load(this.d3ddevice);
				}
			}
			ExtendedMaterial[] matbuff;
			imesh = Mesh.FromFile(ResourcePath + "\\Chip.x",MeshFlags.SystemMemory,d3ddevice,out matbuff);
		}

		///<summery>
		///指定したチップにカーソルを移動する
		///</summery>
		public void SetCursor(RcChipBase Target){
			Cursor.Attach(Target,RcJointPosition.NULL);
		}

		///<summery>
		///カーソルを描画
		///</summery>
		public void DrawCursor(){
			Cursor.SetCursorColor(drawOption.CursorFrontColor,drawOption.CursorBackColor);
			Cursor.DrawChipAll();
		}

		///<summery>
		///チップを描画
		///</summery>
		public void DrawChips(bool enableGuide){
			DrawChips(enableGuide, true);
		}
		public void DrawChips(bool enableGuide, bool enableCursor) {
			Cursor.SetCursorColor(drawOption.CursorFrontColor,drawOption.CursorBackColor);
			//			Cursor.DrawChipAll();
			int i = 0;
			while(ChipLib[i] != null && !(ChipLib[i] is RcChipCowl)){
				ChipLib[i++].DrawChip();
			}
			if(enableGuide){
				foreach(RcChipBase g in Cursor.Child){
					if(g == null)break;
					g.DrawChip();
				}
			}
			while(ChipLib[i] is RcChipCowl){
				ChipLib[i++].DrawChip();
			}
			RcChipCore core = ChipLib[0] as RcChipCore;
			if (core != null && core.ReservedDrawFunc != null) {
				core.ReservedDrawFunc();
				core.ReservedDrawFunc = null;
			}

			if (enableCursor) {
				Cursor.DrawChip();

				Matrix m = Cursor.Parent.FullMatrix;
				float angle90 = ((float)Math.PI / 2);

				switch (Cursor.Parent.AngleType) {
					case RcAngle.x:
						m = Matrix.Translation(0, 0, -0.3f) * m;
						roll.Draw(d3ddevice, drawOption.CursorBackColor, 0x8000, m);
						roll.Draw(d3ddevice, drawOption.CursorFrontColor, 0x8000, Matrix.RotationX(angle90) * m);
						roll.Draw(d3ddevice, drawOption.CursorFrontColor, 0x8000, Matrix.RotationX(angle90 * 2) * m);
						roll.Draw(d3ddevice, drawOption.CursorBackColor, 0x8000, Matrix.RotationX(angle90 * 3) * m);
						break;
					case RcAngle.y:
						m = Matrix.RotationZ(angle90) * Matrix.Translation(0, 0, -0.3f) * m;
						roll.Draw(d3ddevice, drawOption.CursorBackColor, 0x8000, m);
						roll.Draw(d3ddevice, drawOption.CursorFrontColor, 0x8000, Matrix.RotationX(angle90) * m);
						roll.Draw(d3ddevice, drawOption.CursorFrontColor, 0x8000, Matrix.RotationX(angle90 * 2) * m);
						roll.Draw(d3ddevice, drawOption.CursorBackColor, 0x8000, Matrix.RotationX(angle90 * 3) * m);
						break;
					case RcAngle.z:
						m = Matrix.RotationY(angle90) * m;
						roll.Draw(d3ddevice, drawOption.CursorFrontColor, 0x8000, m);
						roll.Draw(d3ddevice, drawOption.CursorBackColor, 0x8000, Matrix.RotationX(angle90) * m);
						roll.Draw(d3ddevice, drawOption.CursorFrontColor, 0x8000, Matrix.RotationX(angle90 * 2) * m);
						roll.Draw(d3ddevice, drawOption.CursorBackColor, 0x8000, Matrix.RotationX(angle90 * 3) * m);
						break;
				}
			}

		}

		///<summery>
		///モデルチップリストに、チップを登録する。
		///</summery>
		public int RegisterChip(RcChipBase c){
			if(c.RegistID > 0)return c.RegistID;
			for(int i = 0;i < MaxChipCount;i++){
				if(ChipLib[i] == null){
					ChipLib[i] = c;
					c.RegistID = i;
					chipCount++;

					string[] attrlist = c.AttrNameList;
					RcVal v;
					for(int j = 0;j < attrlist.Length;j++)
						if((v = c[attrlist[j]].Val) != null)
							v.RefCount++;
					CalcWeightCenter();
					if(!RegisteringAll && OnChipLibraryChanged != null)OnChipLibraryChanged(null);
					return i;
				}
				else if(ChipLib[i] == c){
					c.RegistID = i;
					chipCount++;
					return i;
				}
			}
			throw new Exception("チップ最大数(512)を超えました。");
		}

		///<summery>
		///登録されていたチップをリストから除去する。
		///</summery>
		public void UnregisterChip(RcChipBase c){
			if(c.RegistID < 0 || c.RegistID > RcData.MaxChipCount){
				throw new Exception("指定したチップは登録されていないか、または不正な方法で登録された可能性があります : " + c.ToString());
			}
			ChipLib[c.RegistID] = null;
			string[] attrlist = c.AttrNameList;
			RcVal v;
			for(int j = 0;j < attrlist.Length;j++)
				if((v = c[attrlist[j]].Val) != null)
					v.RefCount--;

			SlideLibraryData(c.RegistID);
			c.RegistID = -1;
			chipCount--;
			CalcWeightCenter();
			if(!RegisteringAll && OnChipLibraryChanged != null)OnChipLibraryChanged(null);
		}

		///<summery>
		///あるチップと、その派生チップすべてを登録する。
		///</summery>
		public void RegisterChipAll(RcChipBase c){
			RegisteringAll = true; 
			RegisterChip(c);
			foreach(RcChipBase cb in c.Child)if(cb != null)
												 RegisterChipAll(cb);
			RegisteringAll = false;
			CalcWeightCenter();
			if(OnChipLibraryChanged != null)OnChipLibraryChanged(null);
		}

		public void UnregisterChipAll(RcChipBase c){
			RegisteringAll = true;
			foreach(RcChipBase cb in c.Child)if(cb != null)
												 UnregisterChipAll(cb);
			UnregisterChip(c);
			RegisteringAll = false;
			CalcWeightCenter();
			if(OnChipLibraryChanged != null)OnChipLibraryChanged(null);
		}

		///<summery>
		///リストの途中の空欄を消す。
		///</summery>
		public void SlideLibraryData(int startat){
			for(int i = startat;i < MaxChipCount -1; i++){
				ChipLib[i] = ChipLib[i + 1];
				if(ChipLib[i] != null)ChipLib[i].RegistID = i;
				else return;
			}
			ChipLib[MaxChipCount -1] = null;
		}

		///<summery>
		///あるチップの親がそのチップより大きいインデックス番号で登録されていると、保存時に問題になるので
		///それを除去する。
		///</summery>
		public void CheckBackTrack(){
			int buff;
			RcChipBase buffc;

			for(int i = 0;i < MaxChipCount-1;i++){
				if(ChipLib[i] is RcChipCowl){
					bool flag = false;
					for(int j = chipCount - 1;j != i;j--){
						if(! (ChipLib[j] is RcChipCowl)){
							buffc = ChipLib[i];
							ChipLib[i] = ChipLib[j];
							ChipLib[j] = buffc;
							ChipLib[i].RegistID = i;
							ChipLib[j].RegistID = j;
							flag = true;
							break;
						}
					}
					if(!flag)break;
				}
			}
			
			for(int i = 0;i < MaxChipCount -1; i++){
				if(ChipLib[i] is RcChipCore)continue;
				if(ChipLib[i] == null || ChipLib[i].Parent == null)return;
				if(ChipLib[i].Parent.RegistID < 0){
					try{
						UnregisterChipAll(ChipLib[i].Parent);
					}catch{}
					continue;
				}
				if(ChipLib[i].RegistID < ChipLib[i].Parent.RegistID){
					buff = ChipLib[i].Parent.RegistID;
					buffc = ChipLib[i].Parent;
					ChipLib[buff] = ChipLib[i];
					ChipLib[i] = buffc;
					ChipLib[buff].RegistID = buff;
					ChipLib[i].RegistID = i;
					i--;
				}
			}

		}

		///<summery>
		///指定したインデックスで登録されているチップを得る。
		///</summery>
		public RcChipBase GetChipFromLib(int id){
			return ChipLib[id];
		}

		///<summery>
		///現在カーソルがあるチップを取得する。複数選択時にはnull。
		///</summery>
		public RcChipBase SelectedChip{
			get{
				if(multiSelect)return null;
				return Cursor.Parent;
			}
			set{
				SetCursor(value);
				multiSelect = false;
				selectedChipArray.Clear();
				SelectedChipCount = 0;
				if(OnSelectedChipChanged != null)
					OnSelectedChipChanged(value);
			}
		}

		///<summery>
		///選択されているチップを増減させる。chipがすでに選択されていれば解除し、されていなければ選択する。
		///</summery>
		public void AssignSelectedChips(RcChipBase chip){
			if(chip == null)return;
			if(!multiSelect)
				selectedChipArray.Add(Cursor.Parent);
			if(selectedChipArray.Contains(chip))
				selectedChipArray.Remove(chip);
			else
				selectedChipArray.Add(chip);

			selectedCount = selectedChipArray.Count;
			if(selectedCount != 0){
				SetCursor(null);
				multiSelect = true;
			}
			else{
				SetCursor(chip);
				multiSelect = false;
			}

			if(OnSelectedChipChanged != null)OnSelectedChipChanged(null);
		}

		public void AssignSelectedChips(RcChipBase[] chips){
			foreach(RcChipBase chip in chips){
				if(chip == null)break;
				if(!multiSelect)
					selectedChipArray.Add(Cursor.Parent);
				if(selectedChipArray.Contains(chip))
					selectedChipArray.Remove(chip);
				else
					selectedChipArray.Add(chip);

				selectedCount = selectedChipArray.Count;
				if(selectedCount != 0){
					SetCursor(null);
					multiSelect = true;
				}
				else{
					SetCursor(chip);
					multiSelect = false;
				}
			}
			if(OnSelectedChipChanged != null)OnSelectedChipChanged(null);
		}

		///<summery>
		///選択されているチップのリストを、chipsに変更する。
		///</summery>
		public void AssignSelectedChips(RcChipBase[] chips,bool SetList){
			if(SetList)selectedChipArray.Clear();
			AssignSelectedChips(chips);
		}

		public void AssignSelectedChips(object[] chips,bool SetList){
			if(SetList)selectedChipArray.Clear();
			AssignSelectedChips(chips);
		}

		public RcChipBase[] AllChip{
			get{
				return ChipLib;
			}
		}

		public void AssignSelectedChips(object[] chips){
			foreach(object o in chips){
				RcChipBase chip = o as RcChipBase;
				if(chip == null)break;
				if(!multiSelect)
					selectedChipArray.Add(Cursor.Parent);
				if(selectedChipArray.Contains(chip))
					selectedChipArray.Remove(chip);
				else
					selectedChipArray.Add(chip);

				selectedCount = selectedChipArray.Count;
				if(selectedCount != 0){
					SetCursor(null);
					multiSelect = true;
				}
				else{
					SetCursor(chip);
					multiSelect = false;
				}
			}
			if(OnSelectedChipChanged != null)OnSelectedChipChanged(null);
		}

		///<summery>
		///チップの選択を解除する。
		///</summery>
		public void ResetSelectedChips(){
			SelectedChipCount = 0;
		}

#if false
		///<summery>
		///.rcdファイルの記述内容から、データを読み取る。
		///</summery>
		public bool Parse(string filedata){
			filedata = Regex.Replace(filedata,"//.*\r\n","\r\n");
			filedata = Regex.Replace(filedata,"//.*\r","\r");
			filedata = Regex.Replace(filedata,"//.*\n","\n");
			Match blocks = Regex.Match(filedata,@"val\s*\{(?<vals>[^}]*)}\s*key\s*\{(?<keys>[^}]*)}\s*body\s*\{(?<body>.*)}\s*(?<scripttype>script|lua)\s*\{(?<script>.*)}",RegexOptions.Singleline | RegexOptions.IgnoreCase);
			if(!blocks.Success){
				blocks = Regex.Match(filedata,@"val\s*\{(?<vals>[^}]*)}\s*key\s*\{(?<keys>[^}]*)}\s*body\s*\{(?<body>.*)}",RegexOptions.Singleline | RegexOptions.IgnoreCase);
			}

			string buffer;
			if(blocks.Success){
				vals.Parse(blocks.Result("${vals}"));
				keys.Parse(blocks.Result("${keys}"),vals);
				model.Parse(blocks.Result("${body}"));
				buffer = blocks.Result("${scripttype}");
				luascript = (string.Compare(buffer,"lua",true) == 0);
				script = blocks.Result("${script}");
				if(script == "${script}")script = "";
				CheckBackTrack();
				return false;
			}
			else
				return true;

		}
#else //次期パース関数
		class SourceIterator : IEnumerable<char> {
			public SourceIterator(string s) {
				str = s;
			}
			string str;
			public IEnumerator<char> GetEnumerator() {
				string _str = str;
				bool inComment = false;
				bool inString = false;
				int blockLevel = 0;
				int i;
				for (i = 0; i < _str.Length; i++) {
					if (blockLevel == 0 && !inComment && !inString &&
						(_str[i + 0] == 'l' || _str[i + 0] == 'L') &&
						(_str[i + 1] == 'u' || _str[i + 1] == 'U') &&
						(_str[i + 2] == 'a' || _str[i + 2] == 'A'))
						break;	// luaはコメントなどの処理は行わない


					if (!inString && _str[i] == '/' && _str[i + 1] == '/') {
						inComment = true;
					}
					if (!inComment && _str[i] == '"') {
						inString = !inString;
					}
					if (_str[i] == '\n') {
						inComment = false;
					}

					if (!inComment) {
						if (_str[i] == '{') blockLevel++;
						else if (_str[i] == '}') blockLevel--;
						yield return _str[i];
					}
				}

				while (i < _str.Length) {
					yield return _str[i++];
				}
			}

			#region IEnumerable メンバ

			IEnumerator IEnumerable.GetEnumerator() {
				throw new NotImplementedException();
			}

			#endregion
		}

		///<summery>
		///.rcdファイルの記述内容から、データを読み取る。
		///<returns>エラーが発生した場合はtrue</returns>
		///</summery>
		public bool Parse(string filedata) {
			var tokenBuffer = new StringBuilder();
			var val = new StringBuilder();
			var key = new StringBuilder();
			var body = new StringBuilder();
			var scr = new StringBuilder();

			int pos = 0;
			bool finished = false;

			var itr = new SourceIterator(filedata);
			foreach (char c in itr) {
				tokenBuffer.Append(c);
			}
			// ここから先は従来の処理

			Match blocks = Regex.Match(filedata, @"val\s*\{(?<vals>[^}]*)}\s*key\s*\{(?<keys>[^}]*)}\s*body\s*\{(?<body>.*)}\s*(?<scripttype>script|lua)\s*\{(?<script>.*)}", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			if (!blocks.Success) {
				blocks = Regex.Match(filedata, @"val\s*\{(?<vals>[^}]*)}\s*key\s*\{(?<keys>[^}]*)}\s*body\s*\{(?<body>.*)}", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			}

			string buffer;
			if (blocks.Success) {
				vals.Parse(blocks.Result("${vals}"));
				keys.Parse(blocks.Result("${keys}"), vals);
				model.Parse(blocks.Result("${body}"));
				buffer = blocks.Result("${scripttype}");
				luascript = (string.Compare(buffer, "lua", true) == 0);
				script = blocks.Result("${script}");
				if (script == "${script}") script = "";
				CheckBackTrack();
				return false;
			}
			else
				return true;
//			throw new Exception("Test finished.");
		}
#endif
		public void Save(string FileName,System.Collections.Specialized.NameValueCollection others){
			StreamWriter file = new StreamWriter(FileName,false,System.Text.Encoding.GetEncoding(932));
			string buffer = "";
			
			file.WriteLine("RCMFORMAT VERSION 1.1");
			
			file.WriteLine("VALS");
			foreach(RcVal v in vals.List){
				file.WriteLine("{0},{1},{2},{3},{4},{5}",v.ValName.ToString(),v.Default.ToString(),v.Min.ToString(),v.Max.ToString(),v.Step.ToString(),v.Disp.ToString());
			}
			file.WriteLine("VALS END");
			
			file.WriteLine("KEYS");
			for(int i = 0;i < RcData.KeyCount;i++){
				foreach(RcKey.RcKeyWork w in keys[i].Works){
					file.WriteLine(keys[i].Key_ID.ToString() + "," + w.Target.ValName + "," + w.Step.ToString("G9"));
				}
			}
			file.WriteLine("KEYS END");

			file.WriteLine("BODY");

			RcChipBase target;
			string[] attrlist = null;
			for(int i = 0;i < MaxChipCount;i++){
				target = ChipLib[i];
				if(target == null)break;
				buffer = ((byte)RcChipBase.CheckType(target)).ToString() + ",";
				buffer += ((byte)target.JointPosition).ToString() + ",";
				buffer += target.Name + ",";
				attrlist = target.AttrNameList;
				for(int j = 0;j < attrlist.Length;j++){
					buffer += target[attrlist[j]].ToString() + ",";
				}
				if(target.Parent == null)
					buffer += "-1";
				else
					buffer += target.Parent.RegistID.ToString();
				foreach(char c in target.Comment)
					buffer += "," + (int)c;

				file.WriteLine(buffer);
			}

			file.WriteLine("BODY END");

			file.WriteLine(luascript ? "SCRIPT LUA" : "SCRIPT");
			string[] lines = script.Split('\n');
			file.WriteLine(lines.Length.ToString());
			for(int i = 0;i < lines.Length;i++)
				file.WriteLine(lines[i].Replace("\r",""));
			file.WriteLine("SCRIPT END");

			file.WriteLine("OTHER");
			for(int i = 0;i < others.Count;i++)
				file.WriteLine("{0}\t{1}",others.GetKey(i),others.Get(i));
			file.WriteLine("OTHER END");
			file.Close();
		}

		
		public NameValueCollection Load(string FileName){
			System.Globalization.NumberFormatInfo formatinfo = System.Globalization.NumberFormatInfo.InvariantInfo;
			StreamReader file = new StreamReader(FileName,System.Text.Encoding.GetEncoding(932));
			NameValueCollection opts = new NameValueCollection();
			string version;
			string input;
			string[] parameters,parts;

			/*
			 * ブロック概要
			 *	VALS
			 *		変数
			 *	KEYS
			 *		キー動作
			 *	BODY
			 *		モデル定義
			 *	SCRIPT
			 *		スクリプト部
			 *	OTHER
			 *		その他、固有の設定など
			 */

			version = file.ReadLine();
			input = file.ReadLine();

			RcVal tmpV;
			RcChipBase chip;
			RcChipBase[] array = {	
									 new RcChipCore(this,null,RcJointPosition.NULL),
									 new RcChipChip(this,null,RcJointPosition.NULL),
									 new RcChipFrame(this,null,RcJointPosition.NULL),
									 new RcChipRudder(this,null,RcJointPosition.NULL),
									 new RcChipRudderF(this,null,RcJointPosition.NULL),
									 new RcChipTrim(this,null,RcJointPosition.NULL),
									 new RcChipTrimF(this,null,RcJointPosition.NULL),
									 new RcChipWheel(this,null,RcJointPosition.NULL),
									 new RcChipRLW(this,null,RcJointPosition.NULL),
									 new RcChipJet(this,null,RcJointPosition.NULL),
									 new RcChipWeight(this,null,RcJointPosition.NULL),
									 new RcChipCowl(this,null,RcJointPosition.NULL),
									 new RcChipArm(this,null,RcJointPosition.NULL)
								 };
			int i;
			RcAttrValue attr = new RcAttrValue();


			try{
				switch(version){
					case "RCMFORMAT VERSION 1.0":
						#region Version 1.0
						while(input != null && input != "VALS"){
							input = file.ReadLine();
						}
						while(input != null && input != "VALS END"){
							input = file.ReadLine();
							parameters = input.Split(',');
							if(parameters.Length != 6)
								continue;
							try{
								tmpV = this.vals.Add(parameters[0]);
								tmpV.Default = float.Parse(parameters[1]);
								tmpV.Min = float.Parse(parameters[2]);
								tmpV.Max = float.Parse(parameters[3]);
								tmpV.Step = float.Parse(parameters[4]);
								tmpV.Disp = bool.Parse(parameters[5]);
							}
							catch{
								continue;
							}
						}

						while(input != null && input != "KEYS"){
							input = file.ReadLine();
						}
						i = 0;
						try{
							while(input != null && input != "KEYS END"){
								input = file.ReadLine();
								parameters = input.Split(',');
								foreach(string s in parameters){
									parts = s.Split(':');
									if(parts.Length < 2)continue;
									keys[i++].AssignWork(vals[parts[0]],float.Parse(parts[1]));
								}
							}
						}catch{}

						while(input != null && input != "BODY"){
							input = file.ReadLine();
						}
						input = file.ReadLine();
						while(input != null && input != "BODY END"){
							parameters = input.Split(',');
							if(parameters[0] == "0"){
								//コアだったときの処理
								model.root.Name = parameters[2];
								model.root.ChipColor.SetValue(parameters[3],vals);
								attr = new RcAttrValue();
								attr.SetValue(parameters[4],vals);
								model.root["User1"] = attr;
								attr = new RcAttrValue();
								attr.SetValue(parameters[5],vals);
								model.root["User2"] = attr;
							}
							else{
								i = 4;
								chip = array[int.Parse(parameters[0])].Clone(false,null);
								chip.JointPosition = (RcJointPosition)(byte.Parse(parameters[1]));
								chip.Name = parameters[2];
								chip.ChipColor.SetValue(parameters[3],vals);
								parts = chip.AttrNameList;
								for(int j = 0;j < parts.Length;j++){
									attr = new RcAttrValue();
									attr.SetValue(parameters[i++],vals);
									chip[parts[j]] = attr;
								}
								chip.Attach(ChipLib[int.Parse(parameters[i])],chip.JointPosition);
							}
							input = file.ReadLine();
						}

						#endregion
						break;
					case "RCMFORMAT VERSION 1.1":
						#region Version 1.1
						while(input != null && input != "VALS"){
							input = file.ReadLine();
						}
						while(input != null && input != "VALS END"){
							input = file.ReadLine();
							parameters = input.Split(',');
							if(parameters.Length != 6)
								continue;
							try{
								tmpV = this.vals.Add(parameters[0]);
								tmpV.Default = float.Parse(parameters[1],formatinfo);
								tmpV.Min = float.Parse(parameters[2],formatinfo);
								tmpV.Max = float.Parse(parameters[3],formatinfo);
								tmpV.Step = float.Parse(parameters[4],formatinfo);
								tmpV.Disp = bool.Parse(parameters[5]);
							}
							catch{
								continue;
							}
						}

						while(input != null && input != "KEYS"){
							input = file.ReadLine();
						}
						i = 0;
					
						input = file.ReadLine();
						while(input != null && input != "KEYS END"){
							parameters = input.Split(',');
							keys[int.Parse(parameters[0])].AssignWork(vals[parameters[1]],float.Parse(parameters[2]));
							input = file.ReadLine();
						}
					

						while(input != null && input != "BODY"){
							input = file.ReadLine();
						}
						input = file.ReadLine();
						while(input != null && input != "BODY END"){
							parameters = input.Split(',');
							if(parameters[0] == "0"){
								//コアだったときの処理
								model.root.Name = parameters[2];
								model.root.ChipColor.SetValue(parameters[3],vals);
								attr = new RcAttrValue();
								attr.SetValue(parameters[4],vals);
								model.root["User1"] = attr;
								attr = new RcAttrValue();
								attr.SetValue(parameters[5],vals);
								model.root["User2"] = attr;
								if(parameters.Length > 7)
									for(int j = 7;j < parameters.Length;j++)
										model.root.Comment += ((char)int.Parse(parameters[j])).ToString();
							}
							else{
								i = 3;
								chip = array[int.Parse(parameters[0])].Clone(false,null);
								chip.JointPosition = (RcJointPosition)(byte.Parse(parameters[1]));
								chip.Name = parameters[2];
								parts = chip.AttrNameList;
								for(int j = 0;j < parts.Length;j++){
									attr = new RcAttrValue();
									attr.SetValue(parameters[i++],vals);
									chip[parts[j]] = attr;
								}
								if(parameters.Length > i+1)
									for(int j = i+1;j < parameters.Length;j++)
										chip.Comment += ((char)int.Parse(parameters[j])).ToString();
								chip.Attach(ChipLib[int.Parse(parameters[i])],chip.JointPosition);
							}
							input = file.ReadLine();
						}

						while(input != null && !input.StartsWith("SCRIPT")){
							input = file.ReadLine();
						}
						if(input.EndsWith("LUA"))
							this.luascript = true;
						input = file.ReadLine();
						while(input != null && input != "SCRIPT END"){
							i = int.Parse(input);
							for(int j = 0;j < i;j++)
								script += (input = file.ReadLine()) + "\r\n";
							input = file.ReadLine();
						}

						while(input != null && input != "OTHER"){
							input = file.ReadLine();
						}
						while(input != null && input != "OTHER END"){
							parameters = (input = file.ReadLine()).Split('\t');
							if(parameters.Length != 2)continue;
							opts.Add(parameters[0],parameters[1]);
						}


						#endregion
						break;
					default:
						break;
				}

			}
			catch{
				opts = null;
			}
			finally{
				file.Close();

				CheckBackTrack();
				SetCursor(model.root);
			}
			return opts;

		}

		///<summery>
		///重心を計算する。
		///</summery>
		public void CalcWeightCenter(){
			if(RegisteringAll)return;
			double totalWeight = 0.0;
			Vector3 totalVector = new Vector3();

			for(int i = 0;i < MaxChipCount;i++){
				if(ChipLib[i] == null)break;
				totalWeight += ChipLib[i].Weight;
				totalVector += ChipLib[i].WeightedVector;
			}

			totalVector.Scale((float)(1 / totalWeight));
			weightCenter = totalVector;
		}

		///<summery>
		///最後に計算された重心を得る。
		///</summery>
		public Vector3 WeightCenter{
			get{
				return weightCenter;
			}
		}


		public MessageCallback OnSelectedChipChanged;
		public MessageCallback OnChipLibraryChanged;

		/// <summary>
		/// RCDデータ用の数値としてパースする
		/// </summary>
		/// <param name="text">パースする文字列</param>
		/// <exception>NumberFormatException</exception>
		/// <returns>パース結果</returns>
		public static float ParseNumber(string text) {
			text = text.Trim();
			if (text[0] == '#') {
				// 十六進数
				return (float)int.Parse(text.Substring(1), System.Globalization.NumberStyles.HexNumber);
			}
			else {
				return float.Parse(text);
			}
		}

		/// <summary>
		/// RCDデータ用の数値としてパースする
		/// </summary>
		/// <param name="text"></param>
		/// <returns>成功すればTrue</returns>
		public static bool TryParseNumber(string text) {
			float dummy;
			return TryParseNumber(text, out dummy);
		}
		/// <summary>
		/// RCDデータ用の数値としてパースする
		/// </summary>
		/// <param name="text"></param>
		/// <returns>成功すればTrue</returns>
		public static bool TryParseNumber(string text, out float result) {
			text = text.Trim();
			if (text[0] == '#') {
				int buffer;
				bool ret = int.TryParse(text.Substring(1), System.Globalization.NumberStyles.HexNumber, null, out buffer);
				result = (float)buffer;
				return ret;
			}
			else {
				return float.TryParse(text, out result);
			}
		}

		bool preview;
		public bool Preview {
			get { return preview; }
			set {
				preview = value;
				foreach (var v in vals.List) {
					if (v != null) {
						v.OnPreview = value;
						v.Now = v.Default;
					}
				}
			}
		}

	}

	/// <summary>
	/// RigidChipsモデルデータ
	/// </summary>
	public class RcModel{
		RcData gen;
		public RcChipBase root;
		public RcModel(RcData gen){
			this.gen = gen;
			root = new RcChipCore(gen,null,RcJointPosition.NULL);
		}
		/// <summary>
		/// モデル内の特定のチップを得る。
		/// </summary>
		public RcChipBase this[int idx]{
			get{
				RcChipBase buff = root.GetChildChip(ref idx);
				if(buff != null)
					return buff;
				else
					throw new Exception("インデックス値が範囲を超えています。");
			}
/*			set{
				RcChipBase t = root.GetChildChip(ref idx),p;
				if(t == null)
					throw new Exception("インデックス値が範囲を超えています。");

				int sabun = 0;
				p = t.Parent;
				for(int i = 0;i < RcData.ChildCapasity;i++){
					if(p.Child[i] == t){
						sabun = value.ChipCount - p.Child[i].ChipCount;
						gen.chipCount += sabun;
						p.Child[i] = value;
						return;
					}
				}
				
				throw new Exception("RcChipBase[]で予期せぬエラーが発生しました。");
			}	*/
		}
		/// <summary>
		/// モデル内の、特定の名前が付いたチップを得る。見つからない場合はnull.
		/// </summary>
		public RcChipBase this[string name]{
			get{
				return null;	// 未実装
			}
		}

		public override string ToString() {
			return "Body{\r\n" + root.ToString(1) + "\r\n}\r\n";
		}

		public void Parse(string input){
			this.root = RcModelParser.Parse(input,(RcChipCore)this.root,this.gen);
		}

	}

	/// <summary>
	/// 変数データ
	/// </summary>
	public class RcVal{
		public RcValList List;
		public string ValName;
		public RcConst Default;
		public RcConst Min;
		public RcConst Max;
		public RcConst Step;
		public bool Disp;
		public bool OnPreview {
			get;
			set;
		}

		float now;
		public float Now{
			get {
				if (OnPreview) {
					ApplyDefault();
					return now;
				}
				else {
					return Default.Value;
				}
			}
			set { now = value; }
		}

		public int RefCount;

		public RcVal(){
			Default = Min = Step = 0f;
			Max = float.PositiveInfinity;
			Disp = true;
			RefCount = 0;
		}

		public RcVal(string name)
			: this() {
			ValName = name;
		}

		/// <summary>
		/// Val項目の文字列を作成
		/// </summary>
		/// <returns>書式化された Val ブロックの項目</returns>
		public override string ToString() {
			string buff = "";
			buff += ValName + "(";
			if(Default != DefaultDefaultValue)buff += "default=" + Default.ToString() + ",";
			if(Min != DefaultMinValue)buff += "min=" + Min.ToString() + ",";
			if(Max != DefaultMaxValue)buff += "max=" + Max.ToString() + ",";
			if(Step != DefaultStepValue)buff += "step=" + Step.ToString() + ",";
			if(!Disp)buff += "disp=0";
			buff = buff.TrimEnd(',');
			buff += ")";
			return buff;
		}

		public const float DefaultDefaultValue = 0f;
		public const float DefaultMinValue = 0f;
		public const float DefaultMaxValue = float.PositiveInfinity;
		public const float DefaultStepValue = 0f;

		bool ticked = false;
		public bool Ticked {
			get {
				return ticked;
			}
			set {
				ticked = value;
				Debug.WriteLine(string.Format("{0} : {1}", this.ValName, value), "Ticked");
			}
		}
		bool steped;
		bool Steped {
			get { return steped; }
			set {
				steped = value;
				Debug.WriteLine(string.Format("{0} : {1}", this.ValName, value), "Steped");
			}
		}
		public float Tick(float StepValue) {
			Steped = true;
			float newval = now + StepValue;
			if (newval > Max)
				newval = Max;
			else if (newval < Min)
				newval = Min;

			return now = newval;
		}

		public void ApplyDefault() {
			if (Ticked) {
				if (!Steped) {
					Debug.WriteLine(this.ValName, "Default Applied");
					if (now < Default - Step) {
						now += Step;
					}
					else if (now > Default + Step) {
						now -= Step;
					}
					else {
						now = Default;
					}
				}
				Ticked = false;
			}
			Steped = false;
		}
	}

	/// <summary>
	/// 変数(RcVal)のリスト
	/// </summary>
	public class RcValList{
		public RcVal[] List;


		public RcValList(){
			List = new RcVal[0];
		}

		public RcVal this[int idx]{
			get{
				return List[idx];
			}
		}
		public RcVal this[string valname]{
			get{
				foreach(RcVal v in List)
					if(string.Compare(v.ValName, valname,true) == 0)return v;
				
				return null;
			}
		}

		public void Swap(RcVal val1,RcVal val2){
			int i,j;
			for(i = 0;i < List.Length;i++){
				if(val1.ValName == List[i].ValName)
					break;
			}
			if(i == List.Length)throw new Exception("この変数はリストに存在しません : " + val1.ValName);

			for(j = 0;j < List.Length;j++){
				if(val2.ValName == List[j].ValName){
					List[i] = val2;
					List[j] = val1;
					return;
				}
			}
			throw new Exception("この変数はリストに存在しません : " + val2.ValName);

		}

		/// <summary>
		/// 変数を作成する。
		/// </summary>
		/// <param name="ValName">変数の識別名。</param>
		/// <returns>新たに作成されたRcValへの参照。</returns>
		public RcVal Add(string ValName){
			if(ValName == null)return null;
			if(this[ValName] != null) throw new Exception("指定された名前の変数はすでに存在します : " + ValName);
			List.CopyTo(List = new RcVal[List.Length +1],0);
			( List[List.Length-1] = new RcVal() ).ValName = ValName;
			List[List.Length - 1].List = this;
			return List[List.Length-1];
		}

		/// <summary>
		/// 指定された名前の変数を削除する。
		/// </summary>
		/// <param name="ValName">削除する変数の識別名。</param>
		public void Remove(string ValName){
			bool removed = false;
			RcVal[] buff = new RcVal[List.Length - 1];
			for(int i = 0;i < List.Length;i++){
				if(removed)
					buff[i-1] = List[i];
				else if(List[i].ValName == ValName){
					List[i].RefCount = -65536;
					removed = true;
				}
				else
					buff[i] = List[i];
			}
			if(!removed)throw new Exception("指定された名前の変数はリストに存在しません : " + ValName);
			List = buff;
		}

		public int Count{
			get{
				return List.Length;
			}
		}

		public override string ToString() {
			string str = "Val{";
			foreach(RcVal v in List){
				str += "\r\n\t" + v.ToString();
			}
			str += "\r\n}\r\n";
			return str;
		}
		public void Parse(string data){
			int start,end;
			RcVal buff;

			start = data.IndexOf('{');
			end = data.LastIndexOf('}');
			if(start >= 0 && end >= 0)
				data = data.Substring(start+1,end-start);
			data = data.Replace(" ","").Replace("\t","").Replace("\n","").Replace("\r","");

			string[] list = data.Split('(',')');
			string[] param;

			for(int i = 0;i < list.Length;i += 2){
				if(list[i] == "")break;
                
				buff = this.Add(list[i]);
				param = list[i+1].Split(',','=');
				for(int j = 0;j < param.Length;j += 2){
					switch(param[j].ToLower()){
						case "default":
							if(param[j+1][0] == '#')
								buff.Default = (float)int.Parse(param[j+1].Substring(1),System.Globalization.NumberStyles.AllowHexSpecifier);
							else
								buff.Default = float.Parse(param[j+1]);
							break;
						case "min":
							if(param[j+1][0] == '#')
								buff.Min = (float)int.Parse(param[j+1].Substring(1),System.Globalization.NumberStyles.AllowHexSpecifier);
							else
								buff.Min = float.Parse(param[j+1]);
							break;
						case "max":
							if(param[j+1][0] == '#')
								buff.Max = (float)int.Parse(param[j+1].Substring(1),System.Globalization.NumberStyles.AllowHexSpecifier);
							else
								buff.Max = float.Parse(param[j+1]);
							break;
						case "step":
							if(param[j+1][0] == '#')
								buff.Step = (float)int.Parse(param[j+1].Substring(1),System.Globalization.NumberStyles.AllowHexSpecifier);
							else
								buff.Step = float.Parse(param[j+1]);
							break;
						case "disp":
							buff.Disp = (float.Parse(param[j+1]) != 0f);
							break;
						case "":
							break;
						default:
							throw new Exception("不明な変数パラメータです : " + param[j]);
					}
				}
			}
		}

		internal void Tick() {
			Debug.WriteLine("ValList::Tick");
			foreach (RcVal v in List)
				if(v != null)
					v.Ticked = true;
		}

		internal void Tack() {
			Debug.WriteLine("ValList::Tack");
			foreach (RcVal v in List)
				if (v != null)
					v.Ticked = false;
		}
	}

	/// <summary>
	/// キー設定情報
	/// </summary>
	public class RcKey{
		public struct RcKeyWork{
			public RcVal Target;
			public float Step;

			public override string ToString() {
				return Target.ValName + string.Format("(Step={0:G})",Step);
			}

		}

		~RcKey(){
			foreach(RcKeyWork kw in Works){
				kw.Target.RefCount --;
			}
		}

		public byte Key_ID;
		public RcKeyWork[] Works;

		public RcKey(){
			Works = new RcKeyWork[0];
		}

		/// <summary>
		/// キーに変数を関連づける。すでに関連づけられている場合はステップを変更する。
		/// </summary>
		/// <param name="Target">ターゲットとなるRcValへの参照。</param>
		/// <param name="Step">ステップ値。</param>
		public void AssignWork(RcVal Target,float Step){
			for(int i = 0;i < Works.Length;i++){
				if(Works[i].Target == Target){
					Works[i].Step = Step;
					return;
				}
			}
			Works.CopyTo(Works = new RcKeyWork[Works.Length + 1],0);
			Works[Works.Length-1].Target = Target;
			Works[Works.Length-1].Step = Step;
			Target.RefCount ++;
		}

		/// <summary>
		/// 変数のキーへの関連づけを削除する。
		/// </summary>
		/// <param name="Target">削除するRcValへの参照。</param>
		public void DeleteWork(RcVal Target){
			bool removed = false;
			RcKeyWork[] buff = new RcKeyWork[Works.Length -1];
			for(int i = 0;i < Works.Length;i++){
				if(removed)
					buff[i-1] = Works[i];

				else if(Works[i].Target == Target)
					removed = true;

				else
					buff[i] = Works[i];
			}

			Works = buff;

			Target.RefCount--;

		}

		/// <summary>
		/// キーの動作を表す文字列を得る。
		/// </summary>
		/// <returns>Keyブロックで使用可能なこのインスタンスの完全文字列。</returns>
		public override string ToString() {
			if(Works.Length == 0)return null;
			string str;
			str = Key_ID.ToString() + ":";
			foreach(RcKeyWork kw in Works){
				str += kw.Target.ValName + "(step=" + kw.Step.ToString("G9") + "),";
			}
			str = str.TrimEnd(',');
			return str;
		}

	}

	/// <summary>
	/// キー(RcKey)のリスト
	/// </summary>
	public class RcKeyList{
		public RcKey[] list;

		public static readonly Keys[] KeyMap = {
			Keys.Up, Keys.Down, Keys.Left, Keys.Right, 
			Keys.Z, Keys.X, Keys.C,
			Keys.A, Keys.S, Keys.D,
			Keys.V, Keys.B,
			Keys.F, Keys.G,
			Keys.Q, Keys.W, Keys.E
		};

		public RcKeyList(int KeyNum){
			list = new RcKey[KeyNum];
			for(int i = 0;i < KeyNum;i++){
				list[i] = new RcKey();
				list[i].Key_ID = (byte)i;
			}
		}

		public RcKey this[int idx]{
			get{
				return list[idx];
			}
		}

		public override string ToString() {
			string s = "Key{\r\n";
			string t;
			for(int i = 0;i < list.Length;i++){
				t = list[i].ToString();
				if(t == null || t == "")continue;
				s += "\t" + t + "\r\n";
			}

			s += "}\r\n";

			return s;
		}

		public void Parse(string input,RcValList vallist){
			input = input.Replace(" ","").Replace("\t","").Replace("\n","").Replace("\r","");
			string[] list = input.Split(':', ',', '(', ')');

			RcKey buff;
			RcVal valbuff;
			string valname;
			string step;
			int i = 0;
			while(i < list.Length -1){
				buff = this.list[int.Parse(list[i++])];
				do{
					if(list[i] == "")i++;

					valname = list[i++];
					step = list[i++];
					if(step.ToLower().StartsWith("step=")){
						step = step.Substring(5);
					}
					valbuff = vallist[valname];
					if(valbuff != null)
						buff.AssignWork(vallist[valname],float.Parse(step));
				}while(i < list.Length - 1 && list[i] == "");
			}
			


		}

	}

	/// <summary>
	/// マテリアル無し、テクスチャ一枚の簡易メッシュ
	/// </summary>
	public class RcXFile{
		public string FileName = null;
		public Mesh mesh = null;
		public Texture texture = null;

		/// <summary>
		/// メッシュを描画する。
		/// </summary>
		/// <param name="d3ddevice">描画する Microsoft.DirectX.Direct3D.Device への参照。</param>
		/// <param name="color">マテリアル色。</param>
		/// <param name="world">配置行列。</param>
		public void Draw(Device d3ddevice,Color color,Matrix world){
			Draw(d3ddevice,color,0x0022,world);
		}

		public void DrawTransparented(Device d3ddevice, Color color,Matrix world){
			d3ddevice.Transform.World = world;
			d3ddevice.SetTexture(0,texture);
			d3ddevice.TextureState[0].AlphaOperation = TextureOperation.BlendDiffuseAlpha;
			//		d3ddevice.RenderState.AlphaBlendEnable = true;

			Material mat = new Material();
			mat.Ambient = color;
			mat.Diffuse = color;

			mat.Emissive = Color.Black;
			mat.Specular = Color.FromArgb(
				color.R * 2 / 15,
				color.G * 2 / 15,
				color.B * 2 / 15);
			mat.SpecularSharpness = 32 / 15f;

			d3ddevice.Material = mat;

			mesh.DrawSubset(0);
		}

		public void Draw(Device d3ddevice, Color color, int materialstate, Matrix world) {
			int transparency = (materialstate & 0xF000) >> 12;
			int emissive = (materialstate & 0x0F00) >> 8;
			int specularSharpness = (materialstate & 0x00F0) >> 4;
			int specular = (materialstate & 0x000F);

			d3ddevice.Transform.World = world;
			d3ddevice.SetTexture(0, texture);
			d3ddevice.TextureState[0].AlphaOperation = TextureOperation.BlendDiffuseAlpha;
			//		d3ddevice.RenderState.AlphaBlendEnable = true;

			Material mat = new Material();
			transparency = (15 - transparency) * 0x11;
			color = Color.FromArgb(transparency, color);
			mat.Ambient = color;
			mat.Diffuse = color;

			mat.Emissive = Color.FromArgb(
				color.R * emissive / 15,
				color.G * emissive / 15,
				color.B * emissive / 15);
			mat.Specular = Color.FromArgb(
				color.R * specular / 15,
				color.G * specular / 15,
				color.B * specular / 15);
			mat.SpecularSharpness = 16 * specularSharpness / 15f;

			d3ddevice.Material = mat;

			mesh.DrawSubset(0);

			d3ddevice.SetTexture(0, null);
		}

		public void Draw2(Device d3ddevice, Color color, int materialstate, Matrix world) {
			int transparency = (materialstate & 0xF000) >> 12;
			int emissive = (materialstate & 0x0F00) >> 8;
			int specularSharpness = (materialstate & 0x00F0) >> 4;
			int specular = (materialstate & 0x000F);

			d3ddevice.Transform.World = world;
			d3ddevice.SetTexture(0, texture);
			d3ddevice.TextureState[0].AlphaOperation = TextureOperation.BlendDiffuseAlpha;
			//		d3ddevice.RenderState.AlphaBlendEnable = true;

			var prevSrcMode = d3ddevice.RenderState.SourceBlend;
			var prevDstMode = d3ddevice.RenderState.DestinationBlend;
			d3ddevice.RenderState.SourceBlend = Blend.SourceAlpha;
			d3ddevice.RenderState.DestinationBlend = Blend.One;

			Material mat = new Material();
			transparency = (15 - transparency) * 0x11;
			color = Color.FromArgb(transparency, color);
			mat.Ambient = color;
			mat.Diffuse = color;

			mat.Emissive = Color.FromArgb(
				color.R * emissive / 15,
				color.G * emissive / 15,
				color.B * emissive / 15);
			mat.Specular = Color.FromArgb(
				color.R * specular / 15,
				color.G * specular / 15,
				color.B * specular / 15);
			mat.SpecularSharpness = 16 * specularSharpness / 15f;

			d3ddevice.Material = mat;

			mesh.DrawSubset(0);

			d3ddevice.SetTexture(0, null);
			d3ddevice.RenderState.SourceBlend = prevSrcMode;
			d3ddevice.RenderState.DestinationBlend = prevDstMode;
		}

		/// <summary>
		/// FileName に指定されたメッシュを読み込む。
		/// </summary>
		/// <param name="d3ddevice">Microsoft.DirectX.Direct3D.Device への参照。</param>
		public void Load(Device d3ddevice){
			ExtendedMaterial[] matbuff;
			try{
				mesh = Mesh.FromFile(this.FileName,MeshFlags.SystemMemory,d3ddevice,out matbuff);
			}
			catch {
				mesh = null;
				return;
			}
			
			if(matbuff[0].TextureFilename != null){
				texture = TextureLoader.FromFile(d3ddevice, matbuff[0].TextureFilename);
			}
		}
		/// <summary>
		/// メッシュを読み込む。
		/// </summary>
		/// <param name="d3ddevice">Microsoft.DirectX.Direct3D.Device への参照。</param>
		/// <param name="FileName">読み込むメッシュのファイル名。</param>
		public void Load(Device d3ddevice,string FileName){
			this.FileName = FileName;
			Load(d3ddevice);
		}
	}


	/*
	public class RcHistory{
		const int HistoryListSize = 16;
		RcData rcdata;
		RcEditAction undo;
		RcEditAction redo;

		public RcHistory(RcData data){
			rcdata = data;
		}

		public void Clear(){
			redo.Previous = undo.Next = null;

			RcEditAction act = redo;
			while(act != null){
				act = redo.Next;
				act.Previous = null;
			}

			RcEditAction act = undo;
			while(act != null){
				act = undo.Previous;
				act.Next = null;
			}

			undo = redo = null;
		}

		public void RegistAction(RcEditAction action){
			redo = null;
			undo.Next = action;
			undo = action;
		}
		public void Undo(){
			if(!CanUndo)
				throw new ApplicationException("アンドゥできません。");

			undo.Undo(rcdata);
			redo = undo;
			undo = undo.Previous;
			
		}
		public void Redo(){
			if(!CanRedo)
				throw new ApplicationException("リドゥできません。");

			redo.Redo(rcdata);
			undo = redo;
			redo = redo.Next;
		}
		public bool CanUndo{
			get{return undo != null;}
		}
		public bool CanRedo{
			get{return redo != null;}
		}
	}

	public abstract class RcEditAction{
		protected RcEditAction prev;
		protected RcEditAction next;

		public RcEditAction(){
			prev = next = null;
		}

		/// <summary>
		/// 実行された処理を戻す作業を行う。
		/// </summary>
		public abstract void Undo(RcData data);
		/// <summary>
		/// 同じ動作を再度行う
		/// </summary>
		public abstract void Redo(RcData data);

		public RcEditAction Previous{
			set{prev = value;}
			get{ return prev;}
		}
		
		public RcEditAction Next{
			set{next = value;}
			get{ return next;}
		}
	}

	public class RcAddAction : RcEditAction{
		public RcAddAction(RcChipBase chip,RcChipBase to) : base(){

		}

		public override void Undo(RcData data) {
			
		}

		public override void Redo(RcData data) {

		}


	}
	public class RcDeleteAction : RcEditAction{
	}
	public class RcModifyAction : RcEditAction{
	}
	*/
	//----------------------------------------------------------------------------------//

	public struct RcConst {
		float val;
		bool hex;
		public float Value { get { return val; } set { val = value; } }
		public bool IsHexagonal { get { return hex; } set { hex = value; } }

		public RcConst(string expression) {
			string s = expression.Trim();

			val = RcData.ParseNumber(s);
			hex = s[0] == '#';
		}
		public RcConst(float val, bool ishex) {
			this.val = val;
			this.hex = ishex;
		}

		public static implicit operator float(RcConst c) {
			return c.val;
		}
		public static implicit operator RcConst(float f) {
			return new RcConst(f, false);
		}

		public override string ToString() {
			if (hex) {
				return "#" + ((int)val).ToString("X6");
			}
			else {
				return val.ToString("#########0.########");
			}
		}
	}

	/// <summary>
	/// チップ属性の値、変数(RcVal)と定数(float)の2項目
	/// </summary>
	public struct RcAttrValue{
		public RcConst Const;
		public RcVal Val;
		public bool isNegative;
		public bool isHexagonal { get { return Const.IsHexagonal; } }

		public RcAttrValue(string DefaultValue,RcValList vallist){
			Const = 0f;
			Val = null;
			isNegative = false;
			SetValue(DefaultValue,vallist);
		}

		public RcAttrValue(int IntValue){
			Const = 0f;
			Val = null;
			isNegative = false;
			SetValue(IntValue);
		}
		
		/// <summary>
		/// 初期状態におけるこの変数の値を得る。
		/// </summary>
		/// <returns>変数の値を表すfloat値。</returns>
		public float Value{
			get{
				checkDeadReference();
				if(Val != null)
					return isNegative ? -Val.Now : Val.Now;
				else
					return Const;
			}
		}

		/// <summary>
		/// 変数名、もしくは定数を文字列で得る。
		/// </summary>
		/// <returns>.rcdで使用可能な変数または定数の文字列。</returns>
		public override string ToString() {
			checkDeadReference();
			if (Val != null) {
				if (isNegative) {
					return "-" + Val.ValName;
				}
				else {
					return Val.ValName;
				}
			}
			else return Const.ToString();
		}

		public void SetValue(string expression,RcValList vallist){
			if(this.Val != null){
				this.Val = null;
			}

			try {
				this.isNegative = false;
				this.Const = new RcConst(expression);
			}
			catch (FormatException) {
				expression = expression.Trim();
				if (expression[0] == '-') {
					isNegative = true;
					Const.IsHexagonal = false;
					expression = expression.Substring(1);
				}
				else {
					isNegative = false;
					Const.IsHexagonal = false;
				}
				Val = vallist[expression];
				if (Val == null)
					throw new Exception("属性の値の設定に失敗しました : " + expression);
			}
			catch (OverflowException) {
				throw new Exception("値の絶対値が大きすぎます : " + expression);
			}
		}

		public void SetValue(int IntValue){
			Const = new RcConst((float)IntValue, true);
			Val = null;
			isNegative = false;
		}

		private void checkDeadReference(){
			if(Val != null && Val.RefCount< 0){
				Const = Val.Default;
				Val = null;
				isNegative = false;
			}
		}

		public Color ToColor(){
			int c = (int)this.Value;
			return Color.FromArgb((c >> 0x10) & 0xFF,(c >> 0x8) & 0xFF,c & 0xFF);
		}

		public static bool operator ==(RcAttrValue x, RcAttrValue y){
			return !(x != y);
		}
		public static bool operator !=(RcAttrValue x, RcAttrValue y) {
			return x.Val != y.Val || x.isNegative != y.isNegative || x.Const != y.Const;
		}

		public override int GetHashCode() {
			return Val.GetHashCode() ^ Const.GetHashCode();
		}

		public override bool Equals(object obj) {
			RcAttrValue v = (RcAttrValue)obj;
			return v == this;
		}

	}

	public struct RcHitStatus{
		public float distance;
		public RcChipBase HitChip;
	}

/*	public struct RcColor{
		public byte Red,Green,Blue;
		public RcColor(int R,int G,int B){
			Red = (byte)R;
			Green = (byte)G;
			Blue = (byte)B;
		}
		public override string ToString() {
			return "#" + Red.ToString("X").PadLeft(2,'0') + Green.ToString("X").PadLeft(2,'0') + Blue.ToString("X").PadLeft(2,'0');
		}
		public static RcColor Parse(string text){
			RcColor ret = new RcColor();
			if(text[0] != '#')
				throw new FormatException("入力文字列の書式が不正です : " + text);
			
			string buff = text.Substring(1);
			ret.Read(int.Parse(buff,System.Globalization.NumberStyles.AllowHexSpecifier));
			return ret;
		}
		public static RcColor Parse(int colorcode){
			RcColor ret = new RcColor();
			ret.Read(colorcode);
			return ret;
		}

		public void Read(string text){
			try{
				Read(int.Parse(text.Substring(1),System.Globalization.NumberStyles.AllowHexSpecifier));
			}catch{
				Color buff = Color.FromName(text);
				Red = buff.R;
				Green = buff.G;
				Blue = buff.B;
			}
		}
		public void Read(int colorcode){
			Red = (byte)((colorcode & 0xFF0000) >> 16);
			Green = (byte)((colorcode & 0xFF00) >> 8);
			Blue = (byte)(colorcode & 0xFF);
		}
		public Color ToColor(){
			return Color.FromArgb(Red,Green,Blue);
		}
		public static RcColor Default{
			get{
				RcColor c = new RcColor();
				c.Red = c.Green = c.Blue = 255;
				return c;
			}
		}
		public override bool Equals(object obj) {
			if(obj is RcColor){
				RcColor b = (RcColor)obj;
				return this.Red == b.Red && this.Green == b.Green && this.Blue == b.Blue;
			}
			return false;
		}

		public static bool operator ==(RcColor a,RcColor b){
			return a.Red == b.Red && a.Green == b.Green && a.Blue == b.Blue;
		}

		public static bool operator !=(RcColor a,RcColor b){
			return !(a == b);
		}

		public override int GetHashCode() {
			return (Red << 16) + (Green << 8) + (Blue);
		}

	}
*/
	//----------------------------------------------------------------------------------//
	/// <summary>
	/// 連結場所
	/// </summary>
	public enum RcJointPosition : byte{
		NULL = 255,
		North = 0,
		East,
		South,
		West 
	}
	/// <summary>
	/// 折り曲げ方式
	/// </summary>
	public enum RcAngle : byte{
		NULL = 255,
		x = 0,
		y,
		z
	}

	public enum RcChipType : byte{
		Unknown = 255,
		Core = 0,
		Chip,
		Frame,
		Rudder,
		RudderF,
		Trim,
		TrimF,
		Wheel,
		RLW,
		Jet,
		Weight,
		Cowl,
		Arm
	}


	//----------------------------------------------------------------------------------//
	//	プロパティ群
	//	フォームに持たせ、RcDataはその参照を拾う
	//
	//	逆でもいいような気がしてきた。まぁいいか。こいつら独立してるし。

	public class RcDrawOptions{
		public Color BackColor = Color.Navy;
		public Color CursorFrontColor = Color.Red;
		public Color CursorBackColor = Color.LightBlue;
		public Color NGuideColor = Color.Blue;
		public Color SGuideColor = Color.Yellow;
		public Color EGuideColor = Color.Red;
		public Color WGuideColor = Color.Green;

		public bool XAxisEnable = true;
		public bool XNegAxisEnable = true;
		public bool YAxisEnable = true;
		public bool YNegAxisEnable = true;
		public bool ZAxisEnable = true;
		public bool ZNegAxisEnable = true;
		public Color XAxisColor = Color.Red;
		public Color YAxisColor = Color.Green;
		public Color ZAxisColor = Color.Blue;
		public Color XNegAxisColor = Color.Cyan;
		public Color YNegAxisColor = Color.Magenta;
		public Color ZNegAxisColor = Color.Yellow;

		public bool ShowCowl = true;
		public int FrameGhostView = 0;
		public bool FrameGhostShow = true;
		public bool BaloonSwelling = true;
		public float BaloonSwellingRatio = 0.5f;

		public bool WeightEnable = true;
		public Color WeightColor = Color.White;
		public bool WeightBallEnable = true;
		public float WeightBallSize = 1.5f;
		public float WeightBallAlpha = 0.5f;
		public bool ShowGuideAlways = false;

		public bool CameraOrtho = false;

		public Color WeightBallColor = Color.Black;

		public bool AutoCamera = true;

		public RcDrawOptions(){
			try{
				System.IO.StreamReader file = new StreamReader(Application.StartupPath + "\\draw.cfg");

				BackColor = Color.FromArgb(int.Parse(file.ReadLine(),System.Globalization.NumberStyles.Number));
				CursorFrontColor = Color.FromArgb(int.Parse(file.ReadLine(),System.Globalization.NumberStyles.Number));
				CursorBackColor = Color.FromArgb(int.Parse(file.ReadLine(),System.Globalization.NumberStyles.Number));
				NGuideColor = Color.FromArgb(int.Parse(file.ReadLine(),System.Globalization.NumberStyles.Number));
				SGuideColor = Color.FromArgb(int.Parse(file.ReadLine(),System.Globalization.NumberStyles.Number));
				EGuideColor = Color.FromArgb(int.Parse(file.ReadLine(),System.Globalization.NumberStyles.Number));
				WGuideColor = Color.FromArgb(int.Parse(file.ReadLine(),System.Globalization.NumberStyles.Number));
				XAxisColor = Color.FromArgb(int.Parse(file.ReadLine(),System.Globalization.NumberStyles.Number));
				YAxisColor = Color.FromArgb(int.Parse(file.ReadLine(),System.Globalization.NumberStyles.Number));
				ZAxisColor = Color.FromArgb(int.Parse(file.ReadLine(),System.Globalization.NumberStyles.Number));
				XNegAxisColor = Color.FromArgb(int.Parse(file.ReadLine(),System.Globalization.NumberStyles.Number));
				YNegAxisColor = Color.FromArgb(int.Parse(file.ReadLine(),System.Globalization.NumberStyles.Number));
				ZNegAxisColor = Color.FromArgb(int.Parse(file.ReadLine(),System.Globalization.NumberStyles.Number));
				WeightColor = Color.FromArgb(int.Parse(file.ReadLine(),System.Globalization.NumberStyles.Number));
				ShowCowl = bool.Parse(file.ReadLine());
				FrameGhostShow = bool.Parse(file.ReadLine());
				string s = file.ReadLine();
				try{
					FrameGhostView = int.Parse(s);
				}
				catch(FormatException){
					FrameGhostView = bool.Parse(s) ? 0 : 1;
				}
				BaloonSwelling = bool.Parse(file.ReadLine());
				ShowGuideAlways = bool.Parse(file.ReadLine());
				XAxisEnable = bool.Parse(file.ReadLine());
				YAxisEnable = bool.Parse(file.ReadLine());
				ZAxisEnable = bool.Parse(file.ReadLine());
				XNegAxisEnable = bool.Parse(file.ReadLine());
				YNegAxisEnable = bool.Parse(file.ReadLine());
				ZNegAxisEnable = bool.Parse(file.ReadLine());
				WeightEnable = bool.Parse(file.ReadLine());
				WeightBallEnable = bool.Parse(file.ReadLine());

				BaloonSwellingRatio = float.Parse(file.ReadLine(),System.Globalization.NumberStyles.Float);
				WeightBallSize = float.Parse(file.ReadLine(),System.Globalization.NumberStyles.Float);
				WeightBallAlpha = float.Parse(file.ReadLine(),System.Globalization.NumberStyles.Float);
				
				CameraOrtho = bool.Parse(file.ReadLine());

				WeightBallColor = Color.FromArgb(int.Parse(file.ReadLine(),System.Globalization.NumberStyles.Number));

				AutoCamera = bool.Parse(file.ReadLine());

				file.Close();
			}
			catch{}
		}

		~RcDrawOptions(){
			StreamWriter file = new StreamWriter(Application.StartupPath + "\\draw.cfg");

			file.WriteLine(BackColor.ToArgb().ToString());
			file.WriteLine(CursorFrontColor.ToArgb().ToString());
			file.WriteLine(CursorBackColor.ToArgb().ToString());
			file.WriteLine(NGuideColor.ToArgb().ToString());
			file.WriteLine(SGuideColor.ToArgb().ToString());
			file.WriteLine(EGuideColor.ToArgb().ToString());
			file.WriteLine(WGuideColor.ToArgb().ToString());
			file.WriteLine(XAxisColor.ToArgb().ToString());
			file.WriteLine(YAxisColor.ToArgb().ToString());
			file.WriteLine(ZAxisColor.ToArgb().ToString());
			file.WriteLine(XNegAxisColor.ToArgb().ToString());
			file.WriteLine(YNegAxisColor.ToArgb().ToString());
			file.WriteLine(ZNegAxisColor.ToArgb().ToString());
			file.WriteLine(WeightColor.ToArgb().ToString());
			file.WriteLine(ShowCowl.ToString());
			file.WriteLine(FrameGhostShow.ToString());
			file.WriteLine(FrameGhostView.ToString());
			file.WriteLine(BaloonSwelling.ToString());
			file.WriteLine(ShowGuideAlways.ToString());
			file.WriteLine(XAxisEnable.ToString());
			file.WriteLine(YAxisEnable.ToString());
			file.WriteLine(ZAxisEnable.ToString());
			file.WriteLine(XNegAxisEnable.ToString());
			file.WriteLine(YNegAxisEnable.ToString());
			file.WriteLine(ZNegAxisEnable.ToString());
			file.WriteLine(WeightEnable.ToString());
			file.WriteLine(WeightBallEnable.ToString());

			file.WriteLine(BaloonSwellingRatio.ToString());
			file.WriteLine(WeightBallSize.ToString());
			file.WriteLine(WeightBallAlpha.ToString());

			file.WriteLine(CameraOrtho.ToString());

			file.WriteLine(WeightBallColor.ToArgb().ToString());

			file.WriteLine(AutoCamera.ToString());

			file.Close();
		}


	}

	public class RcOutputOptions{
		public bool ReturnEndChipBracket = false;
		public bool IndentEnable = true;
		public bool IndentBySpace = false;
		public bool OpenBracketWithChipDefinition = true;
		public bool CommaWithSpace = false;
		public uint IndentNum = 2;
		public bool PrintAllAttributes = false;

		public RcOutputOptions() : this(false){}

		public RcOutputOptions(bool defaultSetting) {
			if (!defaultSetting) {
				try {
					System.IO.StreamReader file = new StreamReader(Application.StartupPath + "\\output.cfg");

					ReturnEndChipBracket = bool.Parse(file.ReadLine());
					IndentEnable = bool.Parse(file.ReadLine());
					IndentBySpace = bool.Parse(file.ReadLine());
					OpenBracketWithChipDefinition = bool.Parse(file.ReadLine());
					CommaWithSpace = bool.Parse(file.ReadLine());
					PrintAllAttributes = bool.Parse(file.ReadLine());
					IndentNum = uint.Parse(file.ReadLine(), System.Globalization.NumberStyles.Integer);

					file.Close();
				}
				catch { }
			}
		}

		~RcOutputOptions(){
			System.IO.StreamWriter file = new StreamWriter(Application.StartupPath + "\\output.cfg");

			file.WriteLine(ReturnEndChipBracket.ToString());
			file.WriteLine(IndentEnable.ToString());
			file.WriteLine(IndentBySpace.ToString());
			file.WriteLine(OpenBracketWithChipDefinition.ToString());
			file.WriteLine(CommaWithSpace.ToString());
			file.WriteLine(PrintAllAttributes.ToString());
			file.WriteLine(IndentNum.ToString());

			file.Close();
		}
	}

	public class RcEditOptions{
		public bool UnvisibleNotSelected = true;	//	非表示のゴーストやカウルはマウスで選択されなくする
		public bool ConvertParentAttributes = false;	//	新規接続時、親チップの属性をコピーする
		public int ScrollFrameNum = 20;
		public int AngleViewGrid = 1;
		public bool AttributeAutoApply = false;

		public RcEditOptions(){
			try{
				System.IO.StreamReader file = new StreamReader(Application.StartupPath + "\\edit.cfg");

				UnvisibleNotSelected = bool.Parse(file.ReadLine());
				ConvertParentAttributes = bool.Parse(file.ReadLine());
				ScrollFrameNum = int.Parse(file.ReadLine());
				AngleViewGrid = int.Parse(file.ReadLine());
				AttributeAutoApply = bool.Parse(file.ReadLine());
			}
			catch{}
		}

		~RcEditOptions(){
			System.IO.StreamWriter file = new StreamWriter(Application.StartupPath + "\\edit.cfg");

			file.WriteLine(UnvisibleNotSelected.ToString());
			file.WriteLine(ConvertParentAttributes.ToString());
			file.WriteLine(ScrollFrameNum.ToString());
			file.WriteLine(AngleViewGrid.ToString());
			file.WriteLine(AttributeAutoApply.ToString());

			file.Close();
		}
	}

	class IterationHelper {
		delegate bool NextFinder<T>(T item, out T nextItem);

	}
}
