using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using DX = Microsoft.DirectX;
using D3D = Microsoft.DirectX.Direct3D;

/* コメント付きは未実装 */

/* さぁ頑張って片っ端から実装していこうか */

namespace RigidChips {
	public class RcChipCore		: RcChipBase{
		RcXFile mesh;
		public RcChipCore(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,null,pos){
			this.matTransform = Matrix.RotationY((float)Math.PI);
			mesh = Generics.GetMesh("Core.x");
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
			mesh.Draw(Generics.d3ddevice,this.ChipColor,/*DX.Matrix.RotationY((float)Math.PI) * */ Matrix);
		}

		public override string[] GetAttrList() {
			return null;
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
				throw new Exception("Coreには設定できる項目はありません。");
			}
			set {
				throw new Exception("Coreには設定できる項目はありません。");
			}
		}

		public override string ToString(int tabs,int prevDirection) {
			//	テンプレとしても使用可能

			string s = "",st = "";
			for(int i = 0;i < tabs;i++)st += "\t";
			s += st;
			s += "Core(";	//	←チップの出力名

			//	属性記述ブロック
			if(ChipColor != Color.White)
				s += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(Name != null && Name != "")
				s += "Name=" + Name + ",";

			//	ここに各チップの属性記述のコードを書く
			s = s.TrimEnd(',');

			//	派生ブロック取得
			s += "){";
			foreach(RcChipBase cb in this.Child)if(cb != null)
				s += "\n" + cb.ToString(tabs + 1,0);

			s += "\n" + st + "}";

			return s;
		}

