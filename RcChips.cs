using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using DX = Microsoft.DirectX;
using D3D = Microsoft.DirectX.Direct3D;
using System.Diagnostics;
using System.Threading;

namespace RigidChips {
	///<summery>
	///コア チップ クラス。
	///</summery>
	public class RcChipCore		: RcChipBase{
		RcXFile mesh;
		public RcChipCore(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,null,pos){
			this.JointPosition = RcJointPosition.NULL;
			this.matTranslation = Matrix.Identity;
			this.matRotation = Matrix.RotationY((float)Math.PI);
			mesh = Generics.GetMesh("Core.x");
		}

		public override float Fuel {
			get{	return 2000000;			}
		}



		public override string ChipType {
			get {
				return "Core";
			}
		}

		public override void Attach(RcChipBase to, RcJointPosition pos) {
			if(to == null)return;
			throw new Exception("Coreは他のチップの派生チップには出来ません。");
		}

		public override string AttrTip(string AttrName) {
			switch(AttrName){
				case "User1":
					return "シナリオ用";
				case "User2":
					return "シナリオ用";
				default:
					return base.AttrTip(AttrName);
			}
		}

		public override void Detach() {
			return;
		}

		public override void DrawChip() {
			if(mesh == null)
				mesh = Generics.GetMesh("Core.x");
			if(mesh != null)
				mesh.Draw(Generics.d3ddevice,this.ChipColor.ToColor(),matRotation * Matrix);
		}

		public ThreadStart ReservedDrawFunc;
		public override void ReserveDraw(System.Threading.ThreadStart fnc) {
			ReservedDrawFunc += fnc;
		}

		public override string[] AttrNameList {
			get{
				string[] s = {"Color","User1","User2"};
				return s;
			}
		}

		public override float[] AttrDefaultValueList {
			get {
				return new float[]{(float)0xFFFFFF,0f,0f};
			}
		}

		public override RcAttrValue[] AttrValList {
			get {
				return new RcAttrValue[]{ChipColor,user1,user2};
			}
		}


		public override Matrix Matrix {
			get {
				return Matrix.Identity;
			}
		}

		public override long MatrixVersion {
			get {
				return long.MinValue;
			}
		}

		public override RcAttrValue this[string AttrName] {
			get {
				if(AttrName == "User1")
					return user1;
				else if(AttrName == "User2")
					return user2;
				else return base[AttrName];
			}
			set {
				if(AttrName == "User1"){
					user1 = value;
				}
				else if(AttrName == "User2"){
					user2 = value;
				}
				else
					base[AttrName] = value;
			}
		}

		/*		public override string ToString() {
					string s = "";
					s += "Core(";	//	←チップの出力名

					//	属性記述ブロック
					if(ChipColor != RcColor.Default)
						s += "Color=" + ChipColor.ToString() + ",";
					if(Name != null && Name != "")
						s += "Name=" + Name + ",";
					if(user1.Val != null || user1.Const != 0f)
						s += "User1=" + user1.ToString() + ",";
					if(user2.Val != null || user2.Const != 0f)
						s += "User2=" + user2.ToString() + ",";

					s = s.TrimEnd(',');

					s += ")";

					return s;
				}

		*/
		public override void UpdateMatrix() {
			this.matTranslation = Matrix.Identity;
			foreach(RcChipBase cb in this.Child)
				if(cb != null)cb.UpdateMatrix();
		}


		public override RcChipBase Clone(bool WithChild,RcChipBase parent) {
			return null;
		}


		public override RcAngle AngleType {
			get {
				return RcAngle.NULL;
			}
		}


		public override float Weight {
			get {
				return 1f;
			}
		}

		public void Read(string data){
			string[] param = data.Split(',',':','(',')','=');
			int index = 1;
			
			RcAttrValue attr = new RcAttrValue();
			try{
				for( ; param[index] != "" ; index += 2){
					param[index] = param[index].ToLower();
					if(param[index] == "name")
						this.Name = param[index+1];
					else if(param[index] == "color")
						this["Color"] = new RcAttrValue(param[index+1],Generics.vals);
					else{
						param[index] = char.ToUpper(param[index][0]).ToString() + param[index].Substring(1);
						attr.SetValue(param[index+1],Generics.vals);
						this[param[index]] = attr;
					}

				}
			}
			catch(IndexOutOfRangeException){
				throw new Exception("不正な属性値指定が存在します。");
			}
			catch(Exception e){
				MessageBox.Show(e.Message);
			}
		}

	}

	///<summery>
	///チップ チップ クラス
	///</summery>
	public class RcChipChip		: RcChipBase{
		protected RcAttrValue damper,spring;
		protected RcXFile mesh;
		public RcChipChip(){}
		public RcChipChip(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			mesh = Generics.GetMesh("Chip.x");
			if(!Generics.EditOption.ConvertParentAttributes || parent == null || parent.GetType() != this.GetType()){
				damper.Val = null;
				damper.Const = 0.5f;
				spring.Val = null;
				spring.Const = 0.5f;
			}
		}



		public override string ChipType {
			get {
				return "Chip";
			}
		}
		public override string AttrTip(string AttrName) {
			switch(AttrName){
				case "Angle":
					return "折り曲げ角度";
				case "Damper":
				case "Dumper":
				case "Danper":
				case "Dunper":
					return "接続部の堅さ";
				case "Spring":
					return "接続部の弾性";
				case "User1":
				case "User2":
					return "シナリオ用";
				default:
					return base.AttrTip(AttrName);
			}
		}

		public override void DrawChip() {
			if(mesh == null)
				mesh = Generics.GetMesh("Chip.x");
			if(mesh != null)
				mesh.Draw(Generics.d3ddevice,ChipColor.ToColor(),this.matRotation * this.Matrix);
		}

		public override string[] AttrNameList {
			get{
				string[] s = {"Color","Angle","Damper","Spring","User1","User2"};
				return s;
			}
		}

		public override float[] AttrDefaultValueList {
			get {
				return new float[]{(float)0xFFFFFF,0f,0.5f,0.5f,0f,0f};
			}
		}

		public override RcAttrValue[] AttrValList {
			get {
				return new RcAttrValue[]{ChipColor,angle,damper,spring,user1,user2};
			}
		}


		/*		public override string ToString() {
					string str = "";
					str += "Chip(";

					if(Name != null && Name != "")
						str += "Name=" + Name + ",";
					if(angle.Val != null || angle.Const != 0f)
						str += "Angle=" + angle.ToString() + ",";
					if(this.ChipColor != RcColor.Default)
						str += "Color=" + ChipColor.ToString() + ",";
					if(damper.Val != null || damper.Const != 0.5f)
						str += "Damper=" + damper.ToString() + ",";
					if(spring.Val != null || spring.Const != 0.5f)
						str += "Spring=" + spring.ToString() + ",";
					if(user1.Val != null || user1.Const != 0f)
						str += "User1=" + user1.ToString() + ",";
					if(user2.Val != null || user2.Const != 0f)
						str += "User2=" + user2.ToString() + ",";
					str = str.TrimEnd(',');
					str += ")";

					return str;

				}
		*/
		public override RcAttrValue this[string AttrName] {
			get {
				switch(AttrName){
					case "Angle":
						return this.angle;
					case "Damper":
					case "Dumper":
					case "Danper":
					case "Dunper":
						return this.damper;
					case "Spring":
						return this.spring;
					case "User1":
						return this.user1;
					case "User2":
						return this.user2;
					default:
						return base[AttrName];
				}
			}
			set {
				switch(AttrName){
					case "Angle":
						this.angle = value;
						return;
					case "Damper":
					case "Dumper":
					case "Danper":
					case "Dunper":
						this.damper = value;
						return;
					case "Spring":
						this.spring = value;
						return;
					case "User1":
						this.user1 = value;
						return;
					case "User2":
						this.user2 = value;
						return;
					default:
						base[AttrName] = value;
						return;
				}
			}
		}


		public override RcChipBase Clone(bool WithChild,RcChipBase parent) {
			RcChipChip copy = new RcChipChip();
			copy.angle = this.angle;
			copy.ChipColor = this.ChipColor;
			copy.damper = this.damper;
			copy.Generics = this.Generics;
			copy.jointPosition = this.jointPosition;
			copy.mesh = this.mesh;
			copy.matVersion = this.matVersion;
			copy.Name = this.Name;
			copy.Parent = parent;
			copy.spring = this.spring;
			copy.user1 = this.user1;
			copy.user2 = this.user2;
			copy.Comment = this.Comment;
			copy.Child = new RcChipBase[RcData.ChildCapasity];
			if(WithChild){
				for(int i = 0;i < RcData.ChildCapasity;i++){
					if(this.Child[i] != null)copy.Add(this.Child[i].JointPosition ,this.Child[i].Clone(WithChild,copy),false);
				}
			}
			
			return copy;
		}

		public override void ReverseY() {
			base.ReverseY();
			if(this.angle.Val != null)
				this.angle.isNegative ^= true;
			else
				this.angle.Const = -this.angle.Const;
		}

		public override float Weight {
			get {
				return 1f;
			}
		}



	}

