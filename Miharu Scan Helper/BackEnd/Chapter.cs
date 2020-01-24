using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
			Pages = new List<Page>();

			DirectoryInfo d = new DirectoryInfo(folderSrc);

			FileInfo [] files = d.GetFiles("*.jpg", SearchOption.TopDirectoryOnly);
			if (files.Length == 0)
				files = d.GetFiles("*.jpeg", SearchOption.TopDirectoryOnly);
			if (files.Length == 0)
				files = d.GetFiles("*.png", SearchOption.TopDirectoryOnly);
			if (files.Length == 0)
				throw new Exception("No images were found in folder " + folderSrc + Environment.NewLine + Environment.NewLine + "Only jpg, jpeg or png files supported.");
			
			
			FileInfo[] sortedFiles = null;
			try {
				sortedFiles = files.OrderBy(x => Int32.Parse(x.Name.Substring(0, x.Name.IndexOf('.')))).ToArray();
			}
			catch (FormatException e) {
				sortedFiles = files.OrderBy(x=> x.Name).ToArray();
			}
			foreach (FileInfo file in sortedFiles) {
				Page p = new Page (file.FullName);
				Pages.Add(p);
			}
			

			Pages[0].Load();
			Task.Run(() => LoadPagesAsync(this, 0));
		}

		public Chapter (string [] filesSrc) {
			Pages = new List<Page>();
			string [] sortedFiles = null;
			try {
				sortedFiles = filesSrc.OrderBy(x => Int32.Parse(x.Substring(x.LastIndexOf("\\"), x.IndexOf('.')))).ToArray();
			}
			catch (FormatException e) {
				sortedFiles = filesSrc.OrderBy(x=> x.Substring(x.LastIndexOf("\\"))).ToArray();
			}

			foreach (string file in sortedFiles) {
				Page p = new Page (file);
				Pages.Add(p);
			}

			Pages[0].Load();
			Task.Run(() => LoadPagesAsync(this, 0));
		}

		[JsonConstructor]
		private Chapter (string path, List<Page> pages) {
			Path = path;
			Pages = pages;
		}

		public void Save (string destPath, int currentPage = 0) {
			StreamWriter writer = null;
			try {
				writer = new StreamWriter(destPath, false, Encoding.UTF8);	
				writer.WriteLine(currentPage);
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

		public static Chapter Load (string src, out int page) {
			Chapter res = null;
			StreamReader reader = null;
			page = 0;
			try {
				reader = new StreamReader(src);
				if (reader.Peek() != '{')
					page = int.Parse(reader.ReadLine());
				res = JsonConvert.DeserializeObject<Chapter>(reader.ReadToEnd());
				res.Pages[page].Load();

				int loadPage = page;
				Task.Run(() => LoadPagesAsync(res, loadPage));
				
				reader.Close();
			}
			catch (Exception e){
				reader?.Close();
				throw e;
			}

			return res;
			
		}

		private static void LoadPagesAsync (Chapter res, int page) {
			int lower = page -1;
			int higher = page +1;
			while (lower >= 0 || higher < res.TotalPages) {
				if (lower >= 0)
					res.Pages[lower--].Load();
				if (higher < res.TotalPages)
					res.Pages[higher++].Load();
			}
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
