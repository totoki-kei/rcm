using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace RigidChips {

	//	TODO: チップベースクラスを作り、それから各種チップを派生させること
	//	TODO: 実際に試験的にチップを組み立て、表示できるかどうかを見る

	//----------------------------------------------------------------------------------//
	public class RcChipBase{
		public RcData Generics;
		public string Name;
		protected RcAttrValue angle;
//		public RcChipType refFormat;
		public Color ChipColor;
		public RcJointPosition JointPosition;
		public RcChipBase Parent;
		public RcChipBase[] Child;

		protected Matrix matTransform;
		protected long matVersion;

		/// <summary>
		/// チップの基本的な初期化を行う。
		/// </summary>
		/// <param name="gen">所属する RcData インスタンス</param>
		/// <param name="parent">接続されるチップ</param>
		public RcChipBase(RcData gen,RcChipBase parent,RcJointPosition pos){
			Child = new RcChipBase[12];	//	仕様が変わっていなければひとつのチップにつけられるチップの最大数は12
			Generics = gen;
			Attach(parent,pos);
			angle.Const = 0f;
			angle.Val = null;
			angle.isNegative = false;
			this.ChipColor = Color.White;
		}
		/// <summary>
		/// ワールド座標変換行列を更新する。
		/// </summary>
		public virtual void UpdateMatrix(){
			float a = angle.Value();
			matTransform =	  Matrix.Translation(0f,0f,-0.3f)
							* Matrix.RotationX((float)(-a / 180f * Math.PI))
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

		/// <summary>
		/// 表示上の位置を表す行列
		/// </summary>
		public virtual Matrix Matrix{
			get{
				if(this.Parent.MatrixVersion > matVersion){
					this.UpdateMatrix();
				}

				return matTransform;
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
		public virtual void Add(RcJointPosition joint,RcChipBase chip){
			for(int i = 0;i < 12;i++){
				if(Child[i] == null){
					Child[i] = chip;
					Child[i].JointPosition = joint;
					Child[i].Parent = this;
					Child[i].UpdateMatrix();
					Generics.RegisterChipAll(chip);
					return;
				}
			}
			throw new Exception("これ以上子チップを格納できません。ひとつのチップに取り付けられるチップの数は12までです。");
		}

		/// <summary>
		/// すでについているチップを取り外す。存在しないチップを指定したときには Exception が発生する。
		/// </summary>
		/// <param name="chip">取り外したいチップ。</param>
		public virtual void Remove(RcChipBase chip){
			bool Removed = false;
			for(int i = 0; i < 12;i++){
				if(Removed){
					Child[i-1] = Child[i];
				}
				else if(Child[i] == chip){
					chip.JointPosition = RcJointPosition.NULL;
					Generics.UnregisterChipAll(chip);
					Removed = true;
				}
			}
			if(Removed)
				return;
			else
				throw new Exception("指定されたチップは見つかりませんでした。");
		}

		/// <summary>
		/// このチップを他のチップに接続する。
		/// </summary>
		/// <param name="to">接続されるチップ。</param>
		/// <param name="pos">接続位置。</param>
		public virtual void Attach(RcChipBase to,RcJointPosition pos){
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
		/// <returns>属性名の配列。</returns>
		public virtual string[] GetAttrList(){
			return null;
		}

		/// <summary>
		/// 指定した名前の属性の説明文を得る。
		/// </summary>
		/// <param name="AttrName">使用可能な属性名。</param>
		/// <returns>指定した属性の説明文。</returns>
		public virtual string AttrTip(string AttrName){
			return null;
		}

		/// <summary>
		/// 指定した属性の値を設定・取得する。
		/// </summary>
		public virtual RcAttrValue this[string AttrName]{
			set{
			}

			get{
				return new RcAttrValue();
			}
		}

		/// <summary>
		/// このチップの情報の文字列を生成する。
		/// </summary>
		/// <returns>このチップの文字列情報。</returns>
		public override string ToString(){
			string str = "";
			str += "Dummy(";

				if(Name != "")str += "Name=" + Name + ",";
				if(JointPosition != RcJointPosition.NULL)str += "Angle=" + angle.ToString() + ",";
				if(this.ChipColor != Color.White)
					str += "Color=#" + ChipColor.R.ToString("X") + ChipColor.G.ToString("X") + ChipColor.B.ToString("X");
			str.TrimEnd(',');
			str += ")";

			return str;
		}

		/// <summary>
		/// このチップとその派生についての完全表現文字列を得る。
		/// </summary>
		/// <param name="tabs">インデントのタブ数</param>
		/// <param name="prevDirection">直前のチップの方角</param>
		/// <returns>.rcd内に使用可能な Body ブロック用文字列</returns>
		public virtual string ToString(int tabs,int prevDirection) {
			return base.ToString ();	//	未実装
		}

		/// <summary>
		///	このチップ及び派生チップの総数。読み取り専用。
		/// </summary>
		public int ChipCount{
			get{
				int c = 1;
				foreach(RcChipBase cb in Child)if(cb != null){
					c += cb.ChipCount;
				}
				return c;
			}
		}

		/// <summary>
		/// このモデルが選択されたかどうかを得る。
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
			
			buff.distance = (Generics.imesh.Intersect(vNear, vDir, ref sectinfo)) ? sectinfo.Dist : float.MaxValue; 
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

		public virtual byte GetRealAngle(){
			byte b;
			if(Parent != null)
				b = Parent.GetRealAngle();
			else
				b = 0;

			return (byte)((b + (int)this.JointPosition) % 4);
		}
	}

	public class RcData{
		public const int MaxChipCount = 512;
		public const int ChipMeshCount = 64;
		public const int KeyCount = 17;

		public Device d3ddevice;
		public RcModel model;
		public RcValList vals;
		public RcKeyList keys;
		public Mesh imesh;

		public int chipCount;
		RcXFile[] meshes;
		public RcChipCursor Cursor;
		RcChipGuide[] Guides;	//	位置表示用の仮チップ

		RcChipBase[] ChipLib;

		/// <summary>
		/// RcData インスタンスを初期化する。
		/// </summary>
		/// <param name="D3DDevice">表示などに用いるMicrosoft.DirectX.Direct3D.Device インスタンス。</param>
		public RcData(Device D3DDevice){

			//	各インスタンスの初期化
			d3ddevice = D3DDevice;

			vals = new RcValList();

			keys = new RcKeyList(KeyCount);

			meshes = new RcXFile[ChipMeshCount];

			model = new RcModel(this);

			ChipLib = new RcChipBase[MaxChipCount];
			
			chipCount = 1;

			Guides = new RcChipGuide[4];
			Cursor = new RcChipCursor(this,model.root,RcJointPosition.NULL);
			Guides[0] = new RcChipGuide(this,Cursor,RcJointPosition.North);
			Guides[1] = new RcChipGuide(this,Cursor,RcJointPosition.South);
			Guides[2] = new RcChipGuide(this,Cursor,RcJointPosition.East);
			Guides[3] = new RcChipGuide(this,Cursor,RcJointPosition.West);

			ExtendedMaterial[] matbuff;
			imesh = Mesh.FromFile("Chip.x",MeshFlags.SystemMemory,d3ddevice,out matbuff);
		}

		/// <summary>
		/// チップ表示用RcXFileクラスへの参照を得る。
		/// </summary>
		/// <param name="FileName">.x ファイルのパス</param>
		/// <returns>ロードしたRcXFileへの参照</returns>
		public RcXFile GetMesh(string FileName){
			for(int i = 0;i < ChipMeshCount;i++){
				if(meshes[i] == null){
					meshes[i] = new RcXFile();
					meshes[i].FileName = FileName;
					meshes[i].Load(this.d3ddevice);
					return meshes[i];
				}else if(meshes[i].FileName == FileName)
					return meshes[i];
			}
			return null;
		}

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

		public void ReloadMeshes(){
			for(int i = 0;i < ChipMeshCount;i++){
				if(meshes[i] != null && meshes[i].FileName != ""){
					meshes[i].Load(this.d3ddevice);
				}
			}
			ExtendedMaterial[] matbuff;
			imesh = Mesh.FromFile("Chip.x",MeshFlags.SystemMemory,d3ddevice,out matbuff);
		}

		public void SetCursor(RcChipBase Target){
			Cursor.Attach(Target,RcJointPosition.NULL);
		}

		public void DrawCursor(){
			Cursor.DrawChipAll();
		}

		public void DrawCursor(Color CursorColor){
			Cursor.ChipColor = CursorColor;
			Cursor.DrawChipAll();
		}

		public int RegisterChip(RcChipBase c){
			for(int i = 0;i < MaxChipCount;i++){
				if(ChipLib[i] == null){
					ChipLib[i] = c;
					return i;
				}
				else if(ChipLib[i] == c)
					return i;
			}
			throw new Exception("チップ最大数(512)を超えました。");
		}

		public void UnregisterChip(RcChipBase c){
			for(int i = 0;i < MaxChipCount;i++){
				if(ChipLib[i] == c){
					ChipLib[i] = null;
					return;
				}
			}
			throw new Exception("指定されたチップは見つかりませんでした。");
		}

		public void RegisterChipAll(RcChipBase c){
			RegisterChip(c);
			foreach(RcChipBase cb in c.Child)if(cb != null)
				RegisterChipAll(cb);
		}

		public void UnregisterChipAll(RcChipBase c){
			foreach(RcChipBase cb in c.Child)if(cb != null)
				UnregisterChipAll(cb);
			UnregisterChip(c);
		}

		public RcChipBase GetChipFromLib(int id){
			return ChipLib[id];
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
				for(int i = 0;i < 12;i++){
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
				return null;
			}
		}
	}

	/// <summary>
	/// 変数データ
	/// </summary>
	public class RcVal{
		public string ValName;
		public float Default;
		public float Min;
		public float Max;
		public float Step;
		public bool Disp;

		public RcVal(){
			Default = Min = Step = 0f;
			Max = 99999999f;
			Disp = true;
		}

		/// <summary>
		/// Val項目の文字列を作成
		/// </summary>
		/// <returns>書式化された Val ブロックの項目</returns>
		public override string ToString() {
			string buff = "";
			buff += ValName + "(";
			if(Default != 0f)buff += "default=" + Default.ToString();
			if(Min != 0f)buff += ",min=" + Min.ToString();
			if(Max != 99999999f)buff += ",max=" + Max.ToString();
			if(Step != 0)buff += ",step=" + Step.ToString();
			if(!Disp)buff += ",disp=0";
			buff += ")";
			return buff;
		}
	}

	/// <summary>
	/// 変数(RcVal)のリスト
	/// </summary>
	public class RcValList{
		RcVal[] list;

		public RcVal this[int idx]{
			get{
				return list[idx];
			}
		}
		public RcVal this[string valname]{
			get{
				foreach(RcVal v in list)
					if(v.ValName == valname)return v;
				
				return null;
			}
		}

		/// <summary>
		/// 変数を作成する。
		/// </summary>
		/// <param name="ValName">変数の識別名。</param>
		/// <returns>新たに作成されたRcValへの参照。</returns>
		public RcVal Add(string ValName){
			if(this[ValName] != null) return null;
			list.CopyTo(list = new RcVal[list.Length +1],0);
			( list[list.Length-1] = new RcVal() ).ValName = ValName;
			return list[list.Length-1];
		}

		/// <summary>
		/// 指定された名前の変数を削除する。
		/// </summary>
		/// <param name="ValName">削除する変数の識別名。</param>
		public void Remove(string ValName){
			bool removed = false;
			for(int i = 0;i < list.Length;i++){
				if(removed)
					list[i-1] = list[i];

				else if(list[i].ValName == ValName)
					removed = true;
			}

			list.CopyTo(list = new RcVal[list.Length -1],0);
		}
	}

	/// <summary>
	/// キー入力情報
	/// </summary>
	public class RcKey{
		public struct RcKeyWork{
			public RcVal Target;
			public float Step;
		}

		public byte Key_ID;
		public RcKeyWork[] Works;

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
		}

		/// <summary>
		/// 変数のキーへの関連づけを削除する。
		/// </summary>
		/// <param name="Target">削除するRcValへの参照。</param>
		public void DeleteWork(RcVal Target){
			bool removed = false;
			for(int i = 0;i < Works.Length;i++){
				if(removed)
					Works[i-1] = Works[i];

				else if(Works[i].Target == Target)
					removed = true;
			}

			Works.CopyTo(Works = new RcKeyWork[Works.Length -1],0);

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
				str += kw.Target.ValName + "(step=" + kw.Step.ToString() + "),";
			}
			str = str.Substring(0,str.Length-1);
			return str;
		}

	}

	/// <summary>
	/// キー(RcKey)のリスト
	/// </summary>
	public class RcKeyList{
		RcKey[] Keys;

		public RcKeyList(int KeyNum){
			Keys = new RcKey[KeyNum];
		}

		public RcKey this[int idx]{
			get{
				return Keys[idx];
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
			d3ddevice.Transform.World = world;
			d3ddevice.SetTexture(0,texture);
			
			Material mat = new Material();
			mat.Ambient = mat.Diffuse = color;
			color = Color.FromArgb(color.A * 3 / 4,	color.R *3 / 4,color.G * 3 / 4,color.B * 3 / 4);
			mat.Emissive = color;

			d3ddevice.Material = mat;

			mesh.DrawSubset(0);
		}

		/// <summary>
		/// FileName に指定されたメッシュを読み込む。
		/// </summary>
		/// <param name="d3ddevice">Microsoft.DirectX.Direct3D.Device への参照。</param>
		public void Load(Device d3ddevice){
			ExtendedMaterial[] matbuff;
			mesh = Mesh.FromFile(this.FileName,MeshFlags.SystemMemory,d3ddevice,out matbuff);
			
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
	//----------------------------------------------------------------------------------//
	/// <summary>
	/// チップ属性の値、変数(RcVal)と定数(float)の2項目
	/// </summary>
	public struct RcAttrValue{
		public float Const;
		public RcVal Val;
		public bool isNegative;
		/// <summary>
		/// 初期状態に置けるこの変数の値を得る。
		/// </summary>
		/// <returns>変数の値を表すfloat値。</returns>
		public float Value(){
			if(Val != null)return isNegative ? -Val.Default : Val.Default;
			else           return Const;
		}

		/// <summary>
		/// 変数名、もしくは定数を文字列で得る。
		/// </summary>
		/// <returns>.rcdで使用可能な変数または定数の文字列。</returns>
		public override string ToString() {
			if(Val != null){
				if(isNegative){
					return "-" + Val.ValName;
				}
				else{
					return Val.ValName;
				}
			}
			else
				return Const.ToString();
		}

	}

	public struct RcHitStatus{
		public float distance;
		public RcChipBase HitChip;
	}
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

}