	///<summery>
	///フレーム チップ クラス
	///</summery>
	public class RcChipFrame	: RcChipChip{
		bool option;
		RcXFile mesh2;
		public RcChipFrame(){}
		public RcChipFrame(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			if(!Generics.EditOption.ConvertParentAttributes || parent == null || parent.GetType() != this.GetType()){
				option = false;
			}
		}

		public override float Fuel {
			get{return base.Fuel/2;}
		}


		public override string ChipType {
			get {
				return "Frame";
			}
		}
		public override void DrawChip() {
			/*
			 * 	a	s
			 * 	f	f	|	no	
			 * 	t	f	|	no
			 * 	f	t	|	frame
			 * 	t	t	|	alpha
			 * 
			 */
			if(mesh2 == null)
				mesh2 = Generics.GetMesh("Chip2.x");

			if(option){
				if(Generics.DrawOption.FrameGhostShow){
					switch(Generics.DrawOption.FrameGhostView){
						case 0:
						if(mesh != null)
							mesh.Draw(Generics.d3ddevice,ChipColor.ToColor(),0xA022,matRotation * matTranslation);
							break;
						case 1:
						if(mesh2 != null)
							mesh2.Draw(Generics.d3ddevice,ChipColor.ToColor(),matRotation * matTranslation);
							break;
						case 2:
							RcXFile x = Generics.GetMesh(Application.StartupPath + "\\Resources\\ChipG.x",true);
							if(x != null)x.Draw(Generics.d3ddevice,ChipColor.ToColor(),matRotation * matTranslation);
							break;
					}
				}
			}
			else{
				if(mesh2 != null)
					mesh2.Draw(Generics.d3ddevice,ChipColor.ToColor(),matRotation * matTranslation);
			}

		}

		public override string AttrTip(string AttrName) {
			if(AttrName == "Option")
				return "0以外でゴースト化";
			return base.AttrTip (AttrName);
		}

		public override string[] AttrNameList {
			get{
				string[] s = base.AttrNameList;
				s.CopyTo(s = new string[s.Length + 1],0);
				s[s.Length -1] = s[s.Length - 2];
				s[s.Length -2] = s[s.Length - 3];
				s[s.Length -3] = "Option";
				return s;
			}
		}

		public override RcAttrValue this[string AttrName] {
			get {
				RcAttrValue v;
				if(AttrName == "Option"){
					v.Const = new RcConst(option ? 1.0f : 0.0f, false);
					v.Val = null;
					v.isNegative = false;
					return v;
				}
				return base[AttrName];
			}
			set {
				if(AttrName == "Option")
					option = (value.Value != 0f);
				else base[AttrName] = value;
			}
		}

		public override float[] AttrDefaultValueList {
			get {
				return new float[]{(float)0xFFFFFF,0f,0.5f,0.5f,0f,0f,0f};
			}
		}

		public override RcAttrValue[] AttrValList {
			get {
				RcAttrValue attropt = new RcAttrValue();
				attropt.Const = (option ? 1 : 0);
				return new RcAttrValue[]{ChipColor,angle,damper,spring,attropt,user1,user2};
			}
		}

		/*		public override string ToString() {
					string s = "";
					s += "Frame(";	//	←チップの出力名

					//	属性記述ブロック
					if(ChipColor != RcColor.Default)
						s += "Color=" + ChipColor.ToString() + ",";
					if(Name != null && Name != "")
						s += "Name=" + Name + ",";
					if(angle.Val != null || angle.Const != 0f)
						s += "Angle=" + angle.ToString() + ",";
					if(damper.Val != null || damper.Const != 0.5f)
						s += "Damper=" + damper.ToString() + ",";
					if(spring.Val != null || spring.Const != 0.5f)
						s += "Spring=" + spring.ToString() + ",";
					if(option)
						s += "Option=1,";
					if(user1.Val != null || user1.Const != 0f)
						s += "User1=" + user1.ToString() + ",";
					if(user2.Val != null || user2.Const != 0f)
						s += "User2=" + user2.ToString() + ",";


					s = s.TrimEnd(',');

					s += ")";

					return s;
				}
		*/

		public override RcChipBase Clone(bool WithChild,RcChipBase parent) {
			RcChipFrame copy = new RcChipFrame();
			copy.Name = this.Name;
			copy.angle = this.angle;
			copy.ChipColor = this.ChipColor;
			copy.Generics = this.Generics;
			copy.jointPosition = this.jointPosition;
			copy.matVersion = this.matVersion;
			copy.Parent = parent;

			copy.damper = this.damper;
			copy.spring = this.spring;
			copy.mesh = this.mesh;
			copy.option = this.option;
			copy.user1 = this.user1;
			copy.user2 = this.user2;
			copy.Comment = this.Comment;

			copy.Child = new RcChipBase[RcData.ChildCapasity];
			if(WithChild){
				for(int i = 0;i < RcData.ChildCapasity;i++){
					if(this.Child[i] != null)copy.Add(this.Child[i].JointPosition ,this.Child[i].Clone(WithChild,copy),false);
				}
			}
			
			return copy;
		}


		public override float Weight {
			get {
				return 0.5f;
			}
		}

		public override RcHitStatus IsHit(int X, int Y, int ScrWidth, int ScrHeight) {
			if(Generics.EditOption.UnvisibleNotSelected && option && !Generics.DrawOption.FrameGhostShow){
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
				return dist;
			}
			else 
				return base.IsHit (X, Y, ScrWidth, ScrHeight);

		}


	}

	///<summery>
	///ラダー チップ クラス
	///</summery>
	public class RcChipRudder	: RcChipChip{
		public RcChipRudder(){}
		public RcChipRudder(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			mesh = Generics.GetMesh("Rudder.x");
		}

		public override string ChipType {
			get {
				return "Rudder";
			}
		}

		public override void DrawChip() {
			if(mesh == null)
				mesh = Generics.GetMesh("Rudder.x");
			if(mesh != null)
				mesh.Draw(Generics.d3ddevice,ChipColor.ToColor(),matRotation * this.Matrix);
		}

		/*		public override string ToString() {
					string str = "";
					str += "Rudder(";

					if(Name != null && Name != "")
						str += "Name=" + Name + ",";
					if(angle.Val != null || angle.Const != 0f)
						str += "Angle=" + angle.ToString() + ",";
					if(this.ChipColor != RcColor.Default)
						str += "Color=" + ChipColor.ToString() + ",";
					if(damper.Val != null || damper.Const != 0.5f)
						str += "Damper=" + damper.ToString() + ",";
					if(spring.Val != null || spring.Const != 0.5f)
						str += "Spring=" + spring.ToString() + ",";
					if(user1.Val != null || user1.Const != 0f)
						str += "User1=" + user1.ToString() + ",";
					if(user2.Val != null || user2.Const != 0f)
						str += "User2=" + user2.ToString() + ",";

					str = str.TrimEnd(',');
					str += ")";

					return str;

				}
		*/
		//public override void UpdateMatrix() {
		//    float a = angle.Value;
		//    Matrix invRotation = matRotation;
		//    invRotation.Invert();
		//    matTranslation =	  invRotation
		//        * Matrix.Translation(0f,0f,0.3f)
		//        * Matrix.RotationY((float)(a / 180f * Math.PI))
		//        * Matrix.Translation(0f,0f,0.3f)
		//        * matRotation;


		//    if(Parent != null)	matTranslation *= Parent.Matrix;
		//    matVersion = System.DateTime.Now.Ticks;

		//    foreach(RcChipBase c in Child)if(c != null){
		//                                      c.UpdateMatrix();
		//                                  }
			
		//}

		public override RcChipBase Clone(bool WithChild,RcChipBase parent) {
			RcChipRudder copy = new RcChipRudder();
			copy.angle = this.angle;
			copy.ChipColor = this.ChipColor;
			copy.damper = this.damper;
			copy.Generics = this.Generics;
			copy.jointPosition = this.jointPosition;
			copy.mesh = this.mesh;
			copy.matVersion = this.matVersion;
			copy.Name = this.Name;
			copy.Parent = parent;
			copy.spring = this.spring;
			copy.user1 = this.user1;
			copy.user2 = this.user2;
			copy.Child = new RcChipBase[RcData.ChildCapasity];
			copy.Comment = this.Comment;
			if(WithChild){
				for(int i = 0;i < RcData.ChildCapasity;i++){
					if(this.Child[i] != null)copy.Add(this.Child[i].JointPosition ,this.Child[i].Clone(WithChild,copy),false);
				}
			}
			
			return copy;
		}

		public override void ReverseX() {
			base.ReverseX ();
			if(this.angle.Val != null)
				this.angle.isNegative ^= true;
			else
				this.angle.Const = -this.angle.Const;
		}
		public override void ReverseY() {
			RcChipBase[] cld = new RcChipBase[RcData.ChildCapasity];
			RcJointPosition jp;
			this.Child.CopyTo(cld,0);

			foreach(RcChipBase cb in cld){
				if(cb == null)continue;
				jp = cb.JointPosition;
				cb.ReverseY();
			}
		}
		public override void ReverseZ() {
			base.ReverseZ();
			if(this.angle.Val != null)
				this.angle.isNegative ^= true;
			else
				this.angle.Const = -this.angle.Const;
		}


