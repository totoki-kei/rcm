namespace RigidChips {
	///<summery>
	///トリム チップ クラス
	///</summery>
	public class TrimChip		: NormalChip{
		public TrimChip(){}
		public TrimChip(Environment gen,ChipBase parent,JointPosition pos) : base(gen,parent,pos){
			mesh = Environment.GetMesh("Trim.x");
		}

		public override ChipType ChipType {
			get {
				return ChipType.Trim;
			}
		}
		public override void DrawChip() {
			if(mesh == null)
				mesh = Environment.GetMesh("Trim.x");
			if(mesh != null)
				mesh.Draw(Environment.d3ddevice,ChipColor.ToColor(),matRotation * this.Matrix);
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

		public override ChipBase Clone(bool WithChild,ChipBase parent) {
			TrimChip copy = new TrimChip();
			copy.Name = this.Name;
			copy.angle = this.angle;
			copy.ChipColor = this.ChipColor;
			copy.Environment = this.Environment;
			copy.jointPosition = this.jointPosition;
			copy.matVersion = this.matVersion;
			copy.Parent = parent;

			copy.damper = this.damper;
			copy.spring = this.spring;
			copy.mesh = this.mesh;
			copy.user1 = this.user1;
			copy.user2 = this.user2;
			copy.Children = new ChipBase[Environment.ChildCapasity];
			copy.Comment = this.Comment;
			if(WithChild){
				for(int i = 0;i < Environment.ChildCapasity;i++){
					if(this.Children[i] != null)copy.Add(this.Children[i].JointPosition ,this.Children[i].Clone(WithChild,copy),false);
				}
			}
			
			return copy;
		}

		public override void ReverseX() {
			base.ReverseX ();
			if(this.angle.Val != null)
				this.angle.IsNegative ^= true;
			else
				this.angle.Const = -this.angle.Const;
		}

		public override void ReverseZ() {
			base.ReverseZ();
			if(this.angle.Val != null)
				this.angle.IsNegative ^= true;
			else
				this.angle.Const = -this.angle.Const;
		}

		public override AngleDirection AngleType {
			get {
				return AngleDirection.z;
			}
		}

	}
}