		public override void UpdateMatrix() {
			this.matTransform = Matrix.RotationY((float)Math.PI);
		}

	}

	public class RcChipChip		: RcChipBase{
		protected RcAttrValue damper,spring;
		protected RcXFile mesh;
		public RcChipChip(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			mesh = Generics.GetMesh("Chip.x");
			damper.Val = null;
			damper.Const = 0.5f;
			spring.Val = null;
			spring.Const = 0.5f;
		}

		public override string AttrTip(string AttrName) {
			switch(AttrName){
				case "Angle":
					return "折り曲げ角度";
				case "Damper":
					return "接続部の堅さ";
				case "Spring":
					return "接続部の弾性";
			}

			return "";
		}

		public override void DrawChip() {
			mesh.Draw(Generics.d3ddevice,ChipColor,this.Matrix);
		}

		public override string[] GetAttrList() {
			string[] s = {"Angle","Damper","Spring"};
			return s;
		}

		public override string ToString(int tabs,int prevDirection) {
			string s = "",st = "";
			for(int i = 0;i < tabs;i++)st += "\t";
			s += st;
			//	接続方位
			int b = (prevDirection + (byte)this.JointPosition) % 4; switch(b){
				case (byte)RcJointPosition.North:
					s += "N:";
					break;
				case (byte)RcJointPosition.South:
					s += "S:";
					break;
				case (byte)RcJointPosition.East:
					s += "E:";
					break;
				case (byte)RcJointPosition.West:
					s += "W:";
					break;
			}

			s += "Chip(";	//	←チップの出力名

			//	属性記述ブロック
			if(ChipColor != Color.White)
				s += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(angle.Val != null || angle.Const != 0f)
				s += "Angle=" + angle.ToString() + ",";
			if(Name != null && Name != "")
				s += "Name=" + Name + ",";
			if(damper.Val != null || damper.Const != 0.5f)
				s += "Damper=" + damper.ToString() + ",";
			if(spring.Val != null || spring.Const != 0.5f)
				s += "Spring=" + spring.ToString() + ",";

			s = s.TrimEnd(',');

			//	派生ブロック取得
			s += "){";
			foreach(RcChipBase cb in this.Child)if(cb != null)
				s += "\n" + cb.ToString(tabs + 1,b);

			s += "\n" + st + "}";

			return s;
		}

		public override string ToString() {
			string str = "";
			str += "Chip(";

				if(Name != null && Name != "")
					str += "Name=" + Name + ",";
				if(angle.Val != null || angle.Const != 0f)
					str += "Angle=" + angle.ToString() + ",";
				if(this.ChipColor != Color.White)
					str += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
				if(damper.Val != null || damper.Const != 0.5f)
					str += "Damper=" + damper.ToString() + ",";
				if(spring.Val != null || spring.Const != 0.5f)
					str += "Spring=" + spring.ToString() + ",";
			str.TrimEnd(',');
			str += ")";

			return str;

		}

		public override RcAttrValue this[string AttrName] {
			get {
				switch(AttrName){
					case "Angle":
						return this.angle;
					case "Damper":
						return this.damper;
					case "Spring":
						return this.spring;
				}
				throw new Exception("指定された名前の属性はありません。");
			}
			set {
				switch(AttrName){
					case "Angle":
						this.angle = value;
						return;
					case "Damper":
						this.damper = value;
						return;
					case "Spring":
						this.spring = value;
						return;
				}
				throw new Exception("指定された名前の属性はありません。");
			}
		}


	}

	public class RcChipFrame	: RcChipChip{
		bool option;
		RcXFile ghostmesh;
		public RcChipFrame(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			mesh = Generics.GetMesh("Chip2.x");
			ghostmesh = Generics.GetMesh("Chip.x_");
			option = false;
		}

		public override void DrawChip() {
			if(!option)
				base.DrawChip ();
			else
				ghostmesh.Draw(Generics.d3ddevice,ChipColor,Matrix);
		}

		public override string AttrTip(string AttrName) {
			if(AttrName == "Option")
				return "0以外でゴースト化";
			return base.AttrTip (AttrName);
		}

		public override string[] GetAttrList() {
			string[] s = base.GetAttrList ();
			s.CopyTo(s = new string[s.Length + 1],0);
			s[s.Length-1] = "Option";
			return s;
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
					option = (value.Value() != 0f);
				else base[AttrName] = value;
			}
		}

		public override string ToString(int tabs,int prevDirection) {
			string s = "",st = "";
			for(int i = 0;i < tabs;i++)st += "\t";
			s += st;
			//	接続方位
			int b = (prevDirection + (byte)this.JointPosition) % 4; switch(b){
				case (byte)RcJointPosition.North:
					s += "N:";
					break;
				case (byte)RcJointPosition.South:
					s += "S:";
					break;
				case (byte)RcJointPosition.East:
					s += "E:";
					break;
				case (byte)RcJointPosition.West:
					s += "W:";
					break;
			}

			s += "Frame(";	//	←チップの出力名

			//	属性記述ブロック
			if(ChipColor != Color.White)
				s += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(Name != null && Name != "")
				s += "Name=" + Name + ",";
			if(angle.Value() != 0 || angle.Val != null)
				s += "Angle=" + angle.ToString() + ",";
			if(damper.Val != null || damper.Const != 0.5f)
				s += "Damper=" + damper.ToString() + ",";
			if(spring.Val != null || spring.Const != 0.5f)
				s += "Spring=" + spring.ToString() + ",";
			if(option)
				s += "Option=1,";

			s = s.TrimEnd(',');

			//	派生ブロック取得
			s += "){";
			foreach(RcChipBase cb in this.Child)if(cb != null)
				s += "\n" + cb.ToString(tabs + 1,b);

			s += "\n" + st + "}";

			return s;
		}


	}

	public class RcChipRudder	: RcChipChip{
		public RcChipRudder(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			mesh = Generics.GetMesh("Rudder.x");
		}

		public override void DrawChip() {
			mesh.Draw(Generics.d3ddevice,ChipColor,DX.Matrix.RotationY((float)Math.PI) * this.Matrix);
		}

		public override string ToString(int tabs,int prevDirection) {
			string s = "",st = "";
			for(int i = 0;i < tabs;i++)st += "\t";
			s += st;
			//	接続方位
			int b = (prevDirection + (byte)this.JointPosition) % 4; 
			switch(b){
				case (byte)RcJointPosition.North:
					s += "N:";
					break;
				case (byte)RcJointPosition.South:
					s += "S:";
					break;
				case (byte)RcJointPosition.East:
					s += "E:";
					break;
				case (byte)RcJointPosition.West:
					s += "W:";
					break;
			}

			s += "Rudder(";	//	←チップの出力名

			//	属性記述ブロック
			if(ChipColor != Color.White)
				s += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(angle.Val != null || angle.Const != 0f)
				s += "Angle=" + angle.ToString() + ",";
			if(Name != null && Name != "")
				s += "Name=" + Name + ",";
			if(damper.Val != null || damper.Const != 0.5f)
				s += "Damper=" + damper.ToString() + ",";
			if(spring.Val != null || spring.Const != 0.5f)
				s += "Spring=" + spring.ToString() + ",";

			s = s.TrimEnd(',');

			//	派生ブロック取得
			s += "){";
			foreach(RcChipBase cb in this.Child)if(cb != null)
													s += "\n" + cb.ToString(tabs + 1,b);

			s += "\n" + st + "}";

			return s;
		}

		public override string ToString() {
			string str = "";
			str += "Rudder(";

				if(Name != null && Name != "")
					str += "Name=" + Name + ",";
				if(angle.Val != null || angle.Const != 0f)
					str += "Angle=" + angle.ToString() + ",";
				if(this.ChipColor != Color.White)
					str += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
				if(damper.Val != null || damper.Const != 0.5f)
					str += "Damper=" + damper.ToString() + ",";
				if(spring.Val != null || spring.Const != 0.5f)
					str += "Spring=" + spring.ToString() + ",";
			str.TrimEnd(',');
			str += ")";

			return str;

		}

		public override void UpdateMatrix() {
			float a = angle.Value();
			matTransform =	  Matrix.Translation(0f,0f,-0.3f)
				* Matrix.RotationY((float)(a / 180f * Math.PI))
				* Matrix.Translation(0f,0f,-0.3f);

			switch(this.JointPosition){
				case RcJointPosition.North:
					break;
				case RcJointPosition.South:
					matTransform *= Matrix.RotationY((float)Math.PI);
					break;
				case RcJointPosition.East:
					matTransform *= Matrix.RotationY((float)(Math.PI * 0.5f));
					break;
				case RcJointPosition.West:
					matTransform *= Matrix.RotationY((float)(Math.PI * 1.5f));
					break;
			}
			matTransform *= Parent.Matrix;
			matVersion = System.DateTime.Now.Ticks;

			foreach(RcChipBase c in Child)if(c != null){
											  c.UpdateMatrix();
										  }
			
		}


	}

	public class RcChipRudderF	: RcChipRudder{
		bool option;
		RcXFile ghostmesh;
		public RcChipRudderF(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			mesh = Generics.GetMesh("RudderF.x");
			ghostmesh = Generics.GetMesh("Chip.x_");
			option = false;
		}

		public override void DrawChip() {
			if(!option)
				base.DrawChip ();
			else
				ghostmesh.Draw(Generics.d3ddevice,ChipColor,Matrix);
		}

		public override string AttrTip(string AttrName) {
			if(AttrName == "Option")
				return "0以外でゴースト化";
			return base.AttrTip (AttrName);
		}

		public override string[] GetAttrList() {
			string[] s = base.GetAttrList ();
			s.CopyTo(s = new string[s.Length + 1],0);
			s[s.Length-1] = "Option";
			return s;
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
					option = (value.Value() != 0f);
				else base[AttrName] = value;
			}
		}

		public override string ToString(int tabs,int prevDirection) {
			string s = "",st = "";
			for(int i = 0;i < tabs;i++)st += "\t";
			s += st;
			//	接続方位
			int b = (prevDirection + (byte)this.JointPosition) % 4; switch(b){
				case (byte)RcJointPosition.North:
					s += "N:";
					break;
				case (byte)RcJointPosition.South:
					s += "S:";
					break;
				case (byte)RcJointPosition.East:
					s += "E:";
					break;
				case (byte)RcJointPosition.West:
					s += "W:";
					break;
			}

			s += "RudderF(";	//	←チップの出力名

			//	属性記述ブロック
			if(ChipColor != Color.White)
				s += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(Name != null && Name != "")
				s += "Name=" + Name + ",";
			if(angle.Value() != 0 || angle.Val != null)
				s += "Angle=" + angle.ToString() + ",";
			if(damper.Val != null || damper.Const != 0.5f)
				s += "Damper=" + damper.ToString() + ",";
			if(spring.Val != null || spring.Const != 0.5f)
				s += "Spring=" + spring.ToString() + ",";
			if(option)
				s += "Option=1,";

			s = s.TrimEnd(',');

			//	派生ブロック取得
			s += "){";
			foreach(RcChipBase cb in this.Child)if(cb != null)
				s += "\n" + cb.ToString(tabs + 1,b);

			s += "\n" + st + "}";

			return s;
		}

		public override string ToString() {
			string str = "";
			str += "RudderF(";

			if(Name != null && Name != "")
				str += "Name=" + Name + ",";
			if(angle.Val != null || angle.Const != 0f)
				str += "Angle=" + angle.ToString() + ",";
			if(this.ChipColor != Color.White)
				str += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(damper.Val != null || damper.Const != 0.5f)
				str += "Damper=" + damper.ToString() + ",";
			if(spring.Val != null || spring.Const != 0.5f)
				str += "Spring=" + spring.ToString() + ",";
			if(option)
				str += "Option=1,";
			str.TrimEnd(',');
			str += ")";

			return str;

		}

	}

	public class RcChipTrim		: RcChipChip{
		public RcChipTrim(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			mesh = Generics.GetMesh("Trim.x");
		}

		public override void DrawChip() {
			mesh.Draw(Generics.d3ddevice,ChipColor,DX.Matrix.RotationY((float)Math.PI) * this.Matrix);
		}

		public override string ToString(int tabs,int prevDirection) {
			string s = "",st = "";
			for(int i = 0;i < tabs;i++)st += "\t";
			s += st;
			//	接続方位
			int b = (prevDirection + (byte)this.JointPosition) % 4; 
			switch(b){
				case (byte)RcJointPosition.North:
					s += "N:";
					break;
				case (byte)RcJointPosition.South:
					s += "S:";
					break;
				case (byte)RcJointPosition.East:
					s += "E:";
					break;
				case (byte)RcJointPosition.West:
					s += "W:";
					break;
			}

			s += "Trim(";	//	←チップの出力名

			//	属性記述ブロック
			if(ChipColor != Color.White)
				s += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(angle.Val != null || angle.Const != 0f)
				s += "Angle=" + angle.ToString() + ",";
			if(Name != null && Name != "")
				s += "Name=" + Name + ",";
			if(damper.Val != null || damper.Const != 0.5f)
				s += "Damper=" + damper.ToString() + ",";
			if(spring.Val != null || spring.Const != 0.5f)
				s += "Spring=" + spring.ToString() + ",";

			s = s.TrimEnd(',');

			//	派生ブロック取得
			s += "){";
			foreach(RcChipBase cb in this.Child)if(cb != null)
													s += "\n" + cb.ToString(tabs + 1,b);

			s += "\n" + st + "}";

			return s;
		}

		public override string ToString() {
			string str = "";
			str += "Trim(";

			if(Name != null && Name != "")
				str += "Name=" + Name + ",";
			if(angle.Val != null || angle.Const != 0f)
				str += "Angle=" + angle.ToString() + ",";
			if(this.ChipColor != Color.White)
				str += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(damper.Val != null || damper.Const != 0.5f)
				str += "Damper=" + damper.ToString() + ",";
			if(spring.Val != null || spring.Const != 0.5f)
				str += "Spring=" + spring.ToString() + ",";
			str.TrimEnd(',');
			str += ")";

			return str;

		}

		public override void UpdateMatrix() {
			float a = angle.Value();
			matTransform =	  Matrix.Translation(0f,0f,-0.3f)
				* Matrix.RotationZ((float)(a / 180f * Math.PI))
				* Matrix.Translation(0f,0f,-0.3f);

			switch(this.JointPosition){
				case RcJointPosition.North:
					break;
				case RcJointPosition.South:
					matTransform *= Matrix.RotationY((float)Math.PI);
					break;
				case RcJointPosition.East:
					matTransform *= Matrix.RotationY((float)(Math.PI * 0.5f));
					break;
				case RcJointPosition.West:
					matTransform *= Matrix.RotationY((float)(Math.PI * 1.5f));
					break;
			}
			matTransform *= Parent.Matrix;
			matVersion = System.DateTime.Now.Ticks;

			foreach(RcChipBase c in Child)if(c != null){
											  c.UpdateMatrix();
										  }
			
		}

	}