		public override RcAngle AngleType {
			get {
				return RcAngle.y;
			}
		}


	}

	///<summery>
	///ラダーフレーム チップ クラス
	///</summery>
	public class RcChipRudderF	: RcChipRudder{
		bool option;
		RcXFile mesh2;
		public RcChipRudderF(){}
		public RcChipRudderF(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			mesh2 = Generics.GetMesh("RudderF.x");
			if(!Generics.EditOption.ConvertParentAttributes || parent == null || parent.GetType() != this.GetType()){
				option = false;
			}
		}

		public override float Fuel {
get{			return base.Fuel / 2;}
		}


		public override string ChipType {
			get {
				return "RudderF";
			}
		}
		public override void DrawChip() {
			if(mesh2 == null)
				mesh2 = Generics.GetMesh("RudderF.x");
			if(option){
				if(Generics.DrawOption.FrameGhostShow){
					switch(Generics.DrawOption.FrameGhostView){
						case 0:
							if(mesh != null)
								mesh.Draw(Generics.d3ddevice,ChipColor.ToColor(),0xA022,matRotation * matTranslation);
							break;
						case 1:
							if(mesh2 != null)
								mesh2.Draw(Generics.d3ddevice,ChipColor.ToColor(),matRotation * matTranslation);
							break;
						case 2:
							RcXFile x = Generics.GetMesh(Application.StartupPath + "\\Resources\\RudderG.x",true);
							if(x != null)x.Draw(Generics.d3ddevice,ChipColor.ToColor(),matRotation * matTranslation);
							break;
					}
				}
			}
			else{
				if(mesh2 != null)
					mesh2.Draw(Generics.d3ddevice,ChipColor.ToColor(),matRotation * matTranslation);
			}
		}

		public override string AttrTip(string AttrName) {
			if(AttrName == "Option")
				return "0以外でゴースト化";
			return base.AttrTip (AttrName);
		}

		public override string[] AttrNameList {
			get{
				string[] s = base.AttrNameList;
				s.CopyTo(s = new string[s.Length + 1],0);
				s[s.Length -1] = s[s.Length - 2];
				s[s.Length -2] = s[s.Length - 3];
				s[s.Length -3] = "Option";
				return s;
			}
		}

		public override float[] AttrDefaultValueList {
			get {
				return new float[]{(float)0xFFFFFF,0f,0.5f,0.5f,0f,0f,0f};
			}
		}

		public override RcAttrValue[] AttrValList {
			get {
				RcAttrValue attropt = new RcAttrValue();
				attropt.Const = (option ? 1 : 0);
				return new RcAttrValue[]{ChipColor,angle,damper,spring,attropt,user1,user2};
			}
		}

		public override RcAttrValue this[string AttrName] {
			get {
				RcAttrValue v;
				if(AttrName == "Option"){
					v.Const = new RcConst(option ? 1f : 0f, false);
					v.isNegative = false;
					v.Val = null;
					return v;
				}
				return base[AttrName];
			}
			set {
				if(AttrName == "Option")
					option = (value.Value != 0f);
				else base[AttrName] = value;
			}
		}

		/*		public override string ToString() {
					string str = "";
					str += "RudderF(";

					if(Name != null && Name != "")
						str += "Name=" + Name + ",";
					if(angle.Val != null || angle.Const != 0f)
						str += "Angle=" + angle.ToString() + ",";
					if(this.ChipColor != RcColor.Default)
						str += "Color=" + ChipColor.ToString() + ",";
					if(damper.Val != null || damper.Const != 0.5f)
						str += "Damper=" + damper.ToString() + ",";
					if(spring.Val != null || spring.Const != 0.5f)
						str += "Spring=" + spring.ToString() + ",";
					if(option)
						str += "Option=1,";
					if(user1.Val != null || user1.Const != 0f)
						str += "User1=" + user1.ToString() + ",";
					if(user2.Val != null || user2.Const != 0f)
						str += "User2=" + user2.ToString() + ",";

					str = str.TrimEnd(',');
					str += ")";

					return str;

				}
		*/
		public override RcChipBase Clone(bool WithChild,RcChipBase parent) {
			RcChipRudderF copy = new RcChipRudderF();
			copy.Name = this.Name;
			copy.angle = this.angle;
			copy.ChipColor = this.ChipColor;
			copy.Generics = this.Generics;
			copy.jointPosition = this.jointPosition;
			copy.matVersion = this.matVersion;
			copy.Parent = parent;

			copy.damper = this.damper;
			copy.spring = this.spring;
			copy.mesh = this.mesh;
			copy.option = this.option;
			copy.user1 = this.user1;
			copy.user2 = this.user2;
			copy.Child = new RcChipBase[RcData.ChildCapasity];
			copy.Comment = this.Comment;
			if(WithChild){
				for(int i = 0;i < RcData.ChildCapasity;i++){
					if(this.Child[i] != null)copy.Add(this.Child[i].JointPosition ,this.Child[i].Clone(WithChild,copy),false);
				}
			}
			
			return copy;
		}


		public override float Weight {
			get {
				return 0.5f;
			}
		}

		public override RcHitStatus IsHit(int X, int Y, int ScrWidth, int ScrHeight) {
			if(option && !Generics.DrawOption.FrameGhostShow){
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
				return dist;
			}
			else 
				return base.IsHit (X, Y, ScrWidth, ScrHeight);

		}


	}
	
	///<summery>
	///トリム チップ クラス
	///</summery>
	public class RcChipTrim		: RcChipChip{
		public RcChipTrim(){}
		public RcChipTrim(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			mesh = Generics.GetMesh("Trim.x");
		}

		public override string ChipType {
			get {
				return "Trim";
			}
		}
		public override void DrawChip() {
			if(mesh == null)
				mesh = Generics.GetMesh("Trim.x");
			if(mesh != null)
				mesh.Draw(Generics.d3ddevice,ChipColor.ToColor(),matRotation * this.Matrix);
		}

		/*		public override string ToString() {
					string str = "";
					str += "Trim(";

					if(Name != null && Name != "")
						str += "Name=" + Name + ",";
					if(angle.Val != null || angle.Const != 0f)
						str += "Angle=" + angle.ToString() + ",";
					if(this.ChipColor != RcColor.Default)
						str += "Color=" + ChipColor.ToString() + ",";
					if(damper.Val != null || damper.Const != 0.5f)
						str += "Damper=" + damper.ToString() + ",";
					if(spring.Val != null || spring.Const != 0.5f)
						str += "Spring=" + spring.ToString() + ",";
					if(user1.Val != null || user1.Const != 0f)
						str += "User1=" + user1.ToString() + ",";
					if(user2.Val != null || user2.Const != 0f)
						str += "User2=" + user2.ToString() + ",";
					str = str.TrimEnd(',');
					str += ")";

					return str;

				}
		*/
		//public override void UpdateMatrix() {
		//    float a = angle.Value;
		//    Matrix invRotation = matRotation;
		//    invRotation.Invert();
		//    matTranslation =	  invRotation
		//        * Matrix.Translation(0f,0f,0.3f)
		//        * Matrix.RotationZ((float)(-a / 180f * Math.PI))
		//        * Matrix.Translation(0f,0f,0.3f)
		//        * matRotation;


		//    if(Parent != null)	matTranslation *= Parent.Matrix;
		//    matVersion = System.DateTime.Now.Ticks;

		//    foreach(RcChipBase c in Child)if(c != null){
		//                                      c.UpdateMatrix();
		//                                  }
			
		//}

		public override RcChipBase Clone(bool WithChild,RcChipBase parent) {
			RcChipTrim copy = new RcChipTrim();
			copy.Name = this.Name;
			copy.angle = this.angle;
			copy.ChipColor = this.ChipColor;
			copy.Generics = this.Generics;
			copy.jointPosition = this.jointPosition;
			copy.matVersion = this.matVersion;
			copy.Parent = parent;

			copy.damper = this.damper;
			copy.spring = this.spring;
			copy.mesh = this.mesh;
			copy.user1 = this.user1;
			copy.user2 = this.user2;
			copy.Child = new RcChipBase[RcData.ChildCapasity];
			copy.Comment = this.Comment;
			if(WithChild){
				for(int i = 0;i < RcData.ChildCapasity;i++){
					if(this.Child[i] != null)copy.Add(this.Child[i].JointPosition ,this.Child[i].Clone(WithChild,copy),false);
				}
			}
			
			return copy;
		}

		public override void ReverseX() {
			base.ReverseX ();
			if(this.angle.Val != null)
				this.angle.isNegative ^= true;
			else
				this.angle.Const = -this.angle.Const;
		}

		public override void ReverseZ() {
			base.ReverseZ();
			if(this.angle.Val != null)
				this.angle.isNegative ^= true;
			else
				this.angle.Const = -this.angle.Const;
		}

		public override RcAngle AngleType {
			get {
				return RcAngle.z;
			}
		}

	}

	///<summery>
	///トリムフレーム チップ クラス
	///</summery>
	public class RcChipTrimF	: RcChipTrim{
		bool option;
		RcXFile mesh2;
		public RcChipTrimF(){}
		public RcChipTrimF(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			mesh2 = Generics.GetMesh("TrimF.x");
			if(!Generics.EditOption.ConvertParentAttributes || parent == null || parent.GetType() != this.GetType()){
				option = false;
			}
		}

