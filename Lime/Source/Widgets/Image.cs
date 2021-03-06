using Yuzu;

namespace Lime
{
	public class Image : Widget, IImageCombinerArg
	{
		private bool skipRender;
		private ITexture texture;
		private IMaterial material;

		[YuzuMember]
		[YuzuSerializeIf(nameof(IsNotRenderTexture))]
		public override sealed ITexture Texture
		{
			get { return texture; }
			set
			{
				if (texture != value) {
					texture = value;
					DiscardMaterial();
					Window.Current?.Invalidate();
				}
			}
		}

		protected override void DiscardMaterial()
		{
			material = null;
		}

		[YuzuMember]
		public Vector2 UV0 { get; set; }

		[YuzuMember]
		public Vector2 UV1 { get; set; }

		public Image()
		{
			Presenter = DefaultPresenter.Instance;
			UV1 = Vector2.One;
			HitTestMethod = HitTestMethod.Contents;
			Texture = new SerializableTexture();
		}

		public Image(ITexture texture)
		{
			Presenter = DefaultPresenter.Instance;
			UV1 = Vector2.One;
			Texture = texture;
			HitTestMethod = HitTestMethod.Contents;
			Size = (Vector2)texture.ImageSize;
		}

		public Image(string texturePath)
			: this(new SerializableTexture(texturePath))
		{
		}

		public override Vector2 CalcContentSize()
		{
			return (Vector2)Texture.ImageSize;
		}

		public override void AddToRenderChain(RenderChain chain)
		{
			if (GloballyVisible && !skipRender && ClipRegionTest(chain.ClipRegion)) {
				AddSelfToRenderChain(chain, Layer);
			}
			skipRender = false;
		}

		public override void Dispose()
		{
			if (Texture != null) {
				Texture.Dispose();
			}
			base.Dispose();
		}

		ITexture IImageCombinerArg.GetTexture()
		{
			return Texture;
		}

		void IImageCombinerArg.SkipRender()
		{
			skipRender = true;
		}

		Matrix32 IImageCombinerArg.UVTransform
		{
			get { return new Matrix32(new Vector2(UV1.X - UV0.X, 0), new Vector2(0, UV1.Y - UV0.Y), UV0); }
		}

		internal protected override bool PartialHitTestByContents(ref HitTestArgs args)
		{
			Vector2 localPoint = LocalToWorldTransform.CalcInversed().TransformVector(args.Point);
			Vector2 size = Size;
			if (size.X < 0) {
				localPoint.X = -localPoint.X;
				size.X = -size.X;
			}
			if (size.Y < 0) {
				localPoint.Y = -localPoint.Y;
				size.Y = -size.Y;
			}
			if (localPoint.X >= 0 && localPoint.Y >= 0 && localPoint.X < size.X && localPoint.Y < size.Y) {
				int u = (int)(Texture.ImageSize.Width * (localPoint.X / size.X));
				int v = (int)(Texture.ImageSize.Height * (localPoint.Y / size.Y));
				return !Texture.IsTransparentPixel(u, v);
			} else {
				return false;
			}
		}

		public override void Render()
		{
			var blending = GlobalBlending;
			var shader = GlobalShader;
			if (material == null) {
				material = WidgetMaterial.GetInstance(blending, shader, null, Texture);
			}
			Renderer.Transform1 = LocalToWorldTransform;
			var uv0 = UV0;
			var uv1 = UV1;
			texture.TransformUVCoordinatesToAtlasSpace(ref uv0);
			texture.TransformUVCoordinatesToAtlasSpace(ref uv1);
			Renderer.DrawSprite(material, GlobalColor, ContentPosition, ContentSize, uv0, uv1, Vector2.Zero, Vector2.Zero);
		}

		public bool IsNotRenderTexture()
		{
			return !(texture is RenderTexture);
		}
	}
}