/**/public class RcChipTrimF	: RcChipTrim{
		bool option;
		RcXFile ghostmesh;
		public RcChipTrimF(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			mesh = Generics.GetMesh("TrimF.x");
			ghostmesh = Generics.GetMesh("Chip.x_");
			option = false;
		}

		public override void DrawChip() {
			if(!option)
				base.DrawChip ();
			else
				ghostmesh.Draw(Generics.d3ddevice,ChipColor,Matrix);
		}

		public override string AttrTip(string AttrName) {
			if(AttrName == "Option")
				return "0以外でゴースト化";
			return base.AttrTip (AttrName);
		}

		public override string[] GetAttrList() {
			string[] s = base.GetAttrList ();
			s.CopyTo(s = new string[s.Length + 1],0);
			s[s.Length-1] = "Option";
			return s;
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
					option = (value.Value() != 0f);
				else base[AttrName] = value;
			}
		}

		public override string ToString(int tabs,int prevDirection) {
			string s = "",st = "";
			for(int i = 0;i < tabs;i++)st += "\t";
			s += st;
			//	接続方位
			int b = (prevDirection + (byte)this.JointPosition) % 4; switch(b){
				case (byte)RcJointPosition.North:
					s += "N:";
					break;
				case (byte)RcJointPosition.South:
					s += "S:";
					break;
				case (byte)RcJointPosition.East:
					s += "E:";
					break;
				case (byte)RcJointPosition.West:
					s += "W:";
					break;
			}

			s += "TrimF(";	//	←チップの出力名

			//	属性記述ブロック
			if(ChipColor != Color.White)
				s += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(Name != null && Name != "")
				s += "Name=" + Name + ",";
			if(angle.Value() != 0 || angle.Val != null)
				s += "Angle=" + angle.ToString() + ",";
			if(damper.Val != null || damper.Const != 0.5f)
				s += "Damper=" + damper.ToString() + ",";
			if(spring.Val != null || spring.Const != 0.5f)
				s += "Spring=" + spring.ToString() + ",";
			if(option)
				s += "Option=1,";

			s = s.TrimEnd(',');

			//	派生ブロック取得
			s += "){";
			foreach(RcChipBase cb in this.Child)if(cb != null)
				s += "\n" + cb.ToString(tabs + 1,b);

			s += "\n" + st + "}";

			return s;
		}

		public override string ToString() {
			string str = "";
			str += "TrimF(";

			if(Name != null && Name != "")
				str += "Name=" + Name + ",";
			if(angle.Val != null || angle.Const != 0f)
				str += "Angle=" + angle.ToString() + ",";
			if(this.ChipColor != Color.White)
				str += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(damper.Val != null || damper.Const != 0.5f)
				str += "Damper=" + damper.ToString() + ",";
			if(spring.Val != null || spring.Const != 0.5f)
				str += "Spring=" + spring.ToString() + ",";
			if(option)
				str += "Option=1,";
			str.TrimEnd(',');
			str += ")";

			return str;

		}

	}

	public class RcChipJet		: RcChipBase{
		RcXFile jet,baloon;
		RcAttrValue power,damper,spring,effect,option;
		public RcChipJet(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			jet = Generics.GetMesh("Jet.x");
			baloon = Generics.GetMesh("Jet2.x");
			damper.Val = null;
			damper.Const = 0.5f;
			spring.Val = null;
			spring.Const = 0.5f;
			effect.Const = 0f;
		}

		public override string AttrTip(string AttrName) {
			switch(AttrName){
				case "Angle":
					return "折り曲げ角度";
				case "Damper":
					return "接続部の堅さ";
				case "Spring":
					return "接続部の弾性";
				case "Power":
					return "ジェット出力:バルーンガス量";
				case "Option":
					return "0:ジェット 1:水素バルーン 2:空気バルーン";
				case "Effect":
					return "1-4:スモークを出す(ジェット時のみ)";
			}

			return "";

		}

		public override void DrawChip() {
			float size = (float)Math.Pow(power.Value(),0.3);
			if(option.Const > 0f)
				baloon.Draw(Generics.d3ddevice,ChipColor,DX.Matrix.Scaling(size + 1f,size + 1f,size + 1f) * Matrix);
			else
				jet.Draw(Generics.d3ddevice,ChipColor,Matrix);
		}

		public override string[] GetAttrList() {
			string[] s = {"Angle","Damper","Spring","Power","Option","Effect"};
			return s;
		}

		public override RcAttrValue this[string AttrName] {
			get {
				switch(AttrName){
					case "Angle":
						return this.angle;
					case "Damper":
						return this.damper;
					case "Spring":
						return this.spring;
					case "Power":
						return this.power;
					case "Option":
						return this.option;
					case "Effect":
						return this.effect;
				}
				throw new Exception("指定された名前の属性はありません。");
			}
			set {
				switch(AttrName){
					case "Angle":
						this.angle = value;
						return;
					case "Damper":
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
				}
				throw new Exception("指定された名前の属性はありません。");
			}
		}

		public override string ToString(int tabs,int prevDirection) {
			string s = "",st = "";
			for(int i = 0;i < tabs;i++)st += "\t";
			s += st;
			//	接続方位
			int b = (prevDirection + (byte)this.JointPosition) % 4; switch(b){
				case (byte)RcJointPosition.North:
					s += "N:";
					break;
				case (byte)RcJointPosition.South:
					s += "S:";
					break;
				case (byte)RcJointPosition.East:
					s += "E:";
					break;
				case (byte)RcJointPosition.West:
					s += "W:";
					break;
			}

			s += "Jet(";	//	←チップの出力名

			//	属性記述ブロック
			if(ChipColor != Color.White)
				s += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(Name != null && Name != "")
				s += "Name=" + Name + ",";
			if(angle.Value() != 0 || angle.Val != null)
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

			s = s.TrimEnd(',');

			//	派生ブロック取得
			s += "){";
			foreach(RcChipBase cb in this.Child)if(cb != null)
													s += "\n" + cb.ToString(tabs + 1,b);

			s += "\n" + st + "}";

			return s;
		}

		public override string ToString() {
			string s = "";
			s += "Jet(";	//	←チップの出力名

			//	属性記述ブロック
			if(ChipColor != Color.White)
				s += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(Name != null && Name != "")
				s += "Name=" + Name + ",";
			if(angle.Value() != 0 || angle.Val != null)
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

			s = s.TrimEnd(',');

			s += ")";

			return s;
		}

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
			
			if(option.Value() != 0f)
				buff.distance = (baloon.mesh.Intersect(vNear, vDir, ref sectinfo)) ? sectinfo.Dist : float.MaxValue; 
			else
				buff.distance = (Generics.imesh.Intersect(vNear, vDir, ref sectinfo)) ? sectinfo.Dist : float.MaxValue; 

			if(dist.distance > buff.distance){
				dist.distance = buff.distance;
				dist.HitChip = this;
			}

			return dist;
		}


	}