		public override float Fuel {
			get{return base.Fuel / 2;}
		}


		public override string ChipType {
			get {
				return "TrimF";
			}
		}
		public override void DrawChip() {
			if(mesh2 == null)
				mesh2 = Generics.GetMesh("TrimF.x");
			if(option){
				if(Generics.DrawOption.FrameGhostShow){
					switch(Generics.DrawOption.FrameGhostView){
						case 0:
							if(mesh != null)
								mesh.Draw(Generics.d3ddevice,ChipColor.ToColor(),0xA022,matRotation * matTranslation);
							break;
						case 1:
							if(mesh2 != null)
								mesh2.Draw(Generics.d3ddevice,ChipColor.ToColor(),matRotation * matTranslation);
							break;
						case 2:
							RcXFile x = Generics.GetMesh(Application.StartupPath + "\\Resources\\TrimG.x",true);
							if(x != null)x.Draw(Generics.d3ddevice,ChipColor.ToColor(),matRotation * matTranslation);
							break;
					}
				}
			}
			else{
				if(mesh2 != null)
					mesh2.Draw(Generics.d3ddevice,ChipColor.ToColor(),matRotation * matTranslation);
			}
		}

		public override string AttrTip(string AttrName) {
			if(AttrName == "Option")
				return "0以外でゴースト化";
			return base.AttrTip (AttrName);
		}

		public override string[] AttrNameList {
			get{
				string[] s = base.AttrNameList;
				s.CopyTo(s = new string[s.Length + 1],0);
				s[s.Length-1] = s[s.Length-2];
				s[s.Length-2] = s[s.Length-3];
				s[s.Length-3] = "Option";
				return s;
			}
		}

		public override float[] AttrDefaultValueList {
			get {
				return new float[]{(float)0xFFFFFF,0f,0.5f,0.5f,0f,0f,0f};
			}
		}

		public override RcAttrValue[] AttrValList {
			get {
				RcAttrValue attropt = new RcAttrValue();
				attropt.Const = (option ? 1 : 0);
				return new RcAttrValue[]{ChipColor,angle,damper,spring,attropt,user1,user2};
			}
		}

		public override RcAttrValue this[string AttrName] {
			get {
				RcAttrValue v;
				if(AttrName == "Option"){
					v.Const = new RcConst(option ? 1f : 0f, false);
					v.isNegative = false;
					v.Val = null;
					return v;
				}
				return base[AttrName];
			}
			set {
				if(AttrName == "Option")
					option = (value.Value != 0f);
				else base[AttrName] = value;
			}
		}

		/*		public override string ToString() {
					string str = "";
					str += "TrimF(";

					if(Name != null && Name != "")
						str += "Name=" + Name + ",";
					if(angle.Val != null || angle.Const != 0f)
						str += "Angle=" + angle.ToString() + ",";
					if(this.ChipColor != RcColor.Default)
						str += "Color=" + ChipColor.ToString() + ",";
					if(damper.Val != null || damper.Const != 0.5f)
						str += "Damper=" + damper.ToString() + ",";
					if(spring.Val != null || spring.Const != 0.5f)
						str += "Spring=" + spring.ToString() + ",";
					if(option)
						str += "Option=1,";
					if(user1.Val != null || user1.Const != 0f)
						str += "User1=" + user1.ToString() + ",";
					if(user2.Val != null || user2.Const != 0f)
						str += "User2=" + user2.ToString() + ",";
					str = str.TrimEnd(',');
					str += ")";

					return str;

				}*/
		public override RcChipBase Clone(bool WithChild,RcChipBase parent) {
			RcChipTrimF copy = new RcChipTrimF();
			copy.Name = this.Name;
			copy.angle = this.angle;
			copy.ChipColor = this.ChipColor;
			copy.Generics = this.Generics;
			copy.jointPosition = this.jointPosition;
			copy.matVersion = this.matVersion;
			copy.Parent = parent;

			copy.damper = this.damper;
			copy.spring = this.spring;
			copy.mesh = this.mesh;
			copy.option = this.option;
			copy.user1 = this.user1;
			copy.user2 = this.user2;
			copy.Comment = this.Comment;

			copy.Child = new RcChipBase[RcData.ChildCapasity];
			if(WithChild){
				for(int i = 0;i < RcData.ChildCapasity;i++){
					if(this.Child[i] != null)copy.Add(this.Child[i].JointPosition ,this.Child[i].Clone(WithChild,copy),false);
				}
			}
			
			return copy;
		}


		public override float Weight {
			get {
				return 0.5f;
			}
		}

		public override RcHitStatus IsHit(int X, int Y, int ScrWidth, int ScrHeight) {
			if(option && !Generics.DrawOption.FrameGhostShow){
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
				return dist;
			}
			else 
				return base.IsHit (X, Y, ScrWidth, ScrHeight);

		}


	}

	///<summery>
	///ジェット/バルーン チップ クラス
	///</summery>
	public class RcChipJet		: RcChipBase{
		RcXFile jet,baloon,fire;
		RcAttrValue power,damper,spring,effect,option;
		public RcChipJet(){}
		public RcChipJet(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			jet = Generics.GetMesh("Jet.x");
			baloon = Generics.GetMesh("Jet2.x");
			fire = Generics.GetMesh("Fire.x");
			if(!Generics.EditOption.ConvertParentAttributes || parent == null || parent.GetType() != this.GetType()){
				damper.Val = null;
				damper.Const = 0.5f;
				spring.Val = null;
				spring.Const = 0.5f;
				effect.Const = 0f;
			}
		}

		public override float Fuel {
			get{return 0;}
		}


		public override string ChipType {
			get {
				return "Jet";
			}
		}
		public override string AttrTip(string AttrName) {
			switch(AttrName){
				case "Angle":
					return "折り曲げ角度";
				case "Damper":
				case "Dumper":
				case "Danper":
				case "Dunper":
					return "接続部の堅さ";
				case "Spring":
					return "接続部の弾性";
				case "Power":
					return "ジェット出力:バルーンガス量";
				case "Option":
					return "0:ジェット 1:水素バルーン 2:空気バルーン";
				case "Effect":
					return "1-4:スモークを出す(ジェット時のみ)";
				case "User1":
				case "User2":
					return "シナリオ用";
				default:
					return base.AttrTip(AttrName);
			}

		}

		public override void DrawChip() {
			if(jet == null)
				jet = Generics.GetMesh("Jet.x");
			if(baloon == null)
				baloon = Generics.GetMesh("Jet2.x");
			float size = (float)Math.Pow(power.Value,0.3);
			if(option.Const == 0f){
				if(jet != null)
					jet.Draw(Generics.d3ddevice,ChipColor.ToColor(),matRotation * Matrix);
				if (Generics.Preview) {
					// 2000で1チップの長さ
					// 最大で2.5チップ分(5000)
					// それ以上は伸びない
					if (fire == null)
						fire = Generics.GetMesh("Fire.x");
					float scale = Math.Abs(this.power.Value);
					if (scale > 5000) scale = 5000;
					scale *= Math.Sign(this.power.Value) / 2000.0f;
					Debug.WriteLine(scale, "jet scale");
					Matrix m =
						  Matrix.Translation(0f, -0.3f, 0f)
						* Matrix.Scaling(1f, scale, 1f)
						* matRotation 
						* Matrix;
					ReserveDraw(() => {
						//bool lighting = Generics.d3ddevice.RenderState.Lighting;
						//Generics.d3ddevice.RenderState.Lighting = false;
						fire.Draw2(Generics.d3ddevice, Color.Transparent, 0x0FFF, m);
						//Generics.d3ddevice.RenderState.Lighting = lighting;
					});
				}
			}
			
			else if(Generics.DrawOption.BaloonSwelling){
				float rate = 0.5f + Generics.DrawOption.BaloonSwellingRatio * (float)Math.Pow(power.Value * 3 / (4f * Math.PI),1.0/3.0);
				/*
					 * V = 4/3 * Pi *r^3 
					 * r^3 = V*3/4 / PI
					 * 
					 * ax^3 + bx^2 + cx + d = ?
					 * 0	:0.5 d = 0.5
					 * 160	:1
					 * 3340	:2
					 */
				rate = Math.Abs(rate);
				if(baloon != null)
					baloon.Draw(Generics.d3ddevice,ChipColor.ToColor(),Matrix.Scaling(rate,rate,rate) * matRotation * Matrix);
			}
			else
				if(baloon != null)
				baloon.Draw(Generics.d3ddevice,ChipColor.ToColor(),matRotation * Matrix);
		}

		public override string[] AttrNameList {
			get{
				string[] s = {"Color","Angle","Damper","Spring","Power","Option","Effect","User1","User2"};
				return s;
			}
		}

		public override float[] AttrDefaultValueList {
			get {
				return new float[]{(float)0xFFFFFF,0f,0.5f,0.5f,0f,0f,0f,0f,0f};
			}
		}

		public override RcAttrValue[] AttrValList {
			get {
				return new RcAttrValue[]{ChipColor,angle,damper,spring,power,option,effect,user1,user2};
			}
		}


