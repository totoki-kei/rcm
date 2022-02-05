namespace RigidChips {
	///<summery>
	///RLW チップ クラス
	///</summery>
	public class RLWChip		: WheelChip{
		public RLWChip(){}
		public RLWChip(Environment gen,ChipBase parent,JointPosition pos) : base(gen,parent,pos){
			mesh = Environment.GetMesh("NWheel.x");
		}

		public override ChipType ChipType {
			get {
				return ChipType.RLW;
			}
		}

		public override string ToString() {
			string s = base.ToString();
			s = s.Substring(s.IndexOf('('));
			s = "RLW" + s;
			return s;
		}
		public override string ToString(OutputOptions outputOptions) {
			string s = base.ToString(outputOptions);
			s = s.Substring(s.IndexOf('('));
			s = "RLW" + s;
			return s;
		}


		public override ChipBase Clone(bool WithChild,ChipBase parent) {
			RLWChip copy = new RLWChip();
			copy.Name = this.Name;
			copy.angle = this.angle;
			copy.ChipColor = this.ChipColor;
			copy.Environment = this.Environment;
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

			copy.Children = new ChipBase[Environment.ChildCapasity];
			if(WithChild){
				for(int i = 0;i < Environment.ChildCapasity;i++){
					if(this.Children[i] != null)copy.Add(this.Children[i].JointPosition ,this.Children[i].Clone(WithChild,copy),false);
				}
			}
			
			return copy;
		}


		public override void ReverseY() {
			base.ReverseY();
			if(this.angle.Val != null)
				this.angle.IsNegative ^= true;
			else
				this.angle.Const = -this.angle.Const;
		}
		public override void ReverseZ() {
			base.ReverseY();
			if(this.angle.Val != null)
				this.angle.IsNegative ^= true;
			else
				this.angle.Const = -this.angle.Const;
		}

	}
}