/**/public class RcChipWheel	: RcChipBase{
		public RcChipWheel(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
		}
	}

/**/public class RcChipRLW		: RcChipWheel{
		public RcChipRLW(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
		}
	}

/**/public class RcChipWeight	: RcChipChip{
		public RcChipWeight(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			mesh = Generics.GetMesh("ChipH.x");
		}

		public override string ToString(int tabs,int prevDirection) {
			string s = "",st = "";
			for(int i = 0;i < tabs;i++)st += "\t";
			s += st;
			//	接続方位
			int b = (prevDirection + (byte)this.JointPosition) % 4; switch(b){
																		case (byte)RcJointPosition.North:
																			s += "N:";
																			break;
																		case (byte)RcJointPosition.South:
																			s += "S:";
																			break;
																		case (byte)RcJointPosition.East:
																			s += "E:";
																			break;
																		case (byte)RcJointPosition.West:
																			s += "W:";
																			break;
																	}

			s += "Weight(";	//	←チップの出力名

			//	属性記述ブロック
			if(ChipColor != Color.White)
				s += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(angle.Val != null || angle.Const != 0f)
				s += "Angle=" + angle.ToString() + ",";
			if(Name != null && Name != "")
				s += "Name=" + Name + ",";
			if(damper.Val != null || damper.Const != 0.5f)
				s += "Damper=" + damper.ToString() + ",";
			if(spring.Val != null || spring.Const != 0.5f)
				s += "Spring=" + spring.ToString() + ",";

			s = s.TrimEnd(',');

			//	派生ブロック取得
			s += "){";
			foreach(RcChipBase cb in this.Child)if(cb != null)
													s += "\n" + cb.ToString(tabs + 1,b);

			s += "\n" + st + "}";

			return s;
		}

		public override string ToString() {
			string str = "";
			str += "Weight(";

			if(Name != null && Name != "")
				str += "Name=" + Name + ",";
			if(angle.Val != null || angle.Const != 0f)
				str += "Angle=" + angle.ToString() + ",";
			if(this.ChipColor != Color.White)
				str += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(damper.Val != null || damper.Const != 0.5f)
				str += "Damper=" + damper.ToString() + ",";
			if(spring.Val != null || spring.Const != 0.5f)
				str += "Spring=" + spring.ToString() + ",";
			str.TrimEnd(',');
			str += ")";

			return str;

		}


	}