		public override RcAttrValue this[string AttrName] {
			get {
				switch(AttrName){
					case "Angle":
						return this.angle;
					case "Damper":
					case "Dumper":
					case "Danper":
					case "Dunper":
						return this.damper;
					case "Spring":
						return this.spring;
					case "Power":
						return this.power;
					case "Option":
						return this.option;
					case "Effect":
						return this.effect;
					case "User1":
						return this.user1;
					case "User2":
						return this.user2;
					default:
						return base[AttrName];
				}
			}
			set {
				switch(AttrName){
					case "Angle":
						this.angle = value;
						return;
					case "Damper":
					case "Dumper":
					case "Danper":
					case "Dunper":
						this.damper = value;
						return;
					case "Spring":
						this.spring = value;
						return;
					case "Power":
						this.power = value;
						return;
					case "Option":
						this.option = value;
						return;
					case "Effect":
						this.effect = value;
						return;
					case "User1":
						this.user1 = value;
						return;
					case "User2":
						this.user2 = value;
						return;
					default:
						base[AttrName] = value;
						return;
				}
			}
		}

		/*		public override string ToString() {
					string s = "";
					s += "Jet(";	//	←チップの出力名

					//	属性記述ブロック
					if(ChipColor != RcColor.Default)
						s += "Color=" + ChipColor.ToString() + ",";
					if(Name != null && Name != "")
						s += "Name=" + Name + ",";
					if(angle.Val != null || angle.Const != 0f)
						s += "Angle=" + angle.ToString() + ",";
					if(damper.Val != null || damper.Const != 0.5f)
						s += "Damper=" + damper.ToString() + ",";
					if(spring.Val != null || spring.Const != 0.5f)
						s += "Spring=" + spring.ToString() + ",";
					if(power.Val != null || power.Const != 0f)
						s += "Power=" + power.ToString() + ",";
					if(option.Val != null || option.Const != 0f)
						s += "Option=" + option.ToString() + ",";
					if(effect.Val != null || effect.Const != 0f)
						s += "Effect=" + effect.ToString() + ",";
					if(user1.Val != null || user1.Const != 0f)
						s += "User1=" + user1.ToString() + ",";
					if(user2.Val != null || user2.Const != 0f)
						s += "User2=" + user2.ToString() + ",";

					s = s.TrimEnd(',');

					s += ")";

					return s;
				}
		*/
		public override RcHitStatus IsHit(int X, int Y, int ScrWidth, int ScrHeight) {
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
			
			if(option.Value != 0f)
				buff.distance = (baloon.mesh.Intersect(vNear, vDir, out sectinfo)) ? sectinfo.Dist : float.MaxValue; 
			else
				buff.distance = (Generics.imesh.Intersect(vNear, vDir, out sectinfo)) ? sectinfo.Dist : float.MaxValue; 

			if(dist.distance > buff.distance){
				dist.distance = buff.distance;
				dist.HitChip = this;
			}

			return dist;
		}

		public override RcChipBase Clone(bool WithChild,RcChipBase parent) {
			RcChipJet copy = new RcChipJet();
			copy.Name = this.Name;
			copy.angle = this.angle;
			copy.ChipColor = this.ChipColor;
			copy.Generics = this.Generics;
			copy.jointPosition = this.jointPosition;
			copy.matVersion = this.matVersion;
			copy.Parent = parent;

			copy.jet = this.jet;
			copy.baloon = this.baloon;

			copy.damper = this.damper;
			copy.spring = this.spring;
			copy.power = this.power;
			copy.option = this.option;
			copy.effect = this.effect;
			copy.user1 = this.user1;
			copy.user2 = this.user2;
			copy.Comment = this.Comment;

			copy.Child = new RcChipBase[RcData.ChildCapasity];
			if(WithChild){
				for(int i = 0;i < RcData.ChildCapasity;i++){
					if(this.Child[i] != null)copy.Add(this.Child[i].JointPosition ,this.Child[i].Clone(WithChild,copy),false);
				}
			}
			
			return copy;
		}

		public override void ReverseY() {
			base.ReverseY();
			if(this.angle.Val != null)
				this.angle.isNegative ^= true;
			else
				this.angle.Const = -this.angle.Const;

			if(this.power.Val != null)
				this.power.isNegative ^= true;
			else
				this.power.Const = -this.power.Const;
		}


		public override float Weight {
			get {
				return 1.47f;
			}
		}

	}

	///<summery>
	///ホイール チップ クラス
	///</summery>
	public class RcChipWheel	: RcChipBase{
		protected RcXFile mesh;
		protected RcXFile tire;
		protected RcAttrValue damper,spring,power,brake,effect;
		protected int option;
		public RcChipWheel(){}
		public RcChipWheel(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			mesh = Generics.GetMesh("Wheel.x");
			tire = Generics.GetMesh("WheelT.x");
			if(!Generics.EditOption.ConvertParentAttributes || parent == null || parent.GetType() != this.GetType()){
				damper.Const = spring.Const = 0.5f;
			}
		}

		public override float Fuel {
			get{return 0;}
		}


		public override string ChipType {
			get {
				return "Wheel";
			}
		}

		public float PreviewRotation {
			get;
			set;
		}
		public override void DrawChip() {
			if(mesh == null)
				mesh = Generics.GetMesh("Wheel.x");

			// 角度反映
			Matrix rot;
			if (Generics.Preview) {
				PreviewRotation += (float)(power.Value * Math.PI / 15000);
				//PreviewRotation += power.Value > 0 ? 0.125f : power.Value < 0 ? -0.125f : 0f;
				rot = Matrix.RotationY(PreviewRotation);
			}
			else {
				PreviewRotation = 0f;
				rot = Matrix.Identity; ;
			}

			if(mesh != null)
				mesh.Draw(Generics.d3ddevice,ChipColor.ToColor(),rot * matRotation * Matrix);

			if((1 <= option && option <= 2) || effect.Value >= 1f){
				float r,w = 1f;
				switch(option){
					case 1:
						r = 0.75f;
						break;
					case 2:
						r = 1f;
						break;
					default:
						r = 0.5f;
						break;
				}
				w = Math.Max(effect.Value,option >0 ? 1f : 0f);
				if(w > 10f)w = 10f;
				if(tire == null)
					tire = Generics.GetMesh("WheelT.x");
				if(tire != null)
					tire.Draw(Generics.d3ddevice,ChipColor.ToColor(),rot * Matrix.Scaling(r,w,r) * matRotation * Matrix);
			}
		}

		public override string AttrTip(string AttrName) {
			switch(AttrName){
				case "Angle":
					return "折り曲げ角度";
				case "Damper":
				case "Dumper":
				case "Danper":
				case "Dunper":
					return "接続部の堅さ";
				case "Spring":
					return "接続部の弾性";
				case "Power":
					return "回転出力";
				case "Brake":
				case "Break":
					return "制動力";
				case "Option":
					return "タイヤ径 1:x1.5 2:x2.0";
				case "Effect":
					return "タイヤ幅(1.0-10.0 物理的な影響なし)";
				case "User1":
				case "User2":
					return "シナリオ用";
				default:
					return base.AttrTip(AttrName);
			}
		}

		public override string[] AttrNameList {
			get{
				string[] s = {"Color","Angle","Damper","Spring","Power","Brake","Option","Effect","User1","User2"};
				return s;
			}
		}

		public override float[] AttrDefaultValueList {
			get {
				return new float[]{(float)0xFFFFFF,0f,0.5f,0.5f,0f,0f,0f,0f,0f,0f};
			}
		}

		public override RcAttrValue[] AttrValList {
			get {
				RcAttrValue attropt = new RcAttrValue();
				attropt.Const = option;
				return new RcAttrValue[]{ChipColor,angle,damper,spring,power,brake,attropt,effect,user1,user2};
			}
		}

		public override RcAttrValue this[string AttrName] {
			get {
				switch(AttrName){
					case "Angle":
						return angle;
					case "Damper":
					case "Dumper":
					case "Danper":
					case "Dunper":
						return damper;
					case "Spring":
						return spring;
					case "Power":
						return power;
					case "Brake":
					case "Break":
						return brake;
					case "Option":
						RcAttrValue a = new RcAttrValue();
						a.Const = (float)option;
						return a;
					case "Effect":
						return effect;
					case "User1":
						return user1;
					case "User2":
						return user2;
					default:
						return base[AttrName];
				}
			}
			set {
				switch(AttrName){
					case "Angle":
						angle = value;
						return;
					case "Damper":
					case "Dumper":
					case "Danper":
					case "Dunper":
						damper = value;
						return;
					case "Spring":
						spring = value;
						return;
					case "Power":
						power = value;
						return;
					case "Brake":
					case "Break":
						brake = value;
						return;
					case "Option":
						option = (int)value.Value;
						return;
					case "Effect":
						effect = value;
						return;
					case "User1":
						user1 = value;
						return;
					case "User2":
						user2 = value;
						return;
					default:
						base[AttrName] = value;
						return;
				}
			}
		}

