using System.Drawing;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace RigidChips {
	/// <summary>
	/// マテリアル無し、テクスチャ一枚の簡易メッシュ
	/// </summary>
	public class XFile{
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
}