/**/public class RcChipCowl		: RcChipBase{
		RcXFile[] meshes;
		int option;
		public RcChipCowl(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			meshes = new RcXFile[6];
			meshes[0] = Generics.GetMesh("Type0.x");
			meshes[1] = Generics.GetMesh("Type1.x");
			meshes[2] = Generics.GetMesh("Type2.x");
			meshes[3] = Generics.GetMesh("Type3.x");
			meshes[4] = Generics.GetMesh("Type4.x");
			meshes[5] = Generics.GetMesh("Type5.x");
			option = 0;
		}

		public override void Add(RcJointPosition joint, RcChipBase chip) {
			if(!(chip is RcChipCowl)){
				throw new Exception("カウルにはカウルしか接続できません。");
			}
			base.Add(joint,chip);
		}

		public override string AttrTip(string AttrName) {
			if(AttrName == "Angle")
				return "折り曲げ角度";
			else if(AttrName == "Option")
				return "形状\n1:枠 2:円 3,4:直角三角形 5:半円 他:四角";
			throw new Exception("指定された属性は存在しません。");
		}

		public override void DrawChip() {
			meshes[(option < 0 || option > 5) ? 0 : option].Draw(Generics.d3ddevice,ChipColor,DX.Matrix.RotationY((float)Math.PI) * this.Matrix);
		}

		public override string[] GetAttrList() {
			string[] s = {"Angle","Option"};
			return s;
		}

		public override RcAttrValue this[string AttrName] {
			get {
				RcAttrValue temp = new RcAttrValue();
				if(AttrName == "Angle"){
					return angle;
				}
				else if(AttrName == "Option"){
					temp.Const = (float)option;
					return temp;
				}
				throw new Exception("指定された属性は存在しません。");
			}
			set {
				if(AttrName == "Angle"){
					angle = value;
					return;
				}
				else if(AttrName == "Option"){
					option = (int)value.Const;
					return;
				}
				throw new Exception("指定された属性は存在しません。");
			}
		}

		public override string ToString(int tabs,int prevDirection) {
			string s = "",st = "";
			for(int i = 0;i < tabs;i++)st += "\t";
			s += st;
			//	接続方位
			int b = (prevDirection + (byte)this.JointPosition) % 4; switch(b){
																		case (byte)RcJointPosition.North:
																			s += "N:";
																			break;
																		case (byte)RcJointPosition.South:
																			s += "S:";
																			break;
																		case (byte)RcJointPosition.East:
																			s += "E:";
																			break;
																		case (byte)RcJointPosition.West:
																			s += "W:";
																			break;
																	}

			s += "Cowl(";	//	←チップの出力名

			//	属性記述ブロック
			if(ChipColor != Color.White)
				s += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(angle.Val != null || angle.Const != 0f)
				s += "Angle=" + angle.ToString() + ",";
			if(Name != null && Name != "")
				s += "Name=" + Name + ",";
			if(option > 1 || option < 5){
				s += "Option=" + option + ",";
			}

			s = s.TrimEnd(',');

			//	派生ブロック取得
			s += "){";
			foreach(RcChipBase cb in this.Child)if(cb != null)
													s += "\n" + cb.ToString(tabs + 1,b);

			s += "\n" + st + "}";

			return s;
		}

		public override string ToString() {
			string str = "";
			str += "Cowl(";	//	←チップの出力名

			//	属性記述ブロック
			if(ChipColor != Color.White)
				str += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(angle.Val != null || angle.Const != 0f)
				str += "Angle=" + angle.ToString() + ",";
			if(Name != null && Name != "")
				str += "Name=" + Name + ",";
			if(option > 1 || option < 5){
				str += "Option=" + option + ",";
			}

			str.TrimEnd(',');
			str += ")";

			return str;

		}

	}