		public override void UpdateMatrix() {
			if (Generics.Preview) {
				PreviewRotation += power.Value / 8192f;
				//				PreviewRotation += power.Value > 0 ? 0.125f : power.Value < 0 ? -0.125f : 0f;
			}
			base.UpdateMatrix();
		}

		/*		public override string ToString() {
					string s = "";
					s += "Wheel(";	//	←チップの出力名

					//	属性記述ブロック
					if(ChipColor != RcColor.Default)
						s += "Color=" + ChipColor.ToString() + ",";
					if(Name != null && Name != "")
						s += "Name=" + Name + ",";
					if(angle.Val != null || angle.Const != 0f)
						s += "Angle=" + angle.ToString() + ",";
					if(damper.Val != null || damper.Const != 0.5f)
						s += "Damper=" + damper.ToString() + ",";
					if(spring.Val != null || spring.Const != 0.5f)
						s += "Spring=" + spring.ToString() + ",";
					if(power.Val != null || power.Const != 0f)
						s += "Power=" + power.ToString() + ",";
					if(brake.Val != null || brake.Const != 0f)
						s += "brake=" + brake.ToString() + ",";
					if(0 < option && option <= 2)
						s += "Option=" + option.ToString() + ",";
					if(effect.Val != null || effect.Const != 0f)
						s += "Effect=" + effect.ToString() + ",";
					if(user1.Val != null || user1.Const != 0f)
						s += "User1=" + user1.ToString() + ",";
					if(user2.Val != null || user2.Const != 0f)
						s += "User2=" + user2.ToString() + ",";

					s = s.TrimEnd(',');

					s += ")";

					return s;
				}
		*/

		public override RcChipBase Clone(bool WithChild,RcChipBase parent) {
			RcChipWheel copy = new RcChipWheel();
			copy.Name = this.Name;
			copy.angle = this.angle;
			copy.ChipColor = this.ChipColor;
			copy.Generics = this.Generics;
			copy.jointPosition = this.jointPosition;
			copy.matVersion = this.matVersion;
			copy.Parent = parent;

			copy.mesh = this.mesh;
			copy.tire = this.tire;

			copy.damper = this.damper;
			copy.spring = this.spring;
			copy.power = this.power;
			copy.brake = this.brake;
			copy.option = this.option;
			copy.effect = this.effect;
			copy.user1 = this.user1;
			copy.user2 = this.user2;
			copy.Comment = this.Comment;

			copy.Child = new RcChipBase[RcData.ChildCapasity];
			if(WithChild){
				for(int i = 0;i < RcData.ChildCapasity;i++){
					if(this.Child[i] != null)copy.Add(this.Child[i].JointPosition ,this.Child[i].Clone(WithChild,copy),false);
				}
			}
			
			return copy;
		}
		public override void ReverseX() {
			base.ReverseX ();
			if(this.power.Val != null)
				this.power.isNegative ^= true;
			else
				this.power.Const = -this.power.Const;
		}


		public override void ReverseY() {
			base.ReverseY();
			if(this.angle.Val != null)
				this.angle.isNegative ^= true;
			else
				this.angle.Const = -this.angle.Const;
		}

		public override float Weight {
			get {
				return 3.14f;
			}
		}

		public override Matrix FullMatrix {
			get {
				Matrix rot;
				if (Generics.Preview) {
					rot = Matrix.RotationY(PreviewRotation);
				}
				else {
					rot = Matrix.Identity; ;
				}
				return rot * base.FullMatrix;
			}
		}



	}

	///<summery>
	///RLW チップ クラス
	///</summery>
	public class RcChipRLW		: RcChipWheel{
		public RcChipRLW(){}
		public RcChipRLW(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			mesh = Generics.GetMesh("NWheel.x");
		}

		public override string ChipType {
			get {
				return "RLW";
			}
		}

		public override string ToString() {
			string s = base.ToString();
			s = s.Substring(s.IndexOf('('));
			s = "RLW" + s;
			return s;
		}
		public override string ToString(RcOutputOptions outputOptions) {
			string s = base.ToString(outputOptions);
			s = s.Substring(s.IndexOf('('));
			s = "RLW" + s;
			return s;
		}


		public override RcChipBase Clone(bool WithChild,RcChipBase parent) {
			RcChipRLW copy = new RcChipRLW();
			copy.Name = this.Name;
			copy.angle = this.angle;
			copy.ChipColor = this.ChipColor;
			copy.Generics = this.Generics;
			copy.jointPosition = this.jointPosition;
			copy.matVersion = this.matVersion;
			copy.Parent = parent;

			copy.mesh = this.mesh;
			copy.tire = this.tire;

			copy.damper = this.damper;
			copy.spring = this.spring;
			copy.power = this.power;
			copy.brake = this.brake;
			copy.option = this.option;
			copy.effect = this.effect;
			copy.user1 = this.user1;
			copy.user2 = this.user2;
			copy.Comment = this.Comment;

			copy.Child = new RcChipBase[RcData.ChildCapasity];
			if(WithChild){
				for(int i = 0;i < RcData.ChildCapasity;i++){
					if(this.Child[i] != null)copy.Add(this.Child[i].JointPosition ,this.Child[i].Clone(WithChild,copy),false);
				}
			}
			
			return copy;
		}


		public override void ReverseY() {
			base.ReverseY();
			if(this.angle.Val != null)
				this.angle.isNegative ^= true;
			else
				this.angle.Const = -this.angle.Const;
		}
		public override void ReverseZ() {
			base.ReverseY();
			if(this.angle.Val != null)
				this.angle.isNegative ^= true;
			else
				this.angle.Const = -this.angle.Const;
		}

	}

	///<summery>
	///ウェイト チップ クラス
	///</summery>
	public class RcChipWeight	: RcChipChip{
		RcAttrValue option;

		public RcChipWeight(){}
		public RcChipWeight(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			option.Const = 1f;
			mesh = Generics.GetMesh("ChipH.x");
		}

		public override float Fuel {
			get { return 6 * base.Fuel * option.Value; }
		}


		public override string ChipType {
			get {
				return "Weight";
			}
		}

		/*		public override string ToString() {
					string str = "";
					str += "Weight(";

					if(Name != null && Name != "")
						str += "Name=" + Name + ",";
					if(angle.Val != null || angle.Const != 0f)
						str += "Angle=" + angle.ToString() + ",";
					if(this.ChipColor != RcColor.Default)
						str += "Color=" + ChipColor.ToString() + ",";
					if(damper.Val != null || damper.Const != 0.5f)
						str += "Damper=" + damper.ToString() + ",";
					if(spring.Val != null || spring.Const != 0.5f)
						str += "Spring=" + spring.ToString() + ",";
					str = str.TrimEnd(',');
					str += ")";

					return str;

				}
		*/
		public override RcChipBase Clone(bool WithChild,RcChipBase parent) {
			RcChipWeight copy = new RcChipWeight();
			copy.angle = this.angle;
			copy.ChipColor = this.ChipColor;
			copy.damper = this.damper;
			copy.Generics = this.Generics;
			copy.jointPosition = this.jointPosition;
			copy.mesh = this.mesh;
			copy.matVersion = this.matVersion;
			copy.Name = this.Name;
			copy.Parent = parent;
			copy.spring = this.spring;
			copy.option = this.option;
			copy.user1 = this.user1;
			copy.user2 = this.user2;
			copy.Child = new RcChipBase[RcData.ChildCapasity];
			copy.Comment = this.Comment;
			if(WithChild){
				for(int i = 0;i < RcData.ChildCapasity;i++){
					if(this.Child[i] != null)copy.Add(this.Child[i].JointPosition ,this.Child[i].Clone(WithChild,copy),false);
				}
			}
			
			return copy;
		}


		public override float Weight {
			get {
				return 4f * option.Value;
			}
		}

		public override RcAttrValue[] AttrValList {
			get {
				return new RcAttrValue[]{ChipColor,angle,damper,spring,option,user1,user2};
			}
		}

		public override RcAttrValue this[string AttrName] {
			get {
				if(AttrName == "Option")
					return option;
				else
					return base[AttrName];
			}
			set {
				if(AttrName == "Option")
					option = value;
				else
					base[AttrName] = value;
			}
		}

		public override float[] AttrDefaultValueList {
			get {
				return new float[]{(float)0xFFFFFF,0f,0.5f,0.5f,1f,0f,0f};
			}
		}

		public override string[] AttrNameList {
			get {
				string[] s = base.AttrNameList;
				s.CopyTo(s = new string[s.Length + 1],0);
				s[s.Length -1] = s[s.Length - 2];
				s[s.Length -2] = s[s.Length - 3];
				s[s.Length -3] = "Option";
				return s;
			}
		}

		public override string AttrTip(string AttrName) {
			if(AttrName == "Option")
				return "重量倍率(1-8)";
			else
				return base.AttrTip (AttrName);
		}

	}

