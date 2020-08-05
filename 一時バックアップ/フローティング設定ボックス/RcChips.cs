using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using DX = Microsoft.DirectX;
using D3D = Microsoft.DirectX.Direct3D;

namespace RigidChips {
	public class RcChipCore		: RcChipBase{
		RcXFile mesh;
		public RcChipCore(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,null,pos){
			this.JointPosition = RcJointPosition.NULL;
			this.matTranslation = Matrix.Identity;
			this.matRotation = Matrix.RotationY((float)Math.PI);
			mesh = Generics.GetMesh("Core.x");
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
			return null;
		}

		public override void Detach() {
			return;
		}

		public override void DrawChip() {
			mesh.Draw(Generics.d3ddevice,this.ChipColor.ToColor(),matRotation * Matrix);
		}

		public override string[] AttrNameList {
			get{
				string[] s = {"User1","User2"};
				return s;
			}
		}

		public override float[] AttrDefaultValueList {
			get {
				return new float[]{0f,0f};
			}
		}

		public override RcAttrValue[] AttrValList {
			get {
				return new RcAttrValue[]{user1,user2};
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
				else throw new Exception("指定された属性は存在しません : " + AttrName);
			}
			set {
				if(AttrName == "User1"){
					user1 = value;
					return;
				}
				else if(AttrName == "User2"){
					user2 = value;
					return;
				}
				else throw new Exception("指定された属性は存在しません : " + AttrName);
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
						this.ChipColor = RcColor.Parse(param[index+1]);
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
			}

			return "";
		}

		public override void DrawChip() {
			mesh.Draw(Generics.d3ddevice,ChipColor.ToColor(),this.matRotation * this.Matrix);
		}

		public override string[] AttrNameList {
			get{
				string[] s = {"Angle","Damper","Spring","User1","User2"};
				return s;
			}
		}

		public override float[] AttrDefaultValueList {
			get {
				return new float[]{0f,0.5f,0.5f,0f,0f};
			}
		}

		public override RcAttrValue[] AttrValList {
			get {
				return new RcAttrValue[]{angle,damper,spring,user1,user2};
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
				}
				throw new Exception("指定された属性は存在しません : " + AttrName);
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
				}
				throw new Exception("指定された属性は存在しません : " + AttrName);
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