/**/public class RcChipArm		: RcChipBase{
		public RcChipArm(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
		}
	}

	//	------------------------------------------------------------------------以下、システム用

	public class RcChipCursor : RcChipBase{
		RcXFile mesh;

		public RcChipCursor(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			this.Attach(parent,RcJointPosition.NULL);
			mesh = Generics.GetMesh("Cursor2.x");
			ChipColor = Color.FromArgb(255,0,0);
		}

		public override void Add(RcJointPosition joint, RcChipBase chip) {
			if(chip is RcChipGuide){
				base.Add(joint,chip);
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
			mesh.Draw(Generics.d3ddevice,Color.FromArgb(127,ChipColor),matTransform);
		}

		public override string[] GetAttrList() {
			return null;
		}

		public override void UpdateMatrix() {
			if(Parent == null)return;
			this.matTransform = Parent.Matrix;
			this.matVersion = System.DateTime.Now.Ticks;
		}

		public override string ToString(int tabs,int prevDirection){
			string s = "";
			for(int i = 0;i < tabs;i++)s += "\t";
			return s + "//C:Guide(){}\n";		//	派生チップは持たない
		}

		public override string ToString() {
			return "//" + "Guide()";
		}

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
			mesh = Generics.GetMesh("guide.x");
		}

		public override void Add(RcJointPosition joint, RcChipBase chip) {
			throw new Exception("RcChipGuideに、Add()は無効です。");
		}

		public override string AttrTip(string AttrName) {
			return "!RcChipGuide has no attributes.";
		}

		public override void DrawChip() {
			if(Parent.Parent == null)return;
			switch(this.JointPosition){
				case RcJointPosition.North:
					ChipColor = Color.Blue;
					break;
				case RcJointPosition.South:
					ChipColor = Color.Yellow;
					break;
				case RcJointPosition.East:
					ChipColor = Color.Red;
					break;
				case RcJointPosition.West:
					ChipColor = Color.Green;
					break;
				default:
					ChipColor = Color.LightGray;
					break;
			}
			ChipColor = Color.FromArgb(0x7F,ChipColor);
			mesh.Draw(Generics.d3ddevice,ChipColor,this.Matrix);
		}

		public override string[] GetAttrList() {
			return null;
		}

		public override RcAttrValue this[string AttrName] {
			get {
				throw new Exception("RcChipGuideは属性値を持ちません。\nRcAttrValueプロパティは使用できません。");
			}
			set {
				throw new Exception("RcChipGuideは属性値を持ちません。\nRcAttrValueプロパティは使用できません。");
			}
		}

		public override string ToString(int tabs,int prevDirection) {
			return "";		//	出力されない
		}
		public override string ToString() {
			return "";
		}

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
			
			buff.distance = (mesh.mesh.Intersect(vNear, vDir, ref sectinfo)) ? sectinfo.Dist : float.MaxValue; 

			

			if(dist.distance > buff.distance){
				dist.distance = buff.distance;
				dist.HitChip = this;
			}

			return dist;
		

		}


	}

}