	///<summery>
	///カウル チップ クラス
	///</summery>
	public class RcChipCowl		: RcChipBase{
		RcXFile[] meshes;
		int option;
		RcAttrValue effect;
		public RcChipCowl(){}
		public RcChipCowl(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			meshes = new RcXFile[6];
			meshes[0] = Generics.GetMesh("Type0.x");
			meshes[1] = Generics.GetMesh("Type1.x");
			meshes[2] = Generics.GetMesh("Type2.x");
			meshes[3] = Generics.GetMesh("Type3.x");
			meshes[4] = Generics.GetMesh("Type4.x");
			meshes[5] = Generics.GetMesh("Type5.x");
			if(!Generics.EditOption.ConvertParentAttributes || parent == null || parent.GetType() != this.GetType()){
				option = 0;
				effect = new RcAttrValue(0x00fb);
			}
			else{
				this.option = ((RcChipCowl)parent).option;
				effect = ((RcChipCowl)parent).effect;
			}
		}

		public override string ChipType {
			get {
				return "Cowl";
			}
		}

		public override void Add(RcJointPosition joint, RcChipBase chip,bool Registeration) {
			if(!(chip is RcChipCowl)){
				throw new Exception("カウルにはカウルしか接続できません。");
			}
			base.Add(joint,chip,Registeration);
		}

		public override string AttrTip(string AttrName) {
			switch(AttrName){
				case "Angle":
					return "折り曲げ角度";
				case "Option":
					return "形状\n1:枠 2:円 3,4:直角三角形 5:半円 他:四角";
				case "Effect":
					return "マテリアルの詳細(4桁16進数)\n左←透明度、発光度、スペキュラの強度、スペキュラ→右";
				case "User1":
				case "User2":
					return "シナリオ用";
				default:
					return base.AttrTip(AttrName);
			}
		}

		public override void DrawChip() {
			ReserveDraw(() => {
				if (Generics.DrawOption.ShowCowl && (((int)this.effect.Value) & 0xF000) != 0xF000)
					if (meshes != null) {
						RcXFile targetMesh = meshes[(option < 0 || option > 5) ? 0 : option];
						targetMesh.Draw(
							Generics.d3ddevice,
							ChipColor.ToColor(),
							(int)effect.Value,
							this.matRotation * this.Matrix);
					}
			});
		}

		public override string[] AttrNameList {
			get{
				string[] s = {"Color","Angle","Option","Effect","User1","User2"};
				return s;
			}
		}

		public override float[] AttrDefaultValueList {
			get {
				return new float[]{(float)0xFFFFFF,0f,0f,0x00fb,0f,0f};
			}
		}

		public override RcAttrValue[] AttrValList {
			get {
				RcAttrValue attropt = new RcAttrValue();
				attropt.Const = option;
				return new RcAttrValue[]{ChipColor,angle,attropt,effect,user1,user2};
			}
		}


		public override RcAttrValue this[string AttrName] {
			get {
				RcAttrValue temp = new RcAttrValue();
				switch(AttrName){
					case  "Angle":
						return angle;
					case "Option":
						temp.Const = (float)option;
						return temp;
					case "Effect":
						return effect;
					case "User1":
						return user1;
					case "User2":
						return user2;
					default:
						return base[AttrName];
				}
			}
			set {
				switch(AttrName){
					case  "Angle":
						angle = value;
						return;
					case "Option":
						option = (int)value.Const;
						return;
					case "Effect":
						effect = value;
						return;
					case "User1":
						user1 = value;
						return;
					case "User2":
						user2 = value;
						return;
					default:
						base[AttrName] = value;
						return;
				}
			}
		}

		/*		public override string ToString() {
					string str = "";
					str += "Cowl(";	//	←チップの出力名

					//	属性記述ブロック
					if(ChipColor != RcColor.Default)
						str += "Color=" + ChipColor.ToString() + ",";
					if(angle.Val != null || angle.Const != 0f)
						str += "Angle=" + angle.ToString() + ",";
					if(Name != null && Name != "")
						str += "Name=" + Name + ",";
					if(option > 1 || option < 5)
						str += "Option=" + option + ",";
					if(user1.Val != null || user1.Const != 0f)
						str += "User1=" + user1.ToString() + ",";
					if(user2.Val != null || user2.Const != 0f)
						str += "User2=" + user2.ToString() + ",";

					str = str.TrimEnd(',');
					str += ")";

					return str;

				}
		*/

		public override RcChipBase Clone(bool WithChild,RcChipBase parent) {
			RcChipCowl copy = new RcChipCowl();
			copy.Name = this.Name;
			copy.angle = this.angle;
			copy.ChipColor = this.ChipColor;
			copy.Generics = this.Generics;
			copy.jointPosition = this.jointPosition;
			copy.matVersion = this.matVersion;
			copy.Parent = parent;

			copy.meshes = new RcXFile[this.meshes.Length];
			this.meshes.CopyTo(copy.meshes,0);

			copy.option = this.option;
			copy.effect = this.effect;
			copy.user1 = this.user1;
			copy.user2 = this.user2;
			copy.Comment = this.Comment;

			copy.Child = new RcChipBase[RcData.ChildCapasity];
			if(WithChild){
				for(int i = 0;i < RcData.ChildCapasity;i++){
					if(this.Child[i] != null)copy.Add(this.Child[i].JointPosition ,this.Child[i].Clone(WithChild,copy),false);
				}
			}
			
			return copy;
		}

		public override void ReverseX() {
			base.ReverseX ();
			if(option == 3)
				option = 4;
			else if(option == 4)
				option = 3;
		}

		public override void ReverseY() {
			base.ReverseY();
			if(this.angle.Val != null)
				this.angle.isNegative ^= true;
			else
				this.angle.Const = -this.angle.Const;
		}


		public override void ReverseZ() {
			base.ReverseZ ();
			if(option == 3)
				option = 4;
			else if(option == 4)
				option = 3;
		}


		public override float Weight {
			get {
				return 0f;
			}
		}
		public override RcHitStatus IsHit(int X, int Y, int ScrWidth, int ScrHeight) {
			if(Generics.EditOption.UnvisibleNotSelected && !Generics.DrawOption.ShowCowl || ((int)this.effect.Value & 0xF000) == 0xF000){
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
				return dist;
			}
			else 
				return base.IsHit (X, Y, ScrWidth, ScrHeight);

		}


	}

	///<summery>
	///アーム チップ クラス
	///</summery>
	public class RcChipArm		: RcChipBase{
		RcXFile mesh;
		RcAttrValue power,damper,spring,option;
		public RcChipArm(){}
		public RcChipArm(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			mesh = Generics.GetMesh("Arm.x");
			if(!Generics.EditOption.ConvertParentAttributes || parent == null || parent.GetType() != this.GetType()){
				damper.Val = null;
				damper.Const = 0.5f;
				spring.Val = null;
				spring.Const = 0.5f;
			}
		}

		public override string ChipType {
			get {
				return "Arm";
			}
		}
		public override string AttrTip(string AttrName) {
			switch(AttrName){
				case "Angle":
					return "折り曲げ角度";
				case "Damper":
				case "Dumper":
				case "Danper":
				case "Dunper":
					return "接続部の堅さ";
				case "Spring":
					return "接続部の弾性";
				case "Power":
					return "トリガ";
				case "Option":
					return "威力\nこれに比例した時間をチャージに要する";
				case "User1":
				case "User2":
					return "シナリオ用";
				default:
					return base.AttrTip(AttrName);
			}
		}

		public override void DrawChip() {
			if(mesh == null)
				mesh = Generics.GetMesh("Arm.x");
			if(mesh != null)
				mesh.Draw(Generics.d3ddevice,ChipColor.ToColor(),matRotation * matTranslation);
		}

		public override string[] AttrNameList {
			get{
				string[] s = {"Color","Angle","Damper","Spring","Power","Option","User1","User2"};
				return s;
			}
		}

		public override RcAttrValue this[string AttrName] {
			get {
				switch(AttrName){
					case "Angle":
						return this.angle;
					case "Damper":
					case "Dumper":
					case "Danper":
					case "Dunper":
						return this.damper;
					case "Spring":
						return this.spring;
					case "Power":
						return this.power;
					case "Option":
						return this.option;
					case "User1":
						return this.user1;
					case "User2":
						return this.user2;
					default:
						return base[AttrName];
				}
			}
			set {
				switch(AttrName){
					case "Angle":
						this.angle = value;
						return;
					case "Damper":
					case "Dumper":
					case "Danper":
					case "Dunper":
						this.damper = value;
						return;
					case "Spring":
						this.spring = value;
						return;
					case "Power":
						this.power = value;
						return;
					case "Option":
						this.option = value;
						return;
					case "User1":
						this.user1 = value;
						return;
					case "User2":
						this.user2 = value;
						return;
					default:
						base[AttrName] = value;
						return;
				}
			}
		}

		public override float[] AttrDefaultValueList {
			get {
				return new float[]{(float)0xFFFFFF,0f,0.5f,0.5f,0f,0f,0f,0f};
			}
		}

		public override RcAttrValue[] AttrValList {
			get {
				return new RcAttrValue[]{ChipColor,angle,damper,spring,power,option,user1,user2};
			}
		}