	public class RcChipFrame	: RcChipChip{
		bool option;
		RcXFile ghostmesh;
		public RcChipFrame(){}
		public RcChipFrame(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			mesh = Generics.GetMesh("Chip2.x");
			ghostmesh = Generics.GetMesh(Application.StartupPath + "\\Resources\\Chip.x_",true);
			if(!Generics.EditOption.ConvertParentAttributes || parent == null || parent.GetType() != this.GetType()){
				option = false;
			}
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
			if(option){
				if(Generics.DrawOption.FrameGhostShow){
					if(Generics.DrawOption.FrameGhostAlpha){
						ghostmesh.Draw(Generics.d3ddevice,ChipColor.ToColor(),matRotation * matTranslation);
					}
					else{
						mesh.Draw(Generics.d3ddevice,ChipColor.ToColor(),matRotation * matTranslation);
					}
				}
			}
			else{
				mesh.Draw(Generics.d3ddevice,ChipColor.ToColor(),matRotation * matTranslation);
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
					v.Const = option ? 1 : 0;
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

		public override float[] AttrDefaultValueList {
			get {
				return new float[]{0f,0.5f,0.5f,0f,0f,0f};
			}
		}

		public override RcAttrValue[] AttrValList {
			get {
				RcAttrValue attropt = new RcAttrValue();
				attropt.Const = (option ? 1 : 0);
				return new RcAttrValue[]{angle,damper,spring,attropt,user1,user2};
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
			copy.ghostmesh = this.ghostmesh;
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
		public override void UpdateMatrix() {
			float a = angle.Value;
			Matrix invRotation = matRotation;
			invRotation.Invert();
			matTranslation =	  invRotation
				* Matrix.Translation(0f,0f,0.3f)
				* Matrix.RotationY((float)(a / 180f * Math.PI))
				* Matrix.Translation(0f,0f,0.3f)
				* matRotation;


			if(Parent != null)	matTranslation *= Parent.Matrix;
			matVersion = System.DateTime.Now.Ticks;

			foreach(RcChipBase c in Child)if(c != null){
											  c.UpdateMatrix();
										  }
			
		}

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

	public class RcChipRudderF	: RcChipRudder{
		bool option;
		RcXFile ghostmesh;
		public RcChipRudderF(){}
		public RcChipRudderF(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			mesh = Generics.GetMesh("RudderF.x");
			ghostmesh = Generics.GetMesh(Application.StartupPath + "\\Resources\\Chip.x_",true);
			if(!Generics.EditOption.ConvertParentAttributes || parent == null || parent.GetType() != this.GetType()){
				option = false;
			}
		}

		public override string ChipType {
			get {
				return "RudderF";
			}
		}
		public override void DrawChip() {
			if(option){
				if(Generics.DrawOption.FrameGhostShow){
					if(Generics.DrawOption.FrameGhostAlpha){
						ghostmesh.Draw(Generics.d3ddevice,ChipColor.ToColor(),matRotation * matTranslation);
					}
					else{
						mesh.Draw(Generics.d3ddevice,ChipColor.ToColor(),matRotation * matTranslation);
					}
				}
			}
			else{
				mesh.Draw(Generics.d3ddevice,ChipColor.ToColor(),matRotation * matTranslation);
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
				return new float[]{0f,0.5f,0.5f,0f,0f,0f};
			}
		}

		public override RcAttrValue[] AttrValList {
			get {
				RcAttrValue attropt = new RcAttrValue();
				attropt.Const = (option ? 1 : 0);
				return new RcAttrValue[]{angle,damper,spring,attropt,user1,user2};
			}
		}

		public override RcAttrValue this[string AttrName] {
			get {
				RcAttrValue v;
				if(AttrName == "Option"){
					v.Const = option ? 1 : 0;
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
			copy.ghostmesh = this.ghostmesh;
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
		public override void UpdateMatrix() {
			float a = angle.Value;
			Matrix invRotation = matRotation;
			invRotation.Invert();
			matTranslation =	  invRotation
				* Matrix.Translation(0f,0f,0.3f)
				* Matrix.RotationZ((float)(-a / 180f * Math.PI))
				* Matrix.Translation(0f,0f,0.3f)
				* matRotation;


			if(Parent != null)	matTranslation *= Parent.Matrix;
			matVersion = System.DateTime.Now.Ticks;

			foreach(RcChipBase c in Child)if(c != null){
											  c.UpdateMatrix();
										  }
			
		}

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

	public class RcChipTrimF	: RcChipTrim{
		bool option;
		RcXFile ghostmesh;
		public RcChipTrimF(){}
		public RcChipTrimF(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			mesh = Generics.GetMesh("TrimF.x");
			ghostmesh = Generics.GetMesh(Application.StartupPath + "\\Resources\\Chip.x_",true);
			if(!Generics.EditOption.ConvertParentAttributes || parent == null || parent.GetType() != this.GetType()){
				option = false;
			}
		}

		public override string ChipType {
			get {
				return "TrimF";
			}
		}
		public override void DrawChip() {
			if(option){
				if(Generics.DrawOption.FrameGhostShow){
					if(Generics.DrawOption.FrameGhostAlpha){
						ghostmesh.Draw(Generics.d3ddevice,ChipColor.ToColor(),matRotation * matTranslation);
					}
					else{
						mesh.Draw(Generics.d3ddevice,ChipColor.ToColor(),matRotation * matTranslation);
					}
				}
			}
			else{
				mesh.Draw(Generics.d3ddevice,ChipColor.ToColor(),matRotation * matTranslation);
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
				return new float[]{0f,0.5f,0.5f,0f,0f,0f};
			}
		}

		public override RcAttrValue[] AttrValList {
			get {
				RcAttrValue attropt = new RcAttrValue();
				attropt.Const = (option ? 1 : 0);
				return new RcAttrValue[]{angle,damper,spring,attropt,user1,user2};
			}
		}

		public override RcAttrValue this[string AttrName] {
			get {
				RcAttrValue v;
				if(AttrName == "Option"){
					v.Const = option ? 1 : 0;
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
			copy.ghostmesh = this.ghostmesh;
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

	public class RcChipJet		: RcChipBase{
		RcXFile jet,baloon;
		RcAttrValue power,damper,spring,effect,option;
		public RcChipJet(){}
		public RcChipJet(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			jet = Generics.GetMesh("Jet.x");
			baloon = Generics.GetMesh("Jet2.x");
			if(!Generics.EditOption.ConvertParentAttributes || parent == null || parent.GetType() != this.GetType()){
				damper.Val = null;
				damper.Const = 0.5f;
				spring.Val = null;
				spring.Const = 0.5f;
				effect.Const = 0f;
			}
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
			}

			return "";

		}

		public override void DrawChip() {
			float size = (float)Math.Pow(power.Value,0.3);
			if(option.Const == 0f)
				jet.Draw(Generics.d3ddevice,ChipColor.ToColor(),matRotation * Matrix);
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
				baloon.Draw(Generics.d3ddevice,ChipColor.ToColor(),Matrix.Scaling(rate,rate,rate) * matRotation * Matrix);
			}
			else
				baloon.Draw(Generics.d3ddevice,ChipColor.ToColor(),matRotation * Matrix);
		}

		public override string[] AttrNameList {
			get{
				string[] s = {"Angle","Damper","Spring","Power","Option","Effect","User1","User2"};
				return s;
			}
		}

		public override float[] AttrDefaultValueList {
			get {
				return new float[]{0f,0.5f,0.5f,0f,0f,0f,0f,0f};
			}
		}

		public override RcAttrValue[] AttrValList {
			get {
				return new RcAttrValue[]{angle,damper,spring,power,option,effect,user1,user2};
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
				}
				throw new Exception("指定された名前の属性はありません : " + AttrName);
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
				}
				throw new Exception("指定された名前の属性はありません : " + AttrName);
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
				buff.distance = (baloon.mesh.Intersect(vNear, vDir, ref sectinfo)) ? sectinfo.Dist : float.MaxValue; 
			else
				buff.distance = (Generics.imesh.Intersect(vNear, vDir, ref sectinfo)) ? sectinfo.Dist : float.MaxValue; 

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

		public override string ChipType {
			get {
				return "Wheel";
			}
		}

		public override void DrawChip() {
			mesh.Draw(Generics.d3ddevice,ChipColor.ToColor(),matRotation * Matrix);
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
				tire.Draw(Generics.d3ddevice,ChipColor.ToColor(),Matrix.Scaling(r,w,r) * matRotation * Matrix);
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
			}
			throw new Exception("指定された属性は存在しません : " + AttrName);
		}

		public override string[] AttrNameList {
			get{
				string[] s = {"Angle","Damper","Spring","Power","Brake","Option","Effect","User1","User2"};
				return s;
			}
		}

		public override float[] AttrDefaultValueList {
			get {
				return new float[]{0f,0.5f,0.5f,0f,0f,0f,0f,0f,0f};
			}
		}

		public override RcAttrValue[] AttrValList {
			get {
				RcAttrValue attropt = new RcAttrValue();
				attropt.Const = option;
				return new RcAttrValue[]{angle,damper,spring,power,brake,attropt,effect,user1,user2};
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
				}
				throw new Exception("指定された属性は存在しません : " + AttrName);
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
				}
				throw new Exception("指定された属性は存在しません : " + AttrName);
			}
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

	}

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
			string s = base.ToString ();
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

	public class RcChipWeight	: RcChipChip{
		public RcChipWeight(){}
		public RcChipWeight(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			mesh = Generics.GetMesh("ChipH.x");
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
				return 4f;
			}
		}

	}

	public class RcChipCowl		: RcChipBase{
		RcXFile[] meshes;
		int option;
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
				case "User1":
				case "User2":
					return "シナリオ用";
			}
			throw new Exception("指定された属性は存在しません : " + AttrName);
		}

		public override void DrawChip() {
			if(Generics.DrawOption.ShowCowl)
				meshes[(option < 0 || option > 5) ? 0 : option].Draw(Generics.d3ddevice,ChipColor.ToColor(),this.matRotation * this.Matrix);
		}

		public override string[] AttrNameList {
			get{
				string[] s = {"Angle","Option","User1","User2"};
				return s;
			}
		}

		public override float[] AttrDefaultValueList {
			get {
				return new float[]{0f,0f,0f,0f};
			}
		}

		public override RcAttrValue[] AttrValList {
			get {
				RcAttrValue attropt = new RcAttrValue();
				attropt.Const = option;
				return new RcAttrValue[]{angle,attropt,user1,user2};
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
					case "User1":
						return user1;
					case "User2":
						return user2;
				}
				throw new Exception("指定された属性は存在しません : " + AttrName);
			}
			set {
				switch(AttrName){
					case  "Angle":
						angle = value;
						return;
					case "Option":
						option = (int)value.Const;
						return;
					case "User1":
						user1 = value;
						return;
					case "User2":
						user2 = value;
						return;
				}
				
				throw new Exception("指定された属性は存在しません : " + AttrName);
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
			if(Generics.EditOption.UnvisibleNotSelected && !Generics.DrawOption.ShowCowl){
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
			}

			return "";

		}

		public override void DrawChip() {
			mesh.Draw(Generics.d3ddevice,ChipColor.ToColor(),matRotation * matTranslation);
		}

		public override string[] AttrNameList {
			get{
				string[] s = {"Angle","Damper","Spring","Power","Option","User1","User2"};
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
				}
				throw new Exception("指定された属性は存在しません : " + AttrName);
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
				}
				throw new Exception("指定された属性は存在しません : " + AttrName);
			}
		}

		public override float[] AttrDefaultValueList {
			get {
				return new float[]{0f,0.5f,0.5f,0f,0f,0f,0f};
			}
		}

		public override RcAttrValue[] AttrValList {
			get {
				return new RcAttrValue[]{angle,damper,spring,power,option,user1,user2};
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

	public class RcChipCursor : RcChipBase{
		RcXFile mesh;
		RcColor backColor;

		public RcChipCursor(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			this.Attach(parent,RcJointPosition.NULL);
			mesh = Generics.GetMesh(Application.StartupPath + "\\Resources\\Cursor4.x",true);
			matRotation = Matrix.Identity;
			ChipColor = new RcColor();
			backColor = new RcColor();
			ChipColor.Read(Generics.DrawOption.CursorFrontColor.ToArgb());
			backColor.Read(Generics.DrawOption.CursorBackColor.ToArgb());
		}

		public void SetCursorColor(Color front,Color back){
			ChipColor = new RcColor(front.R,front.G,front.B);
			backColor = new RcColor(back.R,back.G,back.B);
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
			throw new Exception("RcChipCursorのAddが不正な形で呼び出されました。");
		}

		public override void Attach(RcChipBase to, RcJointPosition pos) {
			this.Parent = to;
			UpdateMatrix();
		}


		public override string AttrTip(string AttrName) {
			return "!RcChipCursor has no attributes.";
		}

		public override void DrawChip() {
			if(Parent == null)return;
			Color c = ChipColor.ToColor();
			if(c.A > 127)
				c = Color.FromArgb(127,c);
			mesh.Draw(Generics.d3ddevice,c,matRotation * Matrix);
			c = Color.FromArgb(c.A, backColor.ToColor());
			mesh.Draw(Generics.d3ddevice,Color.FromArgb(127,c),Matrix.RotationX((float)Math.PI) * matRotation * Matrix);
			
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

	}

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
					ChipColor.Read(Generics.DrawOption.NGuideColor.ToArgb());
					break;
				case RcJointPosition.South:
					ChipColor.Read(Generics.DrawOption.SGuideColor.ToArgb());
					break;
				case RcJointPosition.East:
					ChipColor.Read(Generics.DrawOption.EGuideColor.ToArgb());
					break;
				case RcJointPosition.West:
					ChipColor.Read(Generics.DrawOption.WGuideColor.ToArgb());
					break;
				default:
					ChipColor.Read(Color.LightGray.ToArgb());
					break;
			}
			mesh.Draw(Generics.d3ddevice,Color.FromArgb(0x7F,ChipColor.ToColor()),matRotation * Matrix);
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
			
			dist.distance = (mesh.mesh.Intersect(vNear, vDir, ref sectinfo)) ? sectinfo.Dist : float.MaxValue; 

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

