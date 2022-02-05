namespace RigidChips {
	///<summery>
	///ラダー チップ クラス
	///</summery>
	public class RudderChip	: NormalChip{
		public RudderChip(){}
		public RudderChip(Environment gen,ChipBase parent,JointPosition pos) : base(gen,parent,pos){
			mesh = Environment.GetMesh("Rudder.x");
		}

		public override ChipType ChipType {
			get {
				return ChipType.Rudder;
			}
		}

		public override void DrawChip() {
			if(mesh == null)
				mesh = Environment.GetMesh("Rudder.x");
			if(mesh != null)
				mesh.Draw(Environment.d3ddevice,ChipColor.ToColor(),matRotation * this.Matrix);
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

		public override ChipBase Clone(bool WithChild,ChipBase parent) {
			RudderChip copy = new RudderChip();
			copy.angle = this.angle;
			copy.ChipColor = this.ChipColor;
			copy.damper = this.damper;
			copy.Environment = this.Environment;
			copy.jointPosition = this.jointPosition;
			copy.mesh = this.mesh;
			copy.matVersion = this.matVersion;
			copy.Name = this.Name;
			copy.Parent = parent;
			copy.spring = this.spring;
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
		public override void ReverseY() {
			ChipBase[] cld = new ChipBase[Environment.ChildCapasity];
			JointPosition jp;
			this.Children.CopyTo(cld,0);

			foreach(ChipBase cb in cld){
				if(cb == null)continue;
				jp = cb.JointPosition;
				cb.ReverseY();
			}
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
				return AngleDirection.y;
			}
		}


	}
}