		/*		public override string ToString() {
					string s = "";
					s += "Arm(";	//	←チップの出力名

					//	属性記述ブロック
					if(ChipColor != RcColor.Default)
						s += "Color=" + ChipColor.ToString() + ",";
					if(Name != null && Name != "")
						s += "Name=" + Name + ",";
					if(angle.Val != null || angle.Const != 0f)
						s += "Angle=" + angle.ToString() + ",";
					if(damper.Val != null || damper.Const != 0.5f)
						s += "Damper=" + damper.ToString() + ",";
					if(spring.Val != null || spring.Const != 0.5f)
						s += "Spring=" + spring.ToString() + ",";
					if(power.Val != null || power.Const != 0f)
						s += "Power=" + power.ToString() + ",";
					if(option.Val != null || option.Const != 0f)
						s += "Option=" + option.ToString() + ",";
					if(user1.Val != null || user1.Const != 0f)
						s += "User1=" + user1.ToString() + ",";
					if(user2.Val != null || user2.Const != 0f)
						s += "User2=" + user2.ToString() + ",";

					s = s.TrimEnd(',');

					s += ")";

					return s;
				}
		*/
		public override RcChipBase Clone(bool WithChild,RcChipBase parent) {
			RcChipArm copy = new RcChipArm();
			copy.Name = this.Name;
			copy.angle = this.angle;
			copy.ChipColor = this.ChipColor;
			copy.Generics = this.Generics;
			copy.jointPosition = this.jointPosition;
			copy.matVersion = this.matVersion;
			copy.Parent = parent;

			copy.mesh = this.mesh;

			copy.damper = this.damper;
			copy.spring = this.spring;
			copy.power = this.power;
			copy.option = this.option;
			copy.user1 = this.user1;
			copy.user2 = this.user2;
			copy.Comment = this.Comment;

			copy.Child = new RcChipBase[RcData.ChildCapasity];
			if(WithChild){
				for(int i = 0;i < RcData.ChildCapasity;i++){
					if(this.Child[i] != null)copy.Add(this.Child[i].JointPosition ,this.Child[i].Clone(WithChild,copy),false);
				}
			}
			
			return copy;
		}

		public override void ReverseY() {
			base.ReverseY();
			if(this.angle.Val != null)
				this.angle.isNegative ^= true;
			else
				this.angle.Const = -this.angle.Const;
		}

		public override float Weight {
			get {
				return 2f;
			}
		}

	}


	#region システム用

	///<summery>
	///カーソル クラス
	///</summery>
	public class RcChipCursor : RcChipBase{
		RcXFile mesh;
		RcAttrValue backColor;

		public RcChipCursor(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			this.Attach(parent,RcJointPosition.NULL);
			mesh = Generics.GetMesh(Application.StartupPath + "\\Resources\\Cursor.x",true);
			matRotation = Matrix.Identity;
			backColor = new RcAttrValue(0);
			ChipColor.SetValue(Generics.DrawOption.CursorFrontColor.ToArgb());
			backColor.SetValue(Generics.DrawOption.CursorBackColor.ToArgb());
			GuideEnabled = true;
		}

		public void SetCursorColor(Color front,Color back){
			ChipColor.SetValue(front.ToArgb());
			backColor.SetValue(back.ToArgb());
		}

		public override void Add(RcJointPosition joint, RcChipBase chip,bool Registeration) {
			if(chip is RcChipGuide){
				for(int i = 0;i < RcData.ChildCapasity;i++){
					if(Child[i] == null){
						Child[i] = chip;
						Child[i].JointPosition = joint;
						Child[i].Parent = this;
						Child[i].UpdateMatrix();
						return;
					}
				}
				return;
			}
			throw new Exception("RcChipCursorに追加できるのはRcChipGuideクラスのインスタンスのみです。");
		}

		public override void Attach(RcChipBase to, RcJointPosition pos) {
			this.Parent = to;	// 一方的な参照であり、Attach先のチップには影響を及ぼさない。
								// これにより、cursor.Parentで選択されたチップを取得できる。
			UpdateMatrix();
		}


		public override string AttrTip(string AttrName) {
			return "!RcChipCursor has no attributes.";
		}

		public override void DrawChip() {
			if(Parent == null)return;
			Color c = ChipColor.ToColor();
			//			if(c.A > 127)
			//				c = Color.FromArgb(127,c);
			if(mesh == null)return;
			mesh.Draw(Generics.d3ddevice,c,0x7000,matRotation * Matrix);
			c = backColor.ToColor();
			mesh.Draw(Generics.d3ddevice,c,0x2000,Matrix.RotationZ((float)Math.PI) * matRotation * Matrix);
			
		}

		public override string[] AttrNameList {
			get{
				return null;
			}
		}

		public override void UpdateMatrix() {
			if(Parent == null)return;
			this.matTranslation = Parent.Matrix;
			this.matVersion = System.DateTime.Now.Ticks;

			foreach(RcChipBase c in Child)if(c != null){
											  c.UpdateMatrix();
										  }

		}

		//		public override string ToString(int tabs){
		//			string s = "";
		//			for(int i = 0;i < tabs;i++)s += "\t";
		//			return s + "//C:Guide(){}\n";		//	派生チップは持たない
		//		}
		//
		//		public override string ToString() {
		//			return "//" + "Guide()";
		//		}

		public override RcAttrValue this[string AttrName] {
			get {
				throw new Exception("RcChipCursorは属性値を持ちません。\nRcAttrValueプロパティは使用できません。");
			}
			set {
				throw new Exception("RcChipCursorは属性値を持ちません。\nRcAttrValueプロパティは使用できません。");
			}
		}

		public bool GuideEnabled { get; set; }
		public RcHitStatus IsHit(int X, int Y, int ScrWidth, int ScrHeight, bool guideEnabled) {
			RcHitStatus dist, buff;
			dist.distance = float.MaxValue;
			dist.HitChip = null;
			if (guideEnabled) {
				foreach (RcChipBase c in Child) {
					if (c != null) {
						buff = c.IsHit(X, Y, ScrWidth, ScrHeight);
						if (dist.distance > buff.distance) {
							dist = buff;
						}
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
			if (dist.distance > buff.distance) {
				dist.distance = buff.distance;
				dist.HitChip = this;
			}

			return dist;
		}

	}

	///<summery>
	///チップ追加用 ガイド クラス
	///</summery>
	public class RcChipGuide : RcChipBase{
		RcXFile mesh;
		public RcChipGuide(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			mesh = Generics.GetMesh(Application.StartupPath + "\\Resources\\guide.x",true);
		}

		public override void Add(RcJointPosition joint, RcChipBase chip,bool Registeration) {
			throw new Exception("RcChipGuideに、Add()は無効です。");
		}

		public override string AttrTip(string AttrName) {
			return "!RcChipGuide has no attributes.";
		}

		public override void DrawChip() {
			if(Parent.Parent == null)return;
			switch(this.JointPosition){
				case RcJointPosition.North:
					ChipColor.SetValue(Generics.DrawOption.NGuideColor.ToArgb());
					break;
				case RcJointPosition.South:
					ChipColor.SetValue(Generics.DrawOption.SGuideColor.ToArgb());
					break;
				case RcJointPosition.East:
					ChipColor.SetValue(Generics.DrawOption.EGuideColor.ToArgb());
					break;
				case RcJointPosition.West:
					ChipColor.SetValue(Generics.DrawOption.WGuideColor.ToArgb());
					break;
				default:
					ChipColor.SetValue(Color.LightGray.ToArgb());
					break;
			}
			Generics.d3ddevice.Lights[0].Enabled = false;
			Generics.d3ddevice.Lights[1].Enabled = true;
			Generics.d3ddevice.Lights[0].Update();
			Generics.d3ddevice.Lights[1].Update();
			if(mesh != null)
				mesh.Draw(Generics.d3ddevice,ChipColor.ToColor(),0x7000,matRotation * Matrix);
			Generics.d3ddevice.Lights[0].Enabled = true;
			Generics.d3ddevice.Lights[1].Enabled = false;
			Generics.d3ddevice.Lights[0].Update();
			Generics.d3ddevice.Lights[1].Update();
		}

		public override string[] AttrNameList {
			get{
				return null;
			}
		}

		public override RcAttrValue this[string AttrName] {
			get {
				throw new Exception("RcChipGuideは属性値を持ちません。\nRcAttrValueプロパティは使用できません。");
			}
			set {
				throw new Exception("RcChipGuideは属性値を持ちません。\nRcAttrValueプロパティは使用できません。");
			}
		}

		//		public override string ToString(int tabs) {
		//			return "";		//	出力されない
		//		}
		//		public override string ToString() {
		//			return "";
		//		}

		public override RcHitStatus IsHit(int X, int Y, int ScrWidth, int ScrHeight) {
			RcHitStatus dist = new RcHitStatus();

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
				viewport, projMat, viewMat, matRotation * this.Matrix);
			Vector3 vFar = Vector3.Unproject(new Vector3(X, Y, viewport.MaxZ), 
				viewport, projMat, viewMat, matRotation * this.Matrix);
			Vector3 vDir = Vector3.Normalize(vFar - vNear);
			
			dist.distance = (mesh.mesh.Intersect(vNear, vDir, out sectinfo)) ? sectinfo.Dist : float.MaxValue; 

			if(dist.distance < float.MaxValue)dist.HitChip = this;


			return dist;
		

		}

		public override RcJointPosition JointPosition {
			get {
				return base.JointPosition;
			}
			set {
				base.JointPosition = value;
			}
		}



	}
	#endregion
}

