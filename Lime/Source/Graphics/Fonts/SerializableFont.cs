using System;
using System.Collections.Generic;

using Yuzu;

namespace Lime
{
	public class SerializableFont
	{
		public IFont Instance { get; private set; }

		[YuzuMember]
		public string Name
		{
			get	{
				return name;
			}
			set	{
				name = value;
				Instance = FontPool.Instance[Name];
			}
		}

		private string name;

		public SerializableFont()
		{
			Name = "";
		}

		public SerializableFont(string name)
		{
			Name = name;
		}
	}

	public class FontPool
	{
		public IFont Null = new Font();
		private Dictionary<string, IFont> fonts = new Dictionary<string, IFont>();

		static readonly FontPool instance = new FontPool();
		public static FontPool Instance { get { return instance; } }

		public Func<string, string> FontNameChanger;

		public IFont DefaultFont { get { return this[null]; } }

		public void AddFont(string name, IFont font)
		{
			fonts[name] = font;
		}

		public IFont this[string name]
		{
			get
			{
				if (FontNameChanger != null)
					name = FontNameChanger(name);
				if (string.IsNullOrEmpty(name))
					name = "Default";
				IFont font;
				if (fonts.TryGetValue(name, out font))
					return font;
				string path = "Fonts/" + name + ".fnt";
				if (!AssetsBundle.Initialized || !AssetsBundle.Instance.FileExists(path))
					return Null;
				font = Serialization.ReadObject<Font>(path);
				fonts[name] = font;
				return font;
			}
		}

		public void Clear()
		{
			foreach (var font in fonts.Values) {
				font.Dispose();
			}
			fonts.Clear();
		}

		public void ClearCache()
		{
			foreach (var font in fonts.Values) {
				font.ClearCache();
			}
		}
	}
}