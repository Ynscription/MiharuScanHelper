using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Manga_Scan_Helper.BackEnd
{
    class Chapter
    {


		public string Path {
			get; private set;
		}

		public List<Page> Pages {
			get; private set;
		}

		public int TotalPages {
			get { return Pages.Count; }
		}

		public Chapter (string folderSrc) {
			Path = folderSrc;
			Pages = new List<Page>();

			DirectoryInfo d = new DirectoryInfo(folderSrc);

			FileInfo [] files = d.GetFiles("*.jpg", SearchOption.TopDirectoryOnly);
			if (files.Length == 0)
				files = d.GetFiles("*.jpeg", SearchOption.TopDirectoryOnly);
			if (files.Length == 0)
				files = d.GetFiles("*.png", SearchOption.TopDirectoryOnly);
			if (files.Length == 0)
				throw new Exception("No images were found in folder " + folderSrc + Environment.NewLine + Environment.NewLine + "Only jpg, jpeg or png files supported.");
			

			FileInfo[] sortedFiles = files.OrderBy(x => x.Name).ToArray();
			foreach (FileInfo file in sortedFiles) {
				Page p = new Page (file.FullName);
				Pages.Add(p);
			}
			
		}

		[JsonConstructor]
		private Chapter (string path, List<Page> pages) {
			Path = path;
			Pages = pages;
		}

		public void Save (string destPath) {
			StreamWriter writer = null;
			try {
				writer = new StreamWriter(destPath, false, Encoding.UTF8);				
				writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
				/*writer.WriteLine(Path);
				writer.WriteLine(TotalPages);
				foreach (Page p in Pages)
					p.Save(writer);*/
				writer.Close();
			}
			catch (Exception e) {
				writer?.Close();
				throw e;
			}
			
		}

		public static Chapter Load (string src) {
			Chapter res = null;
			StreamReader reader = null;
			try {
				reader = new StreamReader(src);
				
				res = JsonConvert.DeserializeObject<Chapter>(reader.ReadToEnd());
				foreach (Page p in res.Pages)
					p.Load();
				
				reader.Close();
			}
			catch (Exception e){
				reader?.Close();
				throw e;
			}

			return res;
			
		}

		public void ExportScript (string destPath) {
			StreamWriter writer = null;
			try {
				writer = new StreamWriter(destPath, false, Encoding.UTF8);
				for (int i = 1; i <= Pages.Count; i++) {
					if (Pages [i - 1].TextEntries.Count > 0) {
						writer.WriteLine(i.ToString("D2") + Environment.NewLine);
						Pages[i-1].ExportScript (writer);
						writer.WriteLine();
					}
				}
				writer.Close();
			}
			catch (Exception e) {
				writer?.Close();
				throw e;
			}
		}

		public void ExportJPScript (string destPath) {
			StreamWriter writer = null;
			try {
				writer = new StreamWriter(destPath, false, Encoding.UTF8);
				for (int i = 1; i <= Pages.Count; i++) {
					if (Pages [i - 1].TextEntries.Count > 0) {
						writer.WriteLine(i.ToString("D2") + Environment.NewLine);
						Pages [i - 1].ExportJPScript(writer);
						writer.WriteLine();
					}
				}
				writer.Close();
			}
			catch (Exception e) {
				writer?.Close();
				throw e;
			}
		}

		public void ExportCompleteScript (string destPath) {
			StreamWriter writer = null;
			try {
				writer = new StreamWriter(destPath, false, Encoding.UTF8);
				for (int i = 1; i <= Pages.Count; i++) {
					if (Pages [i - 1].TextEntries.Count > 0) {
						writer.WriteLine(i.ToString("D2") + Environment.NewLine);
						Pages [i - 1].ExportCompleteScript(writer);
						writer.WriteLine();
					}
				}
				writer.Close();
			}
			catch (Exception e) {
				writer?.Close();
				throw e;
			}
		}

		
	}
}
