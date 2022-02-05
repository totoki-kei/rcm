using System;
using System.Drawing;

namespace RigidChips {
	/// <summary>
	/// チップ属性の値、変数(RcVal)と定数(float)の2項目
	/// </summary>
	public struct ChipAttribute{
		public Constant Const;
		public ValEntry Val;
		public bool IsNegative;
		public bool IsHexagonal { get { return Const.IsHexagonal; } }

		public ChipAttribute(string DefaultValue,ValEntryList vallist){
			Const = 0f;
			Val = null;
			IsNegative = false;
			SetValue(DefaultValue,vallist);
		}

		public ChipAttribute(int IntValue){
			Const = 0f;
			Val = null;
			IsNegative = false;
			SetValue(IntValue);
		}
		
		/// <summary>
		/// 初期状態におけるこの変数の値を得る。
		/// </summary>
		/// <returns>変数の値を表すfloat値。</returns>
		public float Value{
			get{
				CheckDeadReference();
				if(Val != null)
					return IsNegative ? -Val.Now : Val.Now;
				else
					return Const;
			}
		}

		/// <summary>
		/// 変数名、もしくは定数を文字列で得る。
		/// </summary>
		/// <returns>.rcdで使用可能な変数または定数の文字列。</returns>
		public override string ToString() {
			CheckDeadReference();
			if (Val != null) {
				if (IsNegative) {
					return "-" + Val.ValName;
				}
				else {
					return Val.ValName;
				}
			}
			else return Const.ToString();
		}

		public void SetValue(string expression,ValEntryList vallist){
			if(this.Val != null){
				this.Val = null;
			}

			try {
				this.IsNegative = false;
				this.Const = new Constant(expression);
			}
			catch (FormatException) {
				expression = expression.Trim();
				if (expression[0] == '-') {
					IsNegative = true;
					Const.IsHexagonal = false;
					expression = expression.Substring(1);
				}
				else {
					IsNegative = false;
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
			Const = new Constant((float)IntValue, true);
			Val = null;
			IsNegative = false;
		}

		private void CheckDeadReference(){
			if(Val != null && Val.RefCount< 0){
				Const = Val.Default;
				Val = null;
				IsNegative = false;
			}
		}

		public Color ToColor(){
			int c = (int)this.Value;
			return Color.FromArgb((c >> 0x10) & 0xFF,(c >> 0x8) & 0xFF,c & 0xFF);
		}

		public static bool operator ==(ChipAttribute x, ChipAttribute y){
			return !(x != y);
		}
		public static bool operator !=(ChipAttribute x, ChipAttribute y) {
			return x.Val != y.Val || x.IsNegative != y.IsNegative || x.Const != y.Const;
		}

		public override int GetHashCode() {
			return Val.GetHashCode() ^ Const.GetHashCode();
		}

		public override bool Equals(object obj) {
			ChipAttribute v = (ChipAttribute)obj;
			return v == this;
		}

	}
}
