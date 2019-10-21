
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows;

namespace Manga_Scan_Helper.BackEnd {
	[JsonObject(MemberSerialization.OptOut)]
	class Page
    {

		private volatile bool _ready = false;
		public bool Ready {
			get => _ready;
			private set => _ready = value;
		}

		public AutoResetEvent WaitHandle {
			get;
			private set;
		}

		public event EventHandler PageChanged;

		public string Path {
			get; private set;
		}

		[JsonIgnoreAttribute]
		public Bitmap Source {
			get; private set;
		}

		public List<Text> TextEntries {
			get; private set;
		}

		

		public Page (string src) {
			Path = src;
			TextEntries = new List<Text>();
			WaitHandle = new AutoResetEvent(false);
		}

		[JsonConstructor]
		public Page (string path, List<Text> textEntries) {
			Path = path;
			TextEntries = textEntries;
			WaitHandle = new AutoResetEvent(false);
		}

		

		public Text AddTextEntry (Rect rect) {
			
			//g.Clear(Color.Red);
			Text txt = new Text(CropImage(rect), rect);
			TextEntries.Add(txt);
			PageChanged?.Invoke(this, new EventArgs());
			return txt;
		}

		public void MoveTextEntry (int index, int newIndex) {
			Text tmp = TextEntries [index];
			TextEntries.RemoveAt(index);
			TextEntries.Insert(newIndex, tmp);
			PageChanged?.Invoke(this, new EventArgs());
		}

		public void RemoveTextEntry (int index) {
			TextEntries.RemoveAt(index);
			PageChanged?.Invoke(this, new EventArgs());
		}

		private Bitmap CropImage (Rect rect) {
			if (rect.Width == 0 || rect.Height == 0)
				return null;

			Bitmap cropped = new Bitmap((int) rect.Width, (int) rect.Height);
			Graphics g = Graphics.FromImage(cropped);
			g.DrawImage(Source, new Rectangle(0, 0, (int) rect.Width, (int) rect.Height),
						new Rectangle((int) rect.X, (int) rect.Y, (int) rect.Width, (int) rect.Height),
						GraphicsUnit.Pixel);
			return cropped;
		}

		public void Load () {
			Source = new Bitmap(Path);
			foreach (Text t in TextEntries)
				t.Load(CropImage(t.Rectangle));
			Ready = true;
			WaitHandle.Set();
		}

		public void ExportScript (StreamWriter writer) {
			foreach (Text t in TextEntries)
				writer.WriteLine(t.TranslatedText + Environment.NewLine);
		}

		public void ExportJPScript (StreamWriter writer) {
			foreach (Text t in TextEntries)
				writer.WriteLine(t.ParsedText + Environment.NewLine);
		}

		public void ExportCompleteScript (StreamWriter writer) {
			foreach (Text t in TextEntries) {
				writer.WriteLine (t.ParsedText + Environment.NewLine);
				writer.WriteLine(t.TranslatedText + Environment.NewLine);
			}
		}

		
	}
}